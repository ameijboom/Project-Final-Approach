// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame.CatomTypes;

public class CatCarbon : Catom
{
	private const float RADIUS = 67.0f;
	private const float MASS = 12.011f;
	private const float ATTRACTION_COEFFICIENT = 15f;

	public CatCarbon(Vec2 spawnPos) : base(spawnPos, RADIUS, MASS, ATTRACTION_COEFFICIENT, "carbon")
	{
	}
}
