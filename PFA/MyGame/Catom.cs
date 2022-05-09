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

	public bool JustBouncedOffPlayer;
	public readonly HashSet<Ball> Bros = new();

	protected Catom(Vec2 spawnPos, float radius, float mass = 0, string assetName = "circle") : base(spawnPos, radius * RADIUS_FAC, mass, assetName)
	{
	}

	public new void Update()
	{
		base.Update();
		Colour = JustBouncedOffPlayer ? Colour.Fuchsia : Colour.White;
		foreach (Ball bro in Bros)
		{
			Gizmos.DrawLine(position + Vec2.Random() * (Radius/2f),
				bro.position + Vec2.Random() * (bro.Radius/2f),
				width: 1, colour:new Colour(255, 127));
			Vec2 diff = bro.position - position;
			float r = diff.Mag();
			Vec2 dir = diff.Normalized();
			Vec2 force = Force(r, dir);
			// Utils.print(Mathf.Round(r), Mathf.Round(force.Mag()));
			ApplyForce(force);
		}
	}

	private Vec2 Force(float r, Vec2 dir)
	{
		//TODO: Expose these constants
		const float b = 55;
		float k = 20 * (1f/Bros.Count);
		// r *= 0.5f;
		// return (r - Mathf.Pow(b / r, 4)) * k * dir;
		// return (r - b) * k * dir;
		return ((r - b) * Mathf.Abs(r - b)) / b * dir * k;
		// return (Mathf.Sqrt(Mathf.Abs(r - b)) * (r - b) / Mathf.Abs(r - b) * k) * dir;
	}
}
