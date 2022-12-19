using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class Door : MonoBehaviour
{
    public PlayerController player;
    private bool playerCloseEnough, open;
    private Vector3 upperDir, lowerDir;
    private GameObject dUpper, dLower;
    private AudioPlay audioPlay;
    // Start is called before the first frame update
    void Start(){
        audioPlay = transform.GetChild(0).gameObject.GetComponent<AudioPlay>();
        open = false;
        dUpper = gameObject.transform.GetChild(1).gameObject;
        dLower = gameObject.transform.GetChild(2).gameObject;

        upperDir = dUpper.transform.position+new Vector3(0,1);
        lowerDir = dUpper.transform.position+new Vector3(0,-2);
    }
    
    void LateUpdate()
    {
        if ((Input.GetKeyDown(KeyCode.E)) && playerCloseEnough && player.gotCard)
        {
            open = true;
            OpenDoor();
        }

        if (open)
        {
            dUpper.transform.position = Vector3.MoveTowards(dUpper.transform.position, upperDir,1.3f*Time.deltaTime);
            dLower.transform.position = Vector3.MoveTowards(dLower.transform.position, lowerDir,1.3f*Time.deltaTime);
        }
    }

    private void OpenDoor(){
        audioPlay.PlayAudio();
        dLower.GetComponent<Collider2D>().isTrigger = true;
        dUpper.GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.CompareTag("Player"))
        {
            playerCloseEnough = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Player"))
        {
            playerCloseEnough = false;
        }
    }
}
