using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
        stateHandler = GetComponent<EnemyStateHandler>();
        stateHandler.LookingTime = LookingTime;
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
            stateHandler.SeeingPlayer(fov.SeeingPlayer);
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
        switch (state)
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
                if (fov.SeeingPlayer)
                {
                    state = EnemyStateHandler.EnemyState.ChasingPlayer;
                }
                break;
            }
            case EnemyStateHandler.EnemyState.LookingForPlayer:
            {
                _spriteRenderer.color = Color.magenta;
                _light2D.color = _spriteRenderer.color;
                lostSightOfPlayerCountDown -= Time.deltaTime;
                if (lostSightOfPlayerCountDown<=0.0f)
                {
                    state = EnemyStateHandler.EnemyState.Normal;
                }
                break;
            }
            case EnemyStateHandler.EnemyState.ChasingPlayer:
            {
                AlertAllTrashrobots();
                Vector3 playerPosition = new Vector3(transform.position.x + (fov.PlayerPosition.x-playerPos.transform.position.x), transform.position.y);
                direction = playerPosition;
                _spriteRenderer.color = Color.red;
                _light2D.color = _spriteRenderer.color;
                if (!fov.SeeingPlayer)
                {
                    state = EnemyStateHandler.EnemyState.LookingForPlayer;
                    lostSightOfPlayerCountDown = LooseSightCountDown;
                }
                break;
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