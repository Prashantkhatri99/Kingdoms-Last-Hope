using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableType type; // Reference the CollectableType enum

    [SerializeField]
    private Sprite icon; // ✅ Drag your collectable icon here in the Inspector

    public Sprite Icon => icon; // Optional: expose it read-only for other scripts

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

       if (playerController)
{
    playerController.inventory.Add(type);
    Debug.Log("Collected: " + type); // ✅ Check the console
    Destroy(this.gameObject);
}

    }
}
