using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //player movement
    public float moveSpeed;
    float dirH;
    float dirV;
    CharacterController cc;
    Animator animator;
    //player gravity
    public bool isGround;
    float radius;
    public LayerMask groundLayerMask;
    public Transform groundCheck;
    float gravity;
    Vector3 fallVelocity;
    //player camera rotation
    float mouseX;
    float mouseY;
    float cameraX;
    public float lookSpeed;
    public Transform cameraTurn;
    float jumpForce = 5;
    //combat
    float hitRate = 1;
    float nextHit = 0.1f;
    public Transform attackPoint;
    public float attackRange = 20;
    public LayerMask enemyLayers;
    public GameObject arrowPrefab;
    public Transform arrowPos;
    float shootRate = 1;
    float nextShoot = 0.5f;
    //health
    public float myHealth;
    float maxHealth = 150;
    public Image healthBar;
    bool isDead = false;
    public AudioClip[] clips = new AudioClip[6];
    public AudioSource aS;
    //food
    public float currentFood;
    float maxFood = 100;
    public Image foodBar;
    public float foodTimer;
    public float currentWater;
    float maxWater = 100;
    public Image waterBar;
    public float waterTimer;
    bool isTakingDamage;

    // Start is called before the first frame update
    void Start()
    {
        currentFood = maxFood;
        currentWater = maxWater;
        myHealth = maxHealth;
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        radius = 0.2f;
        gravity = -50;
        isGround = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFood <= 0 && !isTakingDamage || currentWater <= 0 && !isTakingDamage)
        {
            StartCoroutine(TakeStarveDmg());
        }

        Movement();
        CammeraRotation();
        GravityAndJumping();
        PlayerCombat();
        PlayerHealthSystem();

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Food")
        {
            myHealth += 10;
            currentFood += 10;
            currentFood = Mathf.Clamp(currentFood, 0, 100);
            myHealth = Mathf.Clamp(myHealth, 0, 150);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Drink")
        {
            currentWater += 20;
            currentWater = Mathf.Clamp(currentWater, 0, 100);
            Destroy(collision.gameObject);
        }
        if(collision.gameObject.tag == "Apple")
        {
            currentFood += 5;
            currentFood = Mathf.Clamp(currentFood, 0, 100);
            Destroy(collision.gameObject) ;
        }
    }
    private void Movement()
    {
        dirH = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        dirV = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 movement = transform.forward * dirV + transform.right * dirH;
        cc.Move(movement);
        if (dirH != 0 | dirV != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 rollForceLeft = -transform.right * jumpForce * Time.deltaTime * 2f;
            cc.Move(rollForceLeft);
            animator.SetBool("isRollingLeft", true);
        }
        else
        {
            animator.SetBool("isRollingLeft", false);
        }
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 rollForceRight = transform.right * jumpForce * Time.deltaTime * 2f;
            cc.Move(rollForceRight);
            animator.SetBool("isRollingRight", true);

        }

        else
        {
            animator.SetBool("isRollingRight", false);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("isWalkingBack", true);
        }
        else
        {
            animator.SetBool("isWalkingBack", false);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("isSprint", true);
            moveSpeed += 3;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {

            animator.SetBool("isSprint", false);
            moveSpeed = 5;
        }
        if (WeaponHolder.selectedWep == 1)
        {
            moveSpeed = 4;
            animator.SetBool("isSprint", false);
        }

    }
    private void CammeraRotation()
    {
        mouseX = Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
        transform.Rotate(0, mouseX, 0);
        mouseY = Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        cameraX -= mouseY;
        cameraX = Mathf.Clamp(cameraX, -20, 20);
        cameraTurn.localRotation = Quaternion.Euler(cameraX, 0, 0);
    }
    private void GravityAndJumping()
    {
        if (Physics.CheckSphere(groundCheck.position, radius, groundLayerMask))
        {

            isGround = true;
        }
        else
        {
            isGround = false;
        }
        if (isGround == false)
        {
            fallVelocity.y += gravity * Time.deltaTime;
        }
        else
        {
            fallVelocity.y = 0;
        }
        if (Input.GetButtonDown("Jump") && (isGround == true))
        {
            nextShoot = 0;
            animator.SetBool("isJumping", true);
            aS.PlayOneShot(clips[3]);
            fallVelocity.y += 20f;
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        cc.Move(fallVelocity * Time.deltaTime);
    }

    private void PlayerCombat()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= nextShoot)
        {
            if (WeaponHolder.selectedWep == 2)
            {
                aS.PlayOneShot(clips[0]);
                animator.SetBool("isArrow", true);
                Instantiate(arrowPrefab, arrowPos.transform.position, transform.rotation);
                nextShoot = Time.time + shootRate;
            }
        }
        else
        {
            animator.SetBool("isArrow", false);
        }
        if (Input.GetMouseButtonDown(0) && Time.time >= nextHit)
        {
            if (WeaponHolder.selectedWep == 0)
            {
                aS.PlayOneShot(clips[1]);
                animator.SetTrigger("isHitting");
                nextHit = Time.time + hitRate;
            }
            else if (WeaponHolder.selectedWep == 1 && Time.time >= nextHit)
            {
                aS.PlayOneShot(clips[2], 0.2f);
                animator.SetTrigger("isHitting2");
                nextHit = Time.time + 2f;
            }
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider enemy in hitEnemies)
            {
                BearMovement bearMovement = enemy.GetComponent<BearMovement>();
                if (bearMovement != null && WeaponHolder.selectedWep == 0)
                {
                    bearMovement.currentHealth -= 10;
                }
                else if (bearMovement != null && WeaponHolder.selectedWep == 1)
                {
                    bearMovement.currentHealth -= 20;
                }
                RabbitMovement rabbitMovement = enemy.GetComponent<RabbitMovement>();
                if (rabbitMovement != null && WeaponHolder.selectedWep == 0)
                {
                    rabbitMovement.currentHealth -= 50;
                }
                else if (rabbitMovement != null && WeaponHolder.selectedWep == 1)
                {
                    rabbitMovement.currentHealth -= 100;
                }
                WolfMovement wolfMovement = enemy.GetComponent<WolfMovement>();
                {
                    if (wolfMovement != null && WeaponHolder.selectedWep == 0)
                    {
                        wolfMovement.currentHealth -= 20;
                    }
                    else if (wolfMovement != null && WeaponHolder.selectedWep == 1)
                    {
                        wolfMovement.currentHealth -= 40;
                    }
                }
                TigerMovement tigerMovement = enemy.GetComponent<TigerMovement>();
                {
                    if (tigerMovement != null && WeaponHolder.selectedWep == 0)
                    {
                        tigerMovement.currentHealth -= 20;
                    }
                    else if (tigerMovement != null && WeaponHolder.selectedWep == 1)
                    {
                        tigerMovement.currentHealth -= 40;
                    }
                }

                //instead of foreach :
                /*for (int i = 0; i < hitEnemies.Length; i++)
                {
                    Collider enemy = hitEnemies[i];
                    Debug.Log("Hit");
                }*/
            }
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }
    private void PlayerHealthSystem()
    {
        waterBar.fillAmount = Mathf.Clamp(currentWater / maxWater, 0, 1);
        foodBar.fillAmount = Mathf.Clamp(currentFood / maxFood, 0, 1);
        healthBar.fillAmount = Mathf.Clamp(myHealth / maxHealth, 0, 1);
        if (myHealth <= 0 && !isDead)
        {

            animator.SetTrigger("isDead");
            isDead = true;
            cc.enabled = false;
            enabled = false;
        }

        foodTimer += Time.deltaTime;
        waterTimer += Time.deltaTime;
        if (foodTimer >= 60)
        {
            currentFood -= 20;
            foodTimer = 0;
        }
        if (waterTimer >= 60)
        {
            currentWater -= 20;
            waterTimer = 0;
        }

    }
   
    IEnumerator TakeStarveDmg()
    {
        isTakingDamage = true; 

        while (currentFood <= 0||currentWater<=0)
        {
            myHealth -= 10;
            yield return new WaitForSeconds(60);
        }

        isTakingDamage = false; 
    }
}


 
