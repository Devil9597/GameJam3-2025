using System;
using UnityEngine;

namespace Player.Checkpoints.Serialization
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SerializeRigidbody2D : MonoBehaviour
    {
        [SerializeData]
        public Vector2 Velocity
        {
            get => _rigidbody.linearVelocity;
            set => _rigidbody.linearVelocity = value;
        }

        [SerializeData]
        public bool IsSimulated
        {
            get => _rigidbody.simulated;
            set => _rigidbody.simulated = value;
        }

        private Rigidbody2D _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}