using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornSound : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
