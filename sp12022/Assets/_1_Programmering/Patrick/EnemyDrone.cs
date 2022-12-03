using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using static Platformer.Core.Simulation;

public class EnemyDrone : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        FollowingPlayer,
        LookingForPlayer
    }
    // public PatrolPath path;
    // internal PatrolPath.Mover mover;
    public Vector3 patrol;
    public LayerMask level;
    
    private EdgeCollider2D _collider2D;
    private SpriteRenderer _spriteRenderer;
    private Light2D _light2D;
    
    private Vector3 start, target;
    private bool movingRight, rayCast,killedPlayer;
    private List<Collider2D> collisions;
    private GameObject player;
    private float lostSightOfPlayerCountDown, playerDeathCountDown;

    public float LooseSightCountDown, DeathCountDown;

    private EnemyState enemyState;

    private LineRenderer _lineRenderer;
    
    private void Awake(){
        enemyState = EnemyState.Patrolling;
        _lineRenderer = new LineRenderer();
        _collider2D = GetComponent<EdgeCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        start = transform.position;
        target = transform.position + patrol;
        movingRight = transform.position.x<target.x;

        collisions = new List<Collider2D>();
        _light2D = transform.Find("BeamLight").GetComponent<Light2D>();
    }

    private void Update(){ }
    
    private void LateUpdate(){
        Patrol();
    }
    
    private void Patrol()
    {
        bool rotationCheck = movingRight;
        StateUpdate();

        if (enemyState == EnemyState.Patrolling) {
            var dir = movingRight ? (target - transform.position) : (start - transform.position);
            dir = dir.normalized;
            transform.position += dir * Time.deltaTime;
            movingRight = dir.x>0;
        }
        else
        {
            _spriteRenderer.color = enemyState == EnemyState.FollowingPlayer ? Color.red : Color.magenta;
            _light2D.color = _spriteRenderer.color;
        }
        
        if (rotationCheck!=movingRight) 
            gameObject.transform.Rotate(new Vector3(0,180,0));
    }

    private void StateUpdate(){
        //Handle countdowns
        if (enemyState == EnemyState.LookingForPlayer) 
            lostSightOfPlayerCountDown -= Time.deltaTime;
        if (enemyState == EnemyState.FollowingPlayer) 
            playerDeathCountDown -= Time.deltaTime;
        
        
        //Handle state change
        EnemyState oldState = enemyState;
        if (rayCast && oldState != EnemyState.Patrolling) {
            if (oldState == EnemyState.LookingForPlayer && lostSightOfPlayerCountDown<=0.0f) {
                enemyState = CheckIfPlayerHiddenBehindObstacle()
                    ? EnemyState.FollowingPlayer
                    : EnemyState.Patrolling;
            }
            else {
                enemyState = CheckIfPlayerHiddenBehindObstacle()
                    ? EnemyState.FollowingPlayer
                    : EnemyState.LookingForPlayer;
            }
        }
        else if (rayCast) {
            enemyState = CheckIfPlayerHiddenBehindObstacle()
                ? EnemyState.FollowingPlayer
                : EnemyState.Patrolling;
        }
        else if (lostSightOfPlayerCountDown <= 0.0f) {
            enemyState = EnemyState.Patrolling;
            _spriteRenderer.color = Color.white;
            _light2D.color = _spriteRenderer.color;
        }

        // Set timer if appropriate
        if (enemyState == EnemyState.LookingForPlayer && oldState != EnemyState.LookingForPlayer) 
            lostSightOfPlayerCountDown = LooseSightCountDown;
        if (enemyState == EnemyState.FollowingPlayer && oldState != EnemyState.FollowingPlayer)
            playerDeathCountDown = DeathCountDown;
        
        // Kill player if death-timer ran out.
        if (enemyState == EnemyState.FollowingPlayer && playerDeathCountDown <= 0.0f && !killedPlayer)
        {
            PlayerController playerController;
            if (player!=null)
            {
                playerController = player.GetComponent<PlayerController>();
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = playerController;
            }
            // killedPlayer = true;
        }

    }

    private bool CheckIfPlayerHiddenBehindObstacle(){
        Vector3 dir = player.transform.position - transform.position;
        float dstEnemyPlayer = Vector2.Distance(player.transform.position, transform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dstEnemyPlayer, level);
        return !(hit && hit.transform.gameObject.layer != level) ;
    } // Cast ray at player to see if player visible.

    private void OnTriggerEnter2D(Collider2D col){
        collisions.Add(col);
        bool colIsPlayer = col.CompareTag("Player");
        rayCast = colIsPlayer || rayCast;
        player = colIsPlayer ? col.gameObject : player;
    } // Shoot raycast to see if player is visible to enemy.

    private void OnTriggerExit2D(Collider2D other){
        collisions.Remove(other);
        bool otherIsPlayer = other.CompareTag("Player");
        rayCast = !otherIsPlayer && rayCast;
        player = otherIsPlayer ? null : other.gameObject;
    } // Stop shooting raycast.
}