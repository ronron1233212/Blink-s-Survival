using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WolfMovement : MonoBehaviour
{
    public BoxCollider patrolArea;
    public float moveSpeed = 3f;
    private Vector3 targetPosition;
    public GameObject player;
    public float chaseDistance;
    public float stopDistance;
    Animator animator;
    CharacterController characterController;

    bool isWolfAttacking;
    //combat
    public Transform wolfAttackPoint;
    public float attackRange;
    public LayerMask enemyAttackPlayer;
    //wolfhealth
    float maxHealth = 100;
    public float currentHealth;
    public Image wolfHealthBar;
    public AudioClip attackSound;
    public AudioSource aS;
    
    private void Start()
    {
      
        currentHealth = maxHealth;
        // start at a random position within the patrol area
        targetPosition = GetRandomPointInBox(patrolArea.bounds);
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        WolfHealthSystem();
        

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= stopDistance)
        {
           
            animator.SetBool("isAttacking2", true);
      
            WolfCombat();
            moveSpeed = 0;
            transform.LookAt(player.transform);
            return;
          
        }
        else
        {
            isWolfAttacking = false;
            animator.SetBool("isAttacking", false);
        }

        if (distanceToPlayer <= chaseDistance)
        {
            animator.SetBool("isRunning", true);
            moveSpeed = 7;
            if (distanceToPlayer > stopDistance)
            {
                targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            }
        }
        else
        {
          
            animator.SetBool("isRunning", false);
            moveSpeed = 3;
            if (Vector3.Distance(transform.position, targetPosition) <= 5f)
            {
                targetPosition = GetRandomPointInBox(patrolArea.bounds);
            }
        }
      
    
     
       

        MoveTowardsTarget();
    }
    private void MoveTowardsTarget()
    {
        if (currentHealth <= 0)
        {
            moveSpeed = 0;
            animator.SetBool("isDead", true);
            Destroy(gameObject, 3);
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
                moveSpeed = 2;
                targetPosition = GetRandomPointInBox(patrolArea.bounds);
            }
        }

        // Make the AI object look at the target position
        transform.LookAt(targetPosition);
    }
    private Vector3 GetRandomPointInBox(Bounds bounds)
    {
        Vector3 randomPoint = new Vector3(Random.Range(bounds.min.x, bounds.max.x),transform.position.y, Random.Range(bounds.min.z, bounds.max.z));
        return randomPoint;
    }
    public void WolfCombat()
    {

       
            Collider[] hitEnemies = Physics.OverlapSphere(wolfAttackPoint.position, attackRange, enemyAttackPlayer);
        foreach (Collider enemy in hitEnemies)
        {
            PlayerMovement playerHealth = enemy.GetComponent<PlayerMovement>();
            if (playerHealth != null)
            {
              
                playerHealth.myHealth -= 0.1f;
               
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wolfAttackPoint.position, attackRange);
    }
    private void WolfHealthSystem()
    {
       float distance = Vector3.Distance(transform.position, wolfHealthBar.transform.position);
        if (currentHealth <= 0)
        {
            moveSpeed = 0;
            animator.SetBool("isDead", true);
            Destroy(gameObject, 3);
        }
      
        wolfHealthBar.fillAmount =Mathf.Clamp(currentHealth/maxHealth, 0, 1);
        if (transform.position.y < 195)
        {
            Destroy(gameObject);
        }
    }
   
}

