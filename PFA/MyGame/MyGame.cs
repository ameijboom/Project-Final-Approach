// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.Utils;
using PFA.MyGame.Managers;

namespace PFA.MyGame;

public class MyGame : Game
{
	public static float fElapsedTime = 0f;

	private MyGame(int width, int height) : base(1920, 1080, false, false, width, height)
	{
		// targetFps = 30; //time for a console game
		Utils.print("MyGame initialized");

		// Play Main Menu beginning music
		SoundManager.PlayBackground(SoundManager.BGM.Game);

		SceneManager.ShowScenes();
		SceneManager.ActivateScene("main");

		System.Console.WriteLine(MolecatToMake());

	}

	// ReSharper disable once UnusedMember.Local
	private void Update()
	{
		// if (Input.GetKeyDown(Key.BACKSPACE))
		// {
		// 	System.Console.WriteLine("Press");
		// 	SceneManager.ActivateScene("main");
		// }

		fElapsedTime = Time.deltaTime / 1000f;
// #if DEBUG
// 		Utils.print("FPS: " + currentFps, "fElapsedTime: " + fElapsedTime);
// #endif
		PhysicsManager.Step();

		if (PhysicsManager.selectedBall != null)
		{
			//Draw Cue
			Gizmos.DrawLine(PhysicsManager.selectedBall.CachedPosition, Input.mouse, colour:Colour.Blue); //TODO: Designer overhaul necessary
		}

		// Play Main Menu infinite loop music
		// if (!SoundManager.GetBackground(SoundManager.BGM.MmBegin).IsPlaying()
		//     && !SoundManager.GetBackground(SoundManager.BGM.MmLoop).IsPlaying())
		// {
		// 	SoundManager.PlayBackground(SoundManager.BGM.MmLoop);
		// 	Console.WriteLine("Starting background infinite loop");
		// }



		// Utils.print(GetDiagnostics());
	}

	public string MolecatToMake() {
		string MolecatToMake = "";
		foreach(string k in PhysicsManager.Pairs.First().Keys)
		{
			MolecatToMake+=k;
			MolecatToMake+=PhysicsManager.Pairs.First()[k];
		}
		return MolecatToMake;
	}

	private static void Main()
	{
		new MyGame(1600, 900).Start();
	}
}
