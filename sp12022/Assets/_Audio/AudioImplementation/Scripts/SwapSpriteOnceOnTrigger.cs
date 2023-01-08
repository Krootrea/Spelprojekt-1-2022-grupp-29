using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSpriteOnceOnTrigger : MonoBehaviour
{
    public Sprite newSprite; 
    private Sprite originalSprite;
    private bool collided;
    
    void Start()
    {
        originalSprite = GetComponent<SpriteRenderer>().sprite;
        collided = false;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collided) {
            GetComponent<SpriteRenderer>().sprite = newSprite;
            collided = true;
        }
    }
}
