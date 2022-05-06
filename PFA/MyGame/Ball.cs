// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame;

public class Ball : GameObject
{
	public Vec2 oldPosition;
	public Vec2 Velocity;
	public Vec2 Acceleration;
	public readonly float Radius;
	public readonly float Mass;

	public float fSimTimeRemaining;

	public Ball(float x, float y, float radius, float mass = 0)
	{
		oldPosition = new Vec2(x, y);
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

	private void Physics()
	{
		Velocity += Acceleration;
		position += Velocity;
		Acceleration *= 0;
	}

	public void Render()
	{
		Gizmos.DrawCircle(position, Radius, 4);
		// Vec2 dir = Vec2.SetMag(Velocity, Radius);
		// if (dir == new Vec2())
		// 	dir = new Vec2(Radius, 0);
		// Gizmos.DrawRay(position, dir);
	}
}
