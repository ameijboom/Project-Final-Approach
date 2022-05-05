// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame;

public class MyGame : Game
{
	public static readonly List<Ball> Balls = new();
	public Ball? SelectedBall = null;

	private MyGame() : base(1280, 960, false, true)
	{
		void AddBall(float x, float y)
		{
			Ball ball = new(x, y, Utils.Random(30, 100));
			Balls.Add(ball);
			AddChild(ball);
		}

		// AddBall(width * 0.25f, height * 0.5f);
		// AddBall(width * 0.75f, height * 0.5f);

		for (int i = 0; i < 10; i++)
		{
			AddBall(Utils.Random(0, width), Utils.Random(0, height));
		}
		Utils.print("MyGame initialized");
	}

	// ReSharper disable once UnusedMember.Local
	private void Update()
	{
		float fElapsedTime = Time.deltaTime / 1000f;
		Utils.print("FPS: " + currentFps, "fElapsedTime: " + fElapsedTime);

		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
		{
			SelectedBall = null;
			foreach (Ball ball in Balls.Where(ball => IsPointInCircle(ball, Input.mouse)))
			{
				SelectedBall = ball;
				break;
			}
		}

		if (Input.GetMouseButton(0) && SelectedBall != null)
		{
			SelectedBall.position = Input.mouse;
		}

		if (Input.GetMouseButtonUp(0))
		{
			SelectedBall = null;
		}

		if (Input.GetMouseButtonUp(1))
		{
			if (SelectedBall != null)
			{
				// Apply velocity
				SelectedBall.Velocity = 5.0f * (SelectedBall.position - Input.mouse);
			}

			SelectedBall = null;
		}

		List<Tuple<Ball, Ball>> collidingPairs = new();

		const int nSimulationUpdates = 4;
		float fSimElapsedTime = fElapsedTime / nSimulationUpdates;

		const int nMaxSimulationSteps = 15;

		// Main simulation loop
		for (int i = 0; i < nSimulationUpdates; i++)
		{
			// Set all balls time to maximum for this epoch
			foreach (Ball ball in Balls)
				ball.fSimTimeRemaining = fSimElapsedTime;

			for (int j = 0; j < nMaxSimulationSteps; j++)
			{
				// Update Ball Positions
				foreach (Ball ball in Balls)
				{
					if (ball.fSimTimeRemaining > 0.0f)
					{
						ball.oldPosition = ball.position;

						// Add Drag to emulate rolling friction
						ball.Acceleration = -ball.Velocity * 0.8f;

						// Update Ball Physics
						ball.Velocity += ball.Acceleration * ball.fSimTimeRemaining;
						ball.position += ball.Velocity * ball.fSimTimeRemaining;

						// Wrap the balls around the screen
						if (ball.position.x < 0) ball.x += width;
						if (ball.position.x >= width) ball.x -= width;
						if (ball.position.y < 0) ball.y += height;
						if (ball.position.y >= height) ball.y -= height;

						if (ball.Velocity.MagSq() < 0.01f) ball.Velocity = new Vec2();
					}
				}

				// Static collisions, i.e. overlap
				foreach (Ball ball in Balls)
				{
					foreach (Ball target in Balls)
					{
						if (ball != target)
						{
							if (DoCirclesOverlap(ball, target)) //Static collision resolution
							{
								// Collision has occured
								collidingPairs.Add(new Tuple<Ball, Ball>(ball, target));

								// Distance between ball centers
								float fDistance = Vec2.Dist(ball.position, target.position);
								float fOverlap = 0.5f * (fDistance - ball.Radius - target.Radius);

								// Displace Current Ball
								ball.position -= fOverlap * (ball.position - target.position) / fDistance;

								// Displace Target Ball
								target.position += fOverlap * (ball.position - target.position) / fDistance;
							}
						}
					}

					// Time displacement
					float fIntendedSpeed = ball.Velocity.Mag();
					float fIntendedDistance = fIntendedSpeed * ball.fSimTimeRemaining;
					float fActualDistance = Vec2.Dist(ball.position, ball.oldPosition);
					float fActualTime = fActualDistance / fIntendedSpeed;

					ball.fSimTimeRemaining -= fActualTime;
				}

				// Now work out dynamic collisions
				foreach ((Ball b1, Ball b2) in collidingPairs)
				{
					// Distance between balls
					float fDistance = Vec2.Dist(b1.position, b2.position);

					// Normal
					Vec2 n = (b2.position - b1.position) / fDistance;

					// Tangent
					// Vec2 t = n.GetNormal();

					// Dot Product Tangents
					// float dpTan1 = Vec2.Dot(b1.Velocity, t);
					// float dpTan2 = Vec2.Dot(b2.Velocity, t);

					// Dot Product Normal
					// float dpNorm1 = Vec2.Dot(b1.Velocity, n);
					// float dpNorm2 = Vec2.Dot(b2.Velocity, n);

					// Conservation of momentum in 1D
					// float m1 = (dpNorm1 * (b1.Mass - b2.Mass) + 2.0f * b2.Mass * dpNorm2) / (b1.Mass + b2.Mass);
					// float m2 = (dpNorm2 * (b2.Mass - b1.Mass) + 2.0f * b1.Mass * dpNorm1) / (b1.Mass + b2.Mass);

					// Update ball velocities
					// b1.Velocity = t * dpTan1 + n * m1;
					// b2.Velocity = t * dpTan2 + n * m2;

					// Wikipedia Version
					Vec2 k = b1.Velocity - b2.Velocity;
					float p = 2.0f * Vec2.Dot(k, n) / (b1.Mass + b2.Mass);
					b1.Velocity -= p * b2.Mass * n;
					b2.Velocity += p * b1.Mass * n;
				}
			}
		}

		// Draw Balls
		foreach (Ball ball in Balls)
		{
			ball.Render();
		}

		foreach ((Ball b1, Ball b2) in collidingPairs)
		{
			Gizmos.DrawLine(b1.position, b2.position, colour:Colour.Red);
		}

		if (SelectedBall != null)
		{
			//Draw Cue
			Gizmos.DrawLine(SelectedBall.position, Input.mouse, colour:Colour.Blue);
		}

		// // Draw selected ball
		// if (SelectedBall != null)
		// {
		// 	Gizmos.DrawCircle(SelectedBall.position, SelectedBall.Radius, 20, colour:Colour.Blue, lineWidth:2);
		// }
	}

	private static bool DoCirclesOverlap(Ball c1, Ball c2)
	{
		return Vec2.DistSq(c1.position, c2.position) < Mathf.Sq(c1.Radius + c2.Radius);
	}

	private static bool IsPointInCircle(Ball c, Vec2 p)
	{
		return Vec2.DistSq(c.position, p) < Mathf.Sq(c.Radius);
	}


	private static void Main()
	{
		new MyGame().Start();
	}
}
