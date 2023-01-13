using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Checkpoint : MonoBehaviour
{
    public GameObject SpawnPoint;
    private CheckpointBox Box;
    private BoxCollider2D _collider2D;
    private bool activeCheckpoint;
    private ResetObjectsUponRespawn resetObjects;
    public bool Active => activeCheckpoint;

    private void Awake(){
        Box = transform.Find("checkpointBox").GetComponent<CheckpointBox>();
        _collider2D = GetComponent<BoxCollider2D>();
        resetObjects = GetComponent<ResetObjectsUponRespawn>();
        activeCheckpoint = false;
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.CompareTag("Player"))
        {
            SpawnPoint.transform.position = transform.position;
            activeCheckpoint = true;
            Box.OpenBox(); 
        }
    }

    public void ResetObjects(){
        resetObjects.ResetObjects();
    }
    
}
