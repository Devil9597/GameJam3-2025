using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // References
    public PlayerMovementStats MovementStats;
    [SerializeField] private Collider2D _feetCollider;
    [SerializeField] private Collider2D _bodyCollider;

    private Rigidbody2D _rb;

    // Movement Variables
    private Vector2 _moveVelocity;
    private bool _isFacingRight;

    // Collision Check Variables
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _bumpedHead;

    // Jump Variables
    public float VerticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    // Apex Variables
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    // Jump Buffer Variables
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    // Coyote Time Variables
    private float _coyoteTimer;

    private void Awake()
    {
        _isFacingRight = true;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CountTimers();
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionChecks();

        Jump();

        if (_isGrounded)
        {
            Move(MovementStats.GroundAcceleration, MovementStats.GroundDeceleration, InputManager.Movement);
        }

        else
        {
            Move(MovementStats.AirAcceleration, MovementStats.AirDeceleration, InputManager.Movement);
        }
    }

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            // Check if it needs to turn
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.RunIsHeld)
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * MovementStats.MaxRunSpeed;
            }
            else
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * MovementStats.MaxWalkSpeed;
            }

            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocityY);
        }

        else if (moveInput == Vector2.zero) 
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocityY);
        }
    }

    // Check Methods

    private void TurnCheck(Vector2 moveInput) 
    { 
        if (_isFacingRight == true && moveInput.x < 0)
        {
            Turn(false);
        }

        else if (_isFacingRight == false && moveInput.x > 0) 
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }

        else
        {
            _isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    // Collision Check Methods

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x, MovementStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MovementStats.GroundDetectionRayLength, MovementStats.GroundLayer);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }   
        
        else
        {
            _isGrounded = false;
        }
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCollider.bounds.center.x, _bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x * MovementStats.HeadWidth, MovementStats.HeadDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MovementStats.HeadDetectionRayLength, MovementStats.GroundLayer);
        if (_groundHit.collider != null)
        {
            _bumpedHead = true;
        }

        else
        {
            _bumpedHead = false;
        }
    }

    private void CollisionChecks()
    {
        IsGrounded();
    }

    private void JumpChecks()
    {
        // When jump button is pressed
        if (InputManager.JumpWasPressed)
        {
            _jumpBufferTimer = MovementStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        // When jump button is released
        if (InputManager.JumpWasReleased) 
        { 
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            if (_isJumping && VerticalVelocity > 0f) 
            { 
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MovementStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0;
                }

                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
        // Initiate Jump with Jump Buffering & Coyote Time
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        // Double Jump
        else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed) 
        {
            _isFastFalling = false;
            InitiateJump(1);
        }

        // Air Jump After Coyote Time Lapsed
        else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            _isFastFalling = false;
        }

        // Landed
        if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;
            _numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump (int numberOfJumpsUsed)
    {
        if (!_isJumping) 
        { 
            _isJumping = true;  
        }

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;
        VerticalVelocity = MovementStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        // Apply Gravity While Jumping
        if (_isJumping)
        {
            // Check for Head Bump
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }
        }

        // Gravity on Ascending
        if (VerticalVelocity >= 0f) 
        {
            // Apex Controls
            _apexPoint = Mathf.InverseLerp(MovementStats.InitialJumpVelocity, 0f, VerticalVelocity);

            if (_apexPoint > MovementStats.ApexThreshold)
            {
                if (!_isPastApexThreshold)
                {
                    _isPastApexThreshold = true;
                    _timePastApexThreshold = 0f;
                }

                if (_isPastApexThreshold)
                {
                    _timePastApexThreshold += Time.fixedDeltaTime;
                    if (_timePastApexThreshold < MovementStats.ApexHangTime)
                    {
                        VerticalVelocity = 0f;
                    }

                    else
                    {
                        VerticalVelocity = -0.01f;
                    }
                }
            }

            // Gravity On Ascending But Not Past Apex Threshold
            else
            {
                VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                }
            }
        }

        // Gravity on Descending
        else if (!_isFastFalling)
        {
            VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }

        else if (VerticalVelocity < 0f)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }
        }

        // Jump Cut
        if (_isFastFalling)
        {
            if (_fastFallTime >= MovementStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (_fastFallTime < MovementStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MovementStats.TimeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

        // Normal Gravity While Falling
        if (!_isGrounded && !_isJumping)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }

            VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
        }

        // Clamp Fall Speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementStats.MaxFallSpeed, 50f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, VerticalVelocity);
    }

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;

        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }

        else
        {
            _coyoteTimer = MovementStats.JumpCoyoteTime;
        }
    }

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if (MovementStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else
        {
            speed = -moveSpeed;
        }
        Vector2 velocity = new Vector2(speed, MovementStats.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * MovementStats.TimeTillJumpApex / MovementStats.ArcResolution; // time step for the simulation
        // float totalTime = (2 * MovementStats.TimeTillJumpApex) * MovementStats.ApexHangTime; // total time of the arc including hang time

        for (int i = 0; i < MovementStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < MovementStats.TimeTillJumpApex) // Ascending
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, MovementStats.Gravity) * simulationTime * simulationTime;
            }

            else if (simulationTime < MovementStats.TimeTillJumpApex + MovementStats.ApexHangTime)
            {
                float apexTime = simulationTime - MovementStats.TimeTillJumpApex;
                displacement = velocity * MovementStats.TimeTillJumpApex + 0.5f * new Vector2(0, MovementStats.Gravity) * MovementStats.TimeTillJumpApex * MovementStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;
            }
            else // descending
            {
                float descendTime = simulationTime - (MovementStats.TimeTillJumpApex + MovementStats.ApexHangTime);
                displacement = velocity * MovementStats.TimeTillJumpApex + 0.5f * new Vector2(0, MovementStats.Gravity) * MovementStats.TimeTillJumpApex * MovementStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * MovementStats.ApexHangTime;
                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, MovementStats.Gravity) * descendTime * descendTime;
            }

            drawPoint = startPosition + displacement;

            if (MovementStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), MovementStats.GroundLayer);
                if (hit.collider != null)
                {
                    // If a hit is detected, stop drawing the arc at the hit point
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }

    private void OnDrawGizmos()
    {
        if (MovementStats.ShowWalkJumpArc)
        {
            DrawJumpArc(MovementStats.MaxWalkSpeed, Color.white);
        }

        if (MovementStats.ShowRunJumpArc)
        {
            DrawJumpArc(MovementStats.MaxRunSpeed, Color.white);
        }
    }
}
