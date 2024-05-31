using Sandbox;

public sealed class BallController : Component, Component.ICollisionListener
{
	private Rigidbody rb;
	[Property] [Category( "Components" )] public readonly GameObject Background;
	private bool startDirection = true;

	private Vector3 _ballStartingPosition;

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		if ( rb == null ) return;


		if ( Input.Pressed( "Jump" ) )
			rb.Enabled = !rb.Enabled;
		
	}

	void ICollisionListener.OnCollisionStart( Collision other )
	{

		var otherObject = other.Other;

		if ( otherObject.GameObject.Tags.Contains( "paddle" ) )
		{
			var paddle = other.Other.Body;

			var paddleVelocity = paddle.Velocity;
			
			Log.Info( "Paddle velocity:" + paddleVelocity );

		}

		if ( otherObject.GameObject.Tags.Contains( "goals" ) )
		{
			if ( otherObject.GameObject.Tags.Contains( "left_goal" ) )
			{
				Log.Info( "1+ Score for right side" );
			}
			
			if ( otherObject.GameObject.Tags.Contains( "right_goal" ) )
			{
				Log.Info( "1+ Score for left side" );
			}
		}
	}

	protected override void OnStart()
	{
		rb = Components.Get<Rigidbody>();

		if ( rb == null )
			return;

		GameStart();
		BallStartPosition();
	}

	private void BallStartPosition()
	{
		if ( Background == null )
			return;

		_ballStartingPosition = Background.Transform.Position;
	}

	private void GameStart()
	{
		rb.PhysicsBody.Position = _ballStartingPosition;

		if ( startDirection )
			rb.Velocity = Vector3.Left * 15000f * Time.Delta;
		else
			rb.Velocity = Vector3.Right * 15000f * Time.Delta;
	}
}
