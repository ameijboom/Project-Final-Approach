// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame.CatomTypes;

public class CatOxygen : Catom
{
	private const float RADIUS = 48.0f;
	private const float MASS = 15.9994f;
	private const float ATTRACTION_COEFFICIENT = 19f;

	public CatOxygen(Vec2 spawnPos) : base(spawnPos, RADIUS, MASS, ATTRACTION_COEFFICIENT, "oxygen")
	{
	}
}
