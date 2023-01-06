using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonGeneral : MonoBehaviour
{
    private Collider2D colliderThatPushesButton;
    public bool MovingButton;
    public List<GameObject> ObjectsToActivate, ObjectsToDeactivate;
    public List<TrashRobot> TrashRobotsToActivate;
    public GameObject Player;
    private Vector3 origin, destination, currentTarget;
    private float beforeMovementTimer = 2.0f;
    private bool pushed, movementTime;
    public bool IsButtonPushed => pushed;

    private void Awake(){
        origin = new Vector3(transform.position.x, transform.position.y);
        destination = new Vector3(transform.Find("ButtonDestination").position.x, transform.Find("ButtonDestination").position.y);
        currentTarget = destination;
        colliderThatPushesButton = GetComponent<Collider2D>();
    }

    private void Update(){
        if (movementTime)
        {
            Move();
        }
    }

    private void Move()
    {
        // if (transform.position == origin)
        //     currentTarget = destination;
        // else if (transform.position == destination)
        //     currentTarget = origin;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, 2 * Time.deltaTime);
        if (transform.position == currentTarget)
            movementTime = false;
    }

    private void OnTriggerEnter2D(Collider2D col){
        bool isEnemy = col.transform.gameObject.TryGetComponent(typeof(Enemy), out Component comp);
        ButtonPressAction(isEnemy);
    }

    private void ButtonPressAction(bool isEnemy){
        pushed = isEnemy;
        if (!movementTime) {
            foreach (GameObject gameObject in ObjectsToActivate) {
                gameObject.SetActive(isEnemy);
            }
            foreach (GameObject gameObject in ObjectsToDeactivate) {
                gameObject.SetActive(!isEnemy);
            }
            if (TrashRobotsToActivate.Count>0)
            {
                foreach (TrashRobot tb in TrashRobotsToActivate) 
                {
                    if (isEnemy)
                        tb.Alert(Player.transform.position);
                    else
                        tb.Shutdown();
                }
            }
        }
        if (MovingButton) 
            currentTarget = isEnemy ? destination : origin;
    }
}