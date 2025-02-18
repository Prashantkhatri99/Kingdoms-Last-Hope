using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))] 
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f; // Normal walking speed
    public float runSpeed = 8f;  // Running speed
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    private Vector2 moveInput;   // Stores player movement input
    TouchingDirections touchingDirections;

    // Property to determine current movement speed
   public float CurrentMoveSpeed 
{
    get
    {
        if (IsMoving && !touchingDirections.IsOnWall)
        {
            if (touchingDirections.IsGrounded)
            {
                if (IsRunning)
                    return runSpeed;
                else
                    return walkSpeed;
            }
            // Air Move
            return airWalkSpeed;
        }
        // Idle speed is 0
        return 0;
    }
}


    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            animator.SetBool("isMoving", value); // Update animator parameter
        }
    }

    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning
    {
        get { return _isRunning; }
        private set
        {
            _isRunning = value;
            animator.SetBool("isRunning", value); // Update animator parameter
        }
    }

    // Player direction variables
    public bool _isFacingRight = true;

    public bool IsFacingRight 
    { 
        get { return _isFacingRight; } 
        private set
        {
            if (_isFacingRight != value) // Check if the direction has changed
            {
                _isFacingRight = value; // Update the facing direction
                
                // 🔧 FIX: Ensure scale doesn't go to zero or cause rendering issues
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * (_isFacingRight ? 1 : -1);
                transform.localScale = newScale;
            }
        } 
    }

    private Rigidbody2D rb;
    private Animator animator;
  
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Get Rigidbody2D component
        animator = GetComponent<Animator>(); // Get Animator component
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate()
    {
        // Move the player using the calculated movement speed
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // Read movement input
        IsMoving = moveInput != Vector2.zero; // Check if the player is moving
        SetFacingDirection(moveInput); // Update the facing direction
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            // Face right
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            // Face left
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true; // Enable running
        }
        else if (context.canceled)
        {
            IsRunning = false; // Disable running
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded) 
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse); 
        }
    }
}
