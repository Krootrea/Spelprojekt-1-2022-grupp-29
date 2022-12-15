using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateHandler : MonoBehaviour
{
    [HideInInspector]
    public bool initiatePlayerMaybeLeft;
    private float playerMaybeLeftTimer;
    
    [HideInInspector]
    public enum EnemyState
    {
        Normal,
        ChasingPlayer,
        LookingForPlayer
    }

    private float timeSinceSeeingPlayer;
    [HideInInspector]
    public float LookingTime;
    private bool seeingPlayer;
    private EnemyState currentState;

    public EnemyState CurrentState => currentState;

    private void Awake(){
        seeingPlayer = false;
        currentState = EnemyState.Normal;
    }

    private void Update(){
        StateUpdate(seeingPlayer);
    }

    public void StateUpdate(bool seeingPlayer){
        switch (currentState)
        {
            case EnemyState.ChasingPlayer :
            {
                currentState = seeingPlayer ? 
                    EnemyState.ChasingPlayer : EnemyState.LookingForPlayer;
                return;
            }
            case EnemyState.Normal :
            {
                currentState = seeingPlayer ? 
                        EnemyState.ChasingPlayer : EnemyState.Normal;
                return;
            }
            case EnemyState.LookingForPlayer :
            {
                timeSinceSeeingPlayer += Time.deltaTime;
                if (timeSinceSeeingPlayer >= LookingTime)
                {
                    currentState = EnemyState.Normal;
                    timeSinceSeeingPlayer = 0f;
                }
                return;
            }
        }
    }

    public void SeeingPlayer(bool trueOrFalse){
        seeingPlayer = trueOrFalse;
    }

}
