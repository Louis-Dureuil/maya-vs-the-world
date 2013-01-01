using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class World : Scene
    {
        private Hero hero;
        private List<Enemy> enemies = new List<Enemy>();
        private List<Shot> badShots = new List<Shot>();
        private List<Shot> goodShots = new List<Shot>();

        private double elapsed;

        public World(LuxGame game, Hero hero, List<Enemy> enemies, List<Shot> badShots, List<Shot> goodShots)
            :base(game)
        {
            this.hero = hero;
            this.enemies = enemies;
            this.badShots = badShots;
            this.goodShots = goodShots;
        }

        public World(LuxGame game)
            : base(game)
        {
        }

        public Hero Hero
        {
            get
            {
                return hero;
            }
        }

        public List<Shot> BadShots
        {
            get
            {
                return badShots;
            }
        }

        public List<Shot> GoodShots
        {
            get
            {
                return goodShots;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            //List<string> skinName = new List<string>(4);
            //Texture2D bullet1Texture = this.Content.Load<Texture2D>("bullet001-1");
            //Texture2D bullet2Texture = this.Content.Load<Texture2D>("bullet002-1");
            //Texture2D enemyTexture = this.Content.Load<Texture2D>("commonEnemy");
            //skinName.Add("carre");
            //skinName.Add("bullet001-1");
            //skinName.Add("bullet001-2");
            //skinName.Add("hero");
            hero = new Hero(this.LuxGame, this, 10, 1, 1, 5, 2, null);
            Sprite heroSprite = new Sprite(hero, new List<String>() { "hero" });
            hero.Skin = heroSprite;
            hero.Position = new Vector2(400, 400);
            //FastBoss boss = new FastBoss(this.LuxGame, 1000, 10, 10, null, bullet2Texture, this, 8);
            //boss.Skin = new Sprite(boss, new List<string>() { "boss" });
            //boss.Skin.SetAnimation("boss");
            //boss.Position = new Vector2(400, 50);
            //this.enemies.Add(boss);

            //Game.Components.Add(boss);
            Game.Components.Add(hero);
            Game.Components.Add(heroSprite);

            heroSprite.SetAnimation("hero");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed > 1)
            {
                elapsed = 0;
                System.Console.WriteLine(hero.Position);
                System.Console.WriteLine(badShots.Count);
                System.Console.WriteLine(goodShots.Count);
                System.Console.WriteLine(enemies.Count);
                foreach (Shot badShot in badShots)
                {
                    System.Console.WriteLine(badShot.Position);
                }
                foreach (Shot goodShot in goodShots)
                {
                    System.Console.WriteLine(goodShot.Position);
                }
                foreach (Enemy enemy in enemies)
                {
                    System.Console.WriteLine(enemy.Position);
                }
            }
        }
    }
}
