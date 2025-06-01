using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] private string _targetTag;
    [SerializeField] private UnityEvent<GameObject> _onEnter;
    [SerializeField] private UnityEvent<GameObject> _onStay;
    [SerializeField] private UnityEvent<GameObject> _onExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _onEnter.Invoke(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _onEnter.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))

        {
            _onExit.Invoke(other.gameObject);
        }
    }
}