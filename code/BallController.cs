using System;
using System.Collections;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.VisualBasic;
using Sandbox;

public sealed class BallController : Component, Component.ICollisionListener, Component.ITriggerListener
{
	private Rigidbody rb;
	[Property] [Category( "Components" )] public readonly GameObject Background;
	private bool startDirection = true;

	private Vector3 _ballStartingPosition;

	private float _defaultSpeed = 500f;
	private float _hitSpeed = 250f;

	private Vector3 PreImpactVelocity = new Vector3( 0, 0, 0f );

	Hashtable Scores = new Hashtable { { "left_score", 0 }, { "right_score", 0 } };

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

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		Log.Info( "listener vel" + rb.Velocity );
		PreImpactVelocity = rb.Velocity;
	}

	void ICollisionListener.OnCollisionStart( Collision other )
	{
		var otherObject = other.Other;

		Log.Info( "collision vel" + rb.Velocity );

		if ( otherObject.GameObject.Tags.Contains( "paddle" ) )
		{
			var paddle = other.Other.Body;
			var paddleVelocity = paddle.Velocity;

			if ( otherObject.GameObject.Tags.Contains( "left_paddle" ) )
			{
				ApplyPaddleForceToBall( applyForceLeft: false, paddleVelocity );
			}

			if ( otherObject.GameObject.Tags.Contains( "right_paddle" ) )
			{
				ApplyPaddleForceToBall( applyForceLeft: true, paddleVelocity );
			}
		}

		if ( otherObject.GameObject.Tags.Contains( "boundary" ) )
		{
			ApplyWallForceToBall();
		}

		if ( otherObject.GameObject.Tags.Contains( "goals" ) )
		{
			if ( otherObject.GameObject.Tags.Contains( "left_goal" ) )
			{
				Scores["right_score"] = (int)Scores["left_score"]! + 1;
			}

			if ( otherObject.GameObject.Tags.Contains( "right_goal" ) )
			{
				Scores["left_score"] = (int)Scores["left_score"]! + 1;
			}

			RestartGame();
		}
	}

	private void RestartGame()
	{
		startDirection = !startDirection;
		GameStart();
	}

	private void ApplyPaddleForceToBall( bool applyForceLeft, Vector3 paddleVelocity )
	{
		var ballVelocity = PreImpactVelocity;

		var flippedVelocity = new Vector3( ballVelocity.x, -ballVelocity.y, ballVelocity.z );

		var horizontalForce = applyForceLeft ? Vector3.Left : Vector3.Right;
		horizontalForce *= MathF.Abs( paddleVelocity.y );

		var newVelocity = flippedVelocity + horizontalForce;

		newVelocity.y += ConvertSpeed(newVelocity.y, _hitSpeed);

		rb.Velocity = newVelocity;
	}
	
	private float ConvertSpeed(float velocity, float speed)
	{
		if (velocity < 0)
		{
			return -speed;
		}

		return speed;
	}


	private void ApplyWallForceToBall()
	{
		var ballVelocity = PreImpactVelocity;

		var flippedVelocity = new Vector3( ballVelocity.x, ballVelocity.y, -ballVelocity.z );

		rb.Velocity = flippedVelocity;
	}

	protected override void OnStart()
	{
		rb = Components.Get<Rigidbody>();

		if ( rb == null )
			return;

		BallStartPosition();
		GameStart();
	}

	private void BallStartPosition()
	{
		if ( Background == null )
			return;

		_ballStartingPosition = Background.Transform.Position;
	}

	private void GameStart()
	{
		rb.Velocity = Vector3.Zero;
		rb.PhysicsBody.Position = _ballStartingPosition;

		if ( startDirection )
			rb.Velocity = Vector3.Left * _defaultSpeed;
		else
			rb.Velocity = Vector3.Right * _defaultSpeed;
	}
}
