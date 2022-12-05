using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Button : MonoBehaviour
{
    private bool pushed;
    private BoxCollider2D _collider2D;

    private void Awake(){
        _collider2D = GetComponent<BoxCollider2D>();
        pushed = false;
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (!pushed && col.gameObject.CompareTag("Player"))
        {
            pushed = true;
            transform.Find("Button_Unpressed").gameObject.SetActive(false);
            transform.Find("Door").gameObject.SetActive(false);
        }
    }
}
