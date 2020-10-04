using System;
using System.Collections;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Input
{

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControls : MonoBehaviour
    {
        [Header("Movement Variables")] [Tooltip("Player regular movement speed")]
        public int speed;

        [Header("Dash Variables")] [Tooltip("The speed while dashing")]
        public float dashSpeed;

        [Tooltip("How long the dash lasts")]
        public float dashDuration;

        [Tooltip("How long to wait for next dash")]
        public float dashCooldown;

        [Tooltip("How long to buffer dash input")]
        public float dashInputBuffer;

        [Header("Jump Variables")] [Tooltip("The force applied to jump")]
        public int jumpForce;

        [Tooltip("How many additional gravities per second while falling")]
        public float fallGravityMultiplier;

        [Header("Bomb Grip")]
        [Tooltip("Distance to grab the bomb")]
        public float grabDistance;

        [Header("Collision checks")]
        [Tooltip("Ground Check Notifier")]
        public Trigger2DNotifier groundCheck;

        /// This game object rigid body
        private Rigidbody2D _rb;
        /// Player Animator reference
        private PlayerAnimation _playerAnimation;
        
        /// Movement buffer
        private Vector2 _moveSpeed;
        /// Movement horizontal input 
        private float _horizontalInput = 0;
        /// Timestamp of last dash
        private float _lastDashTime;
        /// True if the player is on the ground or haven't dashed since left the ground
        private bool _canDash;

        /// Player Animation State 
        enum PlayerState
        {
            None,
            Moving,
            Idle,
            Dashing,
            Jumping,
            Falling
        }
        private PlayerState _lastState = PlayerState.None;
        private PlayerState _currentState = PlayerState.None;
        
        /// Gravity multiplier when not jumping or falling 
        private float _standardGravityMultiplier;

        private float _dashTime;
        private bool _isGrounded;
        private bool _hasBomb;
        private Bomb _bomb;
        private Vector2 _currentDirection;

        private void Awake()
        {
            _moveSpeed.y = 0;
            _rb = GetComponent<Rigidbody2D>();
            _bomb = Bomb.Get();
            _playerAnimation = GetComponentInChildren<PlayerAnimation>();
            groundCheck.OnNotifyCollision += (obj, col) =>
            {
                _isGrounded = true;
                _canDash = true;
            };
            _standardGravityMultiplier = _rb.gravityScale;
        }
        
        private void Start()
        {
            var kickAction = InputBroadcaster.Input.actions["Kick"];
            var moveAction = InputBroadcaster.Input.actions["Move"];
            var directionAction = InputBroadcaster.Input.actions["Direction"];
            var jumpAction = InputBroadcaster.Input.actions["Jump"];
            var dashAction = InputBroadcaster.Input.actions["Dash"];
            
            moveAction.performed += (ctx) => _horizontalInput = ctx.ReadValue<float>();
            moveAction.canceled += (ctx) => _horizontalInput = ctx.ReadValue<float>();

            directionAction.performed += SetDirection;
            directionAction.canceled += SetDirection;

            jumpAction.performed += _ => OnJump();
            kickAction.performed += _ => OnKick();
            dashAction.performed += _ => OnDash();
        }
        
        public void Update()
        {
            if (_currentState == PlayerState.Dashing)
                return;
            Move();
            DecideAnimation();
            RotateToMovement();
        }

        private void RotateToMovement()
        {
            var rotation = transform.rotation;
            if (_horizontalInput > 0)
                rotation.y = 0;
            else if (_horizontalInput < 0)
                rotation.y = 180;
            transform.rotation = rotation;
        }
        
        private void DecideAnimation()
        {
            // If Player is dashing, it is locked in the animation, otherwise...
            if (_currentState != PlayerState.Dashing)
            {
                // Calculate the current state
                _lastState = _currentState;
                if (_rb.velocity.y > 0)
                    _currentState = PlayerState.Jumping;
                else if (_rb.velocity.y < 0)
                    _currentState = PlayerState.Falling;
                else if (_horizontalInput == 0)
                    _currentState = PlayerState.Idle;
                else
                {
                    // Rotate the character to match the direction it is moving
                    _currentState = PlayerState.Moving;
                }
                
                // Check if there is need to change the animation
                if (_currentState == _lastState)
                    return;

                switch (_currentState)
                {
                    case PlayerState.Dashing:
                        _playerAnimation.Dash();
                        break;
                    case PlayerState.Jumping:
                        _playerAnimation.Jump();
                        break;
                    case PlayerState.Moving:
                        _playerAnimation.RunRight();
                        break;
                    case PlayerState.Idle:
                        _playerAnimation.Idle();
                        break;
                }
            }
        }

        #region Movement

        private void Move()
        {
            // Standard non-dashing movement
            _moveSpeed.y = _rb.velocity.y;
            _moveSpeed.x = _horizontalInput * speed;
            _rb.velocity = _moveSpeed;
            // Better jumping
            if (_rb.velocity.y < 0)
               _rb.gravityScale += fallGravityMultiplier * Time.deltaTime;
            else
                _rb.gravityScale = _standardGravityMultiplier;
        }
        
        #endregion

        #region Jump        

        private void OnJump()
        {
            if (_isGrounded)
            {
                _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _isGrounded = false;
            }
        }

        #endregion
        
        #region Dash

        private void OnDash()
        {
            var cd = Time.time - _lastDashTime;
            if (cd >= 0 && _canDash)
            {
                if (!_isGrounded)
                    _canDash = false;
                _lastState = _currentState;
                _currentState = PlayerState.Dashing;
                _rb.velocity = dashSpeed * _currentDirection;
                _lastDashTime = Time.time;
                StartCoroutine(PerformDash());
            }
        }
        
        IEnumerator PerformDash()
        {
            _playerAnimation.Dash();
            _rb.gravityScale = 0;
            yield return new WaitForSeconds(dashDuration);
            _rb.gravityScale = _standardGravityMultiplier;
            _lastState = PlayerState.Dashing;
            _currentState = PlayerState.Idle;
            _rb.velocity *= Vector2.right;
            _playerAnimation.StopDash();
        }

        #endregion
        
        #region Kick

        private void OnKick()
        {
            if (!_hasBomb)
            {
                if (CalcDistanaceToBomb() < grabDistance)
                {
                    _bomb.Grab();
                    _hasBomb = true;
                }
            }
            else
            {
                var isRight = _playerAnimation.IsLookingRight();
                _bomb.Kick(isRight);
                _hasBomb = false;
            }
        }

        private float CalcDistanaceToBomb()
        {
            return Vector2.Distance(_bomb.transform.position, transform.position);
        }
        
        #endregion

        #region Setters

        private void SetDirection(InputAction.CallbackContext input)
        {
            if(_currentState != PlayerState.Dashing)
                _currentDirection = input.ReadValue<Vector2>();
        }
        
        #endregion
    }
}
