using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnceOnTrigger : MonoBehaviour
{
    public AudioClip sound;
    public bool playOnce = true;

    private AudioSource audioSource;
    private bool hasTriggered = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !hasTriggered)
        {
            audioSource.PlayOneShot(sound);
            hasTriggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && playOnce)
        {
            hasTriggered = false;
        }
    }
}
