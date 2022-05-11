using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;

namespace PFA.MyGame.Models.Game
{
    public class Field : Sprite
    {
        public Field(TiledObject obj = null) : base("./assets/UI/playingfield.png", addCollider: false)
        {
            // System.Console.WriteLine(File.Exists("./assets/UI/playingfield.png"));
        }
    }
}