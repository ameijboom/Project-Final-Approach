using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GXPEngine;
using GXPEngine.Core;
using Arqan;
// ReSharper disable MemberCanBePrivate.Global

namespace GXPEngine {
	/// <summary>
	/// This class can be used to easily draw line based shapes (like arrows and rectangles),
	/// mostly for debug purposes (it is not made for efficiency). 
	/// For each draw call, shapes are drawn for one frame only, after rendering all sprites. 
	/// See the DrawLine method for more information.
	/// </summary>
	public static class Gizmos {
		private struct DrawLineCall
		{
			public Vec2 start, end;
			public byte width;
			public uint color;

			public DrawLineCall(Vec2 start, Vec2 end, uint color, byte width) {
				this.start = start;
				this.end = end;
				this.color = color;
				this.width = width;
			}
		}

		private static uint _defaultColor = 0xffffffff;
		private static byte _defaultWidth = 3;

		private static readonly List<DrawLineCall> DrawCalls;

		static Gizmos() {
			Game.main.OnAfterRender += DrawLines;
			DrawCalls = new List<DrawLineCall>();
		}

		/// <summary>
		/// Set a default color and line width for the subsequent draw calls.
		/// The color should be given as a uint consisting of four byte values, in the order ARGB.
		/// </summary>
		public static void SetStyle(uint color, byte width) {
			_defaultColor = color;
			_defaultWidth = width;
		}

		/// <summary>
		/// Set a default line width for the subsequent draw calls.
		/// </summary>
		/// <param name="width"></param>
		public static void SetWidth(byte width) {
			_defaultWidth = width;
		}

		/// <summary>
		/// Set the default color for subsequent draw calls.
		/// The R,G,B color and alpha values should be given as floats between 0 and 1.
		/// </summary>
		public static void SetColor(float R, float G, float B, float alpha = 1) {
			_defaultColor = (ToByte(alpha) << 24) + (ToByte(R) << 16) + (ToByte(G) << 8) + (ToByte(B));
		}

		private static uint ToByte(float value) {
			return (uint)(Mathf.Clamp(value, 0, 1) * 255);
		}


		//------------------------------------------------------------------------------------------------------------------------
		//														Line
		//------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// You can call this method from anywhere. A (debug) line will be drawn from start to end,
		/// in the space of the given game object.
		/// If no game object is given, it will be drawn in screen space.
		/// The line will be drawn after drawing all other game objects.
		/// You can give color and line width. If no values are given (=0), the default values are
		/// used. These can be set using SetStyle, SetColor and SetWidth.
		/// </summary>
		public static void DrawLine(Vec2 start, Vec2 end = new(), GameObject space = null, uint color = 0, byte width = 0) {
			if (Game.main == null) {
				throw new Exception("Cannot draw lines before creating a game");
			}
			if (color == 0) {
				color = _defaultColor;
			}
			if (width == 0) {
				width = _defaultWidth;
			}

			if (space == null) {
				DrawCalls.Add(new DrawLineCall(start, end, color, width));
			} else {
				// transform to the given parent space:
				Vec2 worldStart = space.TransformPoint(start);
				Vec2 worldEnd = space.TransformPoint(end);

				DrawCalls.Add(new DrawLineCall(worldStart, worldEnd, color, width));
			}
		}

		public static void DrawLine(float x1, float y1, float x2 = 0f, float y2 = 0f, GameObject space = null, uint color = 0, byte width = 0) {
			DrawLine(new Vec2(x1, y1), new Vec2(x2, y2), space, color, width);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Plus (+)
		//------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Draws a plus shape centered at the center point, with given radius, using DrawLine.
		/// </summary>
		public static void DrawPlus(Vec2 center, float radius, GameObject space = null, uint color = 0, byte width = 0)
		{
			DrawPlus(center.x, center.y, radius, space, color, width);
		}

		/// <summary>
		/// Draws a plus shape centered at the point x,y, with given radius, using DrawLine.
		/// </summary>
		public static void DrawPlus(float x, float y, float radius, GameObject space = null, uint color = 0, byte width = 0)
		{
			DrawLine(x - radius, y, x + radius, y, space, color, width);
			DrawLine(x, y - radius, x, y + radius, space, color, width);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Cross (x)
		//------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Draws a cross shape centered at the center point, with given radius, using DrawLine.
		/// </summary>
		public static void DrawCross(Vec2 center, float radius, GameObject space = null, uint color = 0, byte width = 0)
		{
			DrawCross(center.x, center.y, radius, space, color, width);
		}

		/// <summary>
		/// Draws a cross shape centered at the point x,y, with given radius, using DrawLine.
		/// </summary>
		public static void DrawCross(float x, float y, float radius, GameObject space = null, uint color = 0, byte width = 0) {
			DrawLine(x - radius, y - radius, x + radius, y + radius, space, color, width);
			DrawLine(x - radius, y + radius, x + radius, y - radius, space, color, width);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Ray
		//------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Draws a line segment from point p to p+d, using DrawLine.
		/// </summary>
		public static void DrawRay(Vec2 p, Vec2 d, GameObject space = null, uint color = 0, byte width = 0)
		{
			DrawLine(p, p + d);
		}

		/// <summary>
		/// Draws a line segment from (x,y) to (x+dx, y+dy), using DrawLine.
		/// </summary>
		public static void DrawRay(float x, float y, float dx, float dy, GameObject space = null, uint color = 0, byte width = 0)
		{
			DrawRay(new Vec2(x, y), new Vec2(dx, dy), space, color, width);
		}

		/// <summary>
		/// Draws a line segment starting at point p, with the given length and angle using DrawLine.
		/// </summary>
		public static void DrawRayAngle(Vec2 p, Angle angle, float length, GameObject space = null, uint color = 0, byte width = 0) {
			Vec2 d = Vec2.FromAngle(angle) * length;
			DrawRay(p, d, space, color, width);
		}

		/// <summary>
		/// Draws a line segment starting at point p, with the given length and angle using DrawLine.
		/// </summary>
		public static void DrawRayAngle(float x, float y, Angle angle, float length, GameObject space = null, uint color = 0, byte width = 0)
		{
			Vec2 d = Vec2.FromAngle(angle) * length;
			DrawRay(x, y, d.x, d.y, space, color, width);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Arrow
		//------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Draws an arrow from (x,y) to (x+dx, y+dy), using DrawLine.
		/// The relativeArrowSize is the size of the arrow head compared to the arrow length.
		/// </summary>
		public static void DrawArrow(float x, float y, float dx, float dy, float relativeArrowSize = 0.25f, GameObject space = null, uint color = 0, byte width = 0) {
			DrawLine(x, y, x + dx, y + dy, space, color, width);
			DrawLine(x + dx, y + dy,
				x + dx * (1 - relativeArrowSize) - dy * relativeArrowSize,
				y + dy * (1 - relativeArrowSize) + dx * relativeArrowSize,
				space, color, width);
			DrawLine(x + dx, y + dy,
				x + dx * (1 - relativeArrowSize) + dy * relativeArrowSize,
				y + dy * (1 - relativeArrowSize) - dx * relativeArrowSize,
				space, color, width);
		}

		public static void DrawArrow(Vec2 p, Vec2 d, float relativeArrowSize = 0.25f, GameObject space = null, uint color = 0, byte width = 0) {
			DrawRay(p, d, space, color, width);
			DrawLine(p + d, p + d * (1 - relativeArrowSize) - d.GetNormal() * relativeArrowSize, space, color, width);
		}

		/// <summary>
		/// Draws an arrow starting at (x,y), with the given length and angle in degrees,
		/// using DrawLine.
		/// The relativeArrowSize is the size of the arrow head compared to the arrow length.
		/// </summary>
		public static void DrawArrowAngle(float x, float y, float angleDegrees, float length, float relativeArrowSize = 0.25f, GameObject space = null, uint color = 0, byte width = 0) {
			float dx = Mathf.Cos(angleDegrees * Mathf.PI / 180) * length;
			float dy = Mathf.Sin(angleDegrees * Mathf.PI / 180) * length;
			DrawArrow(x, y, dx, dy, relativeArrowSize, space, color, width);
		}


		//------------------------------------------------------------------------------------------------------------------------
		//														Rectangle
		//------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Draws an axis-aligned rectangle centered at a given point, with a given size, using DrawLine.
		/// </summary>
		public static void DrawRectangle(Vec2 center, Vec2 size, GameObject space = null, uint color = 0, byte lineWidth = 0)
		{
			DrawRectangle(center.x, center.y, size.x, size.y, space, color, lineWidth);
		}

		/// <summary>
		/// Draws an axis-aligned rectangle centered at a given (x, y) coordinate, with given width and height, using DrawLine.
		/// </summary>
		public static void DrawRectangle(float xCenter, float yCenter, float width, float height, GameObject space = null, uint color = 0, byte lineWidth = 0) {
			DrawLine(xCenter - width / 2, yCenter - height / 2, xCenter + width / 2, yCenter - height / 2, space, color, lineWidth);
			DrawLine(xCenter - width / 2, yCenter + height / 2, xCenter + width / 2, yCenter + height / 2, space, color, lineWidth);
			DrawLine(xCenter - width / 2, yCenter - height / 2, xCenter - width / 2, yCenter + height / 2, space, color, lineWidth);
			DrawLine(xCenter + width / 2, yCenter - height / 2, xCenter + width / 2, yCenter + height / 2, space, color, lineWidth);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Circle
		//------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Draws a circle around the center coordinate, with a given radius, using DrawLine.
		/// </summary>
		/// <param name="center">Center position</param>
		/// <param name="radius">The radius for the circle</param>
		/// <param name="sides">How many sides the circle should have</param>
		public static void DrawCircle(Vec2 center, float radius, int sides = 8)
		{
			float stepSize = Angle.TWO_PI / sides;
			Vec2 p1 = new(1, 0);
			for (float i = 0; i < Angle.TWO_PI; i += stepSize)
			{
				Vec2 p2 = Vec2.FromAngle(Angle.FromRadians(i + stepSize));
				DrawLine(center + p1 * radius, center + p2 * radius);
				p1 = p2;
			}
		}

		/// <summary>
		/// Draws a circle around point (x, y), with a given radius, using DrawLine.
		/// </summary>
		/// <param name="x">Center X position</param>
		/// <param name="y">Center Y position</param>
		/// <param name="radius">The radius for the circle</param>
		/// <param name="sides">How many sides the circle should have</param>
		public static void DrawCircle(float x, float y, float radius, int sides = 8)
		{
			DrawCircle(new Vec2(x, y), radius, sides);
		}

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
		/// You can give color and line width. If no values are given (=0), the default values are
		/// used. These can be set using SetStyle, SetColor and SetWidth.
		/// </summary>
		public static void RenderLine(float x1, float y1, float x2, float y2, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false) {
			if (pGlobalCoords) GL.glLoadIdentity();
			GL.glDisable(GL.GL_TEXTURE_2D);
			GL.glLineWidth(pLineWidth);
			GL.glColor4ub((byte)((pColor >> 16) & 0xff), (byte)((pColor >> 8) & 0xff), (byte)((pColor) & 0xff), (byte)((pColor >> 24) & 0xff));
			float[] vertices = { x1, y1, x2, y2 };
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			GL.glVertexPointer(2, GL.GL_FLOAT, 0, vertices.ToIntPtr());
			GL.glDrawArrays(GL.GL_LINES, 0, 2);
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glEnable(GL.GL_TEXTURE_2D);
		}

		private static void DrawLines(GLContext glContext)
		{
			if (DrawCalls.Count <= 0) return;
			foreach (DrawLineCall dc in DrawCalls) {
				RenderLine(dc.start.x, dc.start.y, dc.end.x, dc.end.y, dc.color, dc.width, true);
			}
			DrawCalls.Clear();
		}
	}
}
