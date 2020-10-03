using System;
using UnityEngine;

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

        [Header("Kick Variables")] [Tooltip("The Y velocity after a kick")]
        public float kickSpeed;

        [Tooltip("How long the kick disables gravity for the bomb")]
        public float kickDuration;

        [Tooltip("How long to buffer kick input")]
        public float kickInputBuffer;

        /// Flags that impact the jump input
        [Flags]
        private enum PlayerFlags
        {
            None = 0,
            IsGrounded = 1,

            /// If player is grounded
            IsWalled = 2,

            /// If player is connected to a wall
            IsCarryingBomb = 4 /// If player is carrying the bomb
        }

        /// This game object rigid body
        private Rigidbody2D _rb;

        /// Movement buffer
        private Vector2 _moveTo;

        /// Binary Combination of all player flags (e.g IsCarryingBomb + IsWalled = 6)  
        private PlayerFlags _playerFlags = 0;

        /// Cached movement input to be used in the current Frame 
        private float _moveInput = 0;

        /// If the dash action was triggred this Frame
        private bool _dashTrigger = false;

        /// If the kick action was triggred this Frame
        private bool _kickTrigger = false;

        /// Cached direction input to be used in the current Frame
        private Vector2 _directionInput;

        /// Timestamp of the last player dash
        private float _lastDash = 0;

        private float _jumpTime;
        private bool _isGrounded;
        private bool _isJumping;

        private void Start()
        {
            var moveAction = InputBroadcaster.Input.actions["Move"];
            var directionAction = InputBroadcaster.Input.actions["Direction"];
            var jumpAction = InputBroadcaster.Input.actions["Jump"];
            var kickAction = InputBroadcaster.Input.actions["Kick"];
            var dashAction = InputBroadcaster.Input.actions["Dash"];

            // Cache the movement input
            moveAction.performed += (ctx) => _moveInput = ctx.ReadValue<float>();
            moveAction.canceled += (ctx) => _moveInput = ctx.ReadValue<float>();

            // Cache the direction input
            directionAction.performed += (ctx) => _directionInput = ctx.ReadValue<Vector2>();
            directionAction.canceled += (ctx) => _directionInput = ctx.ReadValue<Vector2>();

            // Register the jump input (Ignore the CallbackContext)
            jumpAction.performed += _ => OnJump();
            // Register the kick input (Ignore the CallbackContext)
            kickAction.performed += _ => OnKick();
            // Register the dash input (Ignore the CallbackContext)
            dashAction.performed += _ => OnDash();

            _rb = GetComponent<Rigidbody2D>();
            _moveTo.y = 0;
        }

        public void Update()
        {
            ApplyVerticalVelocity();
            Move();
        }

        private void ApplyVerticalVelocity()
        {
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
                }
            }
            else
            {
                _moveTo.y = fallSpeed;
            }
        }

        private void Move()
        {
            _moveTo.x = _moveInput * speed;
            _rb.velocity = _moveTo;
        }

        private void OnJump()
        {
            if (_isGrounded)
            {
                _isGrounded = false;
                _isJumping = true;
            }
        }

        private void OnDash()
        {
            var time = Time.time - (_lastDash + dashCooldown);
            // Input buffer logic
            if (time > 0)
            {
                _dashTrigger = time <= dashInputBuffer;
                return;
            }

            _dashTrigger = false;
            // Do the actual dash pointing to direction _directionInput
        }

        private void OnKick()
        {
            var f = _playerFlags | PlayerFlags.IsCarryingBomb;
            _kickTrigger = false;
            if (f == PlayerFlags.None)
                return;
            // Do the actual kicking logic
        }

        public void SetIsGrounded(bool value)
        {
            _isGrounded = value;
            _isJumping = false;
            _jumpTime = 0;
        }
    }
}
