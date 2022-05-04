// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

// #define DEBUG_MODE

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;
using PFA.MyGame.BasicComponents;

namespace PFA.MyGame;

public class MyGame : Game
{
	public static readonly List<Wall> Walls = new();
	public static readonly List<Catom> Catoms = new();

	private MyGame() : base(500, 500, false, false)
	{
		targetFps = 60;
		Utils.print("MyGame initialized");

		void AddMirror(Vec2 start, Vec2 end)
		{
			Wall newWall = new(start, end);
			Walls.Add(newWall);
			AddChild(newWall);
		}

		AddMirror(new Vec2(10, 10), new Vec2(10, height - 10)); // left
		AddMirror(new Vec2(width - 10, 10), new Vec2(width - 10, height - 10)); // right
		AddMirror(new Vec2(10, height - 10), new Vec2(width - 10, height - 10)); // bottom
		AddMirror(new Vec2(10, 10), new Vec2(width - 10, 10)); // top

		for (int i = 0; i < 100; i++)
		{
			Catom catom = new(new Vec2(width / 2f, height / 2f), 20);
			catom.ApplyForce(Vec2.Random() * Utils.Random(3f, 10f));
			// catom.ApplyForce(Vec2.Random() * 200f);
			// catom.ApplyForce(new Vec2(10, 0));
			AddChild(catom);
		}

	}

	// ReSharper disable once UnusedMember.Local
	private void Update()
	{
		Console.WriteLine(currentFps);
		#if DEBUG_MODE
		{
			LineSegment lDiego = new(new Vec2(0, 0), new Vec2(width, height));
			LineSegment lMouse = Input.GetMouseButton(0)
				? new LineSegment(new Vec2(width, 0), new Vec2(Input.mouse.x, Input.mouse.y))
				: new LineSegment(new Vec2(0, height), new Vec2(Input.mouse.x, Input.mouse.y));
			lDiego.Draw();
			lMouse.Draw();

			// Console.WriteLine(lDiego.DistanceToPoint(Input.mouse));

			(bool hit, Vec2 intersectionPoint, Vec2 reflectionVector) = LineSegment.Reflect(lMouse, lDiego);
			// Utils.print(hit, intersectionPoint, reflectionVector);
			if (!hit) return;
			Gizmos.SetColour(new Colour(0, 0, 255, 200));
			Gizmos.DrawRay(intersectionPoint, lDiego.GetDir().GetNormal().SetMag(100)); //diego's normal
			Gizmos.SetColour(new Colour(255, 0, 0));
			Gizmos.DrawCircle(intersectionPoint, 10);
			Gizmos.DrawRay(intersectionPoint, reflectionVector);
		}
		#endif

	}

	private static void Main()
	{
		new MyGame().Start();
	}
}
