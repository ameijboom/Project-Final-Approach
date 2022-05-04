using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;
using PFA.GXPEngine.Utils;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace PFA.GXPEngine.AddOns;

/// <summary>
/// This class can be used to easily draw line based shapes (like arrows and rectangles),
/// mostly for debug purposes (it is not made for efficiency).<br/>
/// For each draw call, shapes are drawn for one frame only, after rendering all sprites.
/// </summary>
/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
public static class Gizmos
{
	private struct DrawLineCall
	{
		public readonly Vec2 Start, End;
		public readonly float Width;
		public readonly Colour Colour;

		public DrawLineCall(Vec2 start, Vec2 end, Colour colour, float width)
		{
			Start = start;
			End = end;
			Colour = colour;
			Width = width;
		}
	}

	private static Colour _defaultColour = new(255);
	private static float _defaultWidth = 3;

	private static readonly List<DrawLineCall> DrawCalls;

	static Gizmos()
	{
		Game.main.OnAfterRender += DrawLines;
		DrawCalls = new List<DrawLineCall>();
	}

	/// <summary>
	/// Set a default colour and line width for the subsequent draw calls.
	/// </summary>
	public static void SetStyle(Colour colour, float width)
	{
		_defaultColour = colour;
		_defaultWidth = width;
	}

	/// <summary>
	/// Set the default colour for subsequent draw calls.
	/// </summary>
	public static void SetColour(Colour colour)
	{
		_defaultColour = colour;
	}

	/// <summary>
	/// Set a default line width for the subsequent draw calls.
	/// </summary>
	public static void SetWidth(float width)
	{
		_defaultWidth = width;
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Line
	//------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// You can call this method from anywhere.<br/>
	/// A (debug) line will be drawn from start to end.
	/// </summary>
	/// <param name="start">Starting point of the drawn line.</param>
	/// <param name="end">Ending point of the drawn line.</param>
	/// <param name="space">The space in which to draw the line. If no value is given, the line will be drawn in screen space.</param>
	/// <param name="colour">The <see cref="Colour"/> of this particular line. If no value is given, the line will be the current default colour: <see cref="SetColour"/></param>
	/// <param name="width">The width of this particular line. If no value is given, the line will be the current default width: <see cref="SetWidth"/></param>
	public static void DrawLine(Vec2 start, Vec2 end = new(), GameObject? space = null, Colour colour = new(), float width = 0)
	{
		if (Game.main == null)
		{
			throw new Exception("Cannot draw lines before creating a game");
		}

		if (colour == new Colour())
		{
			colour = _defaultColour;
		}

		if (width == 0)
		{
			width = _defaultWidth;
		}

		if (space == null)
		{
			DrawCalls.Add(new DrawLineCall(start, end, colour, width));
		}
		else
		{
			// transform to the given parent space:
			Vec2 worldStart = space.TransformPoint(start);
			Vec2 worldEnd = space.TransformPoint(end);

			DrawCalls.Add(new DrawLineCall(worldStart, worldEnd, colour, width));
		}
	}

	/// <summary>
	/// You can call this method from anywhere.<br/>
	/// A (debug) line will be drawn from start to end.
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawLine(float x1, float y1, float x2, float y2,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawLine(new Vec2(x1, y1), new Vec2(x2, y2), space, colour, width);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Plus (+)
	//------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Draws a plus (+) shape centered at the center point, with the given radius
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawPlus(Vec2 center, float radius,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawPlus(center.x, center.y, radius, space, colour, width);
	}

	/// <summary>
	/// Draws a plus (+) shape centered at the point <c>(x,y)</c>, with the given radius
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawPlus(float x, float y, float radius,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawLine(x - radius, y, x + radius, y, space, colour, width);
		DrawLine(x, y - radius, x, y + radius, space, colour, width);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Cross (x)
	//------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Draws a cross (x) shape centered at the center point, with given radius
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawCross(Vec2 center, float radius,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawCross(center.x, center.y, radius, space, colour, width);
	}

	/// <summary>
	/// Draws a cross (x) shape centered at the point <c>(x,y)</c>, with given radius
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawCross(float x, float y, float radius,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawLine(x - radius, y - radius, x + radius, y + radius, space, colour, width);
		DrawLine(x - radius, y + radius, x + radius, y - radius, space, colour, width);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Ray
	//------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Draws a line segment from point p to p+d
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawRay(Vec2 p, Vec2 d,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawLine(p, p + d);
	}

	/// <summary>
	/// Draws a line segment from (x,y) to (x+dx, y+dy)
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawRay(float x, float y, float dx, float dy,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawRay(new Vec2(x, y), new Vec2(dx, dy), space, colour, width);
	}

	/// <summary>
	/// Draws a line segment starting at point p, with the given length and angle
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawRayAngle(Vec2 p, Angle angle, float length,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		Vec2 d = Vec2.FromAngle(angle) * length;
		DrawRay(p, d, space, colour, width);
	}

	/// <summary>
	/// Draws a line segment starting at point p, with the given length and angle
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawRayAngle(float x, float y, Angle angle, float length,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		Vec2 d = Vec2.FromAngle(angle) * length;
		DrawRay(x, y, d.x, d.y, space, colour, width);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Arrow
	//------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Draws an arrow from (x,y) to (x+dx, y+dy)<br/>
	/// The <c>relativeArrowSize</c> is the size of the arrow head compared to the arrow length.
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawArrow(float x, float y, float dx, float dy, float relativeArrowSize = 0.25f,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawLine(x, y, x + dx, y + dy, space, colour, width);
		DrawLine(x + dx, y + dy,
			x + dx * (1 - relativeArrowSize) - dy * relativeArrowSize,
			y + dy * (1 - relativeArrowSize) + dx * relativeArrowSize,
			space, colour, width);
		DrawLine(x + dx, y + dy,
			x + dx * (1 - relativeArrowSize) + dy * relativeArrowSize,
			y + dy * (1 - relativeArrowSize) - dx * relativeArrowSize,
			space, colour, width);
	}

	/// <summary>
	/// Draws an arrow from the given vector <c>p</c> with the given direction vector <c>d</c><br/>
	/// The <c>relativeArrowSize</c> is the size of the arrow head compared to the arrow length.
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawArrow(Vec2 p, Vec2 d, float relativeArrowSize = 0.25f,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		DrawRay(p, d, space, colour, width);
		DrawLine(p + d, p + d * (1 - relativeArrowSize) - d.GetNormal() * relativeArrowSize, space, colour, width);
	}

	/// <summary>
	/// Draws an arrow starting at (x,y), with the given length and angle in degrees<br/>
	/// The <c>relativeArrowSize</c> is the size of the arrow head compared to the arrow length.
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawArrowAngle(float x, float y, float angleDegrees, float length, float relativeArrowSize = 0.25f,
		GameObject? space = null, Colour colour = new(), float width = 0)
	{
		float dx = Mathf.Cos(angleDegrees * Mathf.PI / 180) * length;
		float dy = Mathf.Sin(angleDegrees * Mathf.PI / 180) * length;
		DrawArrow(x, y, dx, dy, relativeArrowSize, space, colour, width);
	}


	//------------------------------------------------------------------------------------------------------------------------
	//														Rectangle
	//------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Draws an axis-aligned rectangle centered at a given point, with a given size
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawRectangle(Vec2 center, Vec2 size,
		GameObject? space = null, Colour colour = new(), float lineWidth = 0)
	{
		DrawRectangle(center.x, center.y, size.x, size.y, space, colour, lineWidth);
	}

	/// <summary>
	/// Draws an axis-aligned rectangle centered at a given (x, y) coordinate, with given width and height, using DrawLine.
	/// </summary>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawRectangle(float xCenter, float yCenter, float width, float height,
		GameObject? space = null, Colour colour = new(), float lineWidth = 0)
	{
		DrawLine(xCenter - width / 2, yCenter - height / 2, xCenter + width / 2, yCenter - height / 2, space, colour, lineWidth);
		DrawLine(xCenter - width / 2, yCenter + height / 2, xCenter + width / 2, yCenter + height / 2, space, colour, lineWidth);
		DrawLine(xCenter - width / 2, yCenter - height / 2, xCenter - width / 2, yCenter + height / 2, space, colour, lineWidth);
		DrawLine(xCenter + width / 2, yCenter - height / 2, xCenter + width / 2, yCenter + height / 2, space, colour, lineWidth);
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Circle
	//------------------------------------------------------------------------------------------------------------------------

	// ReSharper disable InvalidXmlDocComment
	/// <summary>
	/// Draws a circle around the center coordinate, with a given radius
	/// </summary>
	/// <param name="center">Center position</param>
	/// <param name="radius">The radius for the circle</param>
	/// <param name="sides">How many sides the circle should have</param>
	/// <seealso cref="M:PFA.GXPEngine.AddOns.Gizmos.DrawLine(PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.LinAlg.Vec2,PFA.GXPEngine.GameObject,PFA.GXPEngine.Core.Colour,System.Single)"/>
	public static void DrawCircle(Vec2 center, float radius, int sides = 8,
			GameObject? space = null, Colour colour = new(), float lineWidth = 0)
	{
		float stepSize = Mathf.TWO_PI / sides;
		Vec2 p1 = new(1, 0);
		for (float i = 0; i < Mathf.TWO_PI; i += stepSize)
		{
			Vec2 p2 = Vec2.FromAngle(Angle.FromRadians(i + stepSize));
			DrawLine(center + p1 * radius, center + p2 * radius, space, colour, lineWidth);
			p1 = p2;
		}
	}

	/// <summary>
	/// Draws a circle around point <c>(x, y)</c>, with a given radius
	/// </summary>
	/// <param name="x">Center X position</param>
	/// <param name="y">Center Y position</param>
	/// <param name="radius">The radius for the circle</param>
	/// <param name="sides">How many sides the circle should have</param>
	public static void DrawCircle(float x, float y, float radius, int sides = 8,
		GameObject? space = null, Colour colour = new (), float lineWidth = 0)
	{
		DrawCircle(new Vec2(x, y), radius, sides, space, colour, lineWidth);
	}
	// ReSharper restore InvalidXmlDocComment

	//------------------------------------------------------------------------------------------------------------------------
	//														Backend
	//------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// This method should typically be called from the RenderSelf method of a GameObject,
	/// or from the game's OnAfterRender event.
	/// The line from (x1,y1) to (x2,y2) is then drawn immediately,
	/// behind objects that are drawn later.
	/// It is drawn in the space of the game object itself if called from RenderSelf with
	/// pGlobalCoords=false, and in screen space otherwise.
	/// You can give colour and line width. If no values are given (=0), the default values are
	/// used. These can be set using SetStyle, SetColour and SetWidth.
	/// </summary>
	private static void DrawLines(GLContext glContext)
	{
		if (DrawCalls.Count <= 0) return;
		foreach (DrawLineCall dc in DrawCalls)
		{
			glContext.DrawLine(dc.Start, dc.End, dc.Colour, dc.Width);
		}

		DrawCalls.Clear();
	}
}
