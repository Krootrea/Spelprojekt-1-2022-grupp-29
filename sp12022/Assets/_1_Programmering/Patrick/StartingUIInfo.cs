using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingUIInfo : MonoBehaviour
{
    private GameObject move, jump;
    private bool initialMoveDone, initialJumpDone;
    private void Awake(){
        move = transform.GetChild(0).gameObject;
        jump = transform.GetChild(1).gameObject;
        move.SetActive(true);
    }

    private void Update(){
        if (!initialMoveDone)
        {
            if (Input.GetAxis("Horizontal")!=0)
            {
                move.SetActive(false);
                jump.SetActive(true);
                initialMoveDone = true;
            }
        }

        if (initialMoveDone && !initialJumpDone) {
            if (Input.GetAxis("Vertical")!=0) {
                jump.SetActive(false);
                initialJumpDone = true;
            }
        }

        if (initialJumpDone && initialMoveDone) 
            Destroy(this);
    }
}
