using System;
using System.Collections.Generic;
using Player;
using Player.Checkpoints.Serialization;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static PowerUp;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler _input;
    private Rigidbody2D _rigidbody;

    [SerializeField, SerializeData] private float _maxGroundAngle = 60;
    [SerializeField, SerializeData] private float _groundAccel = 50f;
    [SerializeField, SerializeData] private float _airAccel = 20f;
    [SerializeField, SerializeData] private float _maxSpeed = 5;
    [SerializeField, SerializeData] private float _maxAirSpeed = 2;
    [SerializeField, SerializeData] private int _extraJumps = 0;
    [SerializeField, SerializeData] private float _jumpHeight = 10;
    [SerializeField, SerializeData] private float _dashSpeed = 10;
    [SerializeField, SerializeData] private bool _useGravity = true;
    [SerializeField, SerializeData] private Abilities _activeAbilities = Abilities.All;

    [SerializeField, SerializeData] private float _climbAccel = 50;
    [SerializeField, SerializeData] private float _maxClimbSpeed = 5;

    [SerializeField, SerializeData] private float _maxGlideTime = 1f;
    [SerializeField, SerializeData] private float _glideSlowdown = 0.4f;

    [SerializeField] private bool _isHovering;
    // bool is start/stop

    [SerializeField] private UnityEvent _onJump;
    [SerializeField] private UnityEvent _onHover;
    [SerializeField] private UnityEvent _onDash;
    [SerializeField] private UnityEvent _onClimb;
    [SerializeField] private UnityEvent _onGroundTouch;
    [SerializeField] private UnityEvent _startWalkSound;
    [SerializeField] private UnityEvent _stopWalkSound;

    [SerializeField] private UnityEvent _endClimb;
    [SerializeField] private UnityEvent _endHover;


    /// <summary>
    /// the time to wait before applying gravity again
    /// </summary>
    [SerializeField] private float _dashTime = 0.5f;

    [SerializeField] private float _glideTime;

    [SerializeField] private int _totalDashes = 1;

    [SerializeField] private float _gravity = -9;
    [SerializeField] private bool _isDashing;
    [SerializeField] private bool _isGrounded = false;

    [SerializeField, Header("Key UI")] public int keyCount = 0;
    [SerializeField] Text keysText;

    [FormerlySerializedAs("_jumpParticlySystem")] [SerializeField, Header("Visuals")]
    private ParticleSystem _jumpParticleSystem;

    [SerializeField] private ParticleSystem _dashParticleSystem;

    [SerializeField, Header("Collision")] private float _groundSnapDistance = 0.5f;
    [SerializeField] private LayerMask _groundSnapLayerMask;
    [SerializeField] private int _groundContactCount;

    [SerializeField, Header("Debug")] private bool _showDebugLines = false;

    // TODO: if we need more movement states comeback and do some statey like machine or some component based approach to seperarate
    [SerializeField] private bool _isClimbing = false;
    private float _climbDirection = 0;

    private float _targetJump;
    private Vector2 _dashDirection;
    private int _currentDashCount;
    private int _currentJumpCount;
    private float _dashEndTime;
    private int _stepsSinceLastJump;
    [SerializeField] private int _stepsSinceGroundContact;

    private bool _firstHover = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Assert.AreEqual(gameObject.layer, LayerMask.NameToLayer("Player"));

        _rigidbody = GetComponent<Rigidbody2D>();
        _input.Player.Dash.performed += OnDash;
        _input.Player.Jump.performed += OnJump;
        _input.Player.Climb.performed += OnClimb;
        _input.Player.Climb.canceled += OnClimbCanceled;

        void OnClimbCanceled(InputAction.CallbackContext obj)
        {
            _climbDirection = 0;
        }

        void OnClimb(InputAction.CallbackContext obj)
        {
            _climbDirection = obj.ReadValue<float>();
        }

        void OnJump(InputAction.CallbackContext obj)
        {
            if (!_activeAbilities.HasFlag(Abilities.Jump))
            {
                return;
            }

            if (_isGrounded)
            {
                _onJump.Invoke();
                _useGravity = true;
                _targetJump = _jumpHeight;

                //glide only resets on ground based jumps
                _glideTime = 0;
                if (_jumpParticleSystem != null)
                {
                    _jumpParticleSystem.Emit(10);
                }
            }

            if (_activeAbilities.HasFlag(Abilities.DoubleJump) && _isGrounded is false && _currentJumpCount > 0)
            {
                _onJump.Invoke();
                _useGravity = true;
                if (_jumpParticleSystem != null)
                {
                    _jumpParticleSystem.Emit(10);
                }

                _currentJumpCount--;
                _targetJump = _jumpHeight;
            }
        }

        void OnDash(InputAction.CallbackContext obj)
        {
            if (_isGrounded || !_activeAbilities.HasFlag(Abilities.Dash))
            {
                return;
            }

            if (_currentDashCount == 0)
            {
                return;
            }

            _onDash.Invoke();
            _currentDashCount--;

            if (_dashParticleSystem != null)
            {
                _dashParticleSystem.Play();
            }

            _onDash.Invoke();
            _isDashing = true;
            _dashEndTime = Time.time + _dashTime;

            _dashDirection = _input.Player.LeftStickDirection.ReadValue<Vector2>();
        }
    }


    public void AddExtraJump()
    {
        _extraJumps++;
    }

    private void OnFirstClimb()
    {
        _rigidbody.linearVelocity = Vector2.zero;
    }

    private void GroundTouch()
    {
        _currentDashCount = _totalDashes;
        _currentJumpCount = _extraJumps;
        _useGravity = false;
        _isGrounded = true;
    }

    private void OnDashEnd()
    {
        if (_dashParticleSystem != null)
        {
            _dashParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }


    // dont do this, it breaks when using a single collider.
    //private HashSet<Collider2D> _groundContacts = new();
    private HashSet<Vine> _vineContacts = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        var vine = other.gameObject.GetComponent<Vine>();
        if (vine is not null)
        {
            if (_groundContactCount == 0)
            {
                OnFirstClimb();
            }

            _groundContactCount++;
            _isClimbing = true;
            _useGravity = false;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        var vine = other.gameObject.GetComponent<Vine>();
        if (vine is not null)
        {
            _vineContacts.Remove(vine);
            if (_vineContacts.Count == 0)
            {
                _isClimbing = false;
                if (_isGrounded is false)
                {
                    _useGravity = true;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var contacts = new ContactPoint2D[10];
        var count = other.GetContacts(contacts);

        for (int i = 0; i < count; i++)
        {
            if (IsNormalGround(contacts[i].normal))
            {
                // if (_groundContactCount == 0)
                {
                    GroundTouch();
                }


                _isGrounded = true;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        _isGrounded = false;
        _useGravity = true;

        var contacts = new ContactPoint2D[10];
        var count = other.GetContacts(contacts);

        for (int i = 0; i < count; i++)
        {
            if (IsNormalGround(contacts[i].normal))
            {
                if (_showDebugLines)
                {
                    Debug.DrawLine(contacts[i].point, contacts[i].point + contacts[i].normal, Color.green, 0.5f);
                }

                GroundTouch();
                _currentJumpCount = _extraJumps;
                _isGrounded = true;
                _useGravity = false;
            }

            else
            {
                if (_showDebugLines)
                {
                    Debug.DrawLine(contacts[i].point, contacts[i].point + contacts[i].normal, Color.purple, 0.5f);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // Debug.Log($"On Exit: {other.collider.name}");
        // since we are evaluating on stay setting this to false every exit should be fine?
        _isGrounded = false;
        _useGravity = true;
    }

    /*private void OnCollisionExit2D(Collision2D other)
    {
        var contacts = new ContactPoint2D[10];
        var count = other.GetContacts(contacts);

        for (int i = 0; i < count; i++)
        {
            if (IsNormalGround(contacts[i].normal))
            {
                _groundContactCount--;
                if (_groundContactCount == 0)
                {
                    _isGrounded = false;
                    _useGravity = true;
                }
            }
        }
    }
    */

    public void EnableAbilities(Abilities abilityFlags)
    {
        _activeAbilities |= abilityFlags;
    }

    public void DisableAbilities(Abilities abilityFlags)
    {
        _activeAbilities &= ~abilityFlags;
    }

    private bool IsNormalGround(Vector2 normal)
    {
        var angle = Vector2.Angle(Vector2.up, normal);

        if (angle < _maxGroundAngle)
        {
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (keysText != null)
        {
            keysText.text = "Keys: " + $"{keyCount}";
        }
    }

    private void FixedUpdate()
    {
        // all of the state if checks are so bad, but i dont have time to make it "right"

        var movement = _input.Player.Move.ReadValue<float>();
        var velocity = _rigidbody.linearVelocity;
        var groundTarget = movement * _maxSpeed;
        var airTarget = movement * _maxAirSpeed;
        var climbTarget = _climbDirection * _maxClimbSpeed;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (_showDebugLines)
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction.normalized * _groundSnapDistance, Color.red);
        }


        if (Time.time >= _dashEndTime)
        {
            // dash has ended
            _isDashing = false;
            OnDashEnd();
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
            _stepsSinceLastJump = 0;
        }

        if (_dashDirection.magnitude != 0)
        {
            velocity = _dashDirection * _dashSpeed;
        }


        if (_stepsSinceLastJump > 1 && _isGrounded)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, _groundSnapDistance, _groundSnapLayerMask);

            if (hit.collider is not null)
            {
                if (IsNormalGround(hit.normal))
                {
                    float dot = Vector2.Dot(velocity, hit.normal);

                    // if (dot > 0)
                    {
                        velocity = (velocity - hit.normal * dot).normalized * velocity.magnitude;
                        velocity = Vector2.ClampMagnitude(velocity, _maxClimbSpeed);
                    }
                }
            }
        }

        // apply gravity last
        if (_useGravity && _isDashing is false && _isClimbing is false)
        {
            velocity.y += _gravity * Time.deltaTime;
        }

        if (_isClimbing)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, climbTarget, _groundAccel * Time.deltaTime);
        }

        if (_activeAbilities.HasFlag(Abilities.Hover) && _isGrounded is false && _glideTime <= _maxGlideTime)
        {
            // hover by holding jump after a jump
            if (_input.Player.Jump.IsPressed())
            {
                if (float.IsNegative(velocity.y))
                {
                    _isHovering = true;

                    velocity.y *= _glideSlowdown;
                    _glideTime += Time.deltaTime;
                }
            }
            else
            {
                _isHovering = false;
            }
        }
        else
        {
            _isHovering = false;
        }

        if (_isHovering && _firstHover)
        {
            Debug.Log("first hover");
            _onHover.Invoke();
            _firstHover = false;
        }

        if (_isHovering is false)
        {
            _firstHover = true;
            _endHover.Invoke();
        }


        if (_showDebugLines)
        {
            Debug.DrawLine(transform.position, transform.position + (Vector3)velocity, Color.red);
        }

        if (_isGrounded && Mathf.Abs(velocity.x) > 1)
        {
            _startWalkSound.Invoke();
        }
        else
        {
            _stopWalkSound.Invoke();
        }

        if (_isGrounded)
        {
            _stepsSinceGroundContact = 0;
        }

        _rigidbody.linearVelocity = velocity;

        _dashDirection = Vector2.zero;
        _targetJump = 0;
        _stepsSinceLastJump++;
        _stepsSinceGroundContact++;

        // reset is grounded in on stay/enter?
    }
}