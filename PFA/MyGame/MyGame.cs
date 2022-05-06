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

	public static readonly List<LineSegment> Lines = new();
	public LineSegment? SelectedLine = null;
	public bool bSelectedLineStart = false;

	private MyGame() : base(1280, 960, false, true)
	{
		void AddBall(float x, float y)
		{
			Ball ball = new(x, y, 20);
			Balls.Add(ball);
			AddChild(ball);
		}

		// AddBall(width * 0.25f, height * 0.5f);
		// AddBall(width * 0.75f, height * 0.5f);

		for (int i = 0; i < 200; i++)
			AddBall(Utils.Random(0, width), Utils.Random(0, height));

		const float fLineRadius = 20;
		Lines.Add(new LineSegment(100, 100, 500, 100, fLineRadius));
		Lines.Add(new LineSegment(100, 200, 500, 200, fLineRadius));
		Lines.Add(new LineSegment(100, 300, 500, 300, fLineRadius));
		Lines.Add(new LineSegment(100, 400, 500, 400, fLineRadius));


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

			// Check for selected line segment end
			SelectedLine = null;
			foreach (LineSegment line in Lines)
			{
				if (IsPointInCircle(line.Start, line.Radius, Input.mouse))
				{
					SelectedLine = line;
					bSelectedLineStart = true;
					break;
				}

				if (IsPointInCircle(line.End, line.Radius, Input.mouse))
				{
					SelectedLine = line;
					bSelectedLineStart = false;
					break;
				}
			}
		}

		if (Input.GetMouseButton(0))
		{
			if (SelectedBall != null)
			{
				SelectedBall.position = Input.mouse;
			}

			if (SelectedLine != null)
			{
				if (bSelectedLineStart)
					SelectedLine.Start = Input.mouse;
				else
					SelectedLine.End = Input.mouse;
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			SelectedBall = null;
			SelectedLine = null;
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
		List<Ball?> fakeBalls = new();

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
						ball.Acceleration.y += 9.8f; //gravity

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


					// Against Edges
					foreach (LineSegment edge in Lines)
					{
						Vec2 line1 = edge.End - edge.Start;
						Vec2 line2 = ball.position - edge.Start;

						float fEdgeLength = line1.MagSq();
						float fEdgeDot = Vec2.Dot(line1, line2);
						float t = Mathf.Clamp(fEdgeDot, 0.0f, fEdgeLength) / fEdgeLength;

						Vec2 closestPoint = edge.Start + t * line1;

						float fDistance = (ball.position - closestPoint).Mag();

						if (fDistance < ball.Radius + edge.Radius)
						{
							// Static collision has occured
							Ball fakeBall = new(closestPoint.x, closestPoint.y, edge.Radius, ball.Mass)
							{
								Velocity = -ball.Velocity,
							};

							fakeBalls.Add(fakeBall);
							collidingPairs.Add(new Tuple<Ball, Ball>(ball, fakeBall));

							float fOverlap = fDistance - ball.Radius - fakeBall.Radius;

							// Displace Current Ball away from the collision
							ball.position -= fOverlap * (ball.position - fakeBall.position) / fDistance;
						}
					}



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

				// Remove fake balls
				for (int k = 0; k < fakeBalls.Count; k++) fakeBalls[k] = null;
				fakeBalls.Clear();

				// Remove collisions
				collidingPairs.Clear();
			}
		}

		// Draw Lines
		foreach (LineSegment lineSegment in Lines)
		{
			lineSegment.Render();
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

	private static bool IsPointInCircle(Vec2 pc, float r, Vec2 p)
	{
		return Vec2.DistSq(pc, p) < Mathf.Sq(r);
	}

	private static bool IsPointInCircle(Ball c, Vec2 p)
	{
		return IsPointInCircle(c.position, c.Radius, p);
	}

	private static void Main()
	{
		new MyGame().Start();
	}
}
