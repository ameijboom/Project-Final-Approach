namespace PFA.GXPEngine.LinAlg
{
    public interface ISquareMatrix : IMatrix
    {
         public void ToUnitMatrix();

         public float Trace();
         public float Determinant();
         public float[] EigenValues();
         public IVec[] EigenVectors();
         public IMatrix Inverse();
         public bool IsDiagonal();
         public bool IsTriangular(bool upper);
         public bool IsSymmetric();
         public bool IsOrthogonal();
    }
}