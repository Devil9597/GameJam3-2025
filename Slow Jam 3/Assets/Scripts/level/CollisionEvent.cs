using System;
using UnityEngine;
using UnityEngine.Events;

namespace level
{
    public class CollisionEvent : MonoBehaviour
    {
        [SerializeField] private string _targetTag = "Player";
        [SerializeField] private UnityEvent<GameObject> _onEnter;
        [SerializeField] private UnityEvent<GameObject> _onStay;
        [SerializeField] private UnityEvent<GameObject> _onExit;


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(_targetTag))
            {
                _onEnter.Invoke(other.gameObject);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(_targetTag))
            {
                _onStay.Invoke(other.gameObject);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag(_targetTag))
            {
                _onExit.Invoke(other.gameObject);
            }
        }
    }
}