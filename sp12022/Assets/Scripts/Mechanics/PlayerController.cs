using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Unity.Mathematics;
using UnityEngine.Tilemaps;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        private enum wallJumpSide{left, right, noSide}

        private wallJumpSide wallSide;
        
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;

        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public float wallJumpTimer = 0.6f;

        public JumpState jumpState = JumpState.Grounded;

        private bool stopJump;

        /*internal new*/
        public Collider2D collider2d;

        /*internal new*/
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true; 
        
        [HideInInspector]
        public bool gotCard = false;

        bool jump;
        private float wjPossibleCountD;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;
        public TilemapCollider2D tilesCollider;

        void Awake(){
            wallSide = wallJumpSide.noSide;
            wjPossibleCountD = 0f;
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            WallJumpCountDown();
            if (controlEnabled) 
            {
                SetDirectionInWallJump();
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                }
                else if (Input.GetButtonDown("Jump") && wallJumpPossible) 
                {
                    jumpState = JumpState.PrepareToJump;
                    wjPossibleCountD = 0;
                }
                else if (Input.GetButtonUp("Jump")) 
                {
                    // stopJump = true;
                    // Schedule<PlayerStopJump>().player = this;
                }
            }
            else 
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        private void SetDirectionInWallJump(){
            if (wallJumpPossible)
            {
                move.x = 0;
                switch (wallSide)
                {
                    case wallJumpSide.left:
                    {
                        // Rotera till höger
                        break;
                    }
                    case wallJumpSide.right:
                    {
                        // Rotera till vänster
                        break;
                    }
                    case wallJumpSide.noSide : break;
                }
            }
            else
            {
                move.x = Input.GetAxis("Horizontal");
            }
            
        }

        private void SetWallJumpPossible(){
            if (wjPossibleCountD <= 0.0f && !wallJumpPossible) 
            {
                wjPossibleCountD = wallJumpTimer;
                wallJumpPossible = true;
                animator.SetBool("onWall", true);
            }
        }

        private void WallJumpCountDown(){
            if (wjPossibleCountD > 0.0f) {
                wallJumpPossible = true;
                wjPossibleCountD -= Time.deltaTime;
            }

            if (wallJumpPossible && wjPossibleCountD <= 0.0f)
            {
                wallJumpPossible = false;
            }
            if (!wallJumpPossible && wjPossibleCountD > 0)
                wjPossibleCountD = 0;
        }

        private void OnCollisionEnter2D(Collision2D col){
            if (!IsGrounded && col.gameObject.layer == 3) {
                Vector2 direction = col.GetContact(0).normal;
                if (direction.x == 1) // Wall to the left
                {
                    wallSide = wallJumpSide.left;
                    SetWallJumpPossible(); 
                }
                if (direction.x == -1) // Wall to the right
                {
                    wallSide = wallJumpSide.right;
                    SetWallJumpPossible(); 
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D col){
            if (col.gameObject.CompareTag("Card"))
            {
                gotCard = true;
            }
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded) {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded) {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && (IsGrounded||wallJumpPossible)) {
                wallJumpPossible = false;
                animator.SetBool("onWall", false);
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump) {
                stopJump = false;
                if (wallJumpPossible) { }// Wall jump possible, no gravity while possible.
                else if (velocity.y > 0) {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (wallJumpPossible && wallSide == wallJumpSide.left)
            {
                spriteRenderer.flipX = false;
                Debug.Log("vägg till vänster");
            }
            else if (wallJumpPossible && wallSide == wallJumpSide.right)
            {
                spriteRenderer.flipX = true;
                Debug.Log("vägg till höger");
            }
            else if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}