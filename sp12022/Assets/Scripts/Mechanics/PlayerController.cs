using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

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

        public float wallJumpTimer = 0.6f, WallJumpLockTime = 0.6f;

        public JumpState jumpState = JumpState.Grounded;

        private bool stopJump;

        /*internal new*/
        public Collider2D collider2d;

        /*internal new*/
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true; 
        
        [HideInInspector]
        public bool gotCard = false, gotScrew = false;

        bool jump, justWallJumped;
        private float wjPossibleCountD, justWallJumpedCD; 
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

        protected override void Update(){
            if (wallJumpPossible && Input.GetAxis("Vertical")<0)
            {
                wallJumpPossible = false;
            }
            WallJumpCountDown();
            if (controlEnabled) 
            {
                SetDirectionInWallJump();
                if ((jumpState == JumpState.Grounded || justStartedFalling) && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                    justJumped = true;
                    if (!transform.parent.IsUnityNull())
                    {
                        transform.parent = null;
                    }
                }
                else if (Input.GetButtonDown("Jump") && wallJumpPossible) 
                {
                    jumpState = JumpState.PrepareToJump;
                    wjPossibleCountD = 0;
                    justJumped = true;
                    justWallJumped = true;
                    justWallJumpedCD = WallJumpLockTime;
                    if (!transform.parent.IsUnityNull())
                    {
                        transform.parent = null;
                    }
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
            if (justWallJumped)
            {
                if (wallSide == wallJumpSide.left)
                {
                    move.x = 1;
                }
                else if (wallSide == wallJumpSide.right)
                {
                    move.x = -1;
                }
            }
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
                        break;
                    }
                    case wallJumpSide.right:
                    {
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

        private void SetWallJumpPossible(bool possible){
            if (!justJumped && wjPossibleCountD <= 0.0f && !wallJumpPossible) 
            {
                wjPossibleCountD = possible ? wallJumpTimer : 0.0f;
                wallJumpPossible = possible;
                animator.SetBool("onWall", possible);
            }

            if (!possible)
            {
                wjPossibleCountD = 0.0f;
                wallJumpPossible = false;
                animator.SetBool("onWall", false);
            }
        }

        private void WallJumpCountDown(){
            if (wjPossibleCountD > 0.0f) {
                // wallJumpPossible = true;
                wjPossibleCountD -= Time.deltaTime;
            }

            if (wallJumpPossible && wjPossibleCountD <= 0.0f)
            {
                wallJumpPossible = false;
            }
            if (!wallJumpPossible && wjPossibleCountD > 0)
                wjPossibleCountD = 0;
            if (IsGrounded)
            {
                SetWallJumpPossible(false);
            }

            if (justWallJumped)
            {
                justWallJumpedCD -= Time.deltaTime;
                if (justWallJumpedCD<0.0f)
                {
                    justWallJumped = false;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D col){
            if (!IsGrounded && col.gameObject.layer == 3) {
                Vector2 direction = col.GetContact(0).normal;
                if (direction.x == 1) // Wall to the left
                {
                    wallSide = wallJumpSide.left;
                    SetWallJumpPossible(!IsGrounded); 
                }
                if (direction.x == -1) // Wall to the right
                {
                    wallSide = wallJumpSide.right;
                    SetWallJumpPossible(!IsGrounded); 
                }
            }

            if (col.gameObject.CompareTag("MovingPlatform"))
            {
                transform.parent = col.transform;
            }
        }

        private void OnCollisionStay2D(Collision2D collision){
            if (collision.gameObject.CompareTag("MovingPlatform") && Input.GetButtonDown("Jump"))
            {
                transform.parent = null;
            }
        }

        private void OnCollisionExit2D(Collision2D other){
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                transform.parent = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D col){
            if (col.gameObject.CompareTag("Card"))
            {
                gotCard = true;
            }
            if (col.gameObject.CompareTag("Screw"))
            {
                gotScrew = true;
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

            if (justJumped)
            {
                justJumpedCD -= Time.deltaTime;
                if (justJumpedCD<=0.0f)
                {
                    justJumped = false;
                    justJumpedCD = 0.2f;
                }
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
            }
            else if (wallJumpPossible && wallSide == wallJumpSide.right)
            {
                spriteRenderer.flipX = true;
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