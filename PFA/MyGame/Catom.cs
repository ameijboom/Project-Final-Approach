// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;
using PFA.MyGame.BasicComponents;

namespace PFA.MyGame;

public class Catom : GameObject
{
	private readonly Circle _circle;
	private readonly Colour _colour;

	private readonly float _radius;

	private Vec2 _velocity;
	private Vec2 _acceleration;

	public Catom(Vec2 p, float r, Colour colour = new())
	{
		if (colour == new Colour()) colour = Colour.Red;
		position = p;
		_radius = r;
		_colour = colour;

		_circle = new Circle(position, _radius);

		_velocity = new Vec2(0, 0);
		_acceleration = new Vec2(0, 0);
	}

	public void ApplyForce(Vec2 force)
	{
		_acceleration += force;
	}

	public void Update()
	{
		_velocity += _acceleration;

		//Nice and simple reflection. No need to do anything fancier.
		foreach (Wall mirror in MyGame.Walls)
		{
			LineSegment trajectory = new(position, position + _velocity);
			(bool hit, _, _) = LineSegment.Intersect(trajectory, mirror.Line);
			if (!hit) continue;
			_velocity.Reflect(mirror.Line.GetDir().GetNormal());
		}

		position += _velocity;

		//These should never happen, but this is here just in case it does.
		if(x < 0) x = game.width;
		if(x > game.width) x = 0;
		if(y < 0) y = game.height;
		if(y > game.height) y = 0;

		_acceleration *= 0f;
		_circle.Position = position;

		Gizmos.SetColour(_colour);
		Gizmos.DrawCircle(_circle.Position, _radius, 6);
		Gizmos.SetColour(Colour.White);
	}
}
