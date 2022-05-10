// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame;

public class PlayerBall : Ball
{
	private const float RADIUS = 32;
	private const float MASS = 300;

	public PlayerBall(Vec2 spawnPos) : base(spawnPos, RADIUS, MASS)
	{
		Colour = Colour.Green;
	}
}
