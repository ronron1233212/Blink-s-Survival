using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BearMovement : MonoBehaviour
{
    public BoxCollider patrolArea;
    public float moveSpeed = 3f;
    private Vector3 targetPosition;
    public GameObject player;
    public float chaseDistance;
    public float stopDistance;
    Animator animator;
    bool hasRoared = false;
    bool isPlayerInRange = false;
    public float roarDuration = 1f;
    private float roarTimer = 0f;
    CharacterController characterController;
    //health
    float maxHealth;
    public  float currentHealth;
    public Image bearHealthBar;
    //combat
    public Transform bearAttackPoint;
   public  float attackRange = 20;
    public LayerMask enemyAttackPlayer;
    Vector3 gravity =new Vector3 (0, -9.81f, 0);
    public AudioClip roarSound;
    public AudioSource aS;
    //drops
    public GameObject steak;
    public GameObject redBottle;
    bool hasSpawned;
  
    private void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        // start at a random position within the patrol area
        targetPosition = GetRandomPointInBox(patrolArea.bounds);
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
       
    }
    private void Update()
    {
        if (currentHealth <= 0 && !hasSpawned)
        {
            hasSpawned = true;
            StartCoroutine(ItemSpawnDelay());
            return;
        }
      

        Physics.gravity = gravity;
        BearHealthSystem();
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= chaseDistance)
        {
            if (!isPlayerInRange)
            {
                hasRoared = false; // Reset the flag when the player enters the chase distance again
            }
            isPlayerInRange = true;

            if (!hasRoared)
            {
                animator.SetTrigger("isRoar");
                aS.PlayOneShot(roarSound);
                hasRoared = true;
                roarTimer = 0f; // Reset the roar timer   
            }
            animator.SetBool("isRunning", true);
            moveSpeed = 7.7f;
            // Stop moving during the roar duration
            if (roarTimer < roarDuration)
            {
                moveSpeed = 0;
                roarTimer += Time.deltaTime;
            }
            targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        }
        else
        {
            isPlayerInRange = false;
        }
        if (distanceToPlayer <= stopDistance)
        {
            moveSpeed = 0;
            BearCombat();
            transform.LookAt(player.transform);


            animator.SetBool("isAttacking", true);
          

            return;
        } 
        else
        {
            animator.SetBool("isDead", false);
            animator.SetBool("isAttacking", false); 
            animator.SetBool("isDead", false);
          
        }
        MoveTowardsTarget();
    }
    private void MoveTowardsTarget()
    {
        if (currentHealth <= 0)
        {
          
            animator.SetBool("isDead", true);
            animator.SetBool("isRunning",false);
            animator.SetBool("isAttacking", false);
            moveSpeed = 0;
            
            Destroy(gameObject, 4);
         
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 500f)
        {

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= chaseDistance)
            {
                // The AI is chasing, so update the target position to the player's position
                targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            }
            else
            {
                // The AI is not chasing, so pick a new random position within the patrol area
                animator.SetBool("isRunning", false);
                moveSpeed = 3;
                targetPosition = GetRandomPointInBox(patrolArea.bounds);
            }
        }
        transform.LookAt(targetPosition);
    }
    private Vector3 GetRandomPointInBox(Bounds bounds)
    {
        Vector3 randomPoint = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
        return randomPoint;
    }
   public  void BearCombat()
    {
            Collider[] hitEnemies = Physics.OverlapSphere(bearAttackPoint.position, attackRange, enemyAttackPlayer);
        foreach (Collider enemy in hitEnemies)
        {
            PlayerMovement playerHealth = enemy.GetComponent<PlayerMovement>();
            if (playerHealth != null)
            {
                playerHealth.myHealth -= 1;
            }
        }
        if (currentHealth <= 0)
        {
           

            animator.SetBool("isAttacking", false);
            animator.SetBool("isDead", true);
            animator.SetBool("isRunning", false);    
            
            moveSpeed = 0;
            
            Destroy(gameObject, 4);
           
            return;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bearAttackPoint.position, attackRange);
    }
    private void BearHealthSystem()
    {    
        bearHealthBar.fillAmount =Mathf.Clamp(currentHealth/maxHealth, 0, 1);
        if (transform.position.y < 195)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator ItemSpawnDelay()
    {
        yield return new WaitForSeconds(4);
        Instantiate(steak, new Vector3(transform.position.x,transform.position.y+2,transform.position.z), Quaternion.Euler(transform.rotation.x,transform.rotation.y,transform.rotation.z+50));
        Instantiate(redBottle, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + 50));



    }
    

  
}


