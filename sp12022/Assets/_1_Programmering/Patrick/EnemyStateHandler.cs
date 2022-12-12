using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateHandler : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        ChasingPlayer,
        LookingForPlayer
    }

    private float timeSinceSeeingPlayer, lookingTime;
    private EnemyState currentState;

    public EnemyState CurrentState => currentState;

    public EnemyStateHandler(float amountOfTimeEnemyLooksForPlayer){
        lookingTime = amountOfTimeEnemyLooksForPlayer;
        currentState = EnemyState.Patrolling;
    }
    public void CustomUpdate(bool seeingPlayer){
        switch (currentState)
        {
            case EnemyState.ChasingPlayer :
            {
                currentState = seeingPlayer ? 
                    EnemyState.ChasingPlayer : EnemyState.LookingForPlayer;
                return;
            }
            case EnemyState.Patrolling :
            {
                currentState = seeingPlayer ? 
                        EnemyState.ChasingPlayer : EnemyState.Patrolling;
                return;
            }
            case EnemyState.LookingForPlayer :
            {
                timeSinceSeeingPlayer += Time.deltaTime;
                if (timeSinceSeeingPlayer >= lookingTime)
                {
                    currentState = EnemyState.Patrolling;
                    timeSinceSeeingPlayer = 0f;
                }
                return;
            }
        }
    }
}
