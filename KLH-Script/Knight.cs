using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Enemy : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.6f;
    public float attackCooldown = 1.5f; // Time between attacks
    public int attackDamage = 10; // Damage dealt per attack
    public DetectionZone attackZone;

    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Animator animator;
    Damageable damageable;
    private Transform target;
    private Damageable targetHealth;
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
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    [SerializeField]private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value); // Animator parameter: hasTarget
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool("canMove"); } // Animator parameter: canMove
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>(); // This line was missing the closing brace for the Awake method
    } // <-- Close the Awake method properly

 private void Update()
{
    if (attackZone.detectedColliders.Count > 0)
    {
        if (target == null)
        {
            Transform newTarget = attackZone.detectedColliders[0].transform;
            SetTarget(newTarget);
        }

        float distanceToTarget = Vector2.Distance(target.position, transform.position);

        if (distanceToTarget <= 1.5f)
        {
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            if (!isAttacking)
            {
                MoveTowardTarget();
            }
        }
    }
}
public Transform Target => target; // Add a getter for current target

public void ClearTarget()
{
    Debug.Log("Enemy lost target.");
    HasTarget = false;
    target = null;
    targetHealth = null;
}




    private void FixedUpdate()
    {
        if (!isAttacking && !HasTarget)
        {
            if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            {
                FlipDirection();
            }

            if (!damageable.LockVelocity)
            {
                if (CanMove)
                    rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
                else
                    rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate * Time.deltaTime), rb.velocity.y);
            }
        }
    }

    private void MoveTowardTarget()
    {
        if (target == null) return;

        float direction = target.position.x - transform.position.x;
        WalkDirection = (direction > 0) ? WalkableDirection.Right : WalkableDirection.Left;
        rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
    }

    public void SetTarget(Transform newTarget)
{
    HasTarget = true;
    target = newTarget;
    Debug.Log("Enemy has target: " + target.name);
    targetHealth = newTarget.GetComponent<Damageable>();
}

    private IEnumerator Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero; // Stop movement

        Debug.Log("Setting attack trigger!");
        animator.SetTrigger("Attack"); // Animator trigger: Attack

        Debug.Log("Enemy attacking!");

        yield return new WaitForSeconds(0.5f); // Wind-up time

        if (targetHealth != null && targetHealth.IsAlive)
        {
            targetHealth.Hit(attackDamage, Vector2.zero); // knockback is Vector2.zero
            Debug.Log("Enemy dealt " + attackDamage + " damage!");
        }

        yield return new WaitForSeconds(attackCooldown - 0.5f);

        isAttacking = false;
    }

    private void FlipDirection()
    {
        WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
