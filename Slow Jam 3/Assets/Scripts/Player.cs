using System;
using System.Collections.Generic;
using Systems.Stats;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMoveStats _playerMoveStats;
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField] private PlayerMotionManger _playerMotion;

    [SerializeField] private List<MonoBehaviour> _standardMovementComponents = new();
    [SerializeField] private List<MonoBehaviour> _climbMovementComponents = new();

    private StatClamp _climbGravity = new(0, 0);
    private ContactFilter2D _vineContactFilter;

    private bool _isTouchingVine = false;
    private bool _isClimbing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _vineContactFilter = new() { layerMask = LayerMask.NameToLayer("Default"), useTriggers = true };
    }


    private void ReleaseGripOnperformed(InputAction.CallbackContext obj)
    {
        EndClimb();
    }

    public void StartClimb()
    {
        _playerMotion.SetValue(PlayerHorizontalMovement.TR_X_VELOCITY, 0);
        _playerMotion.SetValue(PlayerVerticalMovement.TR_Y_VELOCITY, 0);

        foreach (var c in _standardMovementComponents)
        {
            c.enabled = false;
        }

        foreach (var c in _climbMovementComponents)
        {
            c.enabled = true;
        }
    }

    public void EndClimb()
    {
        _playerMotion.SetValue(PlayerHorizontalMovement.TR_X_VELOCITY, 0);
        _playerMotion.SetValue(PlayerVerticalMovement.TR_Y_VELOCITY, 0);
        foreach (var c in _standardMovementComponents)
        {
            c.enabled = true;
        }

        foreach (var c in _climbMovementComponents)
        {
            c.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private List<Collider2D> _overlaps = new();

    private void FixedUpdate()
    {
        if (_isTouchingVine && _isClimbing is false)
        {
            _isClimbing = true;

            StartClimb();
        }

        if (_isTouchingVine is false)
        {
            _isClimbing = false;
            EndClimb();
        }


        _overlaps.Clear();

        // fuck it im lazy
        _playerCollider.Overlap(_vineContactFilter, _overlaps);

        foreach (var other in _overlaps)
        {
            var climb = other.GetComponent<Climbable>();


            if (climb is not null)
            {
                _isTouchingVine = true;
                return;
            }
        }

        _isTouchingVine = false;
        EndClimb();
    }
}