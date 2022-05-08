// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.Core;

namespace PFA.MyGame;

public class PlayerBall : Ball
{
	private const float RADIUS = 32;
	private const float MASS = 1000;

	public PlayerBall(float x, float y) : base(x, y, RADIUS, MASS)
	{
		Colour = Colour.Green;
	}
}
