using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitch : MonoBehaviour
{
    // Ljudfilerna som ska spelas upp
    public AudioClip sound1;
    public AudioClip sound2;
    public string tagName;

    // En flagga som används för att hålla reda på vilket ljud som ska spelas upp
    private bool playSound1 = true;

    // En referense till AudioSource-komponenten på objektet som scriptet är kopplat till
    private AudioSource audioSource;

    private void Start()
    {
        // Hämta AudioSource-komponenten
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kontrollera vilket ljud som ska spelas upp och spela upp det
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

        // Växla flaggan för att spela upp det andra ljudet nästa gång
        playSound1 = !playSound1;
    }
}
