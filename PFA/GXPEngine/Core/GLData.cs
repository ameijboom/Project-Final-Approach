using Arqan;

namespace PFA.GXPEngine.Core
{
	public class GLData
	{
		public uint[] VBOs;
		public uint[] VAOs;
		public uint[] EBOs;

		public void bindBuffer(int buffer)
		{
            GL.glBindVertexArray(VAOs[buffer]);
            GL.glBindBuffer(GL.GL_ARRAY_BUFFER, VBOs[buffer]);
            GL.glBindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, EBOs[buffer]);
		}
	}
}
