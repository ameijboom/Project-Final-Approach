// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;
using PFA.MyGame.CatomTypes;
using PFA.MyGame.Models.Game;

namespace PFA.MyGame.Managers;

public static class PhysicsManager
{
	// private const int BALLS = 50;
	private const float SHOOT_FORCE = 100f;
	private const float CONNECTED_BALL_DAMP_FAC = 0.0f;

	public static Ball? selectedBall { get; private set; } = null;
	// private static LineSegment? _selectedLine = null;

	private static readonly List<Ball> Balls = new();
	private static readonly List<LineSegment> Lines = new();
	private static bool _bSelectedLineStart = false;
	private static Random rng = new Random(); 
	private const float LINE_RADIUS = 20;

	private static readonly MyGame Game;
	private static Field BOX;
	private static ICollection<string> Catoms = new List<string>() {"H", "O", "N", "C", "P", "Ca", "Na",};
	public static List<Dictionary<string, int>> pairs = Molecats.molecats;

	static PhysicsManager()
	{
		Game = (MyGame)GXPEngine.Game.main;
		BOX = MyGame.main.FindObjectOfType<Field>();

		//TODO (Alwin): Revamp these spawn conditions depending on the required molecats
		// for (int i = 0; i < BALLS; i++)
		// {
		// 	Vec2 spawnPos = RandomSpawnPos();
		// 	Catom catom = Utils.Random(0, 6) switch
		// 	{
		// 		0 => new CatCalcium(spawnPos),
		// 		1 => new CatCarbon(spawnPos),
		// 		2 => new CatHydrogen(spawnPos),
		// 		3 => new CatNitrogen(spawnPos),
		// 		4 => new CatOxygen(spawnPos),
		// 		5 => new CatPhosphorus(spawnPos),
		// 		6 => new CatSodium(spawnPos),
		// 		_ => throw new Exception("aa"),
		// 	};

		// 	AddBall(catom);
		// }

		SpawnCatoms();


		System.Console.WriteLine(MyGame.main.FindObjectOfType<Field>());

		AddLine(0, -LINE_RADIUS, BOX.width, -LINE_RADIUS); //top
		AddLine(-LINE_RADIUS, 0, -LINE_RADIUS, BOX.height); //left
		AddLine(0, BOX.height, BOX.width, BOX.height); //bottom
		AddLine(BOX.width+LINE_RADIUS, 0, BOX.width+LINE_RADIUS, BOX.height); //right
	}
	private static void AddBall(Ball ball)
	{
		Balls.Add(ball);
		Game.AddChild(ball);
	}

	private static Vec2 RandomSpawnPos()
	{
		return new Vec2(Utils.Random(0, BOX.width), Utils.Random(0, BOX.height - LINE_RADIUS));
	}

	public static void RemoveBall(Ball ball)
	{
		Balls.Remove(ball);
		Game.RemoveChild(ball);
	}

	private static void AddLine(float x1, float y1, float x2, float y2)
	{
		LineSegment line = new(x1, y1, x2, y2, LINE_RADIUS);
		Lines.Add(line);
		Game.AddChild(line);
	}

	private static void MouseBehaviour()
	{
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
		{
			selectedBall = null;
			foreach (Ball ball in Balls.Where(ball => Ball.IsPointInCircle(ball, Input.mouse)))
			{
				selectedBall = ball;
				break;
			}
		}

		if (Input.GetMouseButton(0))
		{
			if (selectedBall != null)
			{
				selectedBall.position = Input.mouse;
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			selectedBall = null;
		}

		if (Input.GetMouseButtonUp(1))
		{
			if (selectedBall != null) {
				selectedBall.ApplyForce(SHOOT_FORCE * (selectedBall.CachedPosition - Input.mouse));

				if (selectedBall is Catom catom)
				{
					catom.ReadyToCombine = true;
				}
			}

			selectedBall = null;
		}
	}

	private static void PhysicsBehaviour()
	{
		List<Tuple<Ball, Ball>> collidingPairs = new();
		List<Ball?> fakeBalls = new();

		const int nSimulationUpdates = 3;
		float fSimElapsedTime = MyGame.fElapsedTime / nSimulationUpdates;

		const int nMaxSimulationSteps = 15;

		// Main simulation loop
		for (int i = 0; i < nSimulationUpdates; i++)
		{
			// Set all balls time to maximum for this epoch
			foreach (Ball ball in Balls)
			{
				ball.FSimTimeRemaining = fSimElapsedTime;
				ball.CachedPosition = ball.position;
			}

			for (int j = 0; j < nMaxSimulationSteps; j++)
			{
				// Update Ball Positions
				foreach (Ball ball in Balls)
				{
					if (!(ball.FSimTimeRemaining > 0.0f)) continue;
					ball.OldPosition = ball.CachedPosition;

					// Update Ball Physics
					ball.Velocity += ball.Acceleration * ball.FSimTimeRemaining;
					ball.CachedPosition += ball.Velocity * ball.FSimTimeRemaining;
					ball.Acceleration *= 0f;

					// Make sure the balls stay inside the game view
					if (ball.CachedPosition.x < -ball.Radius)
					{
#if DEBUG
						Console.WriteLine("oh no at left");
#endif
						ball.CachedPosition.x = BOX.width/2f;
						ball.Velocity.Limit(Ball.START_SPEED);
					}

					if (ball.CachedPosition.x >= BOX.width + ball.Radius)
					{
#if DEBUG
						Console.WriteLine("oh no at right");
#endif
						ball.CachedPosition.x = BOX.width/2f;
						ball.Velocity.Limit(Ball.START_SPEED);
					}

					if (ball.CachedPosition.y < -ball.Radius)
					{
#if DEBUG
						Console.WriteLine("oh no at top");
#endif
						ball.CachedPosition.y = BOX.height/2f;
						ball.Velocity.Limit(Ball.START_SPEED);
					}

					if (ball.CachedPosition.y >= BOX.height + ball.Radius)
					{
#if DEBUG
						Console.WriteLine("oh no at bottom");
#endif
						ball.CachedPosition.y = BOX.height/2f;
						ball.Velocity.Limit(Ball.START_SPEED);
					}

					if (ball.Velocity.MagSq() < 0.01f) ball.Velocity = new Vec2();
				}

				// Static collisions, i.e. overlap
				foreach (Ball ball in Balls)
				{
					// Against Edges
					foreach (LineSegment edge in Lines)
					{
						Vec2 line1 = edge.End - edge.Start;
						Vec2 line2 = ball.CachedPosition - edge.Start;

						float fEdgeLength = line1.MagSq();
						float fEdgeDot = Vec2.Dot(line1, line2);
						float t = Mathf.Clamp(fEdgeDot, 0.0f, fEdgeLength) / fEdgeLength;

						Vec2 closestPoint = edge.Start + t * line1;

						float fDistance = (ball.CachedPosition - closestPoint).Mag();

						if (fDistance > ball.Radius + edge.Radius) continue;
						// Static collision has occured
						Ball fakeBall = new(closestPoint, edge.Radius, ball.Mass)
						{
							Velocity = -ball.Velocity,
						};

						fakeBalls.Add(fakeBall);
						collidingPairs.Add(new Tuple<Ball, Ball>(ball, fakeBall));

						float fOverlap = fDistance - ball.Radius - fakeBall.Radius;

						// Displace Current Ball away from the collision
						ball.CachedPosition -= fOverlap * (ball.CachedPosition - fakeBall.CachedPosition) / fDistance;
					}

					foreach (Ball target in Balls)
					{
						if (ball == target) continue;
						if (!Ball.DoCirclesOverlap(ball, target)) continue;

						// Collision has occured
						collidingPairs.Add(new Tuple<Ball, Ball>(ball, target));

						Catom cBall = (Catom) ball;
						Catom cTarget = (Catom) target;
						if (cBall.ReadyToCombine || cTarget.ReadyToCombine)
						{
							// SoundManager.PlayHappyCat();
							
							if (!cTarget.Bros.Contains(cBall))
							{
								cTarget.Bros.Add(cBall);
								cBall.ReadyToCombine = false;
							}

							if (!cBall.Bros.Contains(cTarget))
							{
								cBall.Bros.Add(cTarget);
								cTarget.ReadyToCombine = false;
							}

							if (!pairs.First().ContainsKey(cBall.Symbol) || !pairs.First().ContainsKey(cTarget.Symbol))
							{
								SoundManager.PlaySadCat();
								RemoveBall(cBall);
								RemoveBall(cTarget);
							} else
							{
								if (pairs.First()[cBall.Symbol] != 1)
								{
									pairs.First()[cBall.Symbol] -= 1;
								} else
								{
									pairs.First().Remove(cBall.Symbol);
								}

								SoundManager.PlayHappyCat();
							}

							
						}

						// Make balls visually rotate
						ball.SetAngularVelocity();
						target.SetAngularVelocity();

						// Distance between ball centers
						float fDistance = Vec2.Dist(ball.CachedPosition, target.CachedPosition);
						float fOverlap = 0.5f * (fDistance - ball.Radius - target.Radius);

						// Displace Current Ball
						ball.CachedPosition -= fOverlap * (ball.CachedPosition - target.CachedPosition) / fDistance;

						// Displace Target Ball
						target.CachedPosition += fOverlap * (ball.CachedPosition - target.CachedPosition) / fDistance;
					}

					// Time displacement
					float fIntendedSpeed = ball.Velocity.Mag();
					float fActualDistance = Vec2.Dist(ball.CachedPosition, ball.OldPosition);
					float fActualTime = fActualDistance / fIntendedSpeed;

					ball.FSimTimeRemaining -= fActualTime;
				}

				// Now work out dynamic collisions
				foreach ((Ball b1, Ball b2) in collidingPairs)
				{
					float fac = 1.0f;
					if (b1.GetType() == typeof(Catom) && b2.GetType() == typeof(Catom))
					{
						Catom cB1 = (Catom) b1;
						Catom cB2 = (Catom) b2;
						if(cB1.Bros.Contains(cB2) || cB2.Bros.Contains(cB1)) fac =  CONNECTED_BALL_DAMP_FAC;
					}

					// Distance between balls
					float fDistance = Vec2.Dist(b1.CachedPosition, b2.CachedPosition);

					// Normal
					Vec2 n = (b2.CachedPosition - b1.CachedPosition) / fDistance;

					// Wikipedia Version
					Vec2 k = b1.Velocity - b2.Velocity;
					float p = 2.0f * Vec2.Dot(k, n) / (b1.Mass + b2.Mass);
					b1.Velocity -= p * b2.Mass * n * fac;
					b2.Velocity += p * b1.Mass * n * fac;
				}

				// Remove fake balls
				for (int k = 0; k < fakeBalls.Count; k++) fakeBalls[k] = null;
				fakeBalls.Clear();

				// Remove collisions
				collidingPairs.Clear();
			}
		}
	}

	private static void SpawnCatoms()
	{
		foreach(string catom in Catoms)
		{
			for(int i = 0; i < Molecats.GetCatomCount(catom); i++)
			{
				AddBall(CreateCatom(catom));
			}
		}

		pairs.Shuffle();
	}

	private static Catom CreateCatom(string symbol)
	{
		switch(symbol)
		{
			case "H":
				return new CatHydrogen(RandomSpawnPos());
			case "O":
				return new CatOxygen(RandomSpawnPos());
			case "N":
				return new CatNitrogen(RandomSpawnPos());
			case "C":
				return new CatCarbon(RandomSpawnPos());
			case "P":
				return new CatPhosphorus(RandomSpawnPos());
			case "Ca":
				return new CatCalcium(RandomSpawnPos());
			case "Na":
				return new CatSodium(RandomSpawnPos());
			default:
				throw new Exception("Not a catom dummy >:(");
		}
	} 

	public static void Shuffle<T>(this IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	public static void Step()
	{
		MouseBehaviour();

		PhysicsBehaviour();
	}
}
