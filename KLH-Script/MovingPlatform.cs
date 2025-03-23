using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    private Vector3 targetPos;
    PlayerController playerController;  // Change this from MovementController to PlayerController
    Rigidbody2D rb;
    Vector3 moveDirection;
    Rigidbody2D playerRb;

    private void Awake()
    {
        // Initialize references
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();  // Update to PlayerController
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Set initial target position and calculate direction
        targetPos = posB.position;
        DirectionCalculate();
    }

    private void Update()
    {
        // If the platform is close to one of the positions, change direction
        if (Vector2.Distance(transform.position, posA.position) < 0.05f)
        {
            targetPos = posB.position;
        }
        else if (Vector2.Distance(transform.position, posB.position) < 0.05f)
        {
            targetPos = posA.position;
        }
        
        DirectionCalculate(); // Recalculate direction when position is updated
    }

    private void FixedUpdate()
    {
        // Update platform velocity
        rb.velocity = moveDirection * speed;
    }

    void DirectionCalculate()
    {
        // Calculate the direction towards the target position
        moveDirection = (targetPos - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // The player is on the platform, so enable platform movement
            playerController.isOnPlatform = true;  // Update to use PlayerController
            playerController.platformRb = rb;     // Update to use PlayerController
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // The player leaves the platform, so disable platform movement
            playerController.isOnPlatform = false;  // Update to use PlayerController
        }
    }
}
