using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
     public Inventory inventory; 
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    private Vector2 moveInput;

    TouchingDirections touchingDirections;
    Damageable damageable;
    private Rigidbody2D rb;
    private Animator animator;

   

    public bool LockVelocity
    {
        get => animator.GetBool(AnimationStrings.lockVelocity);
        set => animator.SetBool(AnimationStrings.lockVelocity, value);
    }

    public bool isOnPlatform;
    public Rigidbody2D platformRb;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    return touchingDirections.IsGrounded ? (IsRunning ? runSpeed : walkSpeed) : airWalkSpeed;
                }
                return 0;
            }
            return 0;
        }
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get => _isMoving;
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
        get => _isRunning;
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
        get => _isFacingRight;
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

    public bool CanMove => animator.GetBool(AnimationStrings.canMove);
    public bool IsAlive => animator.GetBool(AnimationStrings.isAlive);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();

        inventory = new Inventory(10); // ✅ Initialize Inventory with 10 slots
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            float targetSpeed = CurrentMoveSpeed * moveInput.x;

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
        if (context.started && touchingDirections.IsGrounded && CanMove)
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

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (animator != null)
        {
            animator.SetBool(AnimationStrings.isAlive, false);
            // animator.SetTrigger(AnimationStrings.deathTrigger);
        }

        rb.velocity = Vector2.zero;
        this.enabled = false;
    }
}
