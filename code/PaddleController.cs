using Sandbox;

public sealed class PaddleController : Component
{

	[Property]
	[Category("Paddle")]
	[Range(1f, 50000f, 1000f)]
	public float PaddleMoveSpeed { get; set; } = 25000f;

	private Rigidbody rb { get; set; }


	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	protected override void OnFixedUpdate() {

		if(rb == null) return;

		if (Input.Down( "Forward" ))
			rb.Velocity = Vector3.Up * PaddleMoveSpeed * Time.Delta;

		if(Input.Down("Backward"))
			rb.Velocity = Vector3.Down * PaddleMoveSpeed * Time.Delta;

		if(Input.Released("Forward") || Input.Released("Backward"))
			rb.Velocity = Vector3.Zero;

	}

	protected override void OnStart() {
		rb = Components.Get<Rigidbody>();
	}

}
