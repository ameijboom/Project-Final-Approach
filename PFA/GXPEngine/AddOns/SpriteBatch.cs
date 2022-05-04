using PFA.GXPEngine.Core;
using PFA.GXPEngine.LinAlg;

namespace PFA.GXPEngine.AddOns;

/// <summary>
/// A SpriteBatch is a GameObject that can be used to render many sprites efficiently, and change the colour, alpha and blend mode of
/// all of those sprites simultaneously.
/// Usage: Add any number of sprites as child to a sprite batch, and then call the Freeze method.
/// Note that this will destroy the individual sprites, so there won't be colliders for them anymore, and
/// the position and rotation of individual sprites cannot be changed anymore.
/// </summary>
public class SpriteBatch : GameObject {
	Dictionary<Texture2D, BufferRenderer> renderers;
	public Colour Colour = Colour.White;

	/// <summary>
	/// Gets or sets the alpha value of the sprite batch.
	/// Setting this value allows you to make the sprite batch (semi-)transparent.
	/// The alpha value should be in the range <c>(0 - 255)</c>, where 0 is fully transparent and 255 is fully opaque.
	/// </summary>
	public byte alpha
	{
		get => Colour.a;
		set => Colour.a = value;
	}

	public BlendMode blendMode = null;
	bool initialized = false;

	Rectangle _bounds;

	/// <summary>
	/// Create a new SpriteBatch game object.
	/// After adding sprites as child to this game object, call the Freeze method to started batched drawing.
	/// </summary>
	public SpriteBatch() : base(false) {
		renderers = new Dictionary<Texture2D, BufferRenderer>();
	}

	/// <summary>
	/// Call this method after adding sprites as child to this game object, and positioning and rotating them correctly.
	/// This will destroy the individual sprites and their colliders.
	/// Note that the individual colour, alpha and blend mode of those sprites is forgotten:
	/// use the colour, alpha and blend mode of the sprite batch instead.
	/// </summary>
	public void Freeze() {
		float boundsMinX = float.MaxValue;
		float boundsMaxX = float.MinValue;
		float boundsMinY = float.MaxValue;
		float boundsMaxY = float.MinValue;

		List<GameObject> children = GetChildren(true); // intentional clone!
		foreach (GameObject child in children) {
			if (child is Sprite) {
				Sprite tile = (Sprite)child;
				tile.parent = null; // To get the proper Extents

				if (!renderers.ContainsKey(tile.texture)) {
					renderers[tile.texture] = new BufferRenderer(tile.texture);
				}

				BufferRenderer rend = renderers[tile.texture];

				Vec2[] bounds = tile.GetExtents();
				float[] uvs = tile.GetUVs(false);
				for (int corner = 0; corner < 4; corner++) {
					rend.AddVert(bounds[corner].x, bounds[corner].y);
					rend.AddUv(uvs[corner * 2], uvs[corner * 2 + 1]);

					if (bounds[corner].x < boundsMinX) boundsMinX = bounds[corner].x;
					if (bounds[corner].x > boundsMaxX) boundsMaxX = bounds[corner].x;
					if (bounds[corner].y < boundsMinY) boundsMinY = bounds[corner].y;
					if (bounds[corner].y > boundsMaxY) boundsMaxY = bounds[corner].y;
				}
				tile.Destroy();
			}
		}
		_bounds = new Rectangle(boundsMinX, boundsMinY, boundsMaxX - boundsMinX, boundsMaxY - boundsMinY);

		// Create buffers:
		foreach (var texture in renderers.Keys) {
			renderers[texture].CreateBuffers();
		}

		initialized = true;
	}

	/// <summary>
	/// Gets the four corners of this object as a set of 4 Vector2s.
	/// </summary>
	/// <returns>
	/// The extents.
	/// </returns>
	public Vec2[] GetExtents() {
		Vec2[] ret = new Vec2[4];
		ret[0] = TransformPoint(_bounds.left, _bounds.top);
		ret[1] = TransformPoint(_bounds.right, _bounds.top);
		ret[2] = TransformPoint(_bounds.right, _bounds.bottom);
		ret[3] = TransformPoint(_bounds.left, _bounds.bottom);
		return ret;
	}

	protected override void OnDestroy() {
		foreach (BufferRenderer rend in renderers.Values) {
			rend.Dispose();
		}
		renderers.Clear();
		initialized = false;
	}


	override protected void RenderSelf(GLContext glContext) {
		if (!initialized) return;

		bool test = false;

		Vec2[] bounds = GetExtents();
		float maxX = float.MinValue;
		float maxY = float.MinValue;
		float minX = float.MaxValue;
		float minY = float.MaxValue;
		for (int i = 0; i < 4; i++) {
			if (bounds[i].x > maxX) maxX = bounds[i].x;
			if (bounds[i].x < minX) minX = bounds[i].x;
			if (bounds[i].y > maxY) maxY = bounds[i].y;
			if (bounds[i].y < minY) minY = bounds[i].y;
		}
		test = (maxX < game.RenderRange.left) || (maxY < game.RenderRange.top) || (minX >= game.RenderRange.right) || (minY >= game.RenderRange.bottom);

		if (test == false) {
			if (blendMode != null) blendMode.enable();
			glContext.SetColour(Colour);

			foreach (BufferRenderer rend in renderers.Values) {
				rend.DrawBuffers(glContext);
			}

			glContext.SetColour(Colour.White);

			if (blendMode != null) BlendMode.NORMAL.enable();
		} else {
			//Console.WriteLine("Not rendering sprite batch");
		}
	}
}

/// <summary>
/// A helper class for SpriteBatches, and possibly other complex objects or collections with larger vertex and uv lists.
/// </summary>
public class BufferRenderer {
	protected float[] verts;
	protected float[] uvs;
	protected int numberOfVertices; // The number of rendered quads is numberOfVertices/4

	Texture2D _texture;

	List<float> vertList = new List<float>();
	List<float> uvList = new List<float>();

	public BufferRenderer(Texture2D texture) {
		_texture = texture;
	}

	public void AddVert(float x, float y) {
		vertList.Add(x);
		vertList.Add(y);
	}
	public void AddUv(float u, float v) {
		uvList.Add(u);
		uvList.Add(v);
	}

	public void CreateBuffers() {
		verts = vertList.ToArray();
		uvs = uvList.ToArray();
		numberOfVertices = verts.Length / 2;
	}

	public void DrawBuffers(GLContext glContext) {
		_texture.Bind();
		float[] vertArray = vertList.ToArray();
		for (int i = 0; i < vertArray.Length; i += 8)
		{
			Vec2[] verts = new Vec2[]
			{
				new Vec2(vertArray[0], vertArray[1]),
				new Vec2(vertArray[2], vertArray[3]),
				new Vec2(vertArray[4], vertArray[5]),
				new Vec2(vertArray[6], vertArray[7])
			};
			float[] uvs = new float[]
			{
				this.uvs[0], this.uvs[1],
				this.uvs[2], this.uvs[3],
				this.uvs[4], this.uvs[5],
				this.uvs[6], this.uvs[7],
			};
			glContext.DrawQuad(verts, uvs);
		}

		_texture.Unbind();
	}

	public void Dispose() {
		// For this backend: nothing needed
	}
}
