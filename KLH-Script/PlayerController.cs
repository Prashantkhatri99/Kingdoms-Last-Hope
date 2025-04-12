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
    private Rigidbody2D rb;
    private Animator animator;

    public bool isOnPlatform;  // Added variable
    public Rigidbody2D platformRb;  // Added variable

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall) // Player can move
                {
                    if (touchingDirections.IsGrounded)
                    {
                        return IsRunning ? runSpeed : walkSpeed;
                    }
                    else
                    {
                        return airWalkSpeed; // Air Move
                    }
                }
                return 0; // Idle speed is 0
            }
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
            if (animator != null) 
                animator.SetBool(AnimationStrings.isMoving, value);
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
            if (animator != null) 
                animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight 
    { 
        get { return _isFacingRight; } 
        private set
        {
            if (_isFacingRight != value) 
            {
                _isFacingRight = value; 
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * (_isFacingRight ? 1 : -1);
                transform.localScale = newScale;
            }
        } 
    }

    public bool CanMove {get
    {
        return animator.GetBool(AnimationStrings.canMove);

    }} 
    public bool IsAlive{get
    {
        return animator.GetBool(AnimationStrings.isAlive);
    }} 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate()
    {
        float targetSpeed = CurrentMoveSpeed * moveInput.x;

        // Apply movement based on whether the player is on a platform
        if (isOnPlatform && platformRb != null)  
        {
            rb.velocity = new Vector2(targetSpeed + platformRb.velocity.x, rb.velocity.y); 
        }
        else
        {
            rb.velocity = new Vector2(targetSpeed, rb.velocity.y);
        }

        if (animator != null)
            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
            IsFacingRight = true;
        else if (moveInput.x < 0 && IsFacingRight)
            IsFacingRight = false;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started) IsRunning = true;
        else if (context.canceled) IsRunning = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove) //Player Can touch direction and grounded
        {
            if (animator != null)
                animator.SetTrigger(AnimationStrings.jumpTrigger); 
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger); 
        }
    }

    // New Code: Detect Obstacle Collision & Handle Death
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle")) // Check if player touches an obstacle
        {
            Die();
        }
    }

    private void Die()
    {
        if (animator != null)
        {
            animator.SetBool(AnimationStrings.isAlive, false); // Set death animation
            animator.SetTrigger(AnimationStrings.deathTrigger);
        }

        rb.velocity = Vector2.zero; // Stop player movement
        this.enabled = false; // Disable the PlayerController script
    }
}
