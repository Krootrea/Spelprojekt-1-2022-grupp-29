using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitch : MonoBehaviour
{
    public AudioClip sound1;
    public AudioClip sound2;
    public string tagName;

    private bool playSound1 = true;
    
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playSound1)
        {
            audioSource.clip = sound1;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = sound2;
            audioSource.Play();
        }
        
        playSound1 = !playSound1;
    }
}
