﻿// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame;

public class MyGame : Game
{
	public static float fElapsedTime = 0f;

	private MyGame(int width, int height) : base(width, height, false, false)
	{
		targetFps = 30; //time for a console game
		Utils.print("MyGame initialized");
	}

	// ReSharper disable once UnusedMember.Local
	private void Update()
	{
		fElapsedTime = Time.deltaTime / 1000f;
		float tempElapsedTime = fElapsedTime;
		fElapsedTime = Mathf.Clamp(Mathf.Map(Input.mouse.x, 0, width, 0, tempElapsedTime), 0, 1);  //TODO: Base this on something else than the mouse X position.
#if DEBUG
		Utils.print("FPS: " + currentFps, "fElapsedTime: " + fElapsedTime);
#endif
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
		new MyGame(1600, 900).Start();
	}
}
