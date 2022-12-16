using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private Collider2D _collider2D;

    private void Awake(){
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(this);
            gameObject.SetActive(false);
        }
    }
}
