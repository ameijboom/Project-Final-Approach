// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;
using PFA.MyGame.BasicComponents;

namespace PFA.MyGame;

public class Wall : GameObject
{
	public readonly LineSegment Line;
	private readonly Colour _colour;
	public Wall(Vec2 start, Vec2 end, Colour colour = new())
	{
		if(colour == new Colour()) colour = Colour.Aqua;
		_colour = colour;
		Line = new LineSegment(start, end);
	}

	public void Update()
	{
		Gizmos.SetColour(_colour);
		Line.Draw();
		Gizmos.SetColour(Colour.White);
	}
}
