using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth; // Make sure this is assigned
    [SerializeField] private Image totalhealthBar; // Make sure this is assigned
    [SerializeField] private Image currenthealthBar; // Make sure this is assigned

    private void Start()
    {
        if (playerHealth == null || totalhealthBar == null || currenthealthBar == null)
        {
            Debug.LogError("Missing references in the Healthbar script.");
            return;
        }

        // Initialize or update health bar here
        currenthealthBar.fillAmount = playerHealth.currentHealth / 10f;  // Ensure currentHealth is accessible
    }
}
