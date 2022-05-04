using PFA.GXPEngine.Utils;

namespace PFA.GXPEngine.LinAlg;

public class Matrix4 : ISquareMatrix
{
	private float[,] _matrix = new float[4, 4];
	private bool _diagonal = true;

	public float Determinant()
	{
		float[,] m = _matrix;
		return
			m[0, 3] * m[1, 2] * m[2, 1] * m[3, 0] - m[0, 2] * m[1, 3] * m[2, 1] * m[3, 0] -
			m[0, 3] * m[1, 1] * m[2, 2] * m[3, 0] + m[0, 1] * m[1, 3] * m[2, 2] * m[3, 0] +
			m[0, 2] * m[1, 1] * m[2, 3] * m[3, 0] - m[0, 1] * m[1, 2] * m[2, 3] * m[3, 0] -
			m[0, 3] * m[1, 2] * m[2, 0] * m[3, 1] + m[0, 2] * m[1, 3] * m[2, 0] * m[3, 1] +
			m[0, 3] * m[1, 0] * m[2, 2] * m[3, 1] - m[0, 0] * m[1, 3] * m[2, 2] * m[3, 1] -
			m[0, 2] * m[1, 0] * m[2, 3] * m[3, 1] + m[0, 0] * m[1, 2] * m[2, 3] * m[3, 1] +
			m[0, 3] * m[1, 1] * m[2, 0] * m[3, 2] - m[0, 1] * m[1, 3] * m[2, 0] * m[3, 2] -
			m[0, 3] * m[1, 0] * m[2, 1] * m[3, 2] + m[0, 0] * m[1, 3] * m[2, 1] * m[3, 2] +
			m[0, 1] * m[1, 0] * m[2, 3] * m[3, 2] - m[0, 0] * m[1, 1] * m[2, 3] * m[3, 2] -
			m[0, 2] * m[1, 1] * m[2, 0] * m[3, 3] + m[0, 1] * m[1, 2] * m[2, 0] * m[3, 3] +
			m[0, 2] * m[1, 0] * m[2, 1] * m[3, 3] - m[0, 0] * m[1, 2] * m[2, 1] * m[3, 3] -
			m[0, 1] * m[1, 0] * m[2, 2] * m[3, 3] + m[0, 0] * m[1, 1] * m[2, 2] * m[3, 3];
	}

	//------------------------------------------------------------------------------------------------------------------------
	//													EigenValues/EigenVectors
	//------------------------------------------------------------------------------------------------------------------------

	/// <remark>
	/// If you really need these, you're welcome to implement them yourself. I'm not doing it.
	/// If you do implement them, please PR them to the NeoGXP repo!
	/// ~Pine
	/// </remark>
	/// <exception cref="NotImplementedException"></exception>
	public float[] EigenValues()
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc cref="EigenValues"/>
	public IVec[] EigenVectors()
	{
		throw new NotImplementedException();
	}

	//------------------------------------------------------------------------------------------------------------------------
	//													Element Getters/Setters
	//------------------------------------------------------------------------------------------------------------------------

	public float GetElement(int i, int j)
	{
		if (i is < 0 or > 3 || j is < 0 or > 3)
		{
			throw new IndexOutOfRangeException();
		}

		return _matrix[i, j];
	}

	public void SetElement(int i, int j, float v)
	{
		if (i is < 0 or > 3 || j is < 0 or > 3)
		{
			throw new IndexOutOfRangeException();
		}

		_matrix[i, j] = v;
	}

	public IMatrix Inverse()
	{
		throw new NotImplementedException();
	}

	public bool IsDiagonal()
	{
		_diagonal = true;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (i == j || _matrix[i, j] == 0) continue;
				_diagonal = false;
				return false;
			}
		}

		return _diagonal;
	}

	/// <remark>
	/// This can trivially be implemented provided Inverse is implemented, which it is not yet. Please update this function when Inverse is implemented.
	/// </remark>
	public bool IsOrthogonal()
	{
		// return Transpose() == Inverse();
		throw new NotImplementedException();
	}

	public bool IsSymmetric()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < i; j++)
			{
				if (Math.Abs(_matrix[i,  j] - _matrix[j, i]) > Mathf.TOLERANCE) return false;
			}
		}

		return true;
	}

	public bool IsTriangular(bool upper)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				// i < j is the bottom part of the matrix, which is empty for an upper triangular matrix.
				// The opposite applies to i > j.
				if (((i < j && upper) || (i > j && !upper)) && _matrix[i, j] != 0)
				{
					return false;
				}
			}
		}

		return true;
	}

	public float Trace()
	{
		float sum = 0.0f;
		for (int i = 0; i < 4; i++)
		{
			sum += _matrix[i, i];
		}

		return sum;
	}

	public IMatrix Transpose()
	{
		Matrix4 transposed = new Matrix4();
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				transposed.SetElement(j, i, _matrix[i, j]);
			}
		}

		return transposed;
	}

	public void ToUnitMatrix()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				_matrix[i, j] = i == j ? 1.0f : 0.0f;
			}
		}
	}
}
