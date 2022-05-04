using Arqan;
using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;

namespace PFA.GXPEngine.AddOns {
	/// <summary>
	/// A class that can be used to create "sub windows" (e.g. mini-map, splitscreen, etc).
	/// This is not a gameobject. Instead, subscribe the RenderWindow method to the main game's 
	/// OnAfterRender event.
	/// </summary>
	public class Window {
		/// <summary>
		/// The x coordinate of the window's left side
		/// </summary>
		private int windowX {
			get => _windowX;
			set {
				_windowX = value;
				_dirty = true;
			}
		}
		/// <summary>
		/// The y coordinate of the window's top
		/// </summary>
		private int windowY {
			get => _windowY;
			set {
				_windowY = value;
				_dirty = true;
			}
		}

		/// <summary>
		/// The position of the window's top-left corner
		/// </summary>
		public Vec2 windowPos
		{
			get => new(windowX, windowY);
			set {
				windowX = (int)value.x;
				windowY = (int)value.y;
			}
		}

		/// <summary>
		/// The x coordinate of the window center
		/// </summary>
		private float centerX => _windowX + _width / 2f;

		/// <summary>
		/// The y coordinate of the window center
		/// </summary>
		private float centerY => _windowY + _height / 2f;

		/// <summary>
		/// The position of the window's center
		/// </summary>
		public Vec2 centerPos => new(centerX, centerY);

		/// <summary>
		/// The window's width
		/// </summary>
		private int width {
			get {
				return _width;
			}
			set {
				_width = value;
				_dirty = true;
			}
		}
		/// <summary>
		/// The window's height
		/// </summary>
		private int height {
			get {
				return _height;
			}
			set {
				_height = value;
				_dirty = true;
			}
		}

		/// <summary>
		/// The window's size
		/// </summary>
		public Vec2 size
		{
			get => new(width, height);
			set {
				width = (int)value.x;
				height = (int)value.y;
			}
		}

		/// <summary>
		/// The game object (which should be in the hierarchy!) that determines the focus point, rotation and scale
		/// of the viewport window.
		/// </summary>
		public GameObject camera;

		// private variables:
		int _windowX, _windowY;
		int _width, _height;
		bool _dirty=true;

		Transformable window;

		/// <summary>
		/// Creates a render window in the rectangle given by x,y,width,height.
		/// The camera determines the focal point, rotation and scale of this window.
		/// </summary>
		public Window(int x, int y, int width, int height, GameObject camera) {
			_windowX = x;
			_windowY = y;
			_width = width;
			_height = height;
			this.camera = camera;
			window = new Transformable ();
		}

		/// <summary>
		/// To render the scene in this window, subscribe this method to the main game's OnAfterRender event.
		/// </summary>
		public void RenderWindow(GLContext glContext) {
			if (_dirty) {
				window.position = new Vec2(
					_windowX + _width / 2,
					_windowY + _height / 2);
				_dirty = false;
			}
			glContext.PushMatrix (window.matrix);

			int pushes = 1;
			GameObject current = camera;
			while (true) {
				Transformable cameraInverse = current.Inverse ();
				glContext.PushMatrix (cameraInverse.matrix);
				pushes++;
				if (current.parent == null)
					break;
				current = current.parent;
			}

			if (current is Game) {// otherwise, the camera is not in the scene hierarchy, so render nothing - not even a black background
				Game main=Game.main;
				Rectangle oldRange = main.RenderRange;
				SetRenderRange();
				main.SetViewport (_windowX, _windowY, _width, _height, false);
				GL.glClear(GL.GL_COLOR_BUFFER_BIT);
				current.Render (glContext);
				main.SetViewport ((int)oldRange.left, (int)oldRange.top, (int)oldRange.width, (int)oldRange.height);
			}
			
			for (int i = 0; i < pushes; i++) {
				glContext.PopMatrix ();
			}
		}

		private void SetRenderRange() {
			Vec2[] worldSpaceCorners = new Vec2[4];
			worldSpaceCorners[0] = camera.TransformPoint(-_width/2, -_height/2);
			worldSpaceCorners[1] = camera.TransformPoint(-_width/2,  _height/2);
			worldSpaceCorners[2] = camera.TransformPoint( _width/2,  _height/2);
			worldSpaceCorners[3] = camera.TransformPoint( _width/2, -_height/2);

			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			for (int i=0; i<4; i++) {
				if (worldSpaceCorners[i].x > maxX) maxX = worldSpaceCorners[i].x;
				if (worldSpaceCorners[i].x < minX) minX = worldSpaceCorners[i].x;
				if (worldSpaceCorners[i].y > maxY) maxY = worldSpaceCorners[i].y;
				if (worldSpaceCorners[i].y < minY) minY = worldSpaceCorners[i].y;
			}

			Game.main.RenderRange = new Rectangle(minX, minY, maxX - minX, maxY - minY);
		}
	}
}
