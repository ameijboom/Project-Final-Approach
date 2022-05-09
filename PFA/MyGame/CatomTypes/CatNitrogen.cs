// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame.CatomTypes;

public class CatNitrogen : Catom
{
	private const float RADIUS = 56.0f;
	private const float MASS = 14.0067f;
	private const float ATTRACTION_COEFFICIENT = 18f;

	public CatNitrogen(Vec2 spawnPos) : base(spawnPos, RADIUS, MASS, ATTRACTION_COEFFICIENT, "nitrogen")
	{
	}
}
