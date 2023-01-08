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
    private float shutdownTimer, blinkTimer;
    public float startupTimer;
    
    // Start is called before the first frame update
    void Start(){
        laser = GetComponent<Laser>();
        Laser = transform.Find("Laser").gameObject;
        Laser.SetActive(false);
        countingDown = false;
        state = GetComponent<EnemyStateHandler>();
        state.LookingTime = LookingTime;
        state.Current = state.Current;
        _collider2D = GetComponent<EdgeCollider2D>();
        sideLight = transform.Find("sideLight").gameObject;
        On = false;
        SetLights(false);
        originalPosition = transform.position;
        shutdownTimer = 0f;
        lightsBlinkState = true;
        playerPos = transform.Find("playerPos").gameObject;
        animator = GetComponent<Animator>();
        animator.SetBool("On", false);
    }

    private void SetLights(bool onOff){
        sideLight.SetActive(onOff);
    }

    // Update is called once per frame
    void Update(){
        if (shutDown)
        {
            On = false;
            animator.SetBool("On", On);
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

        if (countingDown && shutdownTimer<=0.0f && state.Current == EnemyStateHandler.State.LookingForPlayer)
        {
            shutDown = true;
            countingDown = false;
        }
    }

    private void DecideWhereToGo()
    {
        switch (state.Current)
        {
            case EnemyStateHandler.State.Normal:
            {
                alerted = false;
                // Gå till direction
                if (startupTimer<=0.0f)
                {
                    if (fov.SeeingPlayerRayCast)
                    {
                        state.Current = EnemyStateHandler.State.ChasingPlayer;
                        direction = fov.PlayerPosition;
                    }
                    else if (Arrived())
                    {
                        state.Current = EnemyStateHandler.State.LookingForPlayer;
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
            case EnemyStateHandler.State.LookingForPlayer:
            {
                // Framme på direction, leta efter spelare och räkna ner.
                animator.SetBool("Running", false);
                if (alerted)
                {
                    state.Current = EnemyStateHandler.State.Normal;
                }
                else if (fov.SeeingPlayerRayCast)
                {
                    state.Current = EnemyStateHandler.State.ChasingPlayer;
                    direction = fov.PlayerPosition;
                }
                else
                {
                    LookAround();
                    HandleTimers();
                }
                break;
            }
            case EnemyStateHandler.State.ChasingPlayer:
            {
                // Ser spelare, om framme: attackera. Annars gå till direction(spelarens position).

                if (!fov.SeeingPlayerRayCast && Arrived()){
                    state.Current = EnemyStateHandler.State.LookingForPlayer;
                }
                else if (Arrived())
                {
                    state.Current = EnemyStateHandler.State.Attacking;
                }
                animator.SetBool("Running", true);
                Move();
                break;
            }
            case EnemyStateHandler.State.Attacking:
            {
                animator.SetBool("Running", false);
                AttackPlayer();
                if (!fov.SeeingPlayerRayCast)
                {
                    if (laser.CurrentlyFiring())
                    {
                        laser.StopFiring();
                        Laser.SetActive(false);
                        direction = originalPosition;
                        state.Current = EnemyStateHandler.State.Normal;
                    }
                }
                break;
            }
        }
        lastKnownPlayerLocation = fov.SeeingPlayerRayCast ? fov.PlayerPosition : lastKnownPlayerLocation;
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
            animator.SetBool("Running", false);
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
            state.Current = EnemyStateHandler.State.Normal;
            On = true;
            animator.SetBool("On",On);
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
