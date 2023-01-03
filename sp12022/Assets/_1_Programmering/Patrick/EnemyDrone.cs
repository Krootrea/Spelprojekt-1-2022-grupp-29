using System.Collections.Generic;
using System.Linq;
using Platformer.Gameplay;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Platformer.Core.Simulation;

public class EnemyDrone : Enemy
{
    
    private SpriteRenderer _spriteRenderer;
    private Light2D _light2D;
    
    private Vector3 start, target;
    private bool movingTowardsTarget, rayCast,killedPlayer,deathCountDownStarted;
    private GameObject playerPos, droneYellowLamp, droneRedLamp;
    private float lostSightOfPlayerCountDown, playerDeathCountDown, justKilledPlayerCountDown, justAlertedCountDown;

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
        droneYellowLamp = transform.Find("droneYellowLamp").gameObject;
        droneRedLamp = transform.Find("droneRedLamp").gameObject;
        AlertLights(false);
        movingTowardsTarget = true;
        direction = target; 
    }

    private void AlertLights(bool b){
        droneYellowLamp.SetActive(!b);
        droneRedLamp.SetActive(b);
        _light2D.color = b ? Color.red : Color.white;
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
                AlertLights(false);
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
                deathCountDownStarted = false;
                AlertLights(false);
                lostSightOfPlayerCountDown -= Time.deltaTime;
                if (lostSightOfPlayerCountDown<=0.0f)
                {
                    state = EnemyStateHandler.EnemyState.Normal;
                }
                break;
            }
            case EnemyStateHandler.EnemyState.ChasingPlayer:
            {
                if (!deathCountDownStarted)
                {
                    deathCountDownStarted = true;
                    playerDeathCountDown = DeathCountDown;
                }
                AlertAllTrashrobots();
                Vector3 playerPosition = new Vector3(transform.position.x + (fov.PlayerPosition.x-playerPos.transform.position.x), transform.position.y);
                direction = playerPosition;
                AlertLights(true);
                if (!fov.SeeingPlayer)
                {
                    state = EnemyStateHandler.EnemyState.LookingForPlayer;
                    lostSightOfPlayerCountDown = LooseSightCountDown;
                }

                playerDeathCountDown -= Time.deltaTime;
                if (playerDeathCountDown<0.0f)
                {
                    deathCountDownStarted = false;
                    Schedule<PlayerEnteredDeathZone>();
                }
                break;
            }
        }
    }

    private void AlertAllTrashrobots(){
        if(justAlertedCountDown<=0.0f)justAlertedCountDown = 1f;
        justAlertedCountDown -= Time.deltaTime;
        if (TrashrobotsToAlert.Count>0 && justAlertedCountDown<=0.0f) {
            foreach (TrashRobot trashRobot in TrashrobotsToAlert)
            {
                trashRobot.Alert(fov.PlayerPosition);
            }
        }
    }
}