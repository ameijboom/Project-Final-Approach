using System;

namespace NeoGXP.GXPEngine.LinAlg
{
    public static class VecMath
    {
        public static float Dot(IVec vec1, IVec vec2)
        {
            if (vec1.GetSize() != vec2.GetSize()) throw new ArithmeticException("The dot product is only defined for vectors of equal size!");
            float sum = 0.0f;
            while (vec1.MoveNext() && vec2.MoveNext())
            {
                sum += (float)vec1.Current * (float)vec2.Current;
            }
            return sum;
        }
    }
}