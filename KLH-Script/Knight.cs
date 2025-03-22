using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Enemy : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.6f;
    public float attackCooldown = 1.5f; // Time between attacks
    public DetectionZone attackZone;
    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Animator animator;
    private Transform target; 
    private bool isAttacking = false;

    public enum WalkableDirection { Right, Left }
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                // Flip the character's direction
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attackZone.detectedColliders.Count > 0)
        {
            if (!HasTarget)
            {
                HasTarget = true;
                target = attackZone.detectedColliders[0].transform; // Assign target
            }

            if (!isAttacking)
            {
                MoveTowardTarget(); // Move toward player
            }

            if (!isAttacking)
            {
                StartCoroutine(Attack()); // Start attack cycle
            }
        }
        else
        {
            HasTarget = false;
            target = null;
        }
    }

    private void FixedUpdate()
    {
        if (!isAttacking && !HasTarget)
        {
            if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            {
                FlipDirection();
            }

            if (CanMove)
                rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate * Time.deltaTime), rb.velocity.y);
        }
    }

    private void MoveTowardTarget()
    {
        if (target == null) return;

        float direction = target.position.x - transform.position.x;
        WalkDirection = (direction > 0) ? WalkableDirection.Right : WalkableDirection.Left;
        rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero; // Stop movement
        animator.SetTrigger(AnimationStrings.attackTrigger);
        Debug.Log("Enemy attacking!");

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
    }

    private void FlipDirection()
    {
        WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
    }
}