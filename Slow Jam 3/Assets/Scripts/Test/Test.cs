using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Test : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler _input;
    private Rigidbody2D _rigidbody;

    [SerializeField] private float _maxGroundAngle = 60;
    [SerializeField] private float _groundAccel = 50f;
    [SerializeField] private float _airAccel = 20f;
    [SerializeField] private float _maxSpeed = 5;
    [SerializeField] private float _maxAirSpeed = 2;
    [SerializeField] private int _extraJumps = 1;
    [SerializeField] private float _jumpHeight = 10;
    [SerializeField] private float _dashSpeed = 10;
    [SerializeField] private bool _useGravity = true;

    /// <summary>
    /// the time to wait before applying gravity again
    /// </summary>
    [SerializeField] private float _dashTime = 0.5f;

    [SerializeField] private int _totalDashes = 1;

    [SerializeField] private float _gravity = -9;
    [SerializeField] private bool _isDashing;
    [SerializeField] private bool _isGrounded = false;

    [SerializeField, Header("Visuals")] private ParticleSystem _jumpParticlySystem;


    private float _targetJump;
    private Vector2 _dashDirection;
    private int _currentDashCount;
    private int _currentJumpCount;
    private float _dashEndTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _input.Player.Dash.performed += OnDash;
        _input.Player.Jump.performed += OnJump;

        void OnJump(InputAction.CallbackContext obj)
        {
            if (_isGrounded)
            {
                _targetJump = _jumpHeight;
                _jumpParticlySystem.Emit(10);
            }

            if (_isGrounded is false && _currentJumpCount > 0)
            {
                _jumpParticlySystem.Emit(10);
                _currentJumpCount--;
                _targetJump = _jumpHeight;
            }
        }

        void OnDash(InputAction.CallbackContext obj)
        {
            if (_currentDashCount == 0)
            {
                return;
            }

            _currentDashCount--;

            _isDashing = true;
            _dashEndTime = Time.time + _dashTime;

            _dashDirection = _input.Player.LeftStickDirection.ReadValue<Vector2>();
            Debug.Log(_dashDirection);
        }
    }


    private void OnFirstGroundTouch()
    {
        _currentDashCount = _totalDashes;
        _currentJumpCount = _extraJumps;
    }


    private List<Collider2D> _groundContacts = new();

    private void OnCollisionEnter2D(Collision2D other)
    {
        var contacts = new ContactPoint2D[10];
        var count = other.GetContacts(contacts);

        for (int i = 0; i < count; i++)
        {
            var angle = Vector2.Angle(Vector2.up, contacts[i].normal);

            if (angle < _maxGroundAngle)
            {
                if (_groundContacts.Count == 0)
                {
                    //we just touched the ground 
                    OnFirstGroundTouch();
                }

                _groundContacts.Add(other.collider);
                _isGrounded = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _groundContacts.Remove(other.collider);
        if (_groundContacts.Count == 0)
        {
            _isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        var movement = _input.Player.Move.ReadValue<float>();
        var velocity = _rigidbody.linearVelocity;
        var groundTarget = movement * _maxSpeed;
        var airTarget = movement * _maxSpeed;

        if (Time.time >= _dashEndTime)
        {
            _isDashing = false;
        }

        if (_isGrounded)
        {
            // ground movement
            velocity.x = Mathf.MoveTowards(velocity.x, groundTarget, _groundAccel * Time.deltaTime);
        }
        else
        {
            // air movement
            if (_isDashing)
            {
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, airTarget, _airAccel * Time.deltaTime);
            }
        }

        if (_targetJump != 0)
        {
            velocity.y = Mathf.Sqrt(-1f * _gravity * _targetJump);
        }

        if (_dashDirection.magnitude != 0)
        {
            velocity = _dashDirection * _dashSpeed;
        }

        if (_useGravity && _isDashing is false)
        {
            velocity.y += _gravity * Time.deltaTime;
        }


        _rigidbody.linearVelocity = velocity;

        _dashDirection = Vector2.zero;
        _targetJump = 0;
    }
}