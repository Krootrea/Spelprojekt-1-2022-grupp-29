using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class BackAndForthPlatform : MonoBehaviour
{
    private GameObject platform, cog, cog2;
    [SerializeField] public float directionOfPlatformX, Speed;
    private Vector3 direction, otherPosition,originalPosition;
    private bool activated;
    private PlayerController player;
    private AudioPlay audio;
    
    // Start is called before the first frame update
    void Start(){
        activated = false;
        platform = transform.GetChild(0).gameObject;
        cog = transform.GetChild(2).gameObject;
        cog2 = transform.GetChild(3).gameObject;
        audio = transform.GetChild(0).GetChild(0).gameObject.GetComponent<AudioPlay>();
        originalPosition = platform.transform.position;
        otherPosition = new Vector3(platform.transform.position.x+directionOfPlatformX, platform.transform.position.y, platform.transform.position.z);

    }

    // Update is called once per frame
    void Update(){
        WhereToGo();
        if (activated)
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, direction,Speed*Time.deltaTime);
            cog.transform.eulerAngles -= Vector3.forward * 0.6f;
            cog2.transform.eulerAngles -= Vector3.forward * 0.6f;
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
            player = col.gameObject.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerStay2D(Collider2D other){
        if (other.gameObject.CompareTag("Player") && Input.GetButtonDown("Interact"))
        {
            Activate(player);
        }
    }

    public void Activate(PlayerController player){
        if (player.gotCard)
        {
            activated = true;
            audio.PlayAudio();
        }
    }
}
