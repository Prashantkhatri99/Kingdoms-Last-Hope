using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    Animator animator;
    private PlayerController playerController; // Reference to PlayerController

    [SerializeField]
    private int _maxHealth = 100;

    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                IsAlive = false;
                Die(); // Call Die when health drops to 0
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;
    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " + value);
        }
    }

    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    [SerializeField]
    private bool isInvincible = false;

    public bool IsHit // Keep this single definition
    {
        get
        {
            return animator.GetBool(AnimationStrings.isHit);
        }
        private set
        {
            animator.SetBool(AnimationStrings.isHit, value); // ← just set it, don't return
        }
    }

    private float timeSinceHit = 0f;
    public float invincibilityTime = 0.25f;

    [SerializeField]
    private int healthRestore = 20;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>(); // Get PlayerController component
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
    }

    private void Die()
    {
        IsAlive = false;
        if (playerController != null)
        {
            playerController.SendMessage("Die"); // Call Die() in PlayerController
        }
        else
        {
            Debug.LogWarning("No PlayerController found on this GameObject.");
        }
    }

    public void Heal()
    {
        if (IsAlive)
        {
            Health += healthRestore;
            CharacterEvents.characterHealed?.Invoke(gameObject, healthRestore);
        }
    }

    // Added the Hit method to handle damage and knockback
    public bool Hit(int damage, Vector2 knockback)
    {
        if (!IsAlive || isInvincible) return false;

        Health -= damage;
        isInvincible = true;

        // Handle knockback (for example, if you want to move the object after being hit)
        // You can modify this part based on your knockback logic.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }

        animator.SetTrigger(AnimationStrings.hitTrigger);
        LockVelocity = true; // Lock velocity when hit

        // Invoke event if needed
        damageableHit?.Invoke(damage, knockback);

        return true;
    }
}
