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
	private const float RADIUS_FAC = 0.2f;
	private const float SPACE_BETWEEN_CATOMS = 10f; // How far apart should the catoms try to be from each other?

	public bool ReadyToCombine;
	public readonly HashSet<Ball> Bros = new();
	private readonly float _attractionCoefficient;

	protected Catom(Vec2 spawnPos, float radius, float mass, float attractionCoefficient, string assetName = "circle") :
		base(spawnPos, radius * RADIUS_FAC, mass, assetName)
	{
		_attractionCoefficient = attractionCoefficient;
	}

	public new void Update()
	{
		base.Update();
		Colour = ReadyToCombine ? Colour.Fuchsia : Colour.White;
		foreach (Ball bro in Bros)
		{
			Gizmos.DrawLine(
				position + Vec2.Random() * (Radius / 2f),
				bro.position + Vec2.Random() * (bro.Radius / 2f),
				colour: new Colour(255, 127),
				width: 1);

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
