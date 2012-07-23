using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class MainScene : Scene
    {
        public MainScene(LuxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            List<string> skinName = new List<string>(3);
            skinName.Add("carre");
            skinName.Add("bullet001-1");
            skinName.Add("bullet001-2");
            skinName.Add("hero");
            Vector2 vect = new Vector2(0,1);
            Vector2 rien = new Vector2(0,0);
            // Les creations de sprite doivent être dans Initialize.
            
            Hero hero = new Hero(this.LuxGame, 1, 0, 0, null, 5, 2);
            Sprite heroSprite = new Sprite(hero, skinName);
            TestEnemy3 testEnemy = new TestEnemy3(this.LuxGame, 10, 2, 3, false, 100, null);
            Sprite enemySprite = new Sprite(testEnemy, skinName);
            TestEnemy2 testEnemy2 = new TestEnemy2(this.LuxGame, 10, 2, 3, false, 60, null);
            
            testEnemy.Position = new Vector2(0,0);
            testEnemy2.Position = new Vector2(100, 100);
            testEnemy.Skin = enemySprite;
            testEnemy2.Skin = enemySprite;
            hero.Position = new Vector2(400, 200);
            hero.Skin = heroSprite;
            // Il faut ajouter au jeu les élements que vous créez.
            Game.Components.Add(hero);
            Game.Components.Add(heroSprite);
            Game.Components.Add(testEnemy);
            Game.Components.Add(testEnemy2);
            // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.

            heroSprite.SetAnimation("hero");
            enemySprite.SetAnimation("carre");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
