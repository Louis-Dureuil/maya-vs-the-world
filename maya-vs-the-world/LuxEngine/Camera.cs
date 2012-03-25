using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class Camera : Scene
    {
        const float SCROLLINGSTART = 30.0F;
        const float SCROLLINGSTOP = 35.0F;
        float zoom = 1.0F;
        public float ScrollingSpeed = 2.0F * 60;
        public Vector2 ZoomOrigin;
        bool isScrolling = false;
        
        ScrollingAxisX sax;
        ScrollingAxisY say;

        public CameraMode Mode;
        List<int> targets = new List<int>();

        public float Zoom
        {
            get { return zoom; }
            set
            {
                float diff = value - zoom;
                float quot = value / zoom;
                foreach (Scene scene in subScenes)
                {
                    scene.Position = (scene.Position - ZoomOrigin) * quot * new Vector2(((Sprite)scene).Width,((Sprite)scene).Height) + ZoomOrigin;
                    ((Sprite)scene).Width *= quot;
                    ((Sprite)scene).Height *= quot;
                }
            }
        }

        public Camera(Scene parent, Vector2 position)
            : base(parent)
        {
            this.Position = position;
            ZoomOrigin = new Vector2(parent.Game.GraphicsDevice.Viewport.Width / 2, parent.Game.GraphicsDevice.Viewport.Height / 2);
        }
        
        public override void Update(GameTime gameTime)
        {
            if (Mode == CameraMode.FocusOnSingle && targets.Count > 0)
                UpdateScrolling(gameTime, targets[0]);
            else if (Mode == CameraMode.Resize)
            {
                foreach (int index in targets)
                {
                    UpdateScrolling(gameTime, index);
                }
            }
            base.Update(gameTime);
        }

        public void AddTarget(Scene target)
        {
            try
            {
                int i = subScenes.IndexOf(target);
                if (!targets.Contains(i))
                    targets.Add(i);
            }
            catch
            {
                throw new Exception("The requested target is not attached to the camera!");
            }
        }

        public void RemoveTarget(Scene target)
        {
            try
            {
                int i = subScenes.IndexOf(target);
                if (targets.Contains(i))
                    targets.Remove(i);
            }
            catch
            {
                throw new Exception("The requested target is not attached to the camera!");
            }
        }

        public void ClearTargets()
        {
            targets.Clear();
        }

        private void UpdateScrolling(GameTime gameTime, int targetIndex)
        {
            if (targetIndex < 0 || targetIndex > subScenes.Count)
            {
                return;
            }
            //Frame duration, in s (precision 10^(-3) s.)
            float FrameDuration = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;
            Scene target = subScenes[targetIndex]; 
        
            if (this.Position.X + target.Position.X < SCROLLINGSTART*GraphicsDevice.Viewport.Width / 100.0F
                || (sax == ScrollingAxisX.Right && this.Position.X + target.Position.X < SCROLLINGSTOP * GraphicsDevice.Viewport.Width / 100.0F))
            {
                isScrolling = true;
                sax = ScrollingAxisX.Right;
                this.Position.X += ScrollingSpeed * FrameDuration;
            }
            else if (this.Position.X + target.Position.X > (100.0F - SCROLLINGSTART) * GraphicsDevice.Viewport.Width / 100.0F
                || (sax == ScrollingAxisX.Left && this.Position.X + target.Position.X > (100.0F - SCROLLINGSTOP) * GraphicsDevice.Viewport.Width / 100.0F))
            {
                isScrolling = true;
                sax = ScrollingAxisX.Left;
                this.Position.X -= ScrollingSpeed * FrameDuration;
            }
            else
            {
                sax = ScrollingAxisX.None;
            }
            if (this.Position.Y + target.Position.Y > (100.0F - SCROLLINGSTART) * GraphicsDevice.Viewport.Height / 100.0F
                || (say == ScrollingAxisY.Up && this.Position.Y + target.Position.Y > (100.0F - SCROLLINGSTOP) * GraphicsDevice.Viewport.Height / 100.0F))
            {
                isScrolling = true;
                say = ScrollingAxisY.Up;
                this.Position.Y -= ScrollingSpeed * FrameDuration;
            }
            else if (this.Position.Y + target.Position.Y < (SCROLLINGSTART) * GraphicsDevice.Viewport.Height / 100.0F
                || (say == ScrollingAxisY.Down && this.Position.Y + target.Position.Y < SCROLLINGSTOP * GraphicsDevice.Viewport.Height / 100.0F))
            {
                isScrolling = true;
                say = ScrollingAxisY.Down;
                this.Position.Y += ScrollingSpeed * FrameDuration;
            }
            else
            {
                say = ScrollingAxisY.None;
            }

            if (say == ScrollingAxisY.None && sax == ScrollingAxisX.None)
            {
                isScrolling = false;
            }

        }

        
    }

    /// <summary>
    /// Enumerate the possible scrolling horizontal directions.
    /// </summary>
    public enum ScrollingAxisX
    {
        None,
        Left,
        Right
    }

    public enum ScrollingAxisY
    {
        None,
        Up,
        Down
    }

    /// <summary>
    /// Enumerate the camera modes.
    /// When FocusOnSingle or Resize, the targets are updated using the Add* and RemoveTarget methods.
    /// </summary>
    public enum CameraMode
    {
        /// <summary>
        /// The camera will remain stopped unless manually scrolled.
        /// </summary>
        NoScrolling,
        /// <summary>
        /// The camera will follow a single target with srolling.
        /// </summary>
        FocusOnSingle,
        /// <summary>
        /// The camera will follow muliple targets with scrolling and zooming.
        /// </summary>
        Resize
    }
}
