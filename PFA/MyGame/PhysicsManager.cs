﻿// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame;

public static class PhysicsManager
{
	private const float SHOOT_FORCE = 100f;

	public static Ball? selectedBall { get; private set; } = null;
	private static LineSegment? _selectedLine = null;

	private static readonly List<Ball> Balls = new();
	private static readonly List<LineSegment> Lines = new();
	private static bool _bSelectedLineStart = false;

	private static readonly MyGame Game;

	static PhysicsManager()
	{
		Game = (MyGame)GXPEngine.Game.main;
	}

	public static void AddBall(float x, float y)
	{
		Ball ball = new(x, y, 20)
		{
			Velocity = Vec2.Random() * 10,
		};
		Balls.Add(ball);
		Game.AddChild(ball);
	}

	public static void RemoveBall(Ball ball)
	{
		Balls.Remove(ball);
		Game.RemoveChild(ball);
	}

	public static void AddLine(float x1, float y1, float x2, float y2)
	{
		const float fLineRadius = 20;
		LineSegment line = new(x1, y1, x2, y2, fLineRadius);
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

			// Check for selected line segment end
			_selectedLine = null;
			foreach (LineSegment line in Lines)
			{
				if (Ball.IsPointInCircle(line.Start, line.Radius, Input.mouse))
				{
					_selectedLine = line;
					_bSelectedLineStart = true;
					break;
				}

				if (Ball.IsPointInCircle(line.End, line.Radius, Input.mouse))
				{
					_selectedLine = line;
					_bSelectedLineStart = false;
					break;
				}
			}
		}

		if (Input.GetMouseButton(0))
		{
			if (selectedBall != null)
			{
				selectedBall.CachedPosition = Input.mouse;
			}

			if (_selectedLine != null)
			{
				if (_bSelectedLineStart)
					_selectedLine.Start = Input.mouse;
				else
					_selectedLine.End = Input.mouse;
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			selectedBall = null;
			_selectedLine = null;
		}

		if (Input.GetMouseButtonUp(1))
		{
			selectedBall?.ApplyForce(SHOOT_FORCE * (selectedBall.CachedPosition - Input.mouse));

			selectedBall = null;
		}
	}

	private static void PhysicsBehaviour(float fElapsedTime)
	{
		List<Tuple<Ball, Ball>> collidingPairs = new();
		List<Ball?> fakeBalls = new();

		const int nSimulationUpdates = 3;
		float fSimElapsedTime = fElapsedTime / nSimulationUpdates;

		const int nMaxSimulationSteps = 15;

		// Main simulation loop
		for (int i = 0; i < nSimulationUpdates; i++)
		{
			// Set all balls time to maximum for this epoch
			foreach (Ball ball in Balls)
			{
				ball.FSimTimeRemaining = fSimElapsedTime;
				ball.CachedPosition = ball.CachedPosition;
			}

			for (int j = 0; j < nMaxSimulationSteps; j++)
			{
				// Update Ball Positions
				foreach (Ball ball in Balls)
				{
					if (ball.FSimTimeRemaining > 0.0f)
					{
						ball.OldPosition = ball.CachedPosition;

						// Update Ball Physics
						ball.Velocity += ball.Acceleration * ball.FSimTimeRemaining;
						ball.CachedPosition += ball.Velocity * ball.FSimTimeRemaining;
						ball.Acceleration *= 0f;

						// Wrap the balls around the screen
						if (ball.CachedPosition.x < 0) ball.x += Game.width;
						if (ball.CachedPosition.x >= Game.width) ball.x -= Game.width;
						if (ball.CachedPosition.y < 0) ball.y += Game.height;
						if (ball.CachedPosition.y >= Game.height) ball.y -= Game.height;

						if (ball.Velocity.MagSq() < 0.01f) ball.Velocity = new Vec2();
					}
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

						if (!(fDistance < ball.Radius + edge.Radius)) continue;
						// Static collision has occured
						Ball fakeBall = new(closestPoint.x, closestPoint.y, edge.Radius, ball.Mass)
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
						if (ball != target)
						{
							if (Ball.DoCirclesOverlap(ball, target))
							{
								// Collision has occured
								collidingPairs.Add(new Tuple<Ball, Ball>(ball, target));

								// Distance between ball centers
								float fDistance = Vec2.Dist(ball.CachedPosition, target.CachedPosition);
								float fOverlap = 0.5f * (fDistance - ball.Radius - target.Radius);

								// Displace Current Ball
								ball.CachedPosition -= fOverlap * (ball.CachedPosition - target.CachedPosition) / fDistance;

								// Displace Target Ball
								target.CachedPosition += fOverlap * (ball.CachedPosition - target.CachedPosition) / fDistance;
							}
						}
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
					// Distance between balls
					float fDistance = Vec2.Dist(b1.CachedPosition, b2.CachedPosition);

					// Normal
					Vec2 n = (b2.CachedPosition - b1.CachedPosition) / fDistance;

					// Wikipedia Version
					Vec2 k = b1.Velocity - b2.Velocity;
					float p = 2.0f * Vec2.Dot(k, n) / (b1.Mass + b2.Mass);
					b1.Velocity -= p * b2.Mass * n;
					b2.Velocity += p * b1.Mass * n;
				}

				// Remove fake balls
				for (int k = 0; k < fakeBalls.Count; k++) fakeBalls[k] = null;
				fakeBalls.Clear();

				// Remove collisions
				collidingPairs.Clear();
			}
		}
	}

	public static void Step()
	{
		float fElapsedTime = Time.deltaTime / 1000f;
#if DEBUG
		Utils.print("FPS: " + Game.currentFps, "fElapsedTime: " + fElapsedTime);
#endif
		MouseBehaviour();

		PhysicsBehaviour(fElapsedTime);
	}
}