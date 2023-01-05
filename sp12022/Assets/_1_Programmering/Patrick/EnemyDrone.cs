using System.Collections.Generic;
using System.Linq;
using Platformer.Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Platformer.Core.Simulation;

public class EnemyDrone : Enemy
{
    
    private SpriteRenderer _spriteRenderer;
    private Light2D _light2D;
    
    private Vector3 start, target, buttonLocation;
    private bool movingTowardsTarget, rayCast,killedPlayer,deathCountDownStarted;
    private GameObject playerPos, droneYellowLamp, droneRedLamp;
    private float lostSightOfPlayerCountDown, playerDeathCountDown, justKilledPlayerCountDown, justAlertedCountDown, initialPlayerSighting;

    public float LooseSightCountDown, DeathCountDown;
    
    public List<TrashRobot> TrashrobotsToAlert;
    public ButtonGeneral Button;

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
        buttonLocation = Button.transform.position;
        initialPlayerSighting = 0.6f;
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
        if (!fov.Stop || !Button.IsButtonPushed)
            transform.position = Vector3.MoveTowards(transform.position, direction, Speed * Time.deltaTime);
        RotateToCurrentDirection();
    }


    private void Patrol(){
        switch (state)
        {
            case EnemyStateHandler.EnemyState.Normal:
            {
                AlertLights(fov.SeeingPlayer);
                if (transform.position==target)
                    movingTowardsTarget = false;
                else if (transform.position==start)
                    movingTowardsTarget = true;
                direction = movingTowardsTarget ? target : start;
                if (fov.SeeingPlayer)
                {
                    if (initialPlayerSighting>0.0f)
                        initialPlayerSighting -= Time.deltaTime;
                    if (Button.IsUnityNull() && initialPlayerSighting<=0.0f)
                        state = EnemyStateHandler.EnemyState.ChasingPlayer;
                    else if (initialPlayerSighting<=0.0f)
                        state = (Button.IsButtonPushed) ? 
                            EnemyStateHandler.EnemyState.ChasingPlayer : EnemyStateHandler.EnemyState.GoForAlertButton;
                }
                break;
            }
            case EnemyStateHandler.EnemyState.GoForAlertButton:
            {
                if (Button.IsUnityNull())
                {
                    state = EnemyStateHandler.EnemyState.ChasingPlayer;
                    break;
                }
                AlertLights(true);
                if (Button.IsButtonPushed)
                {
                    state = EnemyStateHandler.EnemyState.ChasingPlayer;
                    break;
                }
                Vector3 buttonPosition = new Vector3(buttonLocation.x, transform.position.y);
                direction = buttonPosition;
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
                // AlertAllTrashrobots();
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