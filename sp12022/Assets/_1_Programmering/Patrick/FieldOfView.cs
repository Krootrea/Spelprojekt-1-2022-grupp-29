using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Collider2D fieldOfView;
    private List<Collider2D> collisions;
    private bool playerJumpedWithinCollision, rayCast;
    private float jumpCountDownTimer;
    private GameObject player, playerPos;
    public LayerMask level;

    [HideInInspector]
    public bool SeeingPlayer, BetweenPrefAndEnemy, Stop;
    public Vector3 PlayerPosition
    {
        get {
            if (player.IsUnityNull())
            {
                return Vector3.zero;
            }
            return player.transform.position;
        }
    }

    // Start is called before the first frame update
    void Start(){
        SeeingPlayer = false;
        fieldOfView = GetComponent<Collider2D>();
        playerPos = transform.Find("playerPos").gameObject;
        collisions = new List<Collider2D>();
    }

    private void Update(){
        Timer();
        CheckIfPlayerHiddenBehindObstacle();
        CheckIfPlayerBetweenPreferredPositionAndEnemy();
        Stop = BetweenPrefAndEnemy && SeeingPlayer;
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

    private void Timer(){
        if (playerJumpedWithinCollision)
        {
            jumpCountDownTimer -= Time.deltaTime;
            if (jumpCountDownTimer<=0.0f)
            {
                playerJumpedWithinCollision = false;
                if (!collisions.Contains(player.GetComponent<Collider2D>()))
                {
                    player = null;
                    rayCast = false;
                    collisions.Clear();
                }
            }
        }
    }
    
    private void CheckIfPlayerHiddenBehindObstacle(){
        if (!player.IsUnityNull())
        {
            Vector3 dir = player.transform.position - transform.position;
            float dstEnemyPlayer = Vector2.Distance(player.transform.position, transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dstEnemyPlayer, level);
            SeeingPlayer = !(hit && hit.transform.gameObject.layer != level);
        }
        else
        {
            SeeingPlayer = false;
        }
    } // Cast ray at player to see if player visible.

    private void OnTriggerEnter2D(Collider2D col) // We want to cast ray and look for player.
    {
        collisions.Add(col);
        bool colIsPlayer = col.CompareTag("Player");
        rayCast = colIsPlayer || rayCast;
        player = colIsPlayer ? col.gameObject : player;
    } 

    private void OnTriggerExit2D(Collider2D other) // We DON'T want to cast ray and look for player.
    {
        collisions.Remove(other);
        bool otherIsPlayer = other.CompareTag("Player");
        playerJumpedWithinCollision = true;
        jumpCountDownTimer = 1.5f;
    } 
}
