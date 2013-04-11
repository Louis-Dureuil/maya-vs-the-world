using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class ShootsHero : ShootingPattern
    {
        private World world;
        private double elapsedSec;
        private Vector2 direction;

        public ShootsHero(LuxGame game, World world, ShotPool shots, Enemy enemy)
            : base(game, shots, enemy)
        {
            elapsedSec = 0;
            this.world = world;
        }

        public void Associate(Enemy enemy)
        {
            this.enemy = enemy;
            elapsedSec = 0;
        }

        public Enemy Enemy
        {
            get
            {
                return enemy;
            }
        }

        public override void Update(GameTime gameTime)
        {
            elapsedSec += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedSec > 0.3 && enemy.Life > 0 && Vector2.Distance(enemy.Position,world.Hero.Position) > 50)
            {
                elapsedSec = 0;
                direction = 4 * Vector2.Normalize(world.Hero.Position - enemy.Position);
                shots.Shoot(enemy.Position.X, enemy.Position.Y, direction.X, direction.Y, 0, 0, false, false, false);
            }
            if (enemy.Life <= 0 && elapsedSec > 0)
            {
                elapsedSec = -100;
                direction = 4 * Vector2.Normalize(world.Hero.Position - enemy.Position);
                shots.Shoot(enemy.Position.X, enemy.Position.Y, 3*direction.X, 3*direction.Y, 0, 0, false, false, false);
            }
            if (enemy.Life > 0 && elapsedSec < 0)
            {
                elapsedSec = -100;
            }
            base.Update(gameTime);
        }
    }
}
