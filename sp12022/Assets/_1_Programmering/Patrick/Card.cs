using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Card : MonoBehaviour
{
    private GameObject card, audio;
    private Collider2D _collider2D;
    private bool audioHasBeenPlayed;

    private void Awake(){
        card = transform.GetChild(0).gameObject;
        audio = transform.GetChild(1).gameObject;
        audioHasBeenPlayed = false;
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.CompareTag("Player") && !audioHasBeenPlayed)
        {
            audio.GetComponent<AudioSource>().Play();
            audioHasBeenPlayed = true;
            card.SetActive(false);
        }
    }
}
