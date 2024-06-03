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

	private float DefaultSpeed= 500f;
	private const float SpeedMultiplier = 1.1f;

	private Vector3 PreImpactVelocity = new Vector3( 0, 0, 0f );

	public Hashtable Scores = new Hashtable { { "left_score", 0 }, { "right_score", 0 } };

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
		PreImpactVelocity = rb.Velocity;
	}

	void ICollisionListener.OnCollisionStart( Collision other )
	{
		var otherObject = other.Other;

		if ( otherObject.GameObject.Tags.Contains( "paddle" ) )
		{
			var paddlePosition = other.Other.Body.Transform.Position;

			if ( otherObject.GameObject.Tags.Contains( "left_paddle" ) )
			{
				ApplyPaddleForceToBall(paddlePosition );
			}

			if ( otherObject.GameObject.Tags.Contains( "right_paddle" ) )
			{
				ApplyPaddleForceToBall( paddlePosition );
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
				Scores["right_score"] = (int)Scores["right_score"]! + 1;
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
	
	private void ApplyPaddleForceToBall(Vector3 paddlePosition)
	{

		paddlePosition.x = 0;

		Vector3 newVelocity = Vector3.Reflect(PreImpactVelocity, paddlePosition);

		newVelocity.x = 0;

		newVelocity = newVelocity.Normal * DefaultSpeed;

		rb.Velocity = newVelocity;

		DefaultSpeed *= SpeedMultiplier;
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
		DefaultSpeed = 500f;
		rb.Velocity = Vector3.Zero;
		rb.PhysicsBody.Position = _ballStartingPosition;

		if ( startDirection )
			rb.Velocity = Vector3.Left * DefaultSpeed;
		else
			rb.Velocity = Vector3.Right * DefaultSpeed;
	}
}
