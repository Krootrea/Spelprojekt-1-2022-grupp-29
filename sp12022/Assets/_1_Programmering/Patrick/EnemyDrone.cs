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
    public LayerMask level;
    
    private EdgeCollider2D _collider2D;
    private SpriteRenderer _spriteRenderer;
    
    private Vector3 start, target;
    private bool movingRight, playerWithinLineOfSight, playerHidden;
    private List<Collider2D> collisions;
    private GameObject player;

    private LineRenderer _lineRenderer;
    
    private void Awake(){
        _lineRenderer = new LineRenderer();
        _collider2D = GetComponent<EdgeCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        playerWithinLineOfSight = false;
        start = transform.position;
        target = transform.position + patrol;
        movingRight = transform.position.x<target.x;

        collisions = new List<Collider2D>();
    }

    private void Update(){
        LookForPlayer();
    }
    
    private void LateUpdate(){
        Patrol();
    }
    
    private void Patrol()
    {
        bool startDirMode = movingRight;
        if (movingRight && !playerWithinLineOfSight)
        {
            var dir = target - transform.position;
            dir = dir.normalized;
            transform.position += dir * Time.deltaTime;
            movingRight = dir.x>0;
        }
        else if (!playerWithinLineOfSight)
        {
            var dir = start - transform.position;
            dir = dir.normalized;
            transform.position += dir * Time.deltaTime;
            movingRight = dir.x>0;
        }
        else if (playerWithinLineOfSight) {
            Vector3 dir = player.transform.position - transform.position;
            float dstEnemyPlayer = Vector2.Distance(player.transform.position, transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dstEnemyPlayer, level);
            if (hit && hit.transform.gameObject.layer != level)
                playerHidden = true;
            else
                playerHidden = false;
        }
        if (startDirMode!=movingRight) {
            gameObject.transform.Rotate(new Vector3(0,180,0));
        }
    }

    private void OnTriggerEnter2D(Collider2D col){
        collisions.Add(col);
        if (col.CompareTag("Player"))
        {
            playerWithinLineOfSight = true;
            player = col.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        collisions.Remove(other);
        if (other.CompareTag("Player"))
        {
            playerWithinLineOfSight = false;
            player = null;
        }

}


    private void LookForPlayer(){
        Color lineOfSightColor =playerWithinLineOfSight ? Color.red : Color.white;
        _spriteRenderer.color = playerWithinLineOfSight ? Color.red : Color.white;
        _spriteRenderer.color = playerHidden ? Color.magenta : lineOfSightColor;
    }
}