// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame.CatomTypes;

public class CatHydrogen : Catom
{
	private const float RADIUS = 53.0f;
	private const float MASS = 1.00797f;
	private const float ATTRACTION_COEFFICIENT = 30f;

	public CatHydrogen(Vec2 spawnPos) : base(spawnPos, RADIUS, MASS, ATTRACTION_COEFFICIENT, "hydrogen")
	{
	}
}
