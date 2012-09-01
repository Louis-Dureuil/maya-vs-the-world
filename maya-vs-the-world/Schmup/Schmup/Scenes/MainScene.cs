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
        private Texture2D bullet1Texture;
        private Texture2D bullet2Texture;
        private Texture2D enemyTexture;
        // Commenter une des deux lignes pour obtenir un des deux boss
        // Puis, dans Initialize(), commenter une des deux lignes
        //private BigBoss boss;
        private EasyBoss boss;

        public MainScene(LuxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.bullet1Texture = this.Content.Load<Texture2D>("bullet001-1");
            this.bullet2Texture = this.Content.Load<Texture2D>("bullet002-1");
            this.enemyTexture = this.Content.Load<Texture2D>("commonEnemy");
            List<string> skinName = new List<string>(3);
            skinName.Add("carre");
            skinName.Add("bullet001-1");
            skinName.Add("bullet001-2");
            skinName.Add("hero");
            Vector2 vect = new Vector2(0, 1);
            Vector2 rien = new Vector2(0, 0);
            // Les creations de sprite doivent être dans Initialize.

            // Commenter une des deux lignes pour obtenir un des deux boss
            // Puis, tout en haut, commenter une des deux lignes
            //boss = new BigBoss(this.LuxGame, 1500, 10, 10, false, 1, null, bullet2Texture);
            boss = new EasyBoss(this.LuxGame, 1000, 10, 10, null, bullet2Texture, 8);
            boss.Skin = new Sprite(boss, new List<string>() { "boss" });
            boss.Skin.SetAnimation("boss");
            boss.Position = new Vector2(400, 50);

            // Commenter une des deux lignes
            // Hero change de vitesse avec L, tire avec J
            // TechnicalHero passe à la vitesse lente si J est appuyé, et rapide si J est tapoté
            Hero hero = new Hero(this.LuxGame, 10, 10, 10, 5, 2, null);
            //TechnicalHero hero = new TechnicalHero(this.LuxGame, 10, 10, 10, 5, 2, null);
            Sprite heroSprite = new Sprite(hero, skinName);
            hero.Position = new Vector2(400, 400);
            Common.HeroPosition = hero.Position;
            hero.Skin = heroSprite;
            // Il faut ajouter au jeu les élements que vous créez.
            Game.Components.Add(boss);
            Game.Components.Add(hero);
            Game.Components.Add(heroSprite);

            heroSprite.SetAnimation("hero");
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
            boss.Create();
            Common.BossPosition = boss.Position;
            boss.LifeChange(Common.BossHit);
            base.Update(gameTime);
        }
    }
}
