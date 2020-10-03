using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    /*private Vector2 _currentSpeed;
    private Rigidbody2D _rigidbody2D;
    private bool _isMovingLeft;
    private bool _isMovingUp;

    [SerializeField] private float Speed = 20;
    [SerializeField] private KeyCode down = KeyCode.S;
    [SerializeField] private KeyCode up = KeyCode.W;
    [SerializeField] private KeyCode left = KeyCode.A;
    [SerializeField] private KeyCode right = KeyCode.D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _currentSpeed = Vector2.zero;

        if (Input.GetKey(down))
        {
            _currentSpeed.y = -Speed;
            _isMovingUp = false;
            DisableTutorial();
        }

        if (Input.GetKey(up))
        {
            _currentSpeed.y = Speed;
            _isMovingUp = true;
            DisableTutorial();
        }

        if (Input.GetKey(left))
        {
            _currentSpeed.x = -Speed;
            _isMovingLeft = true;
            DisableTutorial();
        }

        if (Input.GetKey(right))
        {
            _currentSpeed.x = Speed;
            _isMovingLeft = false;
            DisableTutorial();
        }

        _rigidbody2D.velocity = _currentSpeed * Time.deltaTime;

    }

    private void DisableTutorial()
    {
        var miniTutorial = MiniTutorial.Get();
        miniTutorial.Hide();
    }*/
}
