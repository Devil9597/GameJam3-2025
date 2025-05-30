using UnityEngine;
using Utilities.Extensions;

public class PlayerLinearVelocity : PlayerMotionController
{
	public override void Initialize()
	{
		// nop
	}

	public override void ApplyMovement(in float deltaTime)
	{
		Body.linearVelocityX = Manger.GetValue(PlayerHorizontalMovement.TR_X_VELOCITY, out float _);
		Body.linearVelocityY = Manger.GetValue(PlayerVerticalMovement.TR_Y_VELOCITY, out float _);
	}
}
