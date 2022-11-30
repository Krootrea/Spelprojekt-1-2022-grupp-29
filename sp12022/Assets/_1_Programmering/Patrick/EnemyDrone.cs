using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class EnemyDrone : MonoBehaviour
{
    // public PatrolPath path;
    // internal PatrolPath.Mover mover;
    public Vector3 patrol;
    
    private EdgeCollider2D _collider2D;
    private SpriteRenderer _spriteRenderer;
    
    private Vector3 start, target;
    private bool movingRight, seeingPlayer;
    private List<Collider2D> collisions;
    private GameObject player;

    private LineRenderer _lineRenderer;
    
    private void Awake(){
        _lineRenderer = new LineRenderer();
        _collider2D = GetComponent<EdgeCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        seeingPlayer = false;
        start = transform.position;
        target = transform.position + patrol;
        movingRight = transform.position.x<target.x;

        collisions = new List<Collider2D>();
    }

    private void Update(){
        Patrol();
    }

    private void LateUpdate(){
        LookForPlayer();
    }

    private void Patrol(){
        bool startDirMode = movingRight;
        if (movingRight && !seeingPlayer)
        {
            var dir = target - transform.position;
            dir = dir.normalized;
            transform.position += dir * Time.deltaTime;
            movingRight = dir.x>0;
        }
        else if (!seeingPlayer)
        {
            var dir = start - transform.position;
            dir = dir.normalized;
            transform.position += dir * Time.deltaTime;
            movingRight = dir.x>0;
        }
        else if (seeingPlayer)
        {
            Vector3 dir = player.transform.position-transform.position;
            Debug.DrawRay(transform.position,dir, Color.red,0,true);
        }
        if (startDirMode!=movingRight) {
            gameObject.transform.Rotate(new Vector3(0,180,0));
        }
    }

    private void OnTriggerEnter2D(Collider2D col){
        collisions.Add(col);
        if (col.CompareTag("Player"))
        {
            seeingPlayer = true;
            player = col.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        collisions.Remove(other);
        if (other.CompareTag("Player"))
            seeingPlayer = false;
    }


    private void LookForPlayer(){
        _spriteRenderer.color = seeingPlayer ? Color.red : Color.white;
    }
}