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
	private const float RADIUS_FAC = 0.1f;
	private const float SPACE_BETWEEN_CATOMS = 10f; // How far apart should the catoms try to be from each other?

	public bool JustBouncedOffPlayer;
	public readonly HashSet<Ball> Bros = new();

	protected Catom(Vec2 spawnPos, float radius, float mass, string assetName = "circle") : base(spawnPos, radius * RADIUS_FAC, mass, assetName)
	{
	}

	public new void Update()
	{
		base.Update();
		Colour = JustBouncedOffPlayer ? Colour.Fuchsia : Colour.White;
		foreach (Ball bro in Bros)
		{
			Gizmos.DrawLine(
				position + Vec2.Random() * (Radius / 2f),
				bro.position + Vec2.Random() * (bro.Radius / 2f),
				colour: new Colour(255, 127),
				width: 1);

			Vec2 diff = bro.position - position;
			float r = diff.Mag();
			float b = Radius + bro.Radius + SPACE_BETWEEN_CATOMS;
			ApplyForce(Force(r, b) * diff.Normalized());
		}
	}

	/// <param name="r">current distance</param>
	/// <param name="b">distance of equilibrium</param>
	private float Force(float r, float b)
	{
		//TODO: Expose these constants (20, 1f)
		float k = 20 * (1f/Bros.Count);
		return ((r - b) * Mathf.Abs(r - b)) / b * k;
	}
}
