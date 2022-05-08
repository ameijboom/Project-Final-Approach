// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame;

public class Ball : Sprite
{
	public Vec2 CachedPosition;
	public Vec2 OldPosition;
	public Vec2 Velocity;
	public Vec2 Acceleration;
	public readonly float Radius;
	public readonly float Mass;


	public float FSimTimeRemaining;

	public Ball(float x, float y, float radius, float mass = 0) : base("assets/circle.png", true, false)
	{
		OldPosition = new Vec2(x, y);
		position = new Vec2(x, y);
		CachedPosition = position;
		Radius = radius; //TODO: Base this on the radius of a hydrogen atom, and make all other atoms have a relative radius to that.
		if(mass <= 0)
			Mass = radius * 10.0f; //TODO: Same for mass.
		else
			Mass = mass;
		Velocity = new Vec2(0, 0);
		Acceleration = new Vec2(0, 0);

		SetOrigin(width/2f, height/2f);
		width = (int)radius * 2;
		height = (int)radius * 2;
	}

	public void ApplyForce(Vec2 force)
	{
		Acceleration += force;
	}

	// ReSharper disable once UnusedMember.Global
	public void Update()
	{
		// Render();
		position = CachedPosition;
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
