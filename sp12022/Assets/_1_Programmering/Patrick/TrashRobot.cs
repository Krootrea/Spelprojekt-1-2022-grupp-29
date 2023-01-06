using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.Tilemaps;
using UnityEngine;
using static Platformer.Core.Simulation;

public class TrashRobot : Enemy
{
    // public enum TM{MovingToLocation, LookingForPlayer,Idle, ChasingPlayer, Attacking}

    // private TM mode;
    private bool rayCast, shutDown, countingDown, lightsBlinkState, turnLeft, alerted;
    private GameObject sideLight, topLight, playerPos, Laser;
    private Vector3 lastKnownPlayerLocation, originalPosition;
    private Animator animator;

    private Laser laser;
    // private float ShutdownCountDown;
    private float shutdownTimer, blinkTimer, lookTimer;
    public float startupTimer;
    
    // Start is called before the first frame update
    void Start(){
        laser = GetComponent<Laser>();
        Laser = transform.Find("Laser").gameObject;
        Laser.SetActive(false);
        lookTimer = 0.0f;
        countingDown = false;
        // mode = TM.Idle;
        stateHandler = GetComponent<EnemyStateHandler>();
        stateHandler.LookingTime = LookingTime;
        state = stateHandler.CurrentState;
        _collider2D = GetComponent<EdgeCollider2D>();
        sideLight = transform.Find("sideLight").gameObject;
        On = false;
        SetLights(false);
        originalPosition = transform.position;
        shutdownTimer = 0f;
        lightsBlinkState = true;
        playerPos = transform.Find("playerPos").gameObject;
        animator = GetComponent<Animator>();
    }

    private void SetLights(bool onOff){
        sideLight.SetActive(onOff);
    }

    // Update is called once per frame
    void Update(){
        if (shutDown)
        {
            On = false;
            SetLights(false);
            shutDown = false;
        }

        if (On)
        {
            DecideWhereToGo();
            AlertLights();
        }
    }

    private void HandleTimers(){
        // Timer
        if (Arrived() && !countingDown) 
        {
            shutdownTimer = LookingTime;
            countingDown = true;
        }

        if (shutdownTimer > 0.0f && countingDown)
            shutdownTimer -= Time.deltaTime;

        if (countingDown && shutdownTimer<=0.0f && state == EnemyStateHandler.EnemyState.LookingForPlayer)
        {
            shutDown = true;
            countingDown = false;
        }
    }

    private void LookAround(){
        if (lookTimer>2f)
        {
            lookTimer = 0.0f;
            Vector3 scale = transform.localScale;
            if (turnLeft)
                scale.Set(-1, 1, 1);
            else
                scale.Set(1, 1, 1);
            turnLeft = !turnLeft;
            transform.localScale = scale;
        }

        lookTimer += Time.deltaTime;
    }

    private void DecideWhereToGo()
    {
        switch (state)
        {
            case EnemyStateHandler.EnemyState.Normal:
            {
                alerted = false;
                // Gå till direction
                if (startupTimer<=0.0f)
                {
                    if (fov.SeeingPlayer)
                    {
                        state = EnemyStateHandler.EnemyState.ChasingPlayer;
                        direction = fov.PlayerPosition;
                    }
                    else if (Arrived())
                    {
                        state = EnemyStateHandler.EnemyState.LookingForPlayer;
                    }

                    animator.SetBool("Running", true);
                    Move();
                }
                else
                {
                    startupTimer -= Time.deltaTime;
                }
                break;
            }
            case EnemyStateHandler.EnemyState.LookingForPlayer:
            {
                // Framme på direction, leta efter spelare och räkna ner.
                animator.SetBool("Running", false);
                if (alerted)
                {
                    state = EnemyStateHandler.EnemyState.Normal;
                }
                else if (fov.SeeingPlayer)
                {
                    state = EnemyStateHandler.EnemyState.ChasingPlayer;
                    direction = fov.PlayerPosition;
                }
                else
                {
                    LookAround();
                    HandleTimers();
                }
                break;
            }
            case EnemyStateHandler.EnemyState.ChasingPlayer:
            {
                // Ser spelare, om framme: attackera. Annars gå till direction(spelarens position).

                if (!fov.SeeingPlayer && Arrived()){
                    state = EnemyStateHandler.EnemyState.LookingForPlayer;
                }
                else if (Arrived())
                {
                    AttackPlayer();
                }

                if (!fov.SeeingPlayer)
                {
                    if (laser.CurrentlyFiring())
                    {
                        laser.StopFiring();
                        Laser.SetActive(false);
                    }
                }
                Move();
                break;
            }
        }
        lastKnownPlayerLocation = fov.SeeingPlayer ? fov.PlayerPosition : lastKnownPlayerLocation;
    }

    private void Move(){
        Vector3 moveToDir;
        if (direction.x-transform.position.x<0.1f && direction.x-transform.position.x>-0.1f) {
            moveToDir = new Vector3(direction.x, originalPosition.y, originalPosition.z);
        }
        else {
            moveToDir = new Vector3(direction.x-(playerPos.transform.position.x-transform.position.x), originalPosition.y, originalPosition.z);    
        }
        
        transform.position = Vector3.MoveTowards(transform.position, moveToDir, Speed * Time.deltaTime);
        RotateToCurrentDirection();
    }

    private bool Arrived(){
        Vector3 moveToDir;
        if (direction.x-transform.position.x<0.1f && direction.x-transform.position.x>-0.1f) {
            moveToDir = new Vector3(direction.x, originalPosition.y, originalPosition.z);
        }
        else {
            moveToDir = new Vector3(direction.x-(playerPos.transform.position.x-transform.position.x), originalPosition.y, originalPosition.z);    
        }
        return Mathf.Abs(transform.position.x-moveToDir.x)<=0.01f;
    }

    private void AttackPlayer(){
        if (!Laser.activeSelf)
            Laser.SetActive(true);
        if (!laser.CurrentlyFiring())
        {
            laser.Fire();
            Schedule<PlayerEnteredDeathZone>();
        }
    }
    
    private void AlertLights(){
        blinkTimer += Time.deltaTime;
        if (blinkTimer >=0.4f)
        {
            blinkTimer = 0;
            lightsBlinkState = !lightsBlinkState;
        }
    }

    public void Alert(Vector3 lastKnownPosition){
        if (!On)
        {
            state = EnemyStateHandler.EnemyState.Normal;
            On = true;
            SetLights(true);
        }
        alerted = true;
        lastKnownPlayerLocation = lastKnownPosition;
        direction = lastKnownPlayerLocation;
    }

    public void Shutdown(){
        shutDown = true;
    }
}
