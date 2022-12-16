using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class CollectiblesUI : MonoBehaviour
{
    public GameObject player;
    private GameObject whiteCard;
    private PlayerController playerController;

    private void Awake(){
        playerController = player.GetComponent<PlayerController>();
        whiteCard = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        whiteCard.SetActive(false);
    }

    private void LateUpdate(){
        if (playerController.gotCard)
        {
            whiteCard.SetActive(true);
        }
        else
        {
            whiteCard.SetActive(false);
        }
    }
}
