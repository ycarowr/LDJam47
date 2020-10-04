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
        /// Movement buffer
        private Vector2 _moveSpeed;
        /// Cached movement input to be used in the current Frame 
        private float _horizontalInput = 0;

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
        public bool _isGrounded;
        private bool _isLookingRight;
        private bool _hasBomb;
        private bool _isDashing;
        private Bomb _bomb;
        private Vector2 _currentDirection;

        private PlayerAnimation _playerAnimation;
        
        private void Awake()
        {
            _moveSpeed.y = 0;
            _rb = GetComponent<Rigidbody2D>();
            _bomb = Bomb.Get();
            _playerAnimation = GetComponentInChildren<PlayerAnimation>();
            groundCheck.OnNotifyCollision += (obj,  col) => _isGrounded = true;
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
            kickAction.canceled += _ => OnKick();
            dashAction.performed += _ => OnDash();
        }
        
        public void Update()
        {
            // Better jumping
            if (_rb.velocity.y < 0)
               _rb.gravityScale += fallGravityMultiplier * Time.deltaTime;
            //else if (_rb.velocity > 0 && InputBroadcaster.Input.actions["Jump"].activeControl.IsActuated())
            //{
                
            //}
            else
                _rb.gravityScale = _standardGravityMultiplier;
            Move();
            // Rotate to where it is moving
            var rotation = transform.rotation;
            if (_horizontalInput > 0)
                rotation.y = 0;
            else if (_horizontalInput < 0)
                rotation.y = 180;
            transform.rotation = rotation;
            DecideAnimation();
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
            _moveSpeed.y = _rb.velocity.y;
            if(!_isDashing)
                _moveSpeed.x = _horizontalInput * speed;
            _rb.velocity = _moveSpeed;
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
            if (!_isDashing)
            {
                _lastState = _currentState;
                _currentState = PlayerState.Dashing;
            }
        }
        
        private void ApplyDashVelocity()
        {
            _dashTime += Time.deltaTime;
            if (_dashTime > dashDuration)
            {
                _isDashing = false;
                _playerAnimation.StopDash();
                if(_currentDirection.y > 0)
                    _isGrounded = false;
            }
            else
            {
                var hasXInput = Math.Abs(_currentDirection.x) > Mathf.Epsilon;
                if (hasXInput)
                {
                    var dashAmountX = _currentDirection.x > 0 ? dashSpeed : -dashSpeed;
                    _moveSpeed.x = dashAmountX;
                }
                else
                {
                    var dashAmountY = _currentDirection.y > 0 ? dashSpeed : -dashSpeed;
                    _moveSpeed.y = dashAmountY;
                }
            }
        }
        
        #endregion
        
        #region Kick

        private IEnumerator OnKick()
        {
            yield return new WaitForEndOfFrame();
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
            if(!_isDashing)
                _currentDirection = input.ReadValue<Vector2>();
        }
        
        #endregion
    }
}
