namespace NeoGXP.GXPEngine.LinAlg
{
    public interface IMatrix
    {
         public float GetElement(int i, int j);
         public IMatrix Multiply(IMatrix matrix);
         public IVec Multiply(IVec vec);
         public IMatrix Transpose();
    }
}