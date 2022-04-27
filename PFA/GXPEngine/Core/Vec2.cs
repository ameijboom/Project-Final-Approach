// Author: TechnicJelle
// Copyright (c) TechnicJelle. All rights reserved.
//  Except for the functions in this file that are either inspired by, or taken from Processing: https://github.com/processing/processing4/blob/master/core/src/processing/core/PVector.java
// You're allowed to learn from this, but please do not simply copy.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NeoGXP.GXPEngine.LinAlg;

namespace GXPEngine.Core;

//TODO: Polish up XML documentation
public struct Vec2 : IVec
{
	private int _iterationPosition = -1;
	// ReSharper disable InconsistentNaming
	public float x;
	public float y;
	// ReSharper restore InconsistentNaming

	/// <summary>
	/// When comparing values, the values can be off by this much in either direction
	/// before it gets flagged as actually two different numbers
	/// </summary>
	private const float TOLERANCE = 0.0000001f;

    /// <summary>
    /// Constructs a new vector (defaults to (0, 0) )
    /// </summary>
    /// <param name="pX">new x position</param>
    /// <param name="pY">new y position</param>
    public Vec2(float pX = 0.0f, float pY = 0.0f)
	{
		x = pX;
		y = pY;
	}

	public Vec2(double pX = 0.0, double pY = 0.0) : this((float) pX, (float) pY)
	{
	}

	public Vec2(int pX = 0, int pY = 0) : this((float) pX, (float) pY)
	{
	}

	/// <summary>
	/// Constructs a new vector with the same number for both of the elements
	/// </summary>
	public Vec2(float f) : this(f, f)
	{
	}

	/// <summary>
	/// Constructs a new vector with the same number for both of the elements
	/// </summary>
	public Vec2(double d) : this(d, d)
	{
	}

	/// <summary>
	/// Constructs a new vector with the same number for both of the elements
	/// </summary>
	public Vec2(int i) : this(i, i)
	{
	}

	/// <summary>
	/// Returns a new vector pointing in the given direction
	/// </summary>
	public static Vec2 FromAngle(Angle angle)
	{
		return new Vec2(Math.Cos(angle), Math.Sin(angle));
	}

	/// <summary>
	/// Returns a new unit vector pointing in a random direction
	/// </summary>
	public static Vec2 Random()
	{
		return FromAngle(Angle.Random());
	}

	public int GetSize()
	{
		return 2;
	}

    public float GetElement(int i)
    {
        if (i is > 1 or < 0) throw new IndexOutOfRangeException();
		return i == 1 ? x : y;
    }

	/// <summary>
	/// Set the vector to a different point
	/// </summary>
	/// <param name="x">new x position</param>
	/// <param name="y">new y position</param>
	[SuppressMessage("ReSharper", "ParameterHidesMember")]
	// ReSharper disable once InconsistentNaming
	public Vec2 SetXY(float x = 0.0f, float y = 0.0f)
	{
		this.x = x;
		this.y = y;
		return this;
	}

	/// <summary>
	/// Calculates the magnitude of the vector (using sqrt)
	/// </summary>
	/// <returns>The magnitude of the vector</returns>
	public static float Mag(Vec2 v)
	{
		return (float) Math.Sqrt(v.x * v.x + v.y * v.y);
	}

	/// <summary>
	/// Calculates the magnitude of the vector (using sqrt)
	/// </summary>
	/// <returns>The magnitude of the vector</returns>
	public float Mag()
	{
		return Mag(this);
	}

	/// <summary>
	/// Calculates the square magnitude of the vector (so there is no slow sqrt() being called)
	/// </summary>
	/// <returns>The square magnitude of the vector</returns>
	public static float MagSq(Vec2 v)
	{
		return v.x * v.x + v.y * v.y;
	}

	/// <summary>
	/// Calculates the square magnitude of the vector (so there is no slow sqrt() being called)
	/// </summary>
	/// <returns>The square magnitude of the vector</returns>
	public float MagSq()
	{
		return MagSq(this);
	}

	/// <summary>
	/// Calculates a normalized version of the vector
	/// </summary>
	/// <returns>A normalized copy of the vector</returns>
	public Vec2 Normalized()
	{
		return Normalized(this);
	}

	/// <summary>
	/// Calculates a normalized version of the vector
	/// </summary>
	/// <returns>A normalized copy of the vector</returns>
	public static Vec2 Normalized(Vec2 v)
	{
		float mag = v.Mag();
		return mag == 0 ? new Vec2() : new Vec2(v.x / mag, v.y / mag);
	}

	/// <summary>
	/// Modifies the vector to be normalized
	/// </summary>
	/// <returns>The normalized vector</returns>
	public Vec2 Normalize()
	{
		return this = Normalized();
	}

	/// <summary>
	/// Sets the magnitude of this vector
	/// </summary>
	/// <param name="mag">The desired magnitude for this vector</param>
	/// <returns>The modified vector</returns>
	public Vec2 SetMag(float mag)
	{
		Normalize();
		return this *= mag;
	}

	/// <summary>
	/// Limit the magnitude of this vector
	/// </summary>
	/// <param name="max">The maximum magnitude the vector may be</param>
	/// <returns>The modified vector</returns>
	public Vec2 Limit(float max)
	{
		return MagSq() < max * max ? this : SetMag(max);
	}

	/// <summary>
	/// Set vector heading angle (magnitude doesn't change)
	/// </summary>
	/// <returns>The modified vector</returns>
	public Vec2 SetHeading(Angle angle)
	{
		float m = Mag();
		x = (float) (m * Math.Cos(angle));
		y = (float) (m * Math.Sin(angle));
		return this;
	}

	/// <summary>
	/// Gets the vector's heading angle
	/// </summary>
	public Angle Heading()
	{
		return Angle.FromRadians((float) Math.Atan2(y, x));
	}

	/// <summary>
	/// Rotate the vector over the given angle
	/// </summary>
	/// <returns>The modified vector</returns>
	public Vec2 Rotate(Angle angle)
	{
		return this = Rotate(this, angle);
	}

	/// <summary>
	/// Rotate the vector over the given angle
	/// </summary>
	/// <returns>The modified vector</returns>
	public static Vec2 Rotate(Vec2 vec, Angle angle)
	{
		float temp = vec.x;
		vec.x = (float) (vec.x * Math.Cos(angle) - vec.y * Math.Sin(angle));
		vec.y = (float) (temp * Math.Sin(angle) + vec.y * Math.Cos(angle));
		return vec;
	}

	/// <summary>
	/// Rotate the vector around the given point over the given angle
	/// </summary>
	/// <returns></returns>
	public Vec2 RotateAround(Vec2 rotateAround, Angle angle)
	{
		this -= rotateAround;
		Rotate(angle);
		return this += rotateAround;
	}

	/// <summary>
	/// Calculates the absolute components of the vector
	/// </summary>
	/// <returns>A new vector with both components positive</returns>
	public Vec2 GetAbs()
	{
		return new Vec2(Math.Abs(x), Math.Abs(y));
	}

	/// <summary>
	/// Calculates the normal vector on this vector
	/// </summary>
	/// <returns>A new vector that points in the perpendicular direction</returns>
	public Vec2 GetNormal()
	{
		return new Vec2(-y, x);
	}

	/// <summary>
	/// Calculates the distance between this vector and another vector
	/// </summary>
	public float Dist(Vec2 other)
	{
		return Dist(this, other);
	}

	/// <summary>
	/// Calculates the distance between two vectors
	/// </summary>
	public static float Dist(Vec2 v1, Vec2 v2)
	{
		Vec2 d = v1 - v2;
		return d.Mag();
	}

	/// <summary>
	/// Calculates the square distance between this vector and another vector
	/// (so there is no slow sqrt() being called)
	/// </summary>
	public float DistSq(Vec2 other)
	{
		return DistSq(this, other);
	}

	/// <summary>
	/// Calculates the square distance between two vectors
	/// (so there is no slow sqrt() being called)
	/// </summary>
	public static float DistSq(Vec2 v1, Vec2 v2)
	{
		Vec2 d = v1 - v2;
		return d.MagSq();
	}

	/// <summary>
	/// Calculates the dot product between this vector and another vector
	/// </summary>
	public float Dot(Vec2 v)
	{
		return Dot(this, v);
	}

	/// <summary>
	/// Calculates the dot product between two vectors
	/// </summary>
	public static float Dot(Vec2 v1, Vec2 v2)
	{
		return v1.x * v2.x + v1.y * v2.y;
	}

	/// <summary>
	/// Calculates the cross product between this vector and another vector
	/// </summary>
	/// <remarks>
	/// This is the equivalent to taking the cross product of two three-dimensional vectors
	/// with the third component set to zero and taking the magnitude of the result.
	/// </remarks>
	public float Cross(Vec2 other)
	{
		return Cross(this, other);
	}

	/// <inheritdoc cref="Cross(GXPEngine.Core.Vec2)"/>
	/// <summary>
	/// Calculates the cross product between two vectors
	/// </summary>
	public static float Cross(Vec2 v1, Vec2 v2)
	{
		return v1.x * v2.y - v1.y * v2.x;
	}

	/// <summary>
	/// Performs vector/scalar multiplication.
	/// </summary>
	private static Vec2 Scale(Vec2 vec, float scalar)
    {
        return new Vec2(vec.x * scalar, vec.y * scalar);
    }

	/// <summary>
	/// Performs vector addition.
	/// </summary>
	private Vec2 Add(Vec2 vector)
	{
		return new Vec2(x + vector.x, y + vector.y);
	}

	public static Vec2 operator +(Vec2 left, Vec2 right)
	{
		return left.Add(right);
	}

	public static Vec2 operator -(Vec2 vec)
	{
		return new Vec2(-vec.x, -vec.y);
	}

	public static Vec2 operator -(Vec2 left, Vec2 right)
	{
		return left.Add(-right);
	}

	public static Vec2 operator *(Vec2 vec, float f)
	{
		return Scale(vec, f);
	}

	public static Vec2 operator *(float f, Vec2 vec)
	{
		return Scale(vec, f);
	}

	/// <summary>
	/// Element-wise multiplication
	/// </summary>
	/// <remarks>
	/// This isn't really a vector operation, but it's convenient to have it here.<br/>
	/// https://en.wikipedia.org/wiki/Hadamard_product_(matrices)
	/// </remarks>
	public static Vec2 operator *(Vec2 left, Vec2 right)
	{
		return new Vec2(left.x * right.x, left.y * right.y);
	}

	//for the remark:
	/// <inheritdoc cref="op_Multiply(GXPEngine.Core.Vec2,GXPEngine.Core.Vec2)"/>
	/// <summary>
	/// Divide a vector by a number (scalar division)
	/// </summary>
	public static Vec2 operator /(Vec2 vec, float f)
	{
		return vec / new Vec2(f);
	}

	//for the remark:
	/// <inheritdoc cref="op_Multiply(GXPEngine.Core.Vec2,GXPEngine.Core.Vec2)"/>
	/// <summary>
	/// Divide a number by a vector
	/// </summary>
	public static Vec2 operator /(float f, Vec2 vec)
	{
		return new Vec2(f) / vec;
	}

	//for the remark:
	/// <inheritdoc cref="op_Multiply(GXPEngine.Core.Vec2,GXPEngine.Core.Vec2)"/>
	/// <summary>
	/// Element-wise division
	/// </summary>
	public static Vec2 operator /(Vec2 left, Vec2 right)
	{
		return new Vec2(left.x / right.x, left.y / right.y);
	}

	public static bool operator ==(Vec2 left, Vec2 right)
	{
		return Math.Abs(left.x - right.x) < TOLERANCE && Math.Abs(left.y - right.y) < TOLERANCE;
	}

	public static bool operator !=(Vec2 left, Vec2 right)
	{
		return Math.Abs(left.x - right.x) > TOLERANCE || Math.Abs(left.y - right.y) > TOLERANCE;
	}

	public override bool Equals(object obj)
	{
		if (obj is not Vec2 vec2)
			return false;
		return Math.Abs(x - vec2.x) < TOLERANCE && Math.Abs(y - vec2.y) < TOLERANCE;
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + x.GetHashCode();
		hash = hash * 31 + y.GetHashCode();
		return hash;
	}

	public override string ToString()
	{
		return $"({x},{y})";
	}

    public IEnumerator GetEnumerator()
    {
        return this;
    }

    public bool MoveNext()
    {
        _iterationPosition++;
		return _iterationPosition < 2;
    }

    public void Reset()
    {
        _iterationPosition = -1;
    }

	object IEnumerator.Current => (float)_iterationPosition == 0 ? x : y;
}
