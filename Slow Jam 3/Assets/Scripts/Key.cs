using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
	[SerializeField] CharacterController2D CharacterController;
	[SerializeField] GameObject powerUp;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			powerUp.SetActive(false);
			CharacterController.keyCount++;
		}
	}
}
