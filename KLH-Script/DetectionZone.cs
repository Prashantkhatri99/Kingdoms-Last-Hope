using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!detectedColliders.Contains(other))
            {
                detectedColliders.Add(other);
                Debug.Log("Player entered detection zone.");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (detectedColliders.Contains(other))
        {
            detectedColliders.Remove(other);
            Debug.Log("Player exited detection zone.");
        }
    }

    // Optional: Method to get the first detected player
    public Collider2D GetDetectedPlayer()
    {
        if (detectedColliders.Count > 0)
        {
            return detectedColliders[0]; // Return first player in the list
        }
        return null;
    }
}
