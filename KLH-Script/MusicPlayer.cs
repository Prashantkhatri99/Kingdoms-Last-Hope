using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource introSource;
    public AudioSource loopSource;

    void Start()
    {
        // Play the intro immediately
        introSource.Play();

        // Schedule the loop to start right after the intro finishes
        loopSource.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);
    }
}
