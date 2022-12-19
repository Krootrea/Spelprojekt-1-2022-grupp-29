using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class BackAndForthPlatform : MonoBehaviour
{
    private GameObject platform;
    [SerializeField] public float directionOfPlatformX, Speed;
    private Vector3 direction, otherPosition,originalPosition;
    private bool activated;
    
    // Start is called before the first frame update
    void Start(){
        activated = false;
        platform = transform.GetChild(0).gameObject;
        originalPosition = platform.transform.position;
        otherPosition = new Vector3(platform.transform.position.x+directionOfPlatformX, platform.transform.position.y, platform.transform.position.z);

    }

    // Update is called once per frame
    void Update(){
        WhereToGo();
        if (activated)
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, direction,Speed*Time.deltaTime);
        }
    }

    private void WhereToGo(){
        if (platform.transform.position==originalPosition)
        {
            direction = otherPosition;
        }
        else if (platform.transform.position==otherPosition)
        {
            direction = originalPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Activate(col.gameObject.GetComponent<PlayerController>());
            }
        }
    }

    public void Activate(PlayerController player){
        if (player.gotCard)
        {
            activated = true;
        }
    }
}
