using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExit : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D collider;
    private RoomHandler room;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        room = null;
        transform.parent.gameObject.TryGetComponent<RoomHandler>(out room);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && room!=null)
            room.PlayerLeft();
    }

}
