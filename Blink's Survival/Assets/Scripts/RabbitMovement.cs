using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RabbitMovement : MonoBehaviour
{
    public BoxCollider patrolArea;
    public float moveSpeed = 3f;
    private Vector3 targetPosition;
    Animator animator;
    public GameObject player;
    public float spotDistance;
    CharacterController characterController;
    float maxHealth = 100;
    public float currentHealth;
    public Image rabbitHealthBar;
    public GameObject smallBottle;
    public GameObject smallHam;
    bool hasSpawned;
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
        if (currentHealth <= 0 && !hasSpawned)
        {
            hasSpawned = true;
            StartCoroutine(ItemSpawnDelay());
            return;
        }
        RabbitHealthSystem();
        if (CanSeePlayer())
        {  
            moveSpeed = 7;
            animator.speed =1.5f; 
        }
        else
        {      
            moveSpeed = 3f;
   
            animator.speed = 1f; 
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
        /*Vector3 moveDirection = (targetPosition - transform.position).normalized;
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
        float targetHeight = terrainHeight + characterController.height * 0.5f;
        movement.y = targetHeight - transform.position.y;*/
        if (Vector3.Distance(transform.position, targetPosition) <=500f)
        {
            // pick a new random position within the patrol area
            targetPosition = GetRandomPointInBox(patrolArea.bounds);
        }
        // make the AI object look at the target position
        transform.LookAt(targetPosition);
    }
    private Vector3 GetRandomPointInBox(Bounds bounds)
    {
        Vector3 randomPoint = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
        return randomPoint;
    }
    private bool CanSeePlayer()
    {
        // Check if the player is within the spot distance
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= spotDistance;
    }
    private void RabbitHealthSystem()
    {
        if (currentHealth <= 0)
        {
            moveSpeed = 0;
            animator.SetBool("isDead", true);
            Destroy(gameObject, 3);
        }
        rabbitHealthBar.fillAmount=Mathf.Clamp(currentHealth/maxHealth, 0, 1);
        if (transform.position.y < 195)
        {
            Destroy(gameObject);


        }

    }
    IEnumerator ItemSpawnDelay()
    {
        yield return new WaitForSeconds(3);
        Instantiate(smallHam, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + 50));
        Instantiate(smallBottle, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + 50));
    }
}

