using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonGeneral : MonoBehaviour
{
    private Collider2D colliderThatPushesButton;
    public List<GameObject> ObjectsToActivate;
    public GameObject GameobjectThatCanActivateButton;
    private bool pushed;
    public bool IsButtonPushed => pushed;

    private void Awake(){
        colliderThatPushesButton = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col){
        bool isEnemy = col.transform.gameObject.TryGetComponent(typeof(Enemy), out Component comp);
        ButtonPressAction(isEnemy);
        // if (!GameobjectThatCanActivateButton.IsUnityNull() && col.transform.gameObject.Equals(GameobjectThatCanActivateButton)) { 
        //     ButtonPressAction(isEnemy);   
        // }
    }

    private void ButtonPressAction(bool onOrOff){
        pushed = onOrOff;
        foreach (GameObject gameObject in ObjectsToActivate) {
            gameObject.SetActive(onOrOff);
        }
    }
}
