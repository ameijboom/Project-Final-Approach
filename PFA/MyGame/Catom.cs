// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.Core;

namespace PFA.MyGame;

public class Catom : Ball
{
	public bool JustBouncedOffPlayer;

	public Catom(float x, float y, float radius, float mass = 0) : base(x, y, radius, mass)
	{
	}

	public new void Update()
	{
		base.Update();
		Colour = JustBouncedOffPlayer ? Colour.Fuchsia : Colour.White;
	}
}
