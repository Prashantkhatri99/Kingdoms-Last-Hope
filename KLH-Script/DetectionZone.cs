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

    private void OnTriggerEnter2D(Collider2D collision)
    {
if (collision.gameObject.CompareTag("Player")) // Replace "Player" with the correct tag
{
    Debug.Log("Added one: " + collision.gameObject.name);
    detectedColliders.Add(collision);
}

       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")){
                    detectedColliders.Remove(collision);
        }

    }
}
