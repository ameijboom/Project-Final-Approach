// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame.BasicComponents;

public class Circle
{
	public float Radius;
	public Vec2 Position;

	public Circle(Vec2 position, float radius)
	{
		Radius = radius;
		Position = position;
	}

	/// <summary>
	/// Checks if two circles intersect. <br/>
	/// Also returns true if one circle is inside the other.
	/// </summary>
	public static bool Touching(Circle circle1, Circle circle2)
	{
		float distSq = Vec2.DistSq(circle1.Position, circle2.Position);
		float radiSq = Mathf.Sq(circle1.Radius + circle2.Radius);
		bool intersects = distSq < radiSq;
		return intersects;
	}

	/// <summary>
	/// Gives the intersection points of two circles.
	/// </summary>
	/// <remarks>
	/// If one circle is inside the other, there are no intersection points.<br/>
	/// Use <see cref="Touching"/> to check if two circles intersect or are inside each other.
	/// </remarks>
	public static List<Vec2> Intersects(Circle c1, Circle c2)
	{
		//calculate intersection points of the given two circles
		List<Vec2> intersectionPoints = new();
		float distSq = Vec2.DistSq(c1.Position, c2.Position);
		if (distSq > Mathf.Sq(c1.Radius + c2.Radius))
		{
			//no intersection
			return intersectionPoints;
		}

		if (distSq < Mathf.Sq(Mathf.Abs(c1.Radius - c2.Radius)))
		{
			//one circle is contained in the other
			return intersectionPoints;
		}

		//intersection points
		float dist = Mathf.Sqrt(distSq); //only calculate the actual distance now, cause only now it's actually needed
		float c1RSq = Mathf.Sq(c1.Radius);
		float a = (c1RSq - Mathf.Sq(c2.Radius) + distSq) / (2 * dist);
		float h = Mathf.Sqrt(c1RSq - Mathf.Sq(a));
		Vec2 diff = c2.Position - c1.Position;
		Vec2 diffDivDist = diff / dist;
		Vec2 test = c1.Position + a * diffDivDist;
		Vec2 hDiff = h * diffDivDist;
		Vec2 p1 = new(test.x + hDiff.y, test.y - hDiff.x);
		Vec2 p2 = new(test.x - hDiff.y, test.y + hDiff.x);
		intersectionPoints.Add(p1);
		intersectionPoints.Add(p2);
		return intersectionPoints;
	}

	/// <summary>
	/// Returns true if the given point is inside the circle.
	/// </summary>
	public static bool PointInCircle(Vec2 point, Circle circle)
	{
		return (point - circle.Position).MagSq() < Mathf.Sq(circle.Radius);
	}

	/// <summary>
	/// Checks if a line intersects with the circle. <br/>
	/// Also returns true if the line is completely enveloped by the circle.
	/// </summary>
	public static bool LineTouching(LineSegment lineSegment, Circle circle)
	{
		//https://stackoverflow.com/a/48976071/8109619
		if(PointInCircle(lineSegment.Start, circle) || PointInCircle(lineSegment.End, circle))
			return true;
		Vec2 s0s1 = lineSegment.GetDir();
		Vec2 s0qp = circle.Position - lineSegment.Start;
		float rSqr = circle.Radius * circle.Radius;

		float a = Vec2.Dot(s0s1, s0s1);
		if (a == 0) return false;
		float b = Vec2.Dot(s0s1, s0qp);
		float t = b / a; //length of projection of s0qp onto s0s1
		if (t is < 0 or > 1) return false;
		float c = Vec2.Dot(s0qp, s0qp);
		float r2 = c - a * t * t;
		return r2 <= rSqr; //true if collides
	}

	public static List<Vec2> LineIntersect(LineSegment lineSegment, Circle circle)
	{
		//this function could be (way) more optimized, but this is good enough for now
		List<Vec2> intersectionPoints = new();

		if (!LineTouching(lineSegment, circle))
			return intersectionPoints;

		double x1 = lineSegment.Start.x;
		float y1 = lineSegment.Start.y;
		double x2 = lineSegment.End.x;
		float y2 = lineSegment.End.y;
		float x0 = circle.Position.x;
		float y0 = circle.Position.y;
		float r = circle.Radius;

		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (x2 == x1)
		{
			x1 += 0.000000001; //TODO: this is a hack, but it works well enough for now
		}

		double a = (y2 - y1) / (x2 - x1);
		double b = y2 - a * x2;
		double aSq = a*a;
		double aSqP1 = aSq + 1;
		float rSq = r*r;
		double bMinY0 = b - y0;
		double xPrior = Math.Sqrt(aSqP1 * rSq + 2 * a * x0 * (y0 - b) - aSq * x0*x0 - bMinY0*bMinY0);
		if(double.IsNaN(xPrior)) //there is no intersection
			return intersectionPoints;

		double axb = a * b;
		double axy0px0 = a * y0 + x0;

		double x = (xPrior - axb + axy0px0) / aSqP1;
		double y = a * x + b;

		void Add()
		{
			if(double.IsNaN(y)) return;
			Vec2 toAdd =  new Vec2(x, y);
			if (!lineSegment.PointFallsOnLineSegment(toAdd)) return; //TODO: uncurse this
			intersectionPoints.Add(toAdd);
		}
		Add();
		if(xPrior == 0) //circle is tangent to the line
			return intersectionPoints;

		x = (-xPrior - axb + axy0px0) / aSqP1;
		y = a * x + b;
		Add();

		return intersectionPoints;
	}

	public static (bool hit, Vec2 intersectionPoint, Vec2 reflectionVector) LineReflect(LineSegment lineSegment, Circle circle)
	{
		List<Vec2> intersectionPoints = LineIntersect(lineSegment, circle);
		if (intersectionPoints.Count <= 0) return (false, new Vec2(), new Vec2());

		//Finds the point closest to the start of the line segment
		Vec2 closest = intersectionPoints[0];
		foreach (Vec2 point in intersectionPoints.Where(point => point.Dist(lineSegment.Start) < closest.Dist(lineSegment.Start)))
		{
			closest = point;
		}

		Vec2 toiVector = closest - lineSegment.Start;
		Vec2 circleToIntersection = closest - circle.Position;
		float mag = lineSegment.GetDir().Mag();
		Vec2 reflected = Vec2.Reflect(toiVector, circleToIntersection)
			.SetMag(mag * (1-toiVector.Mag()/mag));
		return (true, closest, reflected);
	}
}
