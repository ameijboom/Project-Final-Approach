using SkiaSharp;

namespace GXPEngine 
{
	public enum CenterMode {Min, Center, Max}

	/// <summary>
	/// Creates an easy-to-use layer on top of .NET's System.Drawing methods.
	/// The API is inspired by Processing: internal states are maintained for font, fill/stroke color, etc., 
	/// and everything works with simple methods that have many overloads.
	/// </summary>
	public class EasyDraw : Canvas 
	{
		static SKFont defaultFont = new SKFont (SKTypeface.FromFamilyName("Noto Sans"), 15);

		public CenterMode HorizontalTextAlign=CenterMode.Min;
		public CenterMode VerticalTextAlign=CenterMode.Max;
		public CenterMode HorizontalShapeAlign=CenterMode.Center;
		public CenterMode VerticalShapeAlign=CenterMode.Center;
		public SKFont font		{ get; protected set;}
		public SKPaint pen			{ get; protected set;}
		public SKPaint brush	{ get; protected set;}
		protected bool _stroke=true;
		protected bool _fill=true;

		/// <summary>
		/// Creates a new EasyDraw canvas with the given width and height in pixels.
		/// </summary>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		/// <param name="addCollider">whether the canvas should have a collider</param>
		public EasyDraw (int width, int height, bool addCollider=true) : base (new SKBitmap (width, height),addCollider)
		{
			Initialize ();
		}

		/// <summary>
		/// Creates a new EasyDraw canvas from a given bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap (image) that should be on the canvas</param>
		/// <param name="addCollider">whether the canvas should have a collider</param>
		public EasyDraw (SKBitmap bitmap, bool addCollider=true) : base (bitmap,addCollider)
		{
			Initialize ();
		}

		/// <summary>
		/// Creates a new EasyDraw canvas from a file that contains a sprite (png, jpg).
		/// </summary>
		/// <param name="filename">the name of the file that contains a sprite (png, jpg)</param>
		/// <param name="addCollider">whether the canvas should have a collider</param>
		public EasyDraw (string filename, bool addCollider=true) : base(filename,addCollider)
		{
			Initialize ();
		}

		void Initialize() 
		{
			// pen = new Pen (Color.White, 1);
			pen = new SKPaint {
				IsAntialias = !game.PixelArt,
				Color = SKColors.White,
				Style = SKPaintStyle.Stroke,
				HintingLevel = SKPaintHinting.Full
			};

			brush = new SKPaint {
				IsAntialias = !game.PixelArt,
				Color = SKColors.White,
				Style = SKPaintStyle.Fill,
				HintingLevel = SKPaintHinting.Full
			};

			font = defaultFont;
		}

		//////////// Setting Font

		/// <summary>
		/// Change the font that is used when rendering text (using the Text method).
		/// </summary>
		/// <param name="newFont">The new font (see also Utils.LoadFont)</param>
		public void TextFont(SKFont newFont) 
		{
			font = newFont;
		}

		/// <summary>
		/// Change the font that is used when rendering text (using the Text method), using one of the available system fonts
		/// </summary>
		/// <param name="fontName">The name of the system font (e.g. "Arial", "Verdana", "Vivaldi")</param>
		/// <param name="pointSize">font size in points</param>
		/// <param name="style">font style (e.g. FontStyle.Italic|FontStyle.Bold )</param>
		public void TextFont(string fontName, float pointSize, SKFontStyle style = null) 
		{
			font = new SKFont (SKTypeface.FromFamilyName(fontName, style == null ? SKFontStyle.Normal : style), pointSize);
		}

		/// <summary>
		/// Change the size of the current font.
		/// </summary>
		/// <param name="pointSize">The font size in points</param>
		public void TextSize(float pointSize) 
		{
			font = new SKFont (font.Typeface, pointSize);
		}

		//////////// Setting Alignment for text, ellipses and rects
		
		/// <summary>
		/// Sets the horizontal and vertical alignment of text. 
		/// For instance, when choosing CenterMode.Min for both and calling Text, the x and y coordinates give the top left corner of the
		/// rendered text. 
		/// </summary>
		/// <param name="horizontal">Horizontal alignment</param>
		/// <param name="vertical">Vertical alignment</param>
		public void TextAlign(CenterMode horizontal, CenterMode vertical) 
		{
			HorizontalTextAlign = horizontal;
			VerticalTextAlign = vertical;
		}

		/// <summary>
		/// Sets the horizontal and vertical alignment of shapes.
		/// For instance, when choosing CenterMode.Min for both and calling Ellipse, 
		/// the x and y coordinates give the top left corner of the drawn ellipse.
		/// </summary>
		/// <param name="horizontal">Horizontal alignment</param>
		/// <param name="vertical">Vertical alignment</param>
		public void ShapeAlign(CenterMode horizontal, CenterMode vertical) 
		{
			HorizontalShapeAlign = horizontal;
			VerticalShapeAlign = vertical;
		}

		//////////// Setting Stroke

		/// <summary>
		/// Draw shapes without outline
		/// </summary>
		public void NoStroke() 
		{
			_stroke=false;
		}

		/// <summary>
		/// Set the outline color for drawing shapes
		/// </summary>
		/// <param name="newColor">the color of the outline</param>
		/// <param name="alpha">the opacity of the outline (from 0=transparent to 255=opaque)</param>
		public void Stroke(SKColor newColor, byte alpha=255) 
		{
			pen.Color = new SKColor (newColor.Red, newColor.Green, newColor.Blue, alpha);
			_stroke = true;
		}

		/// <summary>
		/// Set the outline color for drawing shapes to a grayscale value
		/// </summary>
		/// <param name="grayScale">A grayscale value (from 0=black to 255=white)</param>
		/// <param name="alpha">the opacity of the outline (from 0=transparent to 255=opaque)</param>
		public void Stroke(byte grayScale, byte alpha=255) 
		{
			pen.Color = new SKColor(grayScale, grayScale, grayScale, alpha);
			_stroke = true;
		}

		/// <summary>
		/// Set the outline color for drawing shapes.
		/// </summary>
		/// <param name="red">The red value of the color (from 0 to 255)</param>
		/// <param name="green">The green value of the color (from 0 to 255)</param>
		/// <param name="blue">The blue value of the color (from 0 to 255)</param>
		/// <param name="alpha">The opacity of the outline (from 0=transparent to 255=opaque)</param>
		public void Stroke(byte red, byte green, byte blue, byte alpha=255) 
		{
			pen.Color = new SKColor(red, green, blue, alpha);
			_stroke = true;
		}

		/// <summary>
		/// Sets the width of the outline for drawing shapes. (Default value: 1)
		/// </summary>
		/// <param name="width">The width (in pixels)</param>
		public void StrokeWeight(float width) 
		{
			pen.StrokeWidth = width;
			_stroke = true;
		}

		//////////// Setting Fill

		/// <summary>
		/// Draw shapes without fill color.
		/// </summary>
		public void NoFill() 
		{
			_fill = false;
		}

		/// <summary>
		/// Set the fill color for drawing shapes and text.
		/// </summary>
		/// <param name="newColor">the fill color</param>
		/// <param name="alpha">the fill opacity (from 0=transparent to 255=opaque)</param>
		public void Fill(SKColor newColor, byte alpha=255) 
		{
			brush.Color = new SKColor(newColor.Red, newColor.Green, newColor.Blue, alpha);
			_fill = true;
		}

		/// <summary>
		/// Set the fill color for drawing shapes and text to a gray scale value.
		/// </summary>
		/// <param name="grayScale">gray scale value (from 0=black to 255=white)</param>
		/// <param name="alpha">the fill opacity (from 0=transparent to 255=opaque)</param>
		public void Fill(byte grayScale, byte alpha=255) 
		{
			brush.Color = new SKColor(grayScale, grayScale, grayScale, alpha);
			_fill = true;
		}

		/// <summary>
		/// Set the fill color for drawing shapes and text.
		/// </summary>
		/// <param name="red">The red value of the color (from 0 to 255)</param>
		/// <param name="green">The green value of the color (from 0 to 255)</param>
		/// <param name="blue">The blue value of the color (from 0 to 255)</param>
		/// <param name="alpha">The fill opacity (from 0=transparent to 255=opaque)</param>
		public void Fill(byte red, byte green, byte blue, byte alpha=255) 
		{
			brush.Color = new SKColor(red, green, blue, alpha);
			_fill = true;
		}

		//////////// Clear

		/// <summary>
		/// Clear the canvas with a given color
		/// </summary>
		/// <param name="newColor">the clear color</param>
		public void Clear(SKColor newColor) 
		{
			graphics.Clear (newColor);
		}

		/// <summary>
		/// Clear the canvas with a grayscale
		/// </summary>
		/// <param name="grayScale">the grayscale value (between 0=black and 255=white)</param>
		public void Clear(byte grayScale) 
		{
			graphics.Clear(new SKColor(grayScale, grayScale, grayScale, 255));
		}

		/// <summary>
		/// Clear the canvas with a given color.
		/// </summary>
		/// <param name="red">The red value of the clear color (from 0 to 255)</param>
		/// <param name="green">The green value of the clear color (from 0 to 255)</param>
		/// <param name="blue">The blue value of the clear color (from 0 to 255)</param>
		/// <param name="alpha">The opacity of the clear color (from 0=transparent to 255=opaque)</param>
		public void Clear(byte red, byte green, byte blue, byte alpha=255) 
		{
			graphics.Clear(new SKColor(red, green, blue, alpha));
		}

		/// <summary>
		/// Clear the canvas with a transparent color. 
		/// Note that this will fully clear the canvas, but will make the sprites behind the canvas visible.
		/// </summary>
		public void ClearTransparent() {
			graphics.Clear(SKColor.Empty); // same as Clear(0,0,0,0);
		}

		//////////// Draw & measure Text

		/// <summary>
		/// Draw text on the canvas, using the currently selected font, at position x,y.
		/// This uses the current TextAlign values (e.g. if both are CenterMode.Center, (x,y) will be at the center of the rendered text).
		/// </summary>
		/// <param name="text">The text to be rendered</param>
		/// <param name="x">The x coordinate to draw the text, using canvas (pixel) coordinates</param>
		/// <param name="y">The y coordinate to draw the text, using canvas (pixel) coordinates</param>
		public void Text(string text, float x, float y) 
		{
			float twidth,theight;
			TextDimensions (text, out twidth, out theight);
			if (HorizontalTextAlign == CenterMode.Max) 
			{
				x -= twidth;
			} else if (HorizontalTextAlign == CenterMode.Center) 
			{ 
				x -= twidth / 2;
			}
			if (VerticalTextAlign == CenterMode.Max) 
			{
				y -= theight;
			} else if (VerticalTextAlign == CenterMode.Center) 
			{
				y -= theight / 2;
			}
			float offset = 0;
			foreach (string line in text.Split("\n")) 
			{
				graphics.DrawText(line, x, y + offset, font, brush);
				offset += (TextHeight(line) + 1.0f);
			}
			
		}

		/// <summary>
		/// Draw text on the canvas, using the currently selected font.
		/// The text is aligned on the canvas using the current TextAlign values.
		/// </summary>
		/// <param name="text">The text to be rendered</param>
		/// <param name="clear">Whether the canvas should be cleared before drawing the text</param>
		/// <param name="clearAlpha">The opacity of the clear color (from 0=transparent to 255=opaque)</param>
		/// <param name="clearRed">The red value of the clear color (0-255)</param>
		/// <param name="clearGreen">The green value of the clear color (0-255)</param>
		/// <param name="clearBlue">The blue value of the clear color (0-255)</param>
		public void Text(string text, bool clear=false, byte clearAlpha=0, byte clearRed=0, byte clearGreen=0, byte clearBlue=0) {
			if (clear) Clear(clearRed, clearGreen, clearBlue, clearAlpha);
			float tx = 0;
			float ty = 0;
			switch (HorizontalTextAlign) {
				case CenterMode.Center:
					tx = _texture.width/2;
					break;
				case CenterMode.Max:
					tx = _texture.width;
					break;
			}
			switch (VerticalTextAlign) {
				case CenterMode.Center:
					ty = _texture.height / 2;
					break;
				case CenterMode.Max:
					ty = _texture.height;
					break;
			}
			Text(text, tx, ty);
		}

		/// <summary>
		/// Returns the width in pixels of a string, when rendered with the current font.
		/// </summary>
		/// <param name="text">input string</param>
		/// <returns>width in pixels</returns>
		public float TextWidth(string text) 
		{
			return pen.MeasureText(text);
		}

		/// <summary>
		/// Returns the height in pixels of a string, when rendered with the current font.
		/// </summary>
		/// <param name="text">input string</param>
		/// <returns>height in pixels</returns>
		public float TextHeight(string text) 
		{
			SKRect textBounds = new SKRect();
			pen.MeasureText(text, ref textBounds);

			return textBounds.Size.Height; 
		}

		/// <summary>
		/// Returns the width and height in pixels of a string, when rendered with the current font.
		/// </summary>
		/// <param name="text">input string</param>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		public void TextDimensions(string text, out float width, out float height) 
		{
 			SKRect textBounds = new SKRect();
			pen.MeasureText(text, ref textBounds);

			width = textBounds.Size.Width;
			height = textBounds.Size.Height;
		}

		//////////// Draw Shapes
		 
		/// <summary>
		/// Draw an (axis aligned) rectangle with given width and height, using the current stroke and fill settings. 
		/// Uses the current ShapeAlign values to position the rectangle relative to the point (x,y)
		/// </summary>
		/// <param name="x">x position in canvas coordinates</param>
		/// <param name="y">y position in canvas coordinates</param>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		public void Rect(float x, float y, float width, float height) {
			// ShapeAlign (ref x, ref y, width, height);
			if (_fill) {
				graphics.DrawRect (x, y, width, height, brush);
			}
			if (_stroke) {
				graphics.DrawRect (x, y, width, height, pen);
			}
		}

		/// <summary>
		/// Draw an (axis aligned) ellipse (or circle) with given width and height, using the current stroke and fill settings. 
		/// Uses the current ShapeAlign values to position the rectangle relative to the point (x,y)
		/// </summary>
		/// <param name="x">x position in canvas coordinates</param>
		/// <param name="y">y position in canvas coordinates</param>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		public void Ellipse(float x, float y, float width, float height) {
			// ShapeAlign (ref x, ref y, width, height);
			if (_fill) {
				graphics.DrawOval (x, y, width, height, brush);
			}
			if (_stroke) {
				graphics.DrawOval (x, y, width, height, pen);
			}
		}

		/// <summary>
		/// Draws an arc (=segment of an ellipse), where width and height give the ellipse size, and 
		/// start angle and sweep angle can be given (in degrees, clockwise). Uses the current stroke and fill settings.
		/// Uses the current ShapeAlign values to position the ellipse relative to the point (x,y)
		/// </summary>
		/// <param name="x">x position in canvas coordinates</param>
		/// <param name="y">y position in canvas coordinates</param>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		/// <param name="startAngleDegrees">angle in degrees (clockwise) to start drawing</param>
		/// <param name="sweepAngleDegrees">sweep angle in degrees, clockwise. Use e.g. 180 for a half-circle</param>
		public void Arc(float x, float y, float width, float height, float startAngleDegrees, float sweepAngleDegrees) {
			// ShapeAlign (ref x, ref y, width, height);
			if (_fill) {
				graphics.DrawArc (new SKRect(x, y, x+width, y+height), startAngleDegrees, sweepAngleDegrees, true, brush);
			}
			if (_stroke) {
				graphics.DrawArc (new SKRect(x, y, x+width, y+height), startAngleDegrees, sweepAngleDegrees, true, pen);

			}
		}

		/// <summary>
		/// Draw a line segment between two points, using the current stroke settings.
		/// </summary>
		/// <param name="x1">x coordinate of the start point</param>
		/// <param name="y1">y coordinate of the end point</param>
		/// <param name="x2">x coordinate of the start point</param>
		/// <param name="y2">y coordinate of the end point</param>
		public void Line(float x1, float y1, float x2, float y2) {
			if (_stroke) {
				graphics.DrawLine (x1, y1, x2, y2, pen);
			}
		}

		/// <summary>
		/// Draw a triangle between three points, using the current stroke and fill settings.
		/// </summary>
		public void Triangle(float x1, float y1, float x2, float y2, float x3, float y3) {
			Polygon(x1,y1,x2,y2,x3,y3);
		}

		/// <summary>
		/// Draw a quad (="deformed rectangle") between four points, using the current stroke and fill settings.
		/// </summary>
		public void Quad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) {
			Polygon(x1,y1,x2,y2,x3,y3,x4,y4);
		}

		/// <summary>
		/// Draw a polygon shape between any number of points, using the current stroke and fill settings.
		/// This requires passing in an even number of float coordinates,
		/// where the odd parameters are x coordinates and even parameters are y coordinates.
		/// </summary>
		public void Polygon(params float[] pt) {
			SKPoint[] pts = new SKPoint[pt.Length / 2];
			for (int i = 0; i < pts.Length; i++) {
				pts [i] = new SKPoint (pt [2 * i], pt [2 * i + 1]);
			}
			Polygon (pts);
		}

		//TODO: Make Vec2 array the main one

		/// <summary>
		/// Draw a polygon shape between any number of points, using the current stroke and fill settings.
		/// </summary>
		public void Polygon(SKPoint[] pts) {
			if (_fill) {
				graphics.DrawPoints (SKPointMode.Polygon, pts, brush);
			}
			if (_stroke) {
				graphics.DrawPoints (SKPointMode.Polygon, pts, pen);
			}
		}

		protected void ShapeAlign(ref float x, ref float y, float width, float height) {
			if (HorizontalShapeAlign == CenterMode.Max) 
			{
				x -= width;
			} else if (HorizontalShapeAlign == CenterMode.Center) 
			{ 
				x -= width / 2;
			}
			if (VerticalShapeAlign == CenterMode.Max) 
			{
				y -= height;
			} else if (VerticalShapeAlign == CenterMode.Center) 
			{
				y -= height / 2;
			}
		}
	}
}
