using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
	[SerializeField] private string _targetTag = "Player";
	[SerializeField] private UnityEvent<GameObject> _onEnter;
	[SerializeField] private UnityEvent<GameObject> _onStay;
	[SerializeField] private UnityEvent<GameObject> _onExit;

	public event UnityAction<GameObject> OnEnter {
		add => _onEnter.AddListener(value);
		remove => _onEnter.RemoveListener(value);
	}

	public event UnityAction<GameObject> OnStay {
		add => _onStay.AddListener(value);
		remove => _onStay.RemoveListener(value);
	}

	public event UnityAction<GameObject> OnExit {
		add => _onExit.AddListener(value);
		remove => _onExit.RemoveListener(value);
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