// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame.CatomTypes;

public class CatPhosphorus : Catom
{
	private const float RADIUS = 98.0f;
	private const float MASS = 30.97376f;
	private const float ATTRACTION_COEFFICIENT = 27f;

	public CatPhosphorus(Vec2 spawnPos) : base(spawnPos, RADIUS, MASS, ATTRACTION_COEFFICIENT, "phosphorus", "P")
	{
	}
}
