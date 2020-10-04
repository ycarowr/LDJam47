using System;
using System.Collections;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControls : MonoBehaviour
    {
        [Header("Movement Variables")] [Tooltip("Player regular movement speed")]
        public int speed;

        [Header("Dash Variables")] [Tooltip("The speed while dashing")]
        public float dashSpeed;

        [Tooltip("How long the dash lasts")] public float dashDuration;

        [Tooltip("How long to wait for next dash")]
        public float dashCooldown;

        [Tooltip("How long to buffer dash input")]
        public float dashInputBuffer;

        [Header("Jump Variables")] [Tooltip("The Y velocity after a jump")]
        public float jumpSpeed;

        [Header("Jump Variables")] [Tooltip("The Y velocity after a jump")]
        public float fallSpeed;

        [Tooltip("How long the jump disables gravity")]
        public float jumpDuration;

        [Header("Bomb Grip")]
        [Tooltip("Distance to grab the bomb")]
        public float grabDistance;

        /// This game object rigid body
        private Rigidbody2D _rb;

        /// Movement buffer
        private Vector2 _moveTo;

        /// Cached movement input to be used in the current Frame 
        private float _moveInput = 0;

        private float _jumpTime;
        private float _dashTime;
        private bool _isGrounded;
        private bool _isJumping;
        private bool _isLookingRight;
        private bool _hasBomb;
        private bool _isDashing;
        private Bomb _bomb;
        private Vector2 _currentDirection;

        private PlayerAnimation _playerAnimation;
        
        private void Awake()
        {
            _moveTo.y = 0;
            _rb = GetComponent<Rigidbody2D>();
            _bomb = Bomb.Get();
            _playerAnimation = GetComponentInChildren<PlayerAnimation>();
        }
        
        private void Start()
        {
            var kickAction = InputBroadcaster.Input.actions["Kick"];
            var moveAction = InputBroadcaster.Input.actions["Move"];
            var directionAction = InputBroadcaster.Input.actions["Direction"];
            var jumpAction = InputBroadcaster.Input.actions["Jump"];
            var dashAction = InputBroadcaster.Input.actions["Dash"];
            
            moveAction.performed += (ctx) => _moveInput = ctx.ReadValue<float>();
            moveAction.canceled += (ctx) => _moveInput = ctx.ReadValue<float>();

            directionAction.performed += SetDirection;
            directionAction.canceled += SetDirection;

            jumpAction.performed += _ => OnJump();
            kickAction.canceled += _ => OnKick();
            dashAction.performed += _ => OnDash();
        }

        private void SetDirection(InputAction.CallbackContext input)
        {
            if(!_isDashing)
                _currentDirection = input.ReadValue<Vector2>();
        }
        
        public void Update()
        {
            ApplyDashVelocity();
            ApplyVerticalVelocity();
            Move();
            HandleMoveAnimation();
        }

        private void ApplyDashVelocity()
        {
            if (!_isDashing)
                return;
            
            _dashTime += Time.deltaTime;
            if (_dashTime > dashDuration)
            {
                _isDashing = false;
                _isJumping = false;
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
                    _moveTo.x = dashAmountX;
                }
                else
                {
                    var dashAmountY = _currentDirection.y > 0 ? dashSpeed : -dashSpeed;
                    _moveTo.y = dashAmountY;
                }
            }
        }

        private void ApplyVerticalVelocity()
        {
            if (_isDashing)
                return;
            
            if (_isGrounded)
            {
                _moveTo.y = 0;
                return;
            }
            
            if (_isJumping)
            {
                _moveTo.y = jumpSpeed;
                _jumpTime += Time.deltaTime;
                if (_jumpTime > jumpDuration)
                {
                    _isJumping = false;
                    _moveTo.y = fallSpeed;
                    _playerAnimation.Fall();
                }
            }
            else
            {
                _moveTo.y = fallSpeed;
            }
        }

        private void Move()
        {
            if(!_isDashing)
                _moveTo.x = _moveInput * speed;
            _rb.velocity = _moveTo;
        }

        private void HandleMoveAnimation()
        {
            if (_isDashing || _isJumping)
                return;

            if(_rb.velocity.magnitude < Mathf.Epsilon)
                _playerAnimation.Idle();

            if(_rb.velocity.x > 0)
                _playerAnimation.RunRight();

            if(_rb.velocity.x < 0)
                _playerAnimation.RunLeft();
        }

        private void OnJump()
        {
            if (_isGrounded)
            {
                _isGrounded = false;
                _isJumping = true;
                _playerAnimation.Jump();
            }
        }

        private void OnDash()
        {
            if (!_isDashing)
            {
                _isDashing = true;
                _dashTime = 0;
                _playerAnimation.Dash();
            }
        }

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
                var isRight = _moveInput > 0;
                _bomb.Kick(isRight);
                _hasBomb = false;
            }
        }

        private float CalcDistanaceToBomb()
        {
            return Vector2.Distance(_bomb.transform.position, transform.position);
        }

        public void SetIsGrounded(bool value)
        {
            _isGrounded = value;
            _isJumping = false;
            _jumpTime = 0;
            _playerAnimation.Idle();
        }
    }
}
