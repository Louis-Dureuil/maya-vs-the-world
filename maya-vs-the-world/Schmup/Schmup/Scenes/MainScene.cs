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
        // DELETE ME ONCE TESTING OVER
        Interpretable interpretable;
        //private Map map;
        // Textures utilisées pendant tout le combat.
        //private Texture2D bullet1Texture;
        private Texture2D bullet2Texture;
        //private Texture2D enemyTexture;
        // Commenter une des deux lignes pour obtenir un des deux boss
        // Puis, dans Initialize(), commenter une des deux lignes
        //private BigBoss boss;
        //private EasyBoss boss;
        //private FastBoss boss;
        private World world;
        private SpriteBatch sb;
        private Rectangle rect;

        public MainScene(LuxGame game)
            : base(game)
        {
            //map = new Map();
        }

        public override void Initialize()
        {
            /*GraphicsDeviceManager gdm = new GraphicsDeviceManager(this.Game);
            sb = new SpriteBatch(gdm.GraphicsDevice);
            rect = new Rectangle(3, 3, 96, 96);*/
            world = new World(LuxGame);
            Interpreter interpreter = new Interpreter("Scripts/Toto", true, false);
            interpretable = interpreter.GetInterpretable("LuxEngine.TotoSays", this);
            Game.Components.Add(interpretable);
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
            //map.Draw(sb, rect, new Vector2(0, 0));
        }
    }
}
