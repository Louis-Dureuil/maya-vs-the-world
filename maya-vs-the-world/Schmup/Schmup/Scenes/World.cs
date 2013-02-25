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

        // Pour le test!
        private ShotPull shots;

        private double elapsed;

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

        public List<Enemy> Enemies
        {
            get
            {
                return enemies;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            Texture2D bullet2Texture = this.Content.Load<Texture2D>("bullet002-1");
            hero = new Hero(this.LuxGame, this, 200, 1, 1, 5, 2, null);
            Sprite heroSprite = new Sprite(hero, new List<String>() { "hero" });
            hero.Skin = heroSprite;
            hero.Position = new Vector2(400, 400);
            FastBoss boss = new FastBoss(this.LuxGame, 20000, 10, 10, null, bullet2Texture, this, 8);
            boss.Skin = new Sprite(boss, new List<string>() { "boss" });
            boss.Skin.SetAnimation("boss");
            boss.Position = new Vector2(400, 50);
            enemies.Add(boss);

            Game.Components.Add(boss);
            Game.Components.Add(hero);
            Game.Components.Add(heroSprite);

            // Phase de test
            shots = new ShotPull(LuxGame, this);
            Game.Components.Add(shots);

            heroSprite.SetAnimation("hero");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;

            // Gestion Heros -- Balles ennemies
            foreach (Shot badShot in badShots)
            {
                if (Vector2.Distance(badShot.Position, hero.Position) < badShot.Hitbox)
                {
                    hero.Hurt(badShot.Damage);
                    System.Console.Write("Tu t'es fait frapper. Il te reste ");
                    System.Console.Write(hero.Life);
                    System.Console.WriteLine(" points de vie.");

                    //TODO : Changer ça peut-être?
                    badShot.Position = new Vector2(-40, -40);
                    badShot.Speed = new Vector2(0, 0);
                    badShot.Accel = new Vector2(0, 0);
                }
            }

            // Gestion Ennemis -- Balles du heros
            foreach (Enemy enemy in enemies.ToList<Enemy>())
            {
                enemy.Update(gameTime);
                foreach (Shot goodShot in goodShots)
                {
                    if (Vector2.Distance(goodShot.Position, enemy.Position) < goodShot.Hitbox)
                    {
                        // L'ennemi a mal.
                        enemy.Hurt(goodShot.Damage);

                        // La balle disparait.
                        // TODO : Changer ça peut-être?
                        goodShot.Position = new Vector2(-40, -40);
                        goodShot.Speed = new Vector2(0, 0);
                        goodShot.Accel = new Vector2(0, 0);
                    }
                }
            }

            // Gestion Ennemis -- Heros
            foreach (Enemy enemy in enemies)
            {
                // TODO : Trouver une hitbox pour le héros? Comment faire?
                if (Vector2.Distance(hero.Position, enemy.Position) < 20)
                {
                    hero.Collide(enemy.GivenDamageCollision);
                    enemy.Collide(hero.GivenDamageCollision);
                    System.Console.WriteLine("Collision");
                }
            }

            //if (elapsed % 1 < 0.1)
            //{
            //    if (elapsed > 5)
            //    {
            //        shots.Check();
            //        elapsed = 0;
            //    }
            //    shots.Shoot(1,1,1,1,1,1,false,false,false);
            //    shots.Print();
            //}

            // Système de débug
            if (elapsed > 1000)
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
