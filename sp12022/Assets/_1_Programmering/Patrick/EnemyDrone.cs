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

public class EnemyDrone : Enemy
{
    
    private SpriteRenderer _spriteRenderer;
    private Light2D _light2D;
    
    private Vector3 start, target;
    private bool movingTowardsTarget, rayCast,killedPlayer;
    private GameObject playerPos;
    private float lostSightOfPlayerCountDown, playerDeathCountDown, justKilledPlayerCountDown;

    public float LooseSightCountDown, DeathCountDown;
    public List<TrashRobot> TrashrobotsToAlert;
    
    private LineRenderer _lineRenderer;
    
    private void Awake(){
        fov = GetComponent<FieldOfView>();
        state = GetComponent<EnemyStateHandler>();
        state.LookingTime = LookingTime;
        On = true;
        _lineRenderer = new LineRenderer();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        start = transform.position;
        target = transform.position + patrol;
        playerPos = transform.Find("playerPos").gameObject;
        
        _light2D = transform.Find("BeamLight").GetComponent<Light2D>();
        movingTowardsTarget = true;
        direction = target; 
    }

    private void Update(){
        if (On)
        {
            state.SeeingPlayer(fov.SeeingPlayer);
            Patrol();
            Move();
        }
    }
    private void Move(){
        if (!fov.Stop)
            transform.position = Vector3.MoveTowards(transform.position, direction, Speed * Time.deltaTime);
        RotateToCurrentDirection();
    }


    private void Patrol(){
        switch (state.CurrentState)
        {
            case EnemyStateHandler.EnemyState.Normal:
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
                AlertAllTrashrobots();
                Vector3 playerPosition = new Vector3(transform.position.x + (fov.PlayerPosition.x-playerPos.transform.position.x), transform.position.y);
                direction = playerPosition;
                _spriteRenderer.color = Color.red;
                _light2D.color = _spriteRenderer.color;
                return;
            }
        }
    }

    private void AlertAllTrashrobots(){
        foreach (TrashRobot trashRobot in TrashrobotsToAlert)
        {
            trashRobot.Alert(fov.PlayerPosition);
        }
    }
}