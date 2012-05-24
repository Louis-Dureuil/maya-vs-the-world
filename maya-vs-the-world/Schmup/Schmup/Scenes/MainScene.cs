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
            Vector2 vect = new Vector2(0,1);
            Vector2 rien = new Vector2(0,0);
            // Les creations de sprite doivent être dans Initialize.
            
            ShotPattern bPatternTest = new ShotPattern(this.LuxGame, 7, vect, 10);
            bPatternTest.Position = new Vector2(300, 300);
            Hero hero = new Hero(this.LuxGame, 1, 0, 0, null, 5);
            Sprite heroSprite = new Sprite(hero, skinName);
            Shot shot = new Shot(this.LuxGame, 1, null);
            Sprite shotSprite = new Sprite(shot, skinName);
            TestEnemy testEnemy = new TestEnemy(this.LuxGame, 10, 2, 3, false, 100, null);
            Sprite enemySprite = new Sprite(testEnemy, skinName);

            shot.Position = bPatternTest.Position;
            shot.Speed = rien;
            shot.Skin = shotSprite;
            testEnemy.Position = new Vector2(400, 100);
            testEnemy.Skin = enemySprite;
            hero.Skin = heroSprite;
            // Il faut ajouter au jeu les élements que vous créez.
            Game.Components.Add(hero);
            Game.Components.Add(heroSprite);
            Game.Components.Add(shot);
            Game.Components.Add(shotSprite);
            Game.Components.Add(bPatternTest);
            Game.Components.Add(testEnemy);
            // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.

            heroSprite.SetAnimation("bullet001-2");
            shotSprite.SetAnimation("bullet001-1");
            enemySprite.SetAnimation("carre");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
