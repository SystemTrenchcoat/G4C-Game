using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroySound : MonoBehaviour
{
    public AudioClip soundEffect; // Reference to the AudioClip

    private void OnDestroy()
    {
        if (soundEffect != null)
        {
            // Play the sound at the object's position
            AudioSource.PlayClipAtPoint(soundEffect, transform.position);
        }
    }
}
