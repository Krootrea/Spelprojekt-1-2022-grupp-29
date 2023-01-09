using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartingUIInfo : MonoBehaviour
{
    private GameObject move, jump, friend, currentTalkBubble;
    private Queue<GameObject> talkBubbles;
    private bool initialMoveDone, initialJumpDone;
    private void Awake(){
        currentTalkBubble = null;
        move = transform.GetChild(0).gameObject;
        jump = transform.GetChild(1).gameObject;
        friend = transform.Find("Friend").gameObject;
        move.SetActive(true);
        talkBubbles = new Queue<GameObject>();
        FillTalkBubbles();
        HandleTalk();
    }

    private void FillTalkBubbles(){
        foreach (RectTransform bubble in friend.transform)
            talkBubbles.Enqueue(bubble.gameObject);
    }

    private void Update(){
        if (!initialMoveDone)
        {
            if (Input.GetAxis("Horizontal")!=0)
            {
                move.SetActive(false);
                jump.SetActive(true);
                initialMoveDone = true;
            }
        }
        if ((Input.GetKeyUp(KeyCode.E) || Input.GetButtonUp("Interact")))
            HandleTalk();

        if (initialMoveDone && !initialJumpDone) {
            if (Input.GetButtonDown("Jump")) {
                jump.SetActive(false);
                initialJumpDone = true;
            }
        }

        if (initialJumpDone && initialMoveDone && !friend.activeSelf) 
            Destroy(this);
    }

    private void HandleTalk(){
        if (!currentTalkBubble.IsUnityNull())
        {
               currentTalkBubble.gameObject.SetActive(false);
               currentTalkBubble = null;
        }
        if (talkBubbles.Count>0)
        {
            currentTalkBubble = talkBubbles.Dequeue();
            currentTalkBubble.SetActive(true);
        }
        else
            friend.SetActive(false);
    }
}
