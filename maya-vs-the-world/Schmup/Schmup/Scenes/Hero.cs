using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{

    class Hero : Character
    {
        private int mana;

        // Besoin d'un marqueur pour donner l'arme courante du héros,
        // sa vitesse et son arme secondaire (le pouvoir)
        private int currentWeapon;
        private int currentSpeed;
        private int currentPower;

        // Besoin de la liste d'armes dont le héros dispose
        private List<int> weaponList;
        private List<int> speedList;
        private List<int> powerList;

        // TODO : Améliorer le système de gestion des vitesses
        // Pour l'instant, le héros dispose de deux vitesses
        private int speed1;
        private int speed2;
        private bool speedType;

        private World world;

        public bool SpeedType
        {
            get
            {
                return speedType;
            }
            set
            {
                speedType = value;
            }
        }

        public Hero(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, int speed1, int speed2, Sprite skin = null)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.speed1 = speed1;
            this.speed2 = speed2;
            this.currentSpeed = speed1;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void WeakShoot()
        {
            return;
        }

        public void StrongShoot()
        {
            return;
        }

        public void PowerShoot()
        {
            return;
        }

        public override void Update(GameTime gameTime)
        {

            // Gestion du changement de vitesse
            // TODO : A améliorer! Celui qui a fait ça doit être en train de consommer du LSD.
            bool speedChange;

            speedChange = Input.isActionDone(Input.Action.SpeedChange, false);
            if (speedChange)
            {
                if (speedType)
                {
                    speedType = false;
                    currentSpeed = speed1;
                }
                else
                {
                    speedType = true;
                    currentSpeed = speed2;
                }
            }

            // Gestion du déplacement
            if (Input.isActionDone(Input.Action.Up,true))
            {
                if (Input.isActionDone(Input.Action.Right, true))
                {
                    Position.X += currentSpeed / (float) Math.Sqrt(2);
                    Position.Y -= currentSpeed / (float) Math.Sqrt(2);
                }
                else if (Input.isActionDone(Input.Action.Left, true))
                {
                    Position.X -= currentSpeed / (float) Math.Sqrt(2);
                    Position.Y -= currentSpeed / (float) Math.Sqrt(2);
                }
                else
                {
                    Position.Y -= currentSpeed;
                }
            }
            else if (Input.isActionDone(Input.Action.Down, true))
            {
                if (Input.isActionDone(Input.Action.Right, true))
                {
                    Position.X += currentSpeed / (float) Math.Sqrt(2);
                    Position.Y += currentSpeed / (float) Math.Sqrt(2);
                }
                else if (Input.isActionDone(Input.Action.Left, true))
                {
                    Position.X -= currentSpeed / (float) Math.Sqrt(2);
                    Position.Y += currentSpeed / (float) Math.Sqrt(2);
                }
                else
                {
                    Position.Y += currentSpeed;
                }
            }
            else if (Input.isActionDone(Input.Action.Left, true))
            {
                Position.X -= currentSpeed;
            }
            else if (Input.isActionDone(Input.Action.Right, true))
            {
                Position.X += currentSpeed;
            }

            // Gestion des bordures
            // TODO : Mettre des variables globales

            if (Position.X < 0)
            {
                Position.X = 0;
            }
            if (Position.Y < 0)
            {
                Position.Y = 0;
            }
            if (Position.X > 800)
            {
                Position.X = 800;
            }
            if (Position.Y > 480)
            {
                Position.Y = 480;
            }

            // Gestion des tirs
            if (Input.isActionDone(Input.Action.Shoot, true))
            {
                if (speedType)
                {
                    WeakShoot();
                }
                else
                {
                    StrongShoot();
                }
            }

            // Gestion des pouvoirs

            if (Input.isActionDone(Input.Action.Power, false))
            {
                PowerShoot();
            }

            // Mise à jour de la position pour les ennemis
            //world.Hero.Position = this.Position;

            // Possibilité de sommaire
            if (Input.isActionDone(Input.Action.Confirm, false))
            {
                System.Console.Write("Charge tirée ");
                System.Console.Write(0);
                System.Console.WriteLine(" fois");
            }

            base.Update(gameTime);
        }
    }
}
