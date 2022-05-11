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
    }
}