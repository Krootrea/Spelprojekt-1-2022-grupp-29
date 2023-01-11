using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonGeneral : MonoBehaviour, IResetOnRespawn
{
    private Collider2D colliderThatPushesButton;
    private List<Collider2D> collisions;
    public bool MovingButton;
    public List<GameObject> ObjectsToActivate, ObjectsToDeactivate;
    public List<TrashRobot> TrashRobotsToActivate;
    public GameObject Player;
    private Vector3 origin, destination, currentTarget;
    private bool pushed, movementTime;
    public bool IsButtonPushed => pushed;

    private void Awake(){
        collisions = new List<Collider2D>();
        ObjectsToActivate = new List<GameObject>();
        ObjectsToDeactivate = new List<GameObject>();
        origin = new Vector3(transform.position.x, transform.position.y);
        destination = new Vector3(transform.Find("targetLocation").position.x, transform.Find("targetLocation").position.y);
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
        if (transform.position == origin)
            currentTarget = destination;
        else if (transform.position == destination)
            currentTarget = origin;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, 2 * Time.deltaTime);
        if (transform.position == currentTarget)
            movementTime = false;
    }

    private void OnTriggerEnter2D(Collider2D col){
        bool isEnemy = col.CompareTag("Enemy");
        ButtonPressAction(isEnemy);
        if (!collisions.Contains(col))
        {
            collisions.Add(col);
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if (collisions.Contains(other))
        {
            collisions.Remove(other);
        }
    }

    private void ButtonPressAction(bool isEnemy){
        if (!isEnemy)
        {
            foreach (Collider2D col in collisions)
            {
                if (col.CompareTag("Enemy"))
                {
                    return;
                }
            }
        }
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
                        tb.Alert(Player.transform.position, false);
                    else
                        tb.Shutdown();
                }
            }
        }
        if (MovingButton)
            movementTime = true;
    }

    public void RespawnReset(){
        pushed = false;
        transform.position = origin;
        collisions.Clear();
        currentTarget = destination;
    }
}
