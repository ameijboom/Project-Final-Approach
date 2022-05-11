// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame;

public class LineSegment : GameObject
{
	public Vec2 Start;
	public Vec2 End;
	public readonly float Radius;

	public LineSegment(Vec2 start, Vec2 end, float radius)
	{
		position = new Vec2(x, y);
		Start = start;
		End = end;
		Radius = radius;
	}

	// ReSharper disable once UnusedMember.Global
	public void Update()
	{
		// Render();
	}

	private void Render()
	{
		Gizmos.DrawCircle(Start, Radius, colour:Colour.Gray);
		Gizmos.DrawCircle(End, Radius, colour:Colour.Gray);

		Vec2 dir = End - Start;
		Vec2 n = dir.GetNormal().SetMag(Radius);

		Gizmos.DrawLine(Start + n, End + n);
		Gizmos.DrawLine(Start - n, End - n);
	}
}
