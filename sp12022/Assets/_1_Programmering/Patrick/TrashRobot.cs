using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Platformer.Gameplay;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.Tilemaps;
using UnityEngine;
using static Platformer.Core.Simulation;
using Random = UnityEngine.Random;

public class TrashRobot : Enemy
{
    // public enum TM{MovingToLocation, LookingForPlayer,Idle, ChasingPlayer, Attacking}

    // private TM mode;
    private bool rayCast, shutDown, countingDown, lightsBlinkState, turnLeft, alerted, knowsThatPlayerIsHiding;
    private GameObject sideLight, topLight, playerPos, Laser, questionMark;
    private Vector3 lastKnownPlayerLocation, originalPosition, leftPoint, rightPoint;
    private Animator animator;

    private Laser laser;
    // private float ShutdownCountDown;
    private float shutdownTimer, blinkTimer, lfpInitialStandStillTimer, lfpWalkAroundToLookForPlayerTimer, startUp;
    public float startupTimer;
    public bool TurnedOn => On;
    
    // Start is called before the first frame update
    void Start(){
        startUp = startupTimer;
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
        leftPoint = new Vector3(transform.Find("leftSide").transform.position.x, transform.Find("leftSide").transform.position.y);
        rightPoint = new Vector3(transform.Find("rightSide").transform.position.x, transform.Find("rightSide").transform.position.y);
        questionMark = transform.Find("questionMark").gameObject;
        questionMark.SetActive(false);
    }

    private void SetLights(bool onOff){
        sideLight.SetActive(onOff);
    }

    // Update is called once per frame
    void Update(){
        if (shutDown)
        {
            startUp = startupTimer;
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
            Shutdown();
            countingDown = false;
        }

        if (state.Current == EnemyStateHandler.State.LookingForPlayer)
        {
            if (lfpInitialStandStillTimer>0.0f)
                lfpInitialStandStillTimer -= Time.deltaTime;
            else if (lfpWalkAroundToLookForPlayerTimer>0.0f)
                lfpWalkAroundToLookForPlayerTimer -= Time.deltaTime;
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
                if (startUp<=0.0f)
                {
                    if (SeesPlayer())
                    {
                        state.Current = EnemyStateHandler.State.ChasingPlayer;
                        direction = fov.PlayerPosition;
                    }
                    else if (Arrived())
                    {
                        state.Current = EnemyStateHandler.State.LookingForPlayer;
                        LookingForPlayerSetup();
                    }

                    RunningAnimation(true);
                    Move();
                }
                else
                {
                    startUp -= Time.deltaTime;
                }
                break;
            }
            case EnemyStateHandler.State.LookingForPlayer:
            {
                // Framme på direction, leta efter spelare och räkna ner.
                RunningAnimation(false);
                if (alerted)
                {
                    state.Current = EnemyStateHandler.State.Normal;
                    questionMark.SetActive(false);
                }
                else if (SeesPlayer())
                {
                    state.Current = EnemyStateHandler.State.ChasingPlayer;
                    questionMark.SetActive(false);
                    direction = fov.PlayerPosition;
                }
                else
                {
                    if (lfpInitialStandStillTimer>0.0f)
                    {
                        LookAround();
                    }
                    else if (lfpWalkAroundToLookForPlayerTimer>0.0f)
                    {
                        RunningAnimation(false);
                        if (Arrived())
                        {
                            lfpInitialStandStillTimer = 5f;
                            float randomX = RandomNumberGenerator.GetInt32((int)leftPoint.x, (int)rightPoint.x);
                            direction = new Vector3(randomX, transform.position.y);
                        }
                        else
                        {
                            RunningAnimation(true);
                            Move();
                        }
                    }
                    HandleTimers();
                }
                break;
            }
            case EnemyStateHandler.State.ChasingPlayer:
            {
                // Ser spelare, om framme: attackera. Annars gå till direction(spelarens position).

                if (!SeesPlayer() && Arrived()){
                    {
                        state.Current = EnemyStateHandler.State.LookingForPlayer;
                        LookingForPlayerSetup();
                    }
                }
                else if (Arrived())
                {
                    state.Current = EnemyStateHandler.State.Attacking;
                }
                RunningAnimation(true);
                Move();
                break;
            }
            case EnemyStateHandler.State.Attacking:
            {
                RunningAnimation(false);
                AttackPlayer();
                if (!SeesPlayer())
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

    private bool SeesPlayer(){
        if (knowsThatPlayerIsHiding)
            return fov.SeeingPlayerRayCast;
        return fov.SeeingPlayerRayCast && !fov.playerController.Hidden;
    }

    private void RunningAnimation(bool b){
        animator.SetBool("Running", b);
    }

    private void LookingForPlayerSetup(){
        float randomX = RandomNumberGenerator.GetInt32((int)leftPoint.x, (int)rightPoint.x);
        direction = new Vector3(randomX, transform.position.y);
        lfpInitialStandStillTimer = 5;
        lfpWalkAroundToLookForPlayerTimer = 20f;
        questionMark.SetActive(true);
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

    public void Alert(Vector3 lastKnownPosition, bool playerCurrentlyHiding){
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
        knowsThatPlayerIsHiding = playerCurrentlyHiding;
    }

    public void Shutdown(){
        shutDown = true;
        questionMark.SetActive(false);
        RunningAnimation(false);
    }

    private void QuestionMarkRotation(){
        if (questionMark.activeSelf)
        {
            if (transform.localScale.x<0 && questionMark.transform.localScale.x>0 || 
                transform.localScale.x>0 && questionMark.transform.localScale.x<0)
            {
                Vector3 newScale = new Vector3(
                    -questionMark.transform.localScale.x,
                    questionMark.transform.localScale.y, 
                    questionMark.transform.localScale.z);
                
                questionMark.transform.localScale=newScale;
            }
        }
    }

    protected override void LookAround(){
        base.LookAround();
        QuestionMarkRotation();
    }

    protected override void RotateToCurrentDirection(){
        base.RotateToCurrentDirection();
        QuestionMarkRotation();
    }
}
