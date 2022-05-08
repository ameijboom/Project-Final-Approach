// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame;

public class MyGame : Game
{
	private MyGame(int width, int height) : base(width, height, false, false)
	{
		targetFps = 30; //time for a console game
		// AddBall(width * 0.25f, height * 0.5f);
		// AddBall(width * 0.75f, height * 0.5f);
		Utils.print("MyGame initialized");
	}

	// ReSharper disable once UnusedMember.Local
	private void Update()
	{
		PhysicsManager.Step();

		if (PhysicsManager.selectedBall != null)
		{
			//Draw Cue
			Gizmos.DrawLine(PhysicsManager.selectedBall.CachedPosition, Input.mouse, colour:Colour.Blue);
		}
		// Utils.print(GetDiagnostics());
	}

	private static void Main()
	{
		new MyGame(400, 800).Start();
	}
}
