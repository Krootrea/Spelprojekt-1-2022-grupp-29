using System.Collections.Generic;
using System.Linq;
using Platformer.Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static Platformer.Core.Simulation;

public class EnemyDrone : Enemy, IResetOnRespawn
{
    
    private SpriteRenderer _spriteRenderer;
    private Light2D _light2D;
    
    private Vector3 buttonLocation;
    private bool movingTowardsTarget;
    private GameObject playerPos, droneYellowLamp, droneRedLamp, questionMark;
    private float lostSightOfPlayerCountDown, justAlertedCountDown, initialPlayerSighting;
    
    public List<TrashRobot> TrashrobotsToAlert;
    public ButtonGeneral Button;
    public bool WriteStateChangeToConsole;
    public float DiscoverPlayerDelay;

    private LineRenderer _lineRenderer;
    
    // ---- VARIABLER FÖR RESET ---- 

    private Vector3 start;
    private Vector3 originalScale;
    private EnemyStateHandler.State originalState;
    private GameObject alertSFX;
    
    // ----
    
    private void Awake(){
        alertSFX = transform.Find("AlertSFX").gameObject;
        initialPlayerSighting = DiscoverPlayerDelay;
        patrol = new Vector3(transform.Find("patrolDestination").transform.position.x, transform.Find("patrolDestination").transform.position.y);
        fov = GetComponent<FieldOfView>();
        state = GetComponent<EnemyStateHandler>();
        On = true;
        _lineRenderer = new LineRenderer();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        start = transform.position;
        playerPos = transform.Find("playerPos").gameObject;
        _light2D = transform.Find("BeamLight").GetComponent<Light2D>();
        droneYellowLamp = transform.Find("droneYellowLamp").gameObject;
        droneRedLamp = transform.Find("droneRedLamp").gameObject;
        questionMark = transform.Find("questionMark").gameObject;
        questionMark.SetActive(false);
        AlertLights(false);
        movingTowardsTarget = true;
        direction = patrol;
        if (!Button.IsUnityNull())
            buttonLocation = Button.transform.position;
        originalScale = transform.localScale;
    }

    private void AlertLights(bool b){
        droneYellowLamp.SetActive(!b);
        droneRedLamp.SetActive(b);
        _light2D.color = b ? Color.red : Color.white;
    }

    private void Update(){
        if (On)
        {
            Patrol();
            Move();
        }
    }
    private void Move(){
        if (!StandStillAndLookAtPlayer() || !ArrivedAtTarget())
            transform.position = Vector3.MoveTowards(transform.position, direction, Speed * Time.deltaTime);
        RotateToCurrentDirection();
    }

    private bool StandStillAndLookAtPlayer(){
        if (state.Current == EnemyStateHandler.State.ChasingPlayer)
            return !fov.Stop || !Button.IsButtonPushed;
        return (!fov.Stop || !Button.IsUnityNull() && !Button.IsButtonPushed) && !fov.PlayerHiding();
    }

    private bool ArrivedAtTarget(){
        return transform.position == direction;
    }

    private void Patrol(){
        switch (state.Current)
        {
            case EnemyStateHandler.State.Normal:
            {
                if (transform.position==patrol)
                    movingTowardsTarget = false;
                else if (transform.position==start)
                    movingTowardsTarget = true;
                direction = movingTowardsTarget ? patrol : start;
                if (fov.SeeingPlayerRayCast && !fov.PlayerHiding())
                {
                    if (initialPlayerSighting>0.0f)
                        initialPlayerSighting -= Time.deltaTime;
                    if (Button.IsUnityNull() && initialPlayerSighting <= 0.0f)
                    {
                        state.Current = EnemyStateHandler.State.ChasingPlayer;
                        PlayAlertSound();
                        StateChangeToConsole();
                    }
                    else if (initialPlayerSighting<=0.0f)
                    {
                        state.Current = (Button.IsButtonPushed) ? 
                            EnemyStateHandler.State.ChasingPlayer : EnemyStateHandler.State.GoForAlertButton;
                        StateChangeToConsole();
                    }
                }
                break;
            }
            case EnemyStateHandler.State.GoForAlertButton:
            {
                if (Button.IsUnityNull())
                {
                    state.Current = EnemyStateHandler.State.ChasingPlayer;
                    PlayAlertSound();
                    StateChangeToConsole();
                    break;
                }
                AlertLights(true);
                if (Button.IsButtonPushed)
                {
                    state.Current = EnemyStateHandler.State.ChasingPlayer;
                    PlayAlertSound();
                    StateChangeToConsole();
                    break;
                }
                Vector3 buttonPosition = new Vector3(buttonLocation.x, transform.position.y);
                direction = buttonPosition;
                break;
            }
            case EnemyStateHandler.State.LookingForPlayer:
            {
                direction = lastKnownPlayerLocation;
                AlertLights(true);
                lostSightOfPlayerCountDown -= Time.deltaTime;
                if (lostSightOfPlayerCountDown<=0.0f)
                {
                    ChangeStateToNormal();
                }
                else if(fov.SeeingPlayerRayCast && !fov.PlayerHiding())
                {
                    state.Current = Button.IsButtonPushed ? EnemyStateHandler.State.ChasingPlayer : EnemyStateHandler.State.GoForAlertButton;
                    questionMark.SetActive(false);
                    StateChangeToConsole();
                }
                else if (ArrivedAtTarget())
                    LookAround();
                break;
            }
            case EnemyStateHandler.State.ChasingPlayer:
            {
                if (!Button.IsUnityNull() && !Button.IsButtonPushed)
                {
                    state.Current = EnemyStateHandler.State.GoForAlertButton;
                    PlayAlertSound();
                    StateChangeToConsole();
                    break;
                }
                AlertAllTrashrobots();
                Vector3 playerPosition = new Vector3(transform.position.x + (fov.PlayerPosition.x-playerPos.transform.position.x), transform.position.y);
                direction = playerPosition;
                AlertLights(true);
                if (!fov.SeeingPlayerRayCast)
                {
                    state.Current = EnemyStateHandler.State.LookingForPlayer;
                    questionMark.SetActive(true);
                    StateChangeToConsole();
                    lostSightOfPlayerCountDown = LookingTime;
                    lastKnownPlayerLocation = new Vector3(fov.PlayerPosition.x, transform.position.y);
                }
                break;
            }
        }
    }

    private void PlayAlertSound(){
        alertSFX.GetComponent<AudioSource>().Play();
    }

    private void ChangeStateToNormal(){
        state.Current = EnemyStateHandler.State.Normal;
        initialPlayerSighting = DiscoverPlayerDelay;
        AlertLights(false);
        questionMark.SetActive(false);
        StateChangeToConsole();
    }

    private void StateChangeToConsole(){
        if (WriteStateChangeToConsole)
            Debug.Log(state.Current);
    }

    private void AlertAllTrashrobots(){
        if(justAlertedCountDown<=0.0f)justAlertedCountDown = 1f;
        justAlertedCountDown -= Time.deltaTime;
        if (TrashrobotsToAlert.Count>0 && justAlertedCountDown<=0.0f) {
            foreach (TrashRobot trashRobot in TrashrobotsToAlert)
            {
                if (trashRobot.TurnedOn)
                    trashRobot.Alert(fov.PlayerPosition, fov.playerController.Hidden);
            }
        }
    }

    protected override void RotateToCurrentDirection(){
        base.RotateToCurrentDirection();
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

    public void RespawnReset(){
        transform.position = start;
        transform.localScale = originalScale;
        state.Current = originalState;
        questionMark.SetActive(false);
        AlertLights(false);
        movingTowardsTarget = true;
        direction = patrol;
        On = true;
        initialPlayerSighting = DiscoverPlayerDelay;
    }
}