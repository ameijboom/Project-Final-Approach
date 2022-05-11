using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;

namespace PFA.MyGame.Models.Game
{
    public class Next : Sprite
    {
        public Next(TiledObject obj = null) : base("./assets/UI/next.png", addCollider: false)
        {
            // game.AddChild(this);
        }
    }
}