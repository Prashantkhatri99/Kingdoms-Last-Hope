using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    [SerializeField] private bool goNextLevel;
    [SerializeField] private string levelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (SceneController.instance == null)
            {
                Debug.LogError("SceneController instance not found in the scene!");
                return;
            }

            if (goNextLevel)
            {
                SceneController.instance.NextLevel();
            }
            else
            {
                if (string.IsNullOrEmpty(levelName))
                {
                    Debug.LogError("FinishPoint: Level name is empty!");
                    return;
                }

                SceneController.instance.LoadScene(levelName);
            }
        }
    }
}
