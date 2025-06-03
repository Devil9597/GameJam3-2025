using System;
using UnityEngine;
using UnityEngine.Events;

namespace level
{
	public class CollisionEvent : MonoBehaviour
	{
		[SerializeField] protected string _targetTag = "Player";
		[SerializeField] protected UnityEvent<GameObject> _onEnter;
		[SerializeField] protected UnityEvent<GameObject> _onStay;
		[SerializeField] protected UnityEvent<GameObject> _onExit;

		public virtual event UnityAction<GameObject> OnEnter {
			add => _onEnter.AddListener(value);
			remove => _onEnter.RemoveListener(value);
		}

		public virtual event UnityAction<GameObject> OnStay {
			add => _onStay.AddListener(value);
			remove => _onStay.RemoveListener(value);
		}

		public virtual event UnityAction<GameObject> OnExit {
			add => _onExit.AddListener(value);
			remove => _onExit.RemoveListener(value);
		}

		protected virtual void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.CompareTag(_targetTag))
			{
				_onEnter.Invoke(other.gameObject);
			}
		}

		protected virtual void OnCollisionStay2D(Collision2D other)
		{
			if (other.gameObject.CompareTag(_targetTag))
			{
				_onStay.Invoke(other.gameObject);
			}
		}

		protected virtual void OnCollisionExit(Collision other)
		{
			if (other.gameObject.CompareTag(_targetTag))
			{
				_onExit.Invoke(other.gameObject);
			}
		}
	}
}