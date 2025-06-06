using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class HealthText : MonoBehaviour
{
    // Pixels per second
    public Vector3 moveSpeed = new Vector3(0, 75, 0);
    public float timeToFade = 1f;
    private RectTransform textTransform;
    private TextMeshProUGUI textMeshPro;
    private float timeElapsed = 0f;
    private Color startColor;

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        startColor = textMeshPro.color;
    }

    private void Update()
    {
        textTransform.position += moveSpeed * Time.deltaTime;
        timeElapsed += Time.deltaTime;

        if (timeElapsed < timeToFade)
        {
            float fadeAlpha = startColor.a * (1 - (timeElapsed / timeToFade));
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
