using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwapSpriteOnceOnTrigger : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public Sprite newSprite; 
    private Sprite originalSprite;
    private bool collided;
    
    void Start(){
        if (TryGetComponent(out SpriteRenderer sprite))
        {
            _spriteRenderer = sprite;
            originalSprite = _spriteRenderer.sprite;
        }
        else
            originalSprite = null;
        
        collided = false;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collided) {
            if (!_spriteRenderer.IsUnityNull())
                _spriteRenderer.sprite = newSprite;
            collided = true;
        }
    }
}
