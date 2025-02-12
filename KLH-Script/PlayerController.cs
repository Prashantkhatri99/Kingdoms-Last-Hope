using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    private Rigidbody2D rb;

    [SerializeField]
    private float walkSpeed = 5f; // Movement speed

    private float xAxis;

    [Header("Ground Check Settings")]
    [SerializeField] private float jumpForce = 15f;

    [SerializeField] private Transform groundCheckPoint;

    [SerializeField] private float groundCheckY = 0.7f;

    [SerializeField] private float groundCheckX = 0.5f;

    [SerializeField] private LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        HandleJump();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal"); // Get horizontal input
    }

    void MovePlayer()
    {
        // Apply movement based on input and speed
        rb.velocity = new Vector2(xAxis * walkSpeed, rb.velocity.y);
    }

   public bool Grounded()
{
    bool isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) ||
                      Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) ||
                      Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);

    Debug.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckY, Color.red);
    Debug.DrawRay(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down * groundCheckY, Color.red);
    Debug.DrawRay(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down * groundCheckY, Color.red);

    Debug.Log($"Grounded: {isGrounded}");
    return isGrounded;
}


    void HandleJump()
    {
        // Stop upward velocity when jump button is released
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // Allow jump only if the player is grounded and the Jump button is pressed
        if (Input.GetButtonDown("Jump") && Grounded())
        {
            Debug.Log("Jump button pressed");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
