using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerEventSound : MonoBehaviour
{
    public AudioSource wallSound;
    public AudioSource slideSound;
    private Animator anim;
    private bool onWall = false;
    private bool soundPlayed = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        onWall = anim.GetCurrentAnimatorStateInfo(0).IsName("Player-OnWall");
        if (onWall && !soundPlayed)
        {
            wallSound.Play();
            soundPlayed = true;
        }
        else if(!onWall)
        {
            soundPlayed = false;
        }

        if(onWall && (Input.GetKeyDown(KeyCode.S) || Input.GetAxis("Slide") < 0))
        {
            slideSound.Play();
        }
    }
}
