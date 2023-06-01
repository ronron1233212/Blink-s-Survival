using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TigerMovement : MonoBehaviour
{
    public float gravity = 9.81f;
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
    public float currentHealth;
    public Image tigerHealthBar;
    //combat
    public Transform tigerAttackPoint;
    public LayerMask attackPlayerHealth;
    public float attackRange;
    public AudioClip roarSound;
    public AudioSource aS;

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
        TigerHealthSystem();
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
          
            moveSpeed = 7.5f;
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
            TigerCombat();
           
       
            if (currentHealth <= 0)
            {
                moveSpeed = 0;
                Destroy(gameObject, 1);
            }
            moveSpeed = 0;
            transform.LookAt(player.transform);
            return;
        }
        else
        {
            animator.SetBool("isAttacking", false);
           

        }
        MoveTowardsTarget();

        if (currentHealth <=0)
        {
            moveSpeed = 0;
            Destroy(gameObject,1);
        }
    }
    private void MoveTowardsTarget()
    {
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
                moveSpeed = 4;
                targetPosition = GetRandomPointInBox(patrolArea.bounds);
            }
        }

        // Make the AI object look at the target position
        transform.LookAt(targetPosition);
    }
    private Vector3 GetRandomPointInBox(Bounds bounds)
    {
        Vector3 randomPoint = new Vector3(Random.Range(bounds.min.x, bounds.max.x), transform.position.y, Random.Range(bounds.min.z, bounds.max.z));
        return randomPoint;
    }
    public void TigerCombat()
    {
        animator.SetBool("isAttacking", true);
        Collider[] playerAttack = Physics.OverlapSphere(tigerAttackPoint.position, attackRange, attackPlayerHealth);
            foreach (Collider enemy in playerAttack)
        {
            PlayerMovement playerMovement = enemy.GetComponent<PlayerMovement>();   
            if (playerMovement != null)
            {
                playerMovement.myHealth -= 0.1f;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(tigerAttackPoint.position, attackRange);
    }
    private void TigerHealthSystem()
    {

        if (currentHealth <= 0)
        {

            moveSpeed = 0;
            Destroy(gameObject, 3);
        }
        tigerHealthBar.fillAmount = Mathf.Clamp(currentHealth/maxHealth, 0f, 1f);
        if (transform.position.y < 195)
        {
            Destroy(gameObject);


        }
    }
}
