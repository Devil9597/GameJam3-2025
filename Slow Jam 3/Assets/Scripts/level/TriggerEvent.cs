using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] protected string _targetTag = "Player";
    [SerializeField] private bool _shouldKillPlayer = false;
    [SerializeField] protected UnityEvent<GameObject> _onEnter;
    [SerializeField] protected UnityEvent<GameObject> _onStay;
    [SerializeField] protected UnityEvent<GameObject> _onExit;

    public virtual event UnityAction<GameObject> OnEnter
    {
        add => _onEnter.AddListener(value);
        remove => _onEnter.RemoveListener(value);
    }

    public virtual event UnityAction<GameObject> OnStay
    {
        add => _onStay.AddListener(value);
        remove => _onStay.RemoveListener(value);
    }

    public virtual event UnityAction<GameObject> OnExit
    {
        add => _onExit.AddListener(value);
        remove => _onExit.RemoveListener(value);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _onEnter.Invoke(other.gameObject);
            var respawn = other.GetComponent<Respawnable>();
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _onEnter.Invoke(other.gameObject);
            if (_shouldKillPlayer)
            {
                var respawn = other.GetComponent<Respawnable>();
                if (respawn != null)
                {
                    respawn.Respawn();
                }
            }
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