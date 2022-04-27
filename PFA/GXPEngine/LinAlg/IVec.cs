using System.Collections;

namespace NeoGXP.GXPEngine.LinAlg
{
    public interface IVec : IEnumerable, IEnumerator
    {
        public int GetSize();
        public float GetElement(int i);
        public float Mag();
        public float MagSq();
    }
}