using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject SpawnPoint;
    private BoxCollider2D _collider2D;

    private void Awake(){
        _collider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.CompareTag("Player"))
        {
            SpawnPoint.transform.position = transform.position;
        }
    }
}
