using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashRobot : Enemy
{
    public enum TM{MovingToLocation, LookingForPlayer,Idle, ChasingPlayer, Attacking}

    private TM mode;
    private bool rayCast, shutDown, countingDown, lightsBlinkState;
    private GameObject sideLight, topLight;
    private Vector3 lastKnownPlayerLocation, originalPosition;
    public float ShutdownCountDown = 2.5f;
    private float shutdownTimer, blinkTimer;
    
    // Start is called before the first frame update
    void Start(){
        countingDown = false;
        mode = TM.Idle;
        state = GetComponent<EnemyStateHandler>();
        state.LookingTime = LookingTime;
        _collider2D = GetComponent<EdgeCollider2D>();
        sideLight = transform.Find("sideLight").gameObject;
        topLight =  transform.Find("topLight").gameObject;
        On = false;
        SetLights();
        originalPosition = transform.position;
        shutdownTimer = 0f;
        lightsBlinkState = true;
    }

    private void SetLights(){
        sideLight.SetActive(On);
        topLight.SetActive(On);
    }

    // Update is called once per frame
    void Update(){
        if (shutDown)
        {
            On = false;
            shutDown = false;
        }

        if (On)
        {
            ChasePlayer();
            AlertLights();
        }
        // else if (shutDown)
        // {
        //     Debug.Log("shutdown...");
        // }
    }
    
    private void ChasePlayer(){
        if(mode == TM.Attacking) {AttackPlayer();}
        DecideWhereToGo();
        if(mode != TM.Attacking && mode != TM.Idle) {Move();}

        if (mode == TM.LookingForPlayer && 
            direction == lastKnownPlayerLocation) 
        {
            LookAround();
        }
    }

    private void LookAround(){
        Debug.Log("look around...");
    }

    private void DecideWhereToGo(){
        switch (mode)
        {
            case TM.Attacking :
            {
                direction = transform.position;
                break;
            }
            case TM.Idle :
            {
                direction = transform.position;
                break;
            }
            case TM.ChasingPlayer :
            {
                if (fov.PlayerPosition != Vector3.zero)
                {
                    direction = fov.PlayerPosition;
                    break;
                }
                mode = TM.LookingForPlayer;
                break;
            }
            case TM.LookingForPlayer :
            {
                direction = lastKnownPlayerLocation;
                break;
            }
            case TM.MovingToLocation :
            {
                break;
            }
        }
        lastKnownPlayerLocation = fov.SeeingPlayer ? fov.PlayerPosition : lastKnownPlayerLocation;
        // Timer
        shutdownTimer = fov.SeeingPlayer ? ShutdownCountDown : shutdownTimer;
        if (shutdownTimer>0.0f && countingDown)
        {
            shutdownTimer -= Time.deltaTime;
            
        }if (shutdownTimer<0.0f && state.CurrentState == EnemyStateHandler.EnemyState.LookingForPlayer || mode == TM.LookingForPlayer)
        {
            Debug.Log("Shutdowntimer mindre än 0...");
            shutDown = true;
            countingDown = false;
        }
    }

    private void Move()
    {
        direction = new Vector3(direction.x, originalPosition.y, originalPosition.z);
        transform.position = Vector3.MoveTowards(transform.position, direction, Speed * Time.deltaTime);
        if (transform.position==direction && !countingDown)
        {
            mode = fov.SeeingPlayer ? 
                mode = TM.ChasingPlayer : 
                mode = TM.LookingForPlayer;
            shutdownTimer = ShutdownCountDown;
            countingDown = true;
        }
        RotateToCurrentDirection();
    }
    
    private void AttackPlayer(){
        Debug.Log("Attack!");
    }
    
    private void AlertLights(){
        blinkTimer += Time.deltaTime;
        if (blinkTimer >=0.5f)
        {
            blinkTimer = 0;
            topLight.SetActive(lightsBlinkState);
            lightsBlinkState = !lightsBlinkState;
        }
    }

    public void Alert(Vector3 lastKnownPosition){
        if (!On)
        {
            mode = TM.MovingToLocation;
            On = true;
            SetLights();
        }
        lastKnownPlayerLocation = lastKnownPosition;
        direction = lastKnownPlayerLocation;
    }
}
