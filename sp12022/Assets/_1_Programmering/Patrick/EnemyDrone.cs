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
    
    private Vector3 start, buttonLocation;
    private bool movingTowardsTarget, rayCast;
    private GameObject playerPos, droneYellowLamp, droneRedLamp, questionMark;
    private float lostSightOfPlayerCountDown, justAlertedCountDown, initialPlayerSighting;

    public float LooseSightCountDown, DeathCountDown;
    
    public List<TrashRobot> TrashrobotsToAlert;
    public ButtonGeneral Button;
    public bool WriteStateChangeToConsole;

    private LineRenderer _lineRenderer;
    
    private void Awake(){
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
        {
            buttonLocation = Button.transform.position;
        }
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
        return (!fov.Stop || !Button.IsButtonPushed) && !fov.PlayerHiding();
    }

    private bool ArrivedAtTarget(){
        return transform.position == direction;
    }

    private void Patrol(){
        switch (state.Current)
        {
            case EnemyStateHandler.State.Normal:
            {
                AlertLights(fov.SeeingPlayerRayCast && !fov.PlayerHiding());
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
                    StateChangeToConsole();
                    break;
                }
                AlertLights(true);
                if (Button.IsButtonPushed)
                {
                    state.Current = EnemyStateHandler.State.ChasingPlayer;
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
                    state.Current = EnemyStateHandler.State.Normal;
                    questionMark.SetActive(false);
                    StateChangeToConsole();
                }
                else if(fov.SeeingPlayerRayCast && !fov.PlayerHiding())
                {
                    state.Current = EnemyStateHandler.State.ChasingPlayer;
                    questionMark.SetActive(false);
                    StateChangeToConsole();
                }
                else if (ArrivedAtTarget())
                    LookAround();
                break;
            }
            case EnemyStateHandler.State.ChasingPlayer:
            {
                AlertAllTrashrobots();
                Vector3 playerPosition = new Vector3(transform.position.x + (fov.PlayerPosition.x-playerPos.transform.position.x), transform.position.y);
                direction = playerPosition;
                AlertLights(true);
                if (!fov.SeeingPlayerRayCast)
                {
                    state.Current = EnemyStateHandler.State.LookingForPlayer;
                    questionMark.SetActive(true);
                    StateChangeToConsole();
                    lostSightOfPlayerCountDown = LooseSightCountDown;
                    lastKnownPlayerLocation = new Vector3(fov.PlayerPosition.x, transform.position.y);
                }
                break;
            }
        }
    }

    private void StateChangeToConsole(){
        if (WriteStateChangeToConsole)
            Debug.Log(state.Current);
    }

    private void AlertAllTrashrobots(){
        if (!Button.IsUnityNull() && Button.TrashRobotsToActivate.Count>0)
        {
            foreach (TrashRobot trashRobot in Button.TrashRobotsToActivate)
            {
                trashRobot.Alert(fov.PlayerPosition);
            }
        }
        if(justAlertedCountDown<=0.0f)justAlertedCountDown = 1f;
        justAlertedCountDown -= Time.deltaTime;
        if (TrashrobotsToAlert.Count>0 && justAlertedCountDown<=0.0f) {
            foreach (TrashRobot trashRobot in TrashrobotsToAlert)
            {
                trashRobot.Alert(fov.PlayerPosition);
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
}