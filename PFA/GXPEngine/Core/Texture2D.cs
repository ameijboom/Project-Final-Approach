using System.Collections;
using Arqan;
using SkiaSharp;

namespace PFA.GXPEngine.Core
{
	public class Texture2D
	{
		private static Hashtable LoadCache = new Hashtable();
		private static Texture2D lastBound = null;
		
		const int UNDEFINED_GLTEXTURE 	= 0;
		
		private SKBitmap _bitmap;
		private uint[] _glTexture;
		private string _filename = "";
		private int count = 0;
		private bool stayInCache = false;

		//------------------------------------------------------------------------------------------------------------------------
		//														Texture2D()
		//------------------------------------------------------------------------------------------------------------------------
		public Texture2D (int width, int height) {
			if (width == 0) if (height == 0) return;
			SetBitmap (new SKBitmap(width, height));
		}
		public Texture2D (string filename) {
			Load (filename);
		}
		public Texture2D (SKBitmap bitmap) {
			SetBitmap (bitmap);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetInstance()
		//------------------------------------------------------------------------------------------------------------------------
		public static Texture2D GetInstance (string filename, bool keepInCache=false) {
			Texture2D tex2d = LoadCache[filename] as Texture2D;
			if (tex2d == null) {
				tex2d = new Texture2D(filename);
				LoadCache[filename] = tex2d;
			}
			tex2d.stayInCache |= keepInCache; // setting it once to true keeps it in cache
			tex2d.count ++;
			return tex2d;
		}


		//------------------------------------------------------------------------------------------------------------------------
		//														RemoveInstance()
		//------------------------------------------------------------------------------------------------------------------------
		public static void RemoveInstance (string filename)
		{
			if (LoadCache.ContainsKey (filename)) {
				Texture2D tex2D = LoadCache[filename] as Texture2D;
				tex2D.count --;
				if (tex2D.count == 0 && !tex2D.stayInCache) LoadCache.Remove (filename);
			}
		}

		public void Dispose () {
			if (_filename != "") {
				Texture2D.RemoveInstance (_filename);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														bitmap
		//------------------------------------------------------------------------------------------------------------------------
		public SKBitmap bitmap {
			get { return _bitmap; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														filename
		//------------------------------------------------------------------------------------------------------------------------
		public string filename {
			get { return _filename; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														width
		//------------------------------------------------------------------------------------------------------------------------
		public int width {
			get { return _bitmap.Width; }
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														height
		//------------------------------------------------------------------------------------------------------------------------
		public int height {
			get { return _bitmap.Height; }
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Bind()
		//------------------------------------------------------------------------------------------------------------------------
		public void Bind() {
			if (lastBound == this) return;
			lastBound = this;
			GL.glBindTexture(GL.GL_TEXTURE_2D, (uint)_glTexture[0]);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Unbind()
		//------------------------------------------------------------------------------------------------------------------------
		public void Unbind() {
			//GL.BindTexture (GL.TEXTURE_2D, 0);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Load()
		//------------------------------------------------------------------------------------------------------------------------
		private void Load(string filename) {
			_filename = filename;
			SKBitmap bitmap;
			try {
				bitmap = SKBitmap.Decode(filename);
			} catch {
				throw new Exception("Image " + filename + " cannot be found.");
			}
			SetBitmap(bitmap);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														SetBitmap()
		//------------------------------------------------------------------------------------------------------------------------
		private void SetBitmap(SKBitmap bitmap) {
			_bitmap = bitmap;
			CreateGLTexture ();
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														CreateGLTexture()
		//------------------------------------------------------------------------------------------------------------------------
		private void CreateGLTexture ()
		{
			if (_glTexture != null)
				if (_glTexture.Length > 0)
					if (_glTexture[0] != UNDEFINED_GLTEXTURE)
						destroyGLTexture ();
				
			_glTexture = new uint[1];
			if (_bitmap == null)
				_bitmap = new SKBitmap (64, 64);

			GL.glGenTextures (1, _glTexture);
			
			GL.glBindTexture (GL.GL_TEXTURE_2D, _glTexture[0]);
			if (Game.main.PixelArt) {
				GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_NEAREST);
				GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_NEAREST);
			} else {
				GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
				GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);
			}
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP_TO_EDGE);
			GL.glTexParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP_TO_EDGE);	
			
			UpdateGLTexture();
			GL.glBindTexture (GL.GL_TEXTURE_2D, 0);
			lastBound = null;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														UpdateGLTexture()
		//------------------------------------------------------------------------------------------------------------------------
		public void UpdateGLTexture() {             			
			GL.glBindTexture (GL.GL_TEXTURE_2D, _glTexture[0]);
			GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, _bitmap.Width, _bitmap.Height, 0,
			              GL.GL_BGRA, GL.GL_UNSIGNED_BYTE, _bitmap.GetPixels());
			              
			lastBound = null;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														destroyGLTexture()
		//------------------------------------------------------------------------------------------------------------------------
		private void destroyGLTexture() {
			GL.glDeleteTextures(1, _glTexture);
			_glTexture[0] = UNDEFINED_GLTEXTURE;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Clone()
		//------------------------------------------------------------------------------------------------------------------------
		public Texture2D Clone (bool deepCopy=false) {
			SKBitmap bitmap;
			if (deepCopy) {
				bitmap = _bitmap.Copy () as SKBitmap;
			} else {
				bitmap = _bitmap;
			}
			Texture2D newTexture = new Texture2D(0, 0);
			newTexture.SetBitmap(bitmap);
			return newTexture;
		}

		public bool wrap {
			set { 
				GL.glBindTexture (GL.GL_TEXTURE_2D, _glTexture[0]);
				GL.glTextureParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, value ? (int)GL.GL_REPEAT : (int)GL.GL_CLAMP_TO_EDGE);
				GL.glTextureParameteri (GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, value ? (int)GL.GL_REPEAT : (int)GL.GL_CLAMP_TO_EDGE);	
				GL.glBindTexture (GL.GL_TEXTURE_2D, 0);
				lastBound = null;
			}
		}

		public static string GetDiagnostics() {
			string output = "";
			output += "Number of textures in cache: " + LoadCache.Keys.Count+'\n';
			return output;
		}
	}
}

