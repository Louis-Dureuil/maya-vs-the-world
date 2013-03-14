using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace FLBS14.Primitives
{
    public class Sprite
    {
        #region private
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        private Manager.SpriteManager manager;
        private Animation currentAnimation;

        #region dest
        double x;
        double y;
        double w;
        double h;
        float z;
        float angle;
        Rectangle destRectangle = new Rectangle();
        Color color = Color.White;
        Vector2 origin;
        SpriteEffects effect;
        #endregion
        #endregion

        public Dictionary<string, Animation> Animations { get { return animations; } }

        #region dest
        public double X
        {
            get { return x; }
            set { x = value;
            destRectangle.X = (int)value;
            }
        }

        public double Y
        {
            get { return y; }
            set { y = value; 
            destRectangle.Y = (int)value;
            }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public double W
        {
            get { return w; }
            set { w = value;
            destRectangle.Width = (int)value;
            }
        }

        public double H
        {
            get { return h; }
            set { h = value; 
            destRectangle.Height = (int)value;
            }
        }

        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public float OriginX
        {
            get { return origin.X; }
            set { origin.X = value; }
        }

        public float OriginY
        {
            get { return origin.Y; }
            set { origin.Y = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
        }

        public byte R { get { return color.R; } set { color.R = value; } }
        public byte G { get { return color.G; } set { color.G = value; } }
        public byte B { get { return color.B; } set { color.B = value; } }
        public byte A { get { return color.A; } set { color.A = value; } }
        public Color Color { get { return color; } }

        public SpriteEffects Effect { get { return effect; } set { effect = value; } }

        public Rectangle DestRectangle { get { return destRectangle; } }
        #endregion

        public Animation CurrentAnimation
        {
            get { return currentAnimation; }
        }

        public void SetCurrent(string animationName)
        {
            if (animations.ContainsKey(animationName))
                currentAnimation = Animations[animationName];
            
        }

        public Sprite(Manager.SpriteManager manager)
        {
            manager.Load(this);
            this.manager = manager;
        }

        public void LoadAsynchronously(Data.Sprite baseSprite, Manager.AnimationManager animationManager, Manager.ImageManager imageManager)
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                Angle = baseSprite.Angle;
                if (baseSprite.IsHorizontallyMirrored)
                {
                    if (baseSprite.IsVerticallyMirrored)
                    {
                        Angle += (float)Math.PI;
                        effect = SpriteEffects.None;
                    }
                    else
                    {
                        effect = SpriteEffects.FlipHorizontally;
                    }
                }
                else if (baseSprite.IsVerticallyMirrored)
                {
                    effect = SpriteEffects.FlipVertically;
                }
                else
                {
                    effect = SpriteEffects.None;
                }

                A = baseSprite.A;
                B = baseSprite.B;
                G = baseSprite.G;
                R = baseSprite.R;
                OriginX = baseSprite.OriginX;
                OriginY = baseSprite.OriginY;

                X = baseSprite.X;
                Y = baseSprite.Y;
                Z = baseSprite.Z;
                W = baseSprite.W;
                H = baseSprite.H;

                foreach (KeyValuePair<string, string> baseAnimation in baseSprite.Animations)
                {
                    string key = baseAnimation.Value;

                    Data.Animation animation = Data.DataManager.Load<Data.Animation>(baseAnimation.Key);
                    animations[key] = new Animation(animationManager, animation, imageManager);
                }

            });
        }

        public Sprite(Manager.SpriteManager manager, Data.Sprite baseSprite, Manager.AnimationManager animationManager,
            Manager.ImageManager imageManager)
        {
            manager.Load(this);
            this.manager = manager;

            A = baseSprite.A;
            B = baseSprite.B;
            G = baseSprite.G;
            R = baseSprite.R;
            OriginX = baseSprite.OriginX;
            OriginY = baseSprite.OriginY;

            X = baseSprite.X;
            Y = baseSprite.Y;
            Z = baseSprite.Z;
            W = baseSprite.W;
            H = baseSprite.H;

            foreach (KeyValuePair<string, string> baseAnimation in baseSprite.Animations)
            {
                string key = baseAnimation.Value;

                Data.Animation animation = Data.DataManager.Load<Data.Animation>(baseAnimation.Key);
                animations[key] = new Animation(animationManager, animation, imageManager);
            }
        }
    }
}
