// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
// You're allowed to learn from this, but please do not simply copy.

using SkiaSharp;

namespace PFA.GXPEngine.Core;

/// <summary>
/// A simple struct to hold a colour.
/// </summary>
public struct Colour
{
	private SKColor _colour;

	/// <summary>
	/// Get and set the red component of the colour. <c>(0 - 255)</c>
	/// </summary>
	public byte r
	{
		get => _colour.Red;
		set => _colour = new SKColor(value, _colour.Green, _colour.Blue, _colour.Alpha);
	}

	/// <summary>
	/// Get and set the green component of the colour. <c>(0 - 255)</c>
	/// </summary>
	public byte g
	{
		get => _colour.Green;
		set => _colour = new SKColor(_colour.Red, value, _colour.Blue, _colour.Alpha);
	}

	/// <summary>
	/// Get and set the blue component of the colour. <c>(0 - 255)</c>
	/// </summary>
	public byte b
	{
		get => _colour.Blue;
		set => _colour = new SKColor(_colour.Red, _colour.Green, value, _colour.Alpha);
	}

	/// <summary>
	/// Get and set the alpha component of the colour. <c>(0 - 255)</c>
	/// </summary>
	public byte a
	{
		get => _colour.Alpha;
		set => _colour = new SKColor(_colour.Red, _colour.Green, _colour.Blue, value);
	}

	/// <summary>
	/// Get and set the red component of the colour. <c>(0.0f - 1.0f)</c>
	/// </summary>
	public float rf
	{
		get => _colour.Red / 255f;
		set => _colour = new SKColor((byte)(value * 255), _colour.Green, _colour.Blue, _colour.Alpha);
	}

	/// <summary>
	/// Get and set the green component of the colour. <c>(0.0f - 1.0f)</c>
	/// </summary>
	public float gf
	{
		get => _colour.Green / 255f;
		set => _colour = new SKColor(_colour.Red, (byte)(value * 255), _colour.Blue, _colour.Alpha);
	}

	/// <summary>
	/// Get and set the blue component of the colour. <c>(0.0f - 1.0f)</c>
	/// </summary>
	public float bf
	{
		get => _colour.Blue / 255f;
		set => _colour = new SKColor(_colour.Red, _colour.Green, (byte)(value * 255), _colour.Alpha);
	}

	/// <summary>
	/// Get and set the alpha component of the colour. <c>(0.0f - 1.0f)</c>
	/// </summary>
	public float af
	{
		get => _colour.Alpha / 255f;
		set => _colour = new SKColor(_colour.Red, _colour.Green, _colour.Blue, (byte)(value * 255));
	}

	/// <summary>
	/// Make a new RGB colour
	/// </summary>
	/// <param name="r"><c>(0 - 255)</c></param>
	/// <param name="g"><c>(0 - 255)</c></param>
	/// <param name="b"><c>(0 - 255)</c></param>
	/// <param name="a"><c>(0 - 255)</c> (OPTIONAL)</param>
	public Colour(byte r, byte g, byte b, byte a = 255)
	{
		_colour = new SKColor(r, g, b, a);
	}

	/// <summary>
	/// Make a new gray colour
	/// </summary>
	/// <param name="gray"><c>(0 - 255)</c></param>
	/// <param name="a"><c>(0 - 255)</c> (OPTIONAL)</param>
	public Colour(byte gray, byte a = 255)
	{
		_colour = new SKColor(gray, gray, gray, a);
	}

	/// <summary>
	/// Make a new RGB colour
	/// </summary>
	/// <param name="r"><c>(0.0f - 1.0f)</c></param>
	/// <param name="g"><c>(0.0f - 1.0f)</c></param>
	/// <param name="b"><c>(0.0f - 1.0f)</c></param>
	/// <param name="a"><c>(0.0f - 1.0f)</c></param>
	public static Colour FromFloats(float r, float g, float b, float a = 1.0f)
	{
		return new Colour((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
	}

	public static bool operator ==(Colour colourL, Colour colourR)
	{
		return colourL._colour == colourR._colour;
	}

	public static bool operator !=(Colour colourL, Colour colourR)
	{
		return colourL._colour != colourR._colour;
	}

	public override bool Equals(object? obj)
	{
		return _colour.Equals(obj);
	}

	public override int GetHashCode()
	{
		return _colour.GetHashCode();
	}

	public override string ToString()
	{
		return $"{_colour.Red}, {_colour.Green}, {_colour.Blue}, {_colour.Alpha}";
	}

	public static explicit operator SKColor(Colour colour)
	{
		return colour._colour;
	}

	public static readonly Colour White = new(255, 255, 255);
	public static readonly Colour Black = new(0, 0, 0);
	public static readonly Colour Red = new(255, 0, 0);
	public static readonly Colour Green = new(0, 255, 0);
	public static readonly Colour Blue = new(0, 0, 255);
	public static readonly Colour Yellow = new(255, 255, 0);
	public static readonly Colour Cyan = new(0, 255, 255);
	public static readonly Colour Magenta = new(255, 0, 255);
	public static readonly Colour Transparent = new(0, 0, 0, 0);
	public static readonly Colour Aqua = new(0, 255, 255);
	public static readonly Colour Silver = new(192, 192, 192);
	public static readonly Colour Gray = new(128, 128, 128);
	public static readonly Colour Maroon = new(128, 0, 0);
	public static readonly Colour Olive = new(128, 128, 0);
	public static readonly Colour Purple = new(128, 0, 128);
	public static readonly Colour Teal = new(0, 128, 128);
	public static readonly Colour Navy = new(0, 0, 128);
	public static readonly Colour Fuchsia = new(255, 0, 255);
	public static readonly Colour Lime = new(0, 255, 0);
	public static readonly Colour OliveDrab = new(107, 142, 35);
	public static readonly Colour DarkOliveGreen = new(85, 107, 47);
	public static readonly Colour DarkGreen = new(0, 100, 0);
	public static readonly Colour DarkBlue = new(0, 0, 139);
	public static readonly Colour DarkCyan = new(0, 139, 139);
	public static readonly Colour DarkRed = new(139, 0, 0);
	public static readonly Colour DarkMagenta = new(139, 0, 139);
	public static readonly Colour DarkViolet = new(148, 0, 211);
	public static readonly Colour LightGreen = new(144, 238, 144);
	public static readonly Colour LightBlue = new(173, 216, 230);
	public static readonly Colour LightCyan = new(224, 255, 255);
	public static readonly Colour LightRed = new(255, 182, 193);
	public static readonly Colour LightMagenta = new(255, 105, 180);
	public static readonly Colour YellowGreen = new(154, 205, 50);
	public static readonly Colour PaleGreen = new(152, 251, 152);
	public static readonly Colour PaleTurquoise = new(175, 238, 238);
}
