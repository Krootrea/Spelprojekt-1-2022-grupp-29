using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Platformer.Gameplay;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using static Platformer.Core.Simulation;

public class EnemyDrone : MonoBehaviour
{
    // public enum EnemyState
    // {
    //     Patrolling,
    //     FollowingPlayer,
    //     LookingForPlayer
    // }
    // public PatrolPath path;
    // internal PatrolPath.Mover mover;
    public Vector3 patrol;
    public LayerMask level;
    public float Speed;
    
    private EdgeCollider2D _collider2D;
    private SpriteRenderer _spriteRenderer;
    private Light2D _light2D;
    
    private Vector3 start, target, direction;
    private bool movingTowardsTarget, rayCast,killedPlayer, seeingPlayer;
    private List<Collider2D> collisions;
    private GameObject player, playerPos;
    private float lostSightOfPlayerCountDown, playerDeathCountDown, justKilledPlayerCountDown;

    public float LooseSightCountDown, DeathCountDown;

    // private EnemyState enemyState;
    private EnemyStateHandler state;
    
    private LineRenderer _lineRenderer;
    
    private void Awake(){
        // enemyState = EnemyState.Patrolling;
        state = new EnemyStateHandler(LooseSightCountDown);
        _lineRenderer = new LineRenderer();
        _collider2D = GetComponent<EdgeCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        start = transform.position;
        target = transform.position + patrol;
        playerPos = transform.Find("playerPos").gameObject;

        collisions = new List<Collider2D>();
        _light2D = transform.Find("BeamLight").GetComponent<Light2D>();
        movingTowardsTarget = true;
        direction = target;
    }

    private void Update(){
        Patrol();
        Move();
    }

    private void Move(){
        
        bool playerBetweenEnemyAndPlayerPos = false;
        if (!player.IsUnityNull())
        {
            playerBetweenEnemyAndPlayerPos =
                (player.transform.position.x > transform.position.x &&
                 player.transform.position.x < playerPos.transform.position.x) ||
                (player.transform.position.x < transform.position.x &&
                 player.transform.position.x > playerPos.transform.position.x);
        }
        if (!playerBetweenEnemyAndPlayerPos || !seeingPlayer)
            transform.position = Vector3.MoveTowards(transform.position, direction, Speed * Time.deltaTime);
        
        RotateToCurrentDirection(playerBetweenEnemyAndPlayerPos);
    }

    private void RotateToCurrentDirection(bool playerBetweenEnemyAndPlayerPos){
        Vector3 scale = transform.localScale;
        if (direction.x > transform.position.x && !playerBetweenEnemyAndPlayerPos)
            scale.Set(1, 1, 1);
        else if (direction.x < transform.position.x && !playerBetweenEnemyAndPlayerPos)
            scale.Set(-1, 1, 1);
        
        transform.localScale = scale;
    }

    private void Patrol(){
        CheckIfPlayerHiddenBehindObstacle();
        state.CustomUpdate(seeingPlayer);
        switch (state.CurrentState)
        {
            case EnemyStateHandler.EnemyState.Patrolling:
            {
                _spriteRenderer.color = Color.white;
                _light2D.color = _spriteRenderer.color;
                if (transform.position==target)
                    movingTowardsTarget = false;
                else if (transform.position==start)
                    movingTowardsTarget = true;
                direction = movingTowardsTarget ? target : start;
                return;
            }
            case EnemyStateHandler.EnemyState.LookingForPlayer:
            {
                _spriteRenderer.color = Color.magenta;
                _light2D.color = _spriteRenderer.color;
                return;
            }
            case EnemyStateHandler.EnemyState.ChasingPlayer:
            {
                Vector3 playerPosition = new Vector3(transform.position.x + (player.transform.position.x-playerPos.transform.position.x), transform.position.y);
                direction = playerPosition;
                _spriteRenderer.color = Color.red;
                _light2D.color = _spriteRenderer.color;
                return;
            }
        }
        
        // if (enemyState == EnemyState.Patrolling) 
        // {
        //     float oldXDir = direction.x;
        //     if (transform.position==target)
        //         movingTowardsTarget = false;
        //     else if (transform.position==start)
        //         movingTowardsTarget = true;
        //     direction = movingTowardsTarget ? target : start;
        // }
        // else {
        //     _spriteRenderer.color = enemyState == EnemyState.FollowingPlayer ? Color.red : Color.magenta;
        //     _light2D.color = _spriteRenderer.color;
        // }
        // if (enemyState == EnemyState.FollowingPlayer) {
        //     Vector3 playerPosition = new Vector3(transform.position.x + (player.transform.position.x-playerPos.transform.position.x), transform.position.y);
        //     direction = playerPosition;
        // }
    }

    private void StateUpdate(){
        // if (!player.IsUnityNull())
        // {
        //     CheckIfPlayerHiddenBehindObstacle();
        // }
        // //Handle countdowns
        // if (enemyState == EnemyState.LookingForPlayer) 
        //     lostSightOfPlayerCountDown -= Time.deltaTime;
        // if (enemyState == EnemyState.FollowingPlayer) 
        //     playerDeathCountDown -= Time.deltaTime;
        //
        // //Handle state change
        // EnemyState oldState = enemyState;
        // if (rayCast && oldState != EnemyState.Patrolling) {
        //     if (oldState == EnemyState.LookingForPlayer && lostSightOfPlayerCountDown<=0.0f) {
        //         enemyState = seeingPlayer
        //             ? EnemyState.FollowingPlayer
        //             : EnemyState.Patrolling;
        //     }
        //     else {
        //         enemyState = seeingPlayer
        //             ? EnemyState.FollowingPlayer
        //             : EnemyState.LookingForPlayer;
        //     }
        // }
        // else if (rayCast) {
        //     enemyState = seeingPlayer
        //         ? EnemyState.FollowingPlayer
        //         : EnemyState.Patrolling;
        // }
        // else if (lostSightOfPlayerCountDown <= 0.0f && enemyState == EnemyState.LookingForPlayer) {
        //     enemyState = EnemyState.Patrolling;
        //     _spriteRenderer.color = Color.white;
        //     _light2D.color = _spriteRenderer.color;
        // }
        //
        // // Set timer if appropriate
        // if (enemyState == EnemyState.LookingForPlayer && oldState != EnemyState.LookingForPlayer) 
        //     lostSightOfPlayerCountDown = LooseSightCountDown;
        // if (enemyState == EnemyState.FollowingPlayer && oldState != EnemyState.FollowingPlayer)
        //     playerDeathCountDown = DeathCountDown;
        //
        // // Kill player if death-timer ran out.
        // if (enemyState == EnemyState.FollowingPlayer && playerDeathCountDown <= 0.0f && !killedPlayer)
        // {
        //     PlayerController playerController;
        //     if (player!=null)
        //     {
        //         playerController = player.GetComponent<PlayerController>();
        //         var ev = Schedule<PlayerEnemyCollision>();
        //         ev.player = playerController;
        //         justKilledPlayerCountDown = 2f;
        //     }
        //     killedPlayer = true;
        // }
        //
        // if (justKilledPlayerCountDown > 0.0f)
        //     justKilledPlayerCountDown -= Time.deltaTime;
        // else
        //     killedPlayer = false;
    }

    private void CheckIfPlayerHiddenBehindObstacle(){
        if (!player.IsUnityNull())
        {
            Vector3 dir = player.transform.position - transform.position;
            float dstEnemyPlayer = Vector2.Distance(player.transform.position, transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dstEnemyPlayer, level);
            seeingPlayer = !(hit && hit.transform.gameObject.layer != level);
        }
        else
        {
            seeingPlayer = false;
        }
    } // Cast ray at player to see if player visible.

    private void OnTriggerEnter2D(Collider2D col){
        collisions.Add(col);
        bool colIsPlayer = col.CompareTag("Player");
        rayCast = colIsPlayer || rayCast;
        player = colIsPlayer ? col.gameObject : player;
    } // We want to cast ray and look for player.

    private void OnTriggerExit2D(Collider2D other){
        collisions.Remove(other);
        bool otherIsPlayer = other.CompareTag("Player");
        rayCast = !otherIsPlayer && rayCast;
        if (otherIsPlayer)
        {
            player = null;
        }
    } // We DON'T want to cast ray and look for player.
}