using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class MainScene : Scene
    {

        // Textures utilisées pendant tout le combat.
        //private Texture2D bullet1Texture;
        private Texture2D bullet2Texture;
        //private Texture2D enemyTexture;
        // Commenter une des deux lignes pour obtenir un des deux boss
        // Puis, dans Initialize(), commenter une des deux lignes
        //private BigBoss boss;
        //private EasyBoss boss;
        //private FastBoss boss;
        private ThreeEnemies world;

        public MainScene(LuxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            world = new ThreeEnemies(LuxGame);
            base.Initialize();
            world.Initialize();
        }

        //public void Create()
        //{
        //    foreach (TestEnemy enemy in commonEnemies)
        //    {
        //        enemy.Create();
        //    }
        //}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            world.Update(gameTime);
        }
    }
}
