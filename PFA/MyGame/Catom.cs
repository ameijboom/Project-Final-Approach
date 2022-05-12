// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame;

public class Catom : Ball
{
	private const float RADIUS_FAC = 0.5f;
	private const float SPACE_BETWEEN_CATOMS = 10f; // How far apart should the catoms try to be from each other?

	public bool ReadyToCombine;
	public readonly string Symbol;
	public readonly HashSet<Ball> Bros = new();
	private readonly float _attractionCoefficient;

	protected Catom(Vec2 spawnPos, float radius, float mass, float attractionCoefficient, string assetName = "circle", string symbol = "") :
		base(spawnPos, radius * RADIUS_FAC, mass, assetName)
	{
		_attractionCoefficient = attractionCoefficient;
		Symbol = symbol;
	}

	public new void Update()
	{
		base.Update();
		if (ReadyToCombine && Velocity.MagSq() < MAX_SPEED)
		{
			ReadyToCombine = false;
		}
		Colour = ReadyToCombine ? new Colour(235, 185, 202, 120) : Colour.White;
		foreach (Ball bro in Bros)
		{
			Gizmos.DrawLine(position, bro.position, colour: new Colour(255, 164), width: 10);

			Vec2 diff = bro.position - position;
			ApplyForce(Force(diff.Mag(), Radius + bro.Radius + SPACE_BETWEEN_CATOMS) * diff.Normalized());
		}
	}

	/// <param name="r">current distance</param>
	/// <param name="b">distance of equilibrium</param>
	private float Force(float r, float b)
	{
		float k = _attractionCoefficient / Bros.Count;
		r -= b;
		return k * r * Mathf.Abs(r) / b;
	}
}
