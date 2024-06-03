using System;
using Sandbox;

public sealed class PongBotController : Component
{
	[Property] [Category( "Components" )] public BallController BallController;

	[Property]
	[Title("How fast bot can accelerate")]
	[Category( "Bot Settings" )]
	[Range(1,100,1f)]
	public float Acceleration  { get; set; } = 10f;
	
	[Property]
	[Title("Max speed of bot's paddle")]
	[Category( "Bot Settings" )]
	[Range(1,1000,1f)]
	public float MaxSpeed  { get; set; } = 500f;
	
	[Property]
	[Title("How close the bot tries to get the ball to it's center")]
	[Category( "Bot Settings" )]
	[Range(0,1,0.1f)]
	public float Threshold  { get; set; } = 0.1f;

	private Rigidbody BallRigidBody
	{
		get
		{
			return BallController.Components.Get<Rigidbody>();
		}
	}

	private Vector3 BallPosition;

	private Rigidbody PaddleRigidBody
	{
		get
		{
			return Components.Get<Rigidbody>();
		}
	}

	private float _paddleCenter
	{
		get
		{
			return PaddleRigidBody.Transform.Scale.z / 2;
		}
	} 

	protected override void OnFixedUpdate()
	{
		SetBallPosition();
		MovePaddle();
	}
	
	private void FetchBallComponents()
	{
		if ( BallController == null )
		{
			Log.Error( "No ball controller assigned to PongBotController" );
		}

		if ( BallRigidBody == null )
		{
			Log.Error( "No rigid body on ball" );
		}
		else
		{
			SetBallPosition();
		}
	}
	
	private void MovePaddle()
	{
		var paddleCenterZ = PaddleRigidBody.Transform.LocalPosition.z + _paddleCenter;
		var deltaZ = BallPosition.z - paddleCenterZ;


		if (Math.Abs(deltaZ) < Threshold)
		{
			PaddleRigidBody.Velocity = new Vector3(PaddleRigidBody.Velocity.x, PaddleRigidBody.Velocity.y, 0);
		}
		else
		{
			var desiredVelocityZ = deltaZ * Acceleration;
			desiredVelocityZ = Math.Clamp(desiredVelocityZ, -MaxSpeed, MaxSpeed);

			PaddleRigidBody.Velocity = new Vector3(PaddleRigidBody.Velocity.x, PaddleRigidBody.Velocity.y, desiredVelocityZ);
		}
	}

	private void SetBallPosition()
	{
		BallPosition = BallRigidBody.Transform.LocalPosition;
	}

	protected override void OnStart()
	{
		FetchBallComponents();
	}
}
