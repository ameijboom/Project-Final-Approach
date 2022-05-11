// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame.CatomTypes;

public class CatSodium : Catom
{
	private const float RADIUS = 190.0f;
	private const float MASS = 22.98977f;
	private const float ATTRACTION_COEFFICIENT = 23f;

	public CatSodium(Vec2 spawnPos) : base(spawnPos, RADIUS, MASS, ATTRACTION_COEFFICIENT, "sodium")
	{
	}
}
