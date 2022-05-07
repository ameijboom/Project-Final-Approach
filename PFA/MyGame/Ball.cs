// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame;

public class Ball : GameObject
{
	public Vec2 OldPosition;
	public Vec2 Velocity;
	public Vec2 Acceleration;
	public readonly float Radius;
	public readonly float Mass;

	public float FSimTimeRemaining;

	public Ball(float x, float y, float radius, float mass = 0)
	{
		OldPosition = new Vec2(x, y);
		position = new Vec2(x, y);
		Radius = radius; //TODO: Base this on the radius of a hydrogen atom, and make all other atoms have a relative radius to that.
		if(mass <= 0)
			Mass = radius * 10.0f; //TODO: Same for mass.
		else
			Mass = mass;
		Velocity = new Vec2(0, 0);
		Acceleration = new Vec2(0, 0);
	}

	public void ApplyForce(Vec2 force)
	{
		Acceleration += force;
	}

	// ReSharper disable once UnusedMember.Global
	public void Update()
	{
		Render();
	}

	private void Render()
	{
		Gizmos.DrawCircle(position, Radius, 4);
		// Vec2 dir = Vec2.SetMag(Velocity, Radius);
		// if (dir == new Vec2())
		// 	dir = new Vec2(Radius, 0);
		// Gizmos.DrawRay(position, dir);
	}



	public static bool DoCirclesOverlap(Ball c1, Ball c2)
	{
		return Vec2.DistSq(c1.position, c2.position) < Mathf.Sq(c1.Radius + c2.Radius);
	}

	public static bool IsPointInCircle(Vec2 pc, float r, Vec2 p)
	{
		return Vec2.DistSq(pc, p) < Mathf.Sq(r);
	}

	public static bool IsPointInCircle(Ball c, Vec2 p)
	{
		return IsPointInCircle(c.position, c.Radius, p);
	}
}
