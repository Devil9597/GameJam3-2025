using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Timers;
using UnityEngine;


public class WaterDrop : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private float _destroyTime = 5;
    [SerializeField] private Rigidbody2D _rigidbody;


    private bool _isExplode = false;

    /// <summary>
    /// theres no reason rn to keep a ref to the containers, just keep a int count of the ones we are touching
    /// </summary>

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Explode()
    {
        _rigidbody.simulated = false;
        _isExplode = true;
        _sprite.enabled = false;
        _particleSystem.Play();
        Destroy(gameObject, _destroyTime);
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    [SerializeField] private int _containerContacts;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var waterContainer = other.gameObject.GetComponent<WaterContainer>();


        if (waterContainer is null)
        {
            return;
        }

        if (waterContainer.TryConsume())
        {
            Explode();
        }

        _containerContacts++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var waterContainer = other.gameObject.GetComponent<WaterContainer>();
        if (waterContainer is null)
        {
            return;
        }

        _containerContacts--;
        if (_containerContacts == 0)
        {
            Explode();
        }
    }


    private void OnCollisionStay2D(Collision2D other)
    {
        if (_containerContacts is 0 && _isExplode is false)
        {
            Explode();
            return;
        }


        var container = other.gameObject.GetComponent<WaterContainer>();
        if (container == null)
        {
            return;
        }

        if (container.TryConsume())
        {
            Explode();
            return;
        }
    }
}