namespace PFA.GXPEngine.LinAlg
{
    public interface IMatrix
    {
         public float GetElement(int i, int j);
         public IMatrix Transpose();
    }
}