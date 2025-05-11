using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type;  // Reference the CollectableType enum

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the PlayerController component from the collision object
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController)
        {
            // Add the item to the player's inventory
            playerController.inventory.Add(type);

            // Destroy the collectable object after it's picked up
            Destroy(this.gameObject);
        }
    }
}

