using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbMovementController : PlayerMotionController
{
    [SerializeField] private Rigidbody2D _characterBody;
    [SerializeField] private float _force = 5;

    public override void Initialize()
    {
    }


    public override void BeforeMovement()
    {
        base.BeforeMovement();
    }

    public override void ApplyMovement(in float deltaTime)
    {
        var climbDir = InputHandler.ClimbActions.ClimbDirection.ReadValue<Vector2>();

        _characterBody.AddForce(climbDir.normalized * _force);
    }
}