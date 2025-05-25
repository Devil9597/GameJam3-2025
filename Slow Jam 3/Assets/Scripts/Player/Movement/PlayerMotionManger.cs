using UnityEngine;
using Utilities.Extensions;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotionManger : MonoBehaviour
{
	private Rigidbody2D _body;

	[SerializeField] private bool _isGrounded;
	[SerializeField] private PlayerInputHandler _inputHandler;
	[Space]
	[SerializeField] private PlayerMotionController[] _controllers;

	public void Start()
	{
		TryGetComponent(out _body);

		for (int i = 0; i < _controllers.Length; i++)
		{
			_controllers[i].Manger = this;
			_controllers[i].Body = _body;
			_controllers[i].InputHandler = _inputHandler;
		}

		for (int i = 0; i < _controllers.Length; i++)
		{
			_controllers.Initialize();
		}
	}

	public void FixedUpdate()
	{
		void iterateControllers(System.Action<PlayerMotionController> action)
		{
			for (int i = 0; i < _controllers.Length; i++)
			{
				if (_controllers[i] != null && _controllers[i].enabled)
				{
					action(_controllers[i]);
				}
			}

			iterateControllers(c => c.BeforeMovement());
			iterateControllers(c => c.ApplyMovement(Time.fixedDeltaTime));
			iterateControllers(c => c.AfterMovement());
		}
	}

	[ContextMenu("Get Controllers")]
	private void GetControllers()
	{
		_controllers = this.Get().Components<PlayerMotionController>().InChildren();
	}
}
