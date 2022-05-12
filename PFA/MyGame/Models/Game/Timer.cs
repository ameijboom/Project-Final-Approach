using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.GXPEngine.Utils;

namespace PFA.MyGame.Models.Game
{
	public class Timer : Sprite
	{
		private readonly int _millisAtStartRound;
		private const int MAX_MILLIS = 5 * 60 * 1000;

		public Timer(TiledObject obj = null) : base("./assets/UI/timer.png", addCollider: false)
		{
			// game.AddChild(this);
			_millisAtStartRound = Time.time;
		}

		public void Update()
		{
			float xPos = x - 10;
			float yPos = y - 16;
			// Gizmos.DrawCircle(xPos, yPos, 8, colour:Colour.Green);
			MyGame.Text(GetFormattedTime(MAX_MILLIS - GetTimeInMillis()), xPos, yPos);

		}

		private int GetTimeInMillis()
		{
			return Time.time - _millisAtStartRound;
		}

		private static string GetFormattedTime(int millis)
		{
			int i = millis / 1000;
			int minutes = i / 60;
			int seconds = i % 60;
			return minutes + ":" + seconds.ToString("00");
		}

		public bool IsTimeUp()
		{
			return GetTimeInMillis() >= MAX_MILLIS;
		}
	}
}
