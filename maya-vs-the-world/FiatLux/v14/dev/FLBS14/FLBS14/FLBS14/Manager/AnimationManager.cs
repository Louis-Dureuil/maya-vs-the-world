using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLBS14.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FLBS14.Manager
{
    public class AnimationManager
    {
        #region private
        private List<Animation> animations = new List<Animation>();
        #endregion

        public void Update(GameTime gameTime)
        {
            foreach (Animation animation in animations)
            {
                animation.Update(gameTime);
            }
        }

        public void Load(Animation animation)
        {
            animations.Add(animation);
        }

    }
}
