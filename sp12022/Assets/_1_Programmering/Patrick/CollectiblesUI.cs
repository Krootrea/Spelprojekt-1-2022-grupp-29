using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using TMPro;
using UnityEngine;

public class CollectiblesUI : MonoBehaviour
{
    public GameObject player;
    private GameObject cardBackground, whiteCard;
    private PlayerController playerController;
    private TextMeshProUGUI _textMeshProUGUI;
    private int numberOfScrewsCollected;

    private void Awake(){
        numberOfScrewsCollected = 0;
        _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        playerController = player.GetComponent<PlayerController>();
        cardBackground = gameObject.transform.GetChild(0).gameObject;
        whiteCard = gameObject.transform.GetChild(1).GetChild(0).gameObject;
        cardBackground.SetActive(false);
        whiteCard.SetActive(false);
    }

    private void LateUpdate(){
        if (playerController.gotCard)
        {
            whiteCard.SetActive(true);
            cardBackground.SetActive(true);
        }
        else
        {
            whiteCard.SetActive(false);
            cardBackground.SetActive(false);
        }

        if (playerController.gotScrew)
        {
            numberOfScrewsCollected++;
            _textMeshProUGUI.text = numberOfScrewsCollected.ToString();
            playerController.gotScrew = false;
        }
    }
}
