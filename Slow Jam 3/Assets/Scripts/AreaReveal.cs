using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AreaReveal : MonoBehaviour
{
    [SerializeField] GameObject revealArea;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") 
        {
            revealArea.SetActive(false);
        }
    }
}
