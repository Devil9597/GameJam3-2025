using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
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

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag(_targetTag))
		{
			_onEnter.Invoke(other.gameObject);
		}
	}

	protected virtual void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag(_targetTag))
		{
			_onEnter.Invoke(other.gameObject);
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.CompareTag(_targetTag))
		{
			_onExit.Invoke(other.gameObject);
		}
	}
}