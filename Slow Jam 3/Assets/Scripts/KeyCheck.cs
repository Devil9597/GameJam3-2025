using UnityEngine;
using UnityEngine.Events;

public class KeyCheck : MonoBehaviour
{
	public CharacterController2D _player;
	public int _finaleSceneBuildIndex = 0;
	public int _keysRequired = 4;

	public UnityEvent _onKeyCheckSuccess;

	public bool CheckForKeys()
	{
		if (_player == null)
			return false;

		bool value = _keysRequired == 0 || _player.keyCount >= _keysRequired;

		if (value)
		{
			_onKeyCheckSuccess.Invoke();
		}
		return value;
	}

	public void LoadFinale()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(_finaleSceneBuildIndex);
	}
}
