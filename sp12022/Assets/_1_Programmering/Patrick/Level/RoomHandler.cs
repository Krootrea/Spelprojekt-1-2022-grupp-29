using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class RoomHandler : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public bool startRoom = false;
    private RoomExit roomExit;

    // Start is called before the first frame update
    void Start()
    {
        roomExit = null;
        if(transform.Find("RoomExit").TryGetComponent<RoomExit>(out roomExit)){}
        else{Debug.Log("Missing RoomExit");}
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        SetRoomObjects(startRoom);
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player")){
            SetRoomObjects(true);
        }
    }

    public void PlayerLeft(){
        SetRoomObjects(false);
    }

    private void SetRoomObjects(bool val){
        foreach(Transform child in transform){
            child.gameObject.SetActive(val);
        }
    }
}
