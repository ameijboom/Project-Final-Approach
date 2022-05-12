using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;

namespace PFA.MyGame.Models.Game
{
	public class Timer : Sprite
	{
		public Timer(TiledObject obj = null) : base("./assets/UI/timer.png", addCollider: false)
		{
			// game.AddChild(this);
		}

		public void Update()
		{
			float xPos = x - 10;
			float yPos = y - 16;
			// Gizmos.DrawCircle(xPos, yPos, 8, colour:Colour.Green);
			MyGame.Text("5:00", xPos, yPos); //TODO: change to actual timer
		}
	}
}
