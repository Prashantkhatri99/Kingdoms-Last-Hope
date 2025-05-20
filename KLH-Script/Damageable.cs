using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<int, int> healthChanged;

    private Animator animator;
    private PlayerController playerController; // Reference to PlayerController
    private Rigidbody2D rb; // Cache Rigidbody2D for knockback

    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;
    public bool IsAlive
    {
        get => _isAlive;
        set
        {
            _isAlive = value;
            if (animator != null)
                animator.SetBool(AnimationStrings.isAlive, value);
        }
    }

    public bool LockVelocity
    {
        get => animator != null && animator.GetBool(AnimationStrings.lockVelocity);
        set
        {
            if (animator != null)
                animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    [SerializeField]
    private bool isInvincible = false;

    public bool IsHit
    {
        get => animator != null && animator.GetBool(AnimationStrings.isHit);
        private set
        {
            if (animator != null)
                animator.SetBool(AnimationStrings.isHit, value);
        }
    }

    private float timeSinceHit = 0f;
    public float invincibilityTime = 0.25f;

    [SerializeField]
    private int healthRestore = 20;

    private void Awake()
    {
        TryGetComponent(out animator);
        TryGetComponent(out playerController);
        TryGetComponent(out rb);
    }

    private void Update()
    {
        if (isInvincible)
        {
            timeSinceHit += Time.deltaTime;
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0f;
            }
        }
    }

    private void Die()
    {
        if (!IsAlive) return;

        IsAlive = false;

        if (playerController != null)
        {
            playerController.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no PlayerController but tried to Die.");
        }
    }

    public void Heal()
    {
        if (!IsAlive) return;

        Health = Mathf.Min(Health + healthRestore, MaxHealth); // Don't overheal
        CharacterEvents.characterHealed?.Invoke(gameObject, healthRestore);
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (!IsAlive || isInvincible) return false;

        Health -= damage;
        isInvincible = true;
        timeSinceHit = 0f; // Reset timer

        IsHit = true;

        if (rb != null)
        {
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }

        if (animator != null)
        {
            animator.SetTrigger(AnimationStrings.hitTrigger);
        }

        LockVelocity = true;

        damageableHit?.Invoke(damage, knockback);
        CharacterEvents.characterDamaged.Invoke(gameObject, damage);

        return true;
    }
    
    public bool Heal(int healthRestore)
{ 
    if (IsAlive && Health < MaxHealth)
    {
        int maxHeal = Mathf.Max(MaxHealth - Health, 0);
        int actualHeal = Mathf.Min(maxHeal, healthRestore);
        Health += actualHeal;
        CharacterEvents.characterHealed(gameObject, actualHeal);
        return true;
    }
    return false;
}

}
