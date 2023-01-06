using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTurnOff : MonoBehaviour
{
    private Collider2D colliderThatPushesButton;
    public List<GameObject> ObjectsToActivate, ObjectsToDeactivate;
    // Start is called before the first frame update
    
    private void OnTriggerEnter2D(Collider2D col){
        bool isEnemy = col.transform.gameObject.TryGetComponent(typeof(Enemy), out Component comp);
        ButtonPressAction();
    }

    private void ButtonPressAction(){
        foreach (GameObject gameObject in ObjectsToDeactivate)
        {
            gameObject.SetActive(false);
        }
        foreach (GameObject gameObject in ObjectsToActivate)
        {
            gameObject.SetActive(true);
        }
    }
}
