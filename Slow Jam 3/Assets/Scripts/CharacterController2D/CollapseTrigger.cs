using UnityEngine;

public class CollapseTrigger : MonoBehaviour
{
    [SerializeField] GameObject CollapsingFloorHolder;

    public bool activated = false;

    private void Start()
    {
        CollapsingFloorHolder.SetActive(true);
        activated = false;
    }

    void Update()
    {
        if (activated == true)
        {
            CollapsingFloorHolder.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            activated = true;
        }
    }
}
