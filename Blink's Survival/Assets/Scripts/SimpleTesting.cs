using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleTesting : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points
    public float moveSpeed = 5f; // Movement speed of the animal
    public float rotationSpeed = 5f; // Rotation speed of the animal

    private int currentPointIndex = 0; // Index of the current patrol point
    private Transform currentPoint; // Current patrol point
    private bool isMoving = false; // Flag to check if the animal is moving
    private CharacterController characterController; // Reference to the character controller component

    void Start()
    {
        // Set the first patrol point as the current point
        currentPoint = patrolPoints[currentPointIndex];
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isMoving)
        {
            // Start moving towards the next patrol point
            StartCoroutine(MoveToNextPoint());
        }
    }

    IEnumerator MoveToNextPoint()
    {
        isMoving = true;

        // Calculate the path to the next patrol point
        Vector3[] path = Pathfinding(currentPoint.position, patrolPoints[(currentPointIndex + 1) % patrolPoints.Length].position);

        // Traverse the path to the next patrol point
        for (int i = 0; i < path.Length; i++)
        {
            // Rotate the animal towards the next point
            Quaternion targetRotation = Quaternion.LookRotation(path[i] - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move the animal towards the next point
            Vector3 moveDirection = (path[i] - transform.position).normalized;
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            yield return null;
        }

        // Increment the patrol point index
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        currentPoint = patrolPoints[currentPointIndex];
        isMoving = false;
    }

    Vector3[] Pathfinding(Vector3 startPoint, Vector3 endPoint)
    {
        // Replace this method with your custom pathfinding algorithm
        // Here, you can implement the A* algorithm or any other pathfinding approach you prefer
        // to calculate the path from the start point to the end point

        // For simplicity, let's assume a direct path between the points
        Vector3[] path = new Vector3[2];
        path[0] = startPoint;
        path[1] = endPoint;
        return path;
    }
}
