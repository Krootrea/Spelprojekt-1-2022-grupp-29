using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundSpecOrder : MonoBehaviour
{
    public AudioClip sound1;
    public AudioClip sound2;
    private bool droneTriggered = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "DroneTest Shadowcasting" && !droneTriggered)
        {
            AudioSource.PlayClipAtPoint(sound1, transform.position);
            droneTriggered = true;
        }
        else if (collider.gameObject.name == "Player" && droneTriggered)
        {
            AudioSource.PlayClipAtPoint(sound2, transform.position);
            droneTriggered = false;
        }
    }
}
