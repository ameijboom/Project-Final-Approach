using GXPEngine;
using GXPEngine.Core;

namespace PFA;

public class MyGame : Game
{
	private int _frameCount = 0;
	private float _y;
	private MyGame() : base(1280, 720, false, false)
	{
		targetFps = 60;
		_frameCount = 0;
		_y = 0;
		Sprite background = new("assets/background.png");
		Add(background);

		Utils.print("MyGame initialized");
	}

	// ReSharper disable once UnusedMember.Local
	private void Update()
	{
		_y = 500 + Mathf.Sin(_frameCount / 100f) * 100;
		Gizmos.DrawCircle(new Vec2(width/2f, _y), 50); //TODO: Adding this line crashes the game, for some reason... It should not do that.
		Utils.print(_y);

		Utils.print("Update", _frameCount++);
	}

	private static void Main()
	{
		new MyGame().Start();
	}
}
