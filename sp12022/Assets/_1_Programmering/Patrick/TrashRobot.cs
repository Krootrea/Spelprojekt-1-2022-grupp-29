using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrashRobot : Enemy
{
    // public enum TM{MovingToLocation, LookingForPlayer,Idle, ChasingPlayer, Attacking}

    // private TM mode;
    private bool rayCast, shutDown, countingDown, lightsBlinkState;
    private GameObject sideLight, topLight;
    private Vector3 lastKnownPlayerLocation, originalPosition;
    public float ShutdownCountDown = 2.5f;
    private float shutdownTimer, blinkTimer;
    
    // Start is called before the first frame update
    void Start(){
        countingDown = false;
        // mode = TM.Idle;
        stateHandler = GetComponent<EnemyStateHandler>();
        stateHandler.LookingTime = LookingTime;
        state = stateHandler.CurrentState;
        _collider2D = GetComponent<EdgeCollider2D>();
        sideLight = transform.Find("sideLight").gameObject;
        topLight =  transform.Find("topLight").gameObject;
        On = false;
        SetLights(false);
        originalPosition = transform.position;
        shutdownTimer = 0f;
        lightsBlinkState = true;
    }

    private void SetLights(bool onOff){
        sideLight.SetActive(onOff);
        topLight.SetActive(onOff);
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
        if (transform.position == direction && !countingDown) 
        {
            shutdownTimer = ShutdownCountDown;
            countingDown = !fov.SeeingPlayer;
        }
        
        if (shutdownTimer>0.0f && countingDown)
            shutdownTimer -= Time.deltaTime;
        
        if (countingDown && shutdownTimer<=0.0f && state == EnemyStateHandler.EnemyState.LookingForPlayer)
        {
            shutDown = true;
            countingDown = false;
        }
    }

    private void LookAround(){ }

    private void DecideWhereToGo()
    {
        switch (state)
        {
            case EnemyStateHandler.EnemyState.Normal:
            {
                // Gå till direction

                if (fov.SeeingPlayer)
                    state = EnemyStateHandler.EnemyState.ChasingPlayer;
                else if (transform.position == direction)
                    state = EnemyStateHandler.EnemyState.LookingForPlayer;
                Move();
                break;
            }
            case EnemyStateHandler.EnemyState.LookingForPlayer:
            {
                // Framme på direction, leta efter spelare och räkna ner.
                
                if (fov.SeeingPlayer)
                    state = EnemyStateHandler.EnemyState.ChasingPlayer;
                else
                {
                    HandleTimers();
                }
                break;
            }
            case EnemyStateHandler.EnemyState.ChasingPlayer:
            {
                // Ser spelare, om framme: attackera. Annars gå till direction(spelarens position).
                
                if (!fov.SeeingPlayer)
                    state = EnemyStateHandler.EnemyState.LookingForPlayer;
                Move();
                break;
            }
        }
        lastKnownPlayerLocation = fov.SeeingPlayer ? fov.PlayerPosition : lastKnownPlayerLocation;
    }

    private void Move()
    {
        direction = new Vector3(direction.x, originalPosition.y, originalPosition.z);
        transform.position = Vector3.MoveTowards(transform.position, direction, Speed * Time.deltaTime);
        RotateToCurrentDirection();
    }
    
    private void AttackPlayer(){
        Debug.Log("Attack!");
    }
    
    private void AlertLights(){
        blinkTimer += Time.deltaTime;
        if (blinkTimer >=0.4f)
        {
            blinkTimer = 0;
            topLight.SetActive(lightsBlinkState);
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
        lastKnownPlayerLocation = lastKnownPosition;
        direction = lastKnownPlayerLocation;
    }
}
