// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame;

public class Ball : Sprite
{
	private const float MAX_SPEED = 10000f; // How fast a ball may go at maximum
	private const float DRAG_FAC = 0.98f; // If it's above the MAX_SPEED, it will slow down by this factor
	public const float START_SPEED = 10f; // How fast a ball starts
	private const float MAX_ANGULAR_SPEED_FAC = 1f; // How fast a ball may spin at maximum

	public Vec2 CachedPosition;
	public Vec2 OldPosition;
	public Vec2 Velocity;
	public Vec2 Acceleration;
	public readonly float Radius;
	public readonly float Mass;

	public float FSimTimeRemaining;

	private float _angularVelocity;

	public Ball(Vec2 spawnPos, float radius, float mass, string assetName = "circle") : base($"assets/{assetName}.png", true, false)
	{
		OldPosition = spawnPos;
		position = spawnPos;
		CachedPosition = position;
		Radius = radius;
		Mass = mass;
		Velocity = Vec2.Random() * Mathf.Sq(START_SPEED);
		Acceleration = new Vec2(0, 0);

		SetOrigin(width/2f, height/2f);
		width = (int)radius * 2;
		height = (int)radius * 2;

		SetAngularVelocity();
	}

	public void SetAngularVelocity()
	{
		float mag = 1f / Mass * MAX_ANGULAR_SPEED_FAC; //TODO: Make a better formula (now the light ones spin way too fast)
		_angularVelocity = Utils.Random(-mag, mag);
	}

	public void ApplyForce(Vec2 force)
	{
		Acceleration += force;
	}

	// ReSharper disable once UnusedMember.Global
	protected void Update()
	{
		position = CachedPosition;
		rotation += new Angle(_angularVelocity);

		if (Velocity.MagSq() > MAX_SPEED)
		{
			Velocity *= DRAG_FAC;
		}
	}

	private void Render()
	{
		Gizmos.DrawCircle(CachedPosition, Radius, 8, lineWidth:1);
		// Vec2 dir = Vec2.SetMag(Velocity, Radius);
		// if (dir == new Vec2())
		// 	dir = new Vec2(Radius, 0);
		// Gizmos.DrawRay(position, dir);
	}

	public static bool DoCirclesOverlap(Ball c1, Ball c2)
	{
		return Vec2.DistSq(c1.CachedPosition, c2.CachedPosition) < Mathf.Sq(c1.Radius + c2.Radius);
	}

	public static bool IsPointInCircle(Vec2 pc, float r, Vec2 p)
	{
		return Vec2.DistSq(pc, p) < Mathf.Sq(r);
	}

	public static bool IsPointInCircle(Ball c, Vec2 p)
	{
		return IsPointInCircle(c.CachedPosition, c.Radius, p);
	}
}
