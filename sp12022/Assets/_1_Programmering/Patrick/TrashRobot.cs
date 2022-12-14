using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashRobot : Enemy
{
    private bool rayCast;
    private GameObject sideLight, topLight;
    private Vector3 lastKnownPlayerLocation, originalPosition;
    
    // Start is called before the first frame update
    void Start(){
        state = GetComponent<EnemyStateHandler>();
        state.LookingTime = LookingTime;
        _collider2D = GetComponent<EdgeCollider2D>();
        sideLight = transform.Find("sideLight").gameObject;
        topLight =  transform.Find("topLight").gameObject;
        SetTurnedOn(false);
        SetLights(OnOff);
        originalPosition = transform.position;
    }

    private void SetLights(activeStatus b){
        sideLight.SetActive(b == activeStatus.on);
        topLight.SetActive(b == activeStatus.on);
    }

    // Update is called once per frame
    void Update()
    {
        if (OnOff == activeStatus.on)
        {
            SetLights(OnOff);
            ChasePlayer();
        }
    }
    
    private void ChasePlayer(){
        AttackPlayerIfWithinRange();
        DecideWhereToGo();
        Move();
    }

    private void DecideWhereToGo(){
        direction = /*fov.SeeingPlayer ? fov.PlayerPosition :*/ lastKnownPlayerLocation;
    }

    private void Move(){
        direction = new Vector3(direction.x, originalPosition.y, originalPosition.z);
        transform.position = Vector3.MoveTowards(transform.position, direction, Speed * Time.deltaTime);
        RotateToCurrentDirection();
    }
    

    private void AttackPlayerIfWithinRange(){
        // Attackera
    }


    private void AlertLights(){
        // Red toplight blinks at given interval
    }

    public void Alert(Vector3 lastKnownPosition){
        if (OnOff == activeStatus.off)
        {
            SetTurnedOn(true);
            SetLights(activeStatus.on);
        }
        lastKnownPlayerLocation = lastKnownPosition;
    }
}
