using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class HideBehind : MonoBehaviour
{
    private BoxCollider2D _collider2D;

    private void Awake(){
        _collider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.CompareTag("Player"))
        {
            PlayerController player = col.GetComponent<PlayerController>();
            player.Hidden = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.Hidden = false;
        }
    }
}
