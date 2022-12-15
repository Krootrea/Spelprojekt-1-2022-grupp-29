using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public Vector3 patrol;
    public float Speed = 1f, LookingTime = 2f;
    
    protected FieldOfView fov;
    protected Collider2D _collider2D;
    protected bool On;
    protected EnemyStateHandler state;
    protected Vector3 direction;

    private void Awake(){
        fov = GetComponent<FieldOfView>();
        // GameObject gObject = new GameObject("EnemyStateHandler");
        // state = gObject.AddComponent<EnemyStateHandler>();
    }

    protected void RotateToCurrentDirection(){
        Vector3 scale = transform.localScale;
        if (direction.x > transform.position.x && !fov.BetweenPrefAndEnemy)
            scale.Set(1, 1, 1);
        else if (direction.x < transform.position.x && !fov.BetweenPrefAndEnemy)
            scale.Set(-1, 1, 1);
        
        transform.localScale = scale;
    }
}
