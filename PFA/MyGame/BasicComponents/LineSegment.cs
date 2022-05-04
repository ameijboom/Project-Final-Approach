// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame.BasicComponents;

public class LineSegment
{
	public Vec2 Start;
	public Vec2 End;

	public LineSegment(Vec2 start, Vec2 end)
	{
		Start = start;
		End = end;
	}

	public Vec2 GetDir()
	{
		return End - Start;
	}

	public void Move(Vec2 delta)
	{
		Start += delta;
		End += delta;
	}

	public void Draw()
	{
		Gizmos.DrawLine(Start.x, Start.y, End.x, End.y);
	}

	public bool PointFallsOnLineSegment(Vec2 point)
	{
		Vec2 dir = GetDir();
		Vec2 toPoint = point - Start;
		float dot = dir.Dot(toPoint);

		return dot >= 0 && dot <= dir.MagSq();
	}

	public float DistanceToPoint(Vec2 point)
	{
		//TODO: Refactor and understand this
		float x = point.x, y = point.y;
		float x1 = Start.x, y1 = Start.y;
		float x2 = End.x, y2 = End.y;

		float A = x - x1;
		float B = y - y1;
		float C = x2 - x1;
		float D = y2 - y1;

		float dot = A * C + B * D;
		float len_sq = C * C + D * D;
		float param = -1;
		if (len_sq != 0) //in case of 0 length line
			param = dot / len_sq;

		float xx, yy;

		if (param < 0)
		{
			xx = x1;
			yy = y1;
		}
		else if (param > 1)
		{
			xx = x2;
			yy = y2;
		}
		else
		{
			xx = x1 + param * C;
			yy = y1 + param * D;
		}

		float dx = x - xx;
		float dy = y - yy;
		return Mathf.Sqrt(dx * dx + dy * dy);
	}

	public static (bool hit, Vec2 intersectionPoint, float toi) Intersect(LineSegment a, LineSegment b)
	{
		Vec2 av = a.GetDir();
		Vec2 bv = b.GetDir();
		float d = Vec2.Cross(av, bv);
		if (d == 0) return (false, new Vec2(), 0);

		Vec2 dir = a.Start - b.Start;
		float ua = Vec2.Cross(bv, dir) / d;
		float ub = Vec2.Cross(av, dir) / d;

		if (ua is < 0 or > 1 || ub is < 0 or > 1) return (false, new Vec2(), 0);

		return (true, a.Start + av * ua, ua);
	}

	public static (bool hit, Vec2 intersectionPoint, Vec2 reflectionVector) Reflect(LineSegment dynamic, LineSegment @static)
	{
		(bool hit, Vec2 intersectionPoint, float toi) = Intersect(dynamic, @static);
		// Console.WriteLine(toi);
		if(!hit) return (false, new Vec2(), new Vec2());
		Vec2 toiVector = intersectionPoint - dynamic.Start;
		Vec2 reflected = Vec2.Reflect(toiVector, @static.GetDir().GetNormal())
			.SetMag(dynamic.GetDir().Mag() * (1f - toi));
		return (true, intersectionPoint, reflected);
	}
}
