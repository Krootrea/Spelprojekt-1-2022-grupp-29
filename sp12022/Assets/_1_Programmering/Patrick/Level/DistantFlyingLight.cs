using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DistantFlyingLight : MonoBehaviour
{
    private Vector3 target, origin;
    private Light2D _light2D;

    public float BlinkTime, Speed;
    public bool BlinkingLight;
    
    private float blinkTimer;

    // Start is called before the first frame update
    void Start(){
        origin = new Vector3(transform.position.x, transform.position.y);
        blinkTimer = BlinkTime;
        target = transform.Find("Target").position;
        _light2D = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update(){
        Move();
        Blink();
    }

    private void Blink(){
        if (BlinkingLight)
        {
            if (blinkTimer<0.0f)
            {
                _light2D.enabled = !_light2D.enabled;
                blinkTimer = BlinkTime;
            }
            blinkTimer -= Time.deltaTime;
        }
    }

    private void Move(){
        transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);
        if (transform.position==target)
            transform.position = origin;
    }
}
