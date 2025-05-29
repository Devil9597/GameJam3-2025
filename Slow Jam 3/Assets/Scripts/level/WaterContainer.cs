using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// for now this is used like a tag to determine if the water should explode
/// </summary>
public class WaterContainer : MonoBehaviour
{
    [SerializeField] private bool ShouldConsume;

    [SerializeField] private UnityEvent OnConsume = new();

    public bool TryConsume()
    {
        if (ShouldConsume)
        {
            OnConsume.Invoke();
            return true;
        }

        return false;
    }
}