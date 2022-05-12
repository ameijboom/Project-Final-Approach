using PFA.GXPEngine;
using PFA.GXPEngine.AddOns;
using PFA.MyGame.Managers;

namespace PFA.MyGame.Models.Game
{
    public class Next : Sprite
    {
        public Next(TiledObject obj = null) : base("./assets/UI/next.png", addCollider: false)
        {

        }

        void Update()
        {
            MyGame.Text(MyGame.MolecatToMake(), x, y);
        }
    }
}