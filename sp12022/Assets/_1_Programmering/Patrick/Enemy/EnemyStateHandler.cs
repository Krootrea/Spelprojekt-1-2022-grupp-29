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
    public enum State
    {
        Normal,
        ChasingPlayer,
        LookingForPlayer,
        GoForAlertButton,
        Attacking
    }

    private float timeSinceSeeingPlayer;
    [HideInInspector]
    public float LookingTime;
    public State Current;

    private void Awake(){
        Current = State.Normal;
    }


}
