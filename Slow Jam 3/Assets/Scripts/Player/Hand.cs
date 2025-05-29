using System;
using System.Collections.Generic;
using Systems.Stats;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private Collider2D _leftCollider;
    [SerializeField] private Collider2D _rightCollider;
    [SerializeField] private float _force = 10;
    [SerializeField] private PlayerMoveStats _playerMoveStats;

    [SerializeField] private StatMultiplier _grabSpeedMulti;

    private Collider2D _activeCollider;

    private bool isGrabing = false;
    [SerializeField] private Grabbable _target;
    private ContactFilter2D _contactFilter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _contactFilter = new ContactFilter2D() { layerMask = LayerMask.NameToLayer("Ground"), useTriggers = true};
        _activeCollider = _leftCollider;
        _leftCollider.gameObject.SetActive(true);
        _rightCollider.gameObject.SetActive(false);

        _inputHandler.Player.Move.performed += MoveOnperformed;
        _inputHandler.Player.Grab.performed += GrabOnperformed;
    }

    private void GrabOnperformed(InputAction.CallbackContext obj)
    {
        if (isGrabing)
        {
            _playerMoveStats.MaxSpeed.RemoveModifier(_grabSpeedMulti);
            _target = null;
            isGrabing = false;
            return;
        }

        List<Collider2D> overlaps = new();
        _activeCollider.Overlap(_contactFilter, overlaps);

        foreach (var overlap in overlaps)
        {
            var grabbable = overlap.GetComponent<Grabbable>();

            if (grabbable is not null)
            {
                _target = grabbable;
                break;
            }
        }

        if (_target is null)
        {
            isGrabing = false;
            return;
        }

        _playerMoveStats.MaxSpeed.AddModifier(_grabSpeedMulti);
        isGrabing = true;
    }

    private void FixedUpdate()
    {
        if (isGrabing)
        {
            var dist = (_activeCollider.transform.position - _target.transform.position).magnitude;
            if (dist < 1)
            {
                return;
            }

            var rb = _target.GetComponent<Rigidbody2D>();
            var force = (_activeCollider.transform.position - _target.transform.position).normalized * _force;
            force.y = 0;

            Debug.Log(force);

            rb.AddForce(force);
        }
    }

    private void MoveOnperformed(InputAction.CallbackContext obj)
    {
        if (isGrabing == false)
        {
            var dir = obj.ReadValue<float>();
            if (dir < 0)
            {
                //left
                _rightCollider.gameObject.SetActive(false);
                _leftCollider.gameObject.SetActive(true);
                _activeCollider = _leftCollider;
            }

            if (dir > 0)
            {
                //right
                _rightCollider.gameObject.SetActive(true);
                _leftCollider.gameObject.SetActive(false);
                _activeCollider = _rightCollider;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}