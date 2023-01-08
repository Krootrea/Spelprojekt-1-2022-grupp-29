using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Collider2D fieldOfView;
    private List<Collider2D> collisions;
    private bool playerJumpedWithinCollision, rayCast;
    private float jumpCountDownTimer;
    private GameObject player, playerPos;
    private Vector3 lastKnownPlayerPosition;
    
    public LayerMask level;

    [HideInInspector]
    public bool SeeingPlayerRayCast, BetweenPrefAndEnemy, Stop;

    public PlayerController playerController;
    public Vector3 PlayerPosition
    {
        get {
            if (player.IsUnityNull())
            {
                return lastKnownPlayerPosition;
            }
            return player.transform.position;
        }
    }

    // Start is called before the first frame update
    void Start(){
        SeeingPlayerRayCast = false;
        fieldOfView = GetComponent<Collider2D>();
        playerPos = transform.Find("playerPos").gameObject;
        lastKnownPlayerPosition = Vector3.zero;
        collisions = new List<Collider2D>();
    }

    private void Update(){
        Timer();
        CheckIfPlayerHiddenBehindObstacle();
        CheckIfPlayerBetweenPreferredPositionAndEnemy();
        Stop = BetweenPrefAndEnemy && SeeingPlayerRayCast;
    }

    private void CheckIfPlayerBetweenPreferredPositionAndEnemy(){
        BetweenPrefAndEnemy = false;
        if (!player.IsUnityNull())
        {
            BetweenPrefAndEnemy =
                (player.transform.position.x > transform.position.x &&
                 player.transform.position.x < playerPos.transform.position.x) ||
                (player.transform.position.x < transform.position.x &&
                 player.transform.position.x > playerPos.transform.position.x);
        }
    }

    public bool PlayerHiding(){
        if (!playerController.IsUnityNull())
        {
            return playerController.Hidden;
        }
        return true;
    }

    private void Timer(){
        if (playerJumpedWithinCollision)
        {
            jumpCountDownTimer -= Time.deltaTime;
            if (jumpCountDownTimer<=0.0f)
            {
                playerJumpedWithinCollision = false;
                if (!CollisionsContainsPlayer())
                {
                    player = null;
                    rayCast = false;
                    collisions.Clear();
                }
            }
        }
    }

    private bool CollisionsContainsPlayer(){
        if (collisions.Count>0)
        {
            foreach (Collider2D col2D in collisions)
            {
                if (col2D.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void CheckIfPlayerHiddenBehindObstacle(){
        if (!player.IsUnityNull())
        {
            Vector3 dir = player.transform.position - transform.position;
            float dstEnemyPlayer = Vector2.Distance(player.transform.position, transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dstEnemyPlayer, level);
            SeeingPlayerRayCast = !(hit && hit.transform.gameObject.layer != level)/* && !playerController.Hidden*/;
        }
        else
        {
            SeeingPlayerRayCast = false;
        }
    } // Cast ray at player to see if player visible.

    private void OnTriggerEnter2D(Collider2D col) // We want to cast ray and look for player.
    {
        collisions.Add(col);
        bool colIsPlayer = col.CompareTag("Player");
        rayCast = colIsPlayer || rayCast;
        player = colIsPlayer ? col.gameObject : player;
        if (colIsPlayer)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerStay2D(Collider2D other){
        bool otherIsPlayer = other.CompareTag("Player");
        if (otherIsPlayer)
        {
            
        }
    }

    private void OnTriggerExit2D(Collider2D other) // We DON'T want to cast ray and look for player.
    {
        collisions.Remove(other);
        bool otherIsPlayer = other.CompareTag("Player");
        playerJumpedWithinCollision = true;
        jumpCountDownTimer = 1.5f;
        if (otherIsPlayer)
        {
            lastKnownPlayerPosition = playerController.transform.position;
            playerController = null;
        }
    } 
}
