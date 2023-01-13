using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTurnOff : MonoBehaviour, IResetOnRespawn
{
    private Collider2D colliderThatPushesButton;
    public List<GameObject> ObjectsToActivate, ObjectsToDeactivate;

    private GameObject lit, unlit;
    // Start is called before the first frame update
    private void Awake(){
        lit = transform.Find("ButtonLit").gameObject;
        unlit = transform.Find("ButtonUnlit").gameObject;
    }

    private void OnTriggerEnter2D(Collider2D col){
        if (col.CompareTag("Player"))
            ButtonPressAction();
    }

    private void ButtonPressAction(){
        unlit.SetActive(false);
        lit.SetActive(true);
        foreach (GameObject gameObject in ObjectsToDeactivate)
        {
            gameObject.SetActive(false);
        }
        foreach (GameObject gameObject in ObjectsToActivate)
        {
            gameObject.SetActive(true);
        }
    }

    public void RespawnReset(){
        foreach (GameObject gameObject in ObjectsToActivate)
        {
            gameObject.SetActive(false);
        }
        foreach (GameObject gameObject in ObjectsToDeactivate)
        {
            gameObject.SetActive(true);
        }
    }
}
