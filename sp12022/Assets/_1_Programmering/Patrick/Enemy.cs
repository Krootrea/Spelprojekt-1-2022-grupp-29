using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    protected Vector3 patrol;
    public float Speed = 1f, LookingTime = 2f;
    
    protected FieldOfView fov;
    protected Collider2D _collider2D;
    protected bool On;
    protected EnemyStateHandler state;
    protected Vector3 direction, lastKnownPlayerLocation;

    private float lookTimer;
    protected bool turnLeft;

    private void Awake(){
        lookTimer = 0.0f;
        fov = GetComponent<FieldOfView>();
        // GameObject gObject = new GameObject("EnemyStateHandler");
        // state = gObject.AddComponent<EnemyStateHandler>();
    }

    protected virtual void RotateToCurrentDirection(){
        Vector3 scale = transform.localScale;
        if (direction.x > transform.position.x && !fov.BetweenPrefAndEnemy)
            scale.Set(1, 1, 1);
        else if (direction.x < transform.position.x && !fov.BetweenPrefAndEnemy)
            scale.Set(-1, 1, 1);
        
        transform.localScale = scale;
    }
    
    protected virtual void LookAround(){
        if (lookTimer>2f)
        {
            lookTimer = 0.0f;
            Vector3 scale = transform.localScale;
            if (turnLeft)
                scale.Set(-1, 1, 1);
            else
                scale.Set(1, 1, 1);
            turnLeft = !turnLeft;
            transform.localScale = scale;
        }

        lookTimer += Time.deltaTime;
    }
}
