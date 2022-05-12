// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;

namespace PFA.MyGame.CatomTypes;

public class CatCalcium : Catom
{
	private const float RADIUS = 175.0f;
	private const float MASS = 40.08f;
	private const float ATTRACTION_COEFFICIENT = 32f;

	public CatCalcium(Vec2 spawnPos) : base(spawnPos, RADIUS, MASS, ATTRACTION_COEFFICIENT, "calcium", "CA")
	{
	}
}
