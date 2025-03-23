using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    private Animator animator;
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
            _health = Mathf.Clamp(value, 0, MaxHealth); // Ensure health never goes below 0
            if (_health <= 0 && IsAlive)
            {
                Die(); // Call the Die function when health is zero
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

    [SerializeField]
    private bool isInvincible = false;
    private float timeSinceHit = 0f;
    public float invincibilityTime = 0.25f;

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

    public void Hit(int damage)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
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
}
