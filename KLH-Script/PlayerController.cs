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

   public float CurrentMoveSpeed
{
    get
    {
        if (CanMove)
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
                else
                {
                    // Air Move
                    return airWalkSpeed;
                }
            }
            // Idle speed is 0
            return 0;
        }
        else
        {
            return 0;
        }
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

    public bool CanMove
{
    get 
    { 
        return animator.GetBool(AnimationStrings.canMove); 
    }
}


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        if (animator != null)
            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;
        SetFacingDirection(moveInput);
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
        if (context.started && touchingDirections.IsGrounded && CanMove) 
        {
            if (animator != null)
                animator.SetTrigger(AnimationStrings.jumpTrigger); // Updated
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger); // Updated
        }
    }
}
