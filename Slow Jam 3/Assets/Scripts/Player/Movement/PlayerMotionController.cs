using UnityEngine;

public abstract class PlayerMotionController : MonoBehaviour
{
	public PlayerMotionManger Manger { get; set; }
	public Rigidbody2D Body { get; set; }
	public PlayerInputHandler InputHandler { get; set; }

	public abstract void Initialize();

	/// <summary>
	/// This method is run by the <see cref="Manager"/> before any movement has been applied.
	/// </summary>
	public virtual void BeforeMovement() { }
	/// <summary>
	/// This method is run by the <see cref="Manager"/> when it's tme for this object to apply movement.
	/// </summary>
	/// <param name="deltaTime"></param>
	public abstract void ApplyMovement(in float deltaTime);
	/// <summary>
	/// This method is run by the <see cref="Manager"/> after all movement has been applied.
	/// </summary>
	public virtual void AfterMovement() { }
}

public delegate void ApplyMovementDelegate(in float deltaTime);
