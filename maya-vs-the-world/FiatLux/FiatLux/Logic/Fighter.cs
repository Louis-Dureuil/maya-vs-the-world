/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Text;
using LuxEngine;

namespace FiatLux.Logic
{
    public enum Direction
    {
        Up,
        Left,
        Right,
        Down,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    /// <summary>
    ///  A fighter is an abstract class that describe those who take part in battle.
    /// There are basically two types of Fighter: The Ennemy and the Characters.
    /// A Fighter owns an in-game spritesheet (in order to be displayed), fighting statistics, Actions/Reactions sets, and also a position on the battlefield.
    /// </summary>
    public abstract class Fighter
    {
        #region Consts
        const float DISTANCEPRECISION = 0.10F;
        

        #endregion
        float speed = 0.0F;
        float baseSpeed = 10.0F;

        public bool GodMode = false;

        public abstract bool HasCommand(int Index);
        public abstract bool CanUseCommand(int Index);

        public virtual float Speed
        {
            get { return baseSpeed; }
        }
        Vector2 position;
        Vector2? destination;
        Direction? direction;
        Vector2? oldDestination;

        public Sprite Sprite;

        public string StanceSpriteName;

        public string WoundCue;
        public string VeryWoundCue;
        public string DeathCue;
        public string AttackCue;
        public string WordCue;

        public Prop Stats = new Prop(Constants.STATNAMES, Constants.STATCONVNAMES, Constants.STATSHAVEMAX);

        public string faceName;

        bool striked = false;

        float stunDuration = 0.0F;
        
        public bool Unlimited
        {
            get { return unlimitedGauge >= this.Stats["End", true]; }
            set { unlimitedGauge = value ? this.Stats["End", true] : 0; }
        }

        public int unlimitedGauge;

        public int[] ActionCommands = new int[4]; // The ID of the action commands.
        public int[] ReactionCommands = new int[4]; // The ID of the reaction commands

        #region Properties

        #region Identity information

        public string Name { get; set; }

        #endregion

        public bool isStriked
        {
            get
            {
                if (destination != null && this.IsCloseTo(destination.Value,1.0F))
                    striked = false;
                return (striked && Position != destination);
            }
        }
        
        public bool isDead { get { return Stats["End"] == 0; } }
        public bool isStun
        {
            get { return stunDuration > 0; }
            set
            {
                if (!value) stunDuration = 0;
                else stunDuration = 10.0F;
            }
        }

        public Vector2 Position { get { return position; } set { position = value; } }

        #endregion

        public virtual void Update(float gameTime)
        {

            if (isStun)
            {
                this.Sprite.HasAfterImage = true;
                this.Sprite.AfterImageColor = Color.Red;
            }
            else if (Unlimited)
            {
                this.Sprite.HasAfterImage = true;
                this.Sprite.AfterImageColor = Color.LightYellow;
            }
            else
            {
                this.Sprite.HasAfterImage = false;
            }
            Vector2 oldPosition = position;
            UpdatePosition(gameTime);
            Sprite.Position = Position;
            if (isStun)
                stunDuration -= gameTime;
        }

        #region Method
        #region Movement

        void UpdatePosition(float gameTime)
        {
            if (destination != null)
            {
                if ((destination.Value - position).Length() >= DISTANCEPRECISION)
                    position += (destination.Value - position) * (1 / (destination.Value - position).Length()) * speed;
                // If path invalidated, pathfinding here.
            }
            else if (direction != null)
            {
                switch (direction)
                {
                    case Direction.Down:
                        position.Y += speed;
                        break;
                    case Direction.DownLeft:
                        position.X -= speed * (float)Math.Sqrt(2.0);
                        position.Y += speed * (float)Math.Sqrt(2.0);
                        break;
                    case Direction.DownRight:
                        position.X += speed * (float)Math.Sqrt(2.0);
                        position.Y += speed * (float)Math.Sqrt(2.0);
                        break;
                    case Direction.Left:
                        position.X -= speed;
                        break;
                    case Direction.Right:
                        position.X += speed;
                        break;
                    case Direction.Up:
                        position.Y -= speed;
                        break;
                    case Direction.UpLeft:
                        position.X -= speed * (float)Math.Sqrt(2.0);
                        position.Y -= speed * (float)Math.Sqrt(2.0);
                        break;
                    case Direction.UpRight:
                        position.X += speed * (float)Math.Sqrt(2.0);
                        position.Y -= speed * (float)Math.Sqrt(2.0);
                        break;
                }
            }
            else if (oldDestination != null)
            {
                destination = oldDestination;
                oldDestination = null;
                UpdatePosition(gameTime);
            }
        }

        public void Stop()
        {
            if (direction != null)
                direction = null;
            if (destination != null)
                destination = null;
            if (oldDestination != null)
                oldDestination = null;
        }

        // We need two different way to move:
        // The "blind" way, where a fighter only moves in a direction regardless of obstacles.
        // The "aware" way, where a figther searchs his path from his position to a given destination.
        // The move() method will take care of that.
        // If a destination is set, then the fighter will head to it.
        // Else, if a direction is set, the fighter will head towards this direction.

        /// <summary>
        /// If a destination is set, move towards this destination at given speed (handle the pathfinding/collision)
        /// Else, if a direction is set, move towards this direction at given speed (handle collisions).
        /// Else, move randomly at given speed (handle collisions).
        /// </summary>
        public void Move()
        {
            if (destination != null)
            {
                if ((destination.Value - position).Length() >= DISTANCEPRECISION)
                position += (destination.Value - position) * (1 / (destination.Value - position).Length()) * speed;
                // If path invalidated, pathfinding here.
            }
            else if (direction != null)
            {
                switch (direction)
                {
                    case Direction.Down:
                        position.Y += speed;
                        break;
                    case Direction.DownLeft:
                        position.X -= speed * (float)Math.Sqrt(2.0);
                        position.Y += speed * (float)Math.Sqrt(2.0);
                        break;
                    case Direction.DownRight:
                        position.X += speed * (float)Math.Sqrt(2.0);
                        position.Y += speed * (float)Math.Sqrt(2.0);
                        break;
                    case Direction.Left:
                        position.X -= speed;
                        break;
                    case Direction.Right:
                        position.X += speed;
                        break;
                    case Direction.Up:
                        position.Y -= speed;
                        break;
                    case Direction.UpLeft:
                        position.X -= speed * (float)Math.Sqrt(2.0);
                        position.Y -= speed * (float)Math.Sqrt(2.0);
                        break;
                    case Direction.UpRight:
                        position.X += speed * (float)Math.Sqrt(2.0);
                        position.Y -= speed * (float)Math.Sqrt(2.0);
                        break;
                }
            }
            else if (oldDestination != null)
            {
                destination = oldDestination;
                oldDestination = null;
                Move();
            }
            else
            {
                // random moves at given speed.
            }
            // Take care of the collision
        }

        
        /// <summary>
        /// If a destination is set, move towards this destination at given speed (handle the pathfinding/collision)
        /// Else, if a direction is set, move towards this direction at given speed (handle collisions).
        /// Else, move randomly at given speed (handle collisions).
        /// </summary>
        /// <param name="Speed">The speed to which move.</param>
        public void Move(float Speed)
        {
            // Triggers changing speed animation.
            speed = Speed;
            //Move();
        }

        /// <summary>
        /// Move towards the destination at given speed.
        /// </summary>
        /// <param name="Speed">The speed to which move.</param>
        /// <param name="Destination">The destination towards which move.</param>
        /// <param name="OverwriteDirection">Wheteher a possible previous Direction should be overwritten (if note overwritten, will be pursued after the move).</param>
        public void Move(float Speed, Vector2 Destination, bool OverwriteDirection)
        {
            destination = Destination;
            if (OverwriteDirection)
                direction = null;
            Move(Speed);
        }

        /// <summary>
        /// Move towards the direction at given speed.
        /// </summary>
        /// <param name="Speed">The speed to which move.</param>
        /// <param name="Direction">The direction towards which move</param>
        /// <param name="OverwriteDestination">Whether a possible previous Destination should be overwritten (if not overwritten, will be pursued after the move).</param>
        public void Move(float Speed, Direction Direction, bool OverwriteDestination)
        {
            direction = Direction;
            if (!OverwriteDestination)
                oldDestination = destination;
            destination = null;
            Move(Speed);
        }


        /// <summary>
        /// Indicates whether the fighter is in the circle of center Destination and radius MaximalDistance
        /// </summary>
        /// <param name="Destination">The Destination to which we want to be close to.</param>
        /// <param name="MaximumDistance">The maximum allowed distance</param>
        /// <returns>Whether the fighter is close enough of the destination</returns>
        internal bool IsCloseTo(Vector2 Destination, float MaximumDistance)
        {
            return (Destination - Position).Length()
                <= MaximumDistance + speed;
        }

        public float Distance(Vector2 Destination)
        {
            return (Destination - Position).Length();
        }

        internal void Stun(float duration)
        {
            if (duration >= 0)
                stunDuration = (stunDuration + duration) / 2;
            else if (stunDuration >= 0)
                stunDuration += duration;
        }

        internal void Strike(Vector2 Direction, float Distance, float StrikeSpeed)
        {
            Vector2 NormalizedDestination = Position - Direction;
            NormalizedDestination.Normalize();
            striked = true;
            Move(StrikeSpeed, position + NormalizedDestination * Distance, true);
        }
        #endregion

        /// <summary>
        /// Get a copy of the fighter.
        /// </summary>
        /// <param name="order">a string to append to the fighter's name</param>
        public virtual Fighter GetCopy(string order)
        {
            Fighter Copy = (Fighter)this.MemberwiseClone();
            Copy.Stats = new Prop(Constants.STATNAMES, Constants.STATCONVNAMES, Constants.STATSHAVEMAX);
            for (int i = 0; i < Constants.STATNUMBER; i++)
            {
                Copy.Stats[i, true] = this.Stats[i, true];
                Copy.Stats[i] = this.Stats[i];
            }
            if (!string.IsNullOrEmpty(order))
            {
                Copy.Name += " " + order;
            }
            return Copy;
        }
        #endregion
    }
}
