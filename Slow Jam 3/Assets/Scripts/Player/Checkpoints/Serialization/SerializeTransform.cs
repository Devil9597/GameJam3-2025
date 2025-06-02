using UnityEngine;

namespace Player.Checkpoints.Serialization
{
    public class SerializeTransform : MonoBehaviour
    {
        [SerializeData]
        public Vector2 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        [SerializeData]
        public Vector3 Rotation
        {
            get => transform.rotation.eulerAngles;
            set => transform.rotation = Quaternion.Euler(value);
        }
    }
}