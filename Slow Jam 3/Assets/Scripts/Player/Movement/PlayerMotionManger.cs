using System;
using UnityEngine;
using Utilities.Extensions;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotionManger : MonoBehaviour
{
	private Rigidbody2D _body;

	public bool isGrounded;
	public PlayerInputHandler inputHandler;
	public PlayerMoveStats stats;
	[Space]
	[Tooltip("The controllers managed by this script. Each controller is processed in order of the array.")]
	public PlayerMotionController[] controllers;

	public void Start()
	{
		TryGetComponent(out _body);

		Action initialize = delegate { };
		for (int i = 0; i < controllers.Length; i++)
		{
			controllers[i].Manger = this;
			controllers[i].Body = _body;
			controllers[i].InputHandler = inputHandler;
			controllers[i].Stats = stats;

			initialize += controllers[i].Initialize;
		}
		initialize();
	}

	public void FixedUpdate()
	{
		void iterateControllers(System.Action<PlayerMotionController> action)
		{
			for (int i = 0; i < controllers.Length; i++)
			{
				if (controllers[i] != null && controllers[i].enabled)
				{
					action(controllers[i]);
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
		controllers = this.Get().Components<PlayerMotionController>().InChildren();
	}
}
