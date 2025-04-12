using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // See if it can be hit
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            // Hit the target
            damageable.Hit(attackDamage);
            Debug.Log(collision.name + " hit for " + attackDamage);
        }
    }
}
