/* LuxEngine V0.1.12
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LuxEngine
{
    /// <summary>
    /// A generic graphical component that has a parent and children.
    /// Components that have hierarchical relations should inherits this class.
    /// Inherits from DrawableGameComponent.
    /// </summary>
    public class Scene : DrawableGameComponent
    {
        public delegate void PausedEventHandler(object sender, EventArgs e);

        public delegate void UnPausedEventHandler(object sender, EventArgs e);

        public event PausedEventHandler Paused;
        public event UnPausedEventHandler UnPaused;

        
        protected List<Scene> subScenes = new List<Scene>();



        public Vector2 Position;

        bool resumeEnabled;

        bool paused;

        /// <summary>
        /// Get whether the scene is paused or not.
        /// </summary>
        public bool isPaused { get { return paused; } }

        /// <summary>
        /// Toogle the paused state of the scene.
        /// </summary>
        public void TooglePause(Scene sender, EventArgs e)
        {
            if (paused)
                UnPause(sender, e);
            else
                Pause(sender, e);
        }

        /// <summary>
        /// Pause the game, then fire the Paused event.
        /// </summary>
        public void Pause(Scene sender, EventArgs e)
        {
            paused = true;
            if (Paused != null)
                Paused(sender, e);
            foreach (Scene subScene in subScenes)
                subScene.Pause(this, e);
        }

        /// <summary>
        /// unpause the game, then fire the Unpaused event.
        /// </summary>
        public void UnPause(Scene sender, EventArgs e)
        {
            paused = false;
            if (UnPaused != null)
                UnPaused(sender, e);
            foreach (Scene subScene in subScenes)
                subScene.UnPause(this, e);
        }

        protected Vector2 positionWithParent
        {
            get { return this.Position + ((parent != null) ? parent.positionWithParent : Vector2.Zero); }
        }

        protected Scene parent;

        public Scene Parent
        {
            get { return parent; }
            set
            {
                if (parent != null)
                    parent.Remove(this);
                parent = value;
                parent.Add(this);
                Game.Components.Add(this);
            }
        }

        protected SpriteFont spriteFont;

        protected ContentManager Content;
        
        public LuxGame LuxGame
        {
            get { return (LuxGame)Game; }
        }

        public Scene this[int index]
        {
            get
            {
                return subScenes[index];
            }
            set
            {
                subScenes[index] = value;
            }
        }

        public Scene(LuxGame game)
            : base(game)
        {
            Content = new ContentManager(game.Services,"Content");
            //this.Game.Components.Add(this);
        }

        public Scene(Scene parent)
            : base(parent.Game)
        {
            Content = new ContentManager(parent.Game.Services,"Content");
            this.parent = parent;
            this.Enabled = parent.Enabled;
            this.Visible = parent.Visible;
            this.DrawOrder = parent != null ? parent.DrawOrder + 1 : 0;
            this.parent.Add(this);
        }

        public override void Initialize()
        {   
            this.EnabledChanged += new EventHandler<EventArgs>(Scene_EnabledChanged);
            this.VisibleChanged += new EventHandler<EventArgs>(Scene_VisibleChanged);
            //this.DrawOrder = parent != null ? parent.DrawOrder+1 : 0;
            base.Initialize();
        }

        void Scene_VisibleChanged(object sender, EventArgs e)
        {
            foreach (Scene subScene in subScenes)
            {
                subScene.Visible = Visible;
            }
        }

        void Scene_EnabledChanged(object sender, EventArgs e)
        {
            foreach (Scene subScene in subScenes)
            {
                subScene.Enabled = Enabled;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public void Add(Scene item)
        {
            item.Enabled = this.Enabled;
            item.Visible = this.Visible;
            subScenes.Add(item);
            //Game.Components.Add(item);
        }

        public void Remove(Scene item)
        {
            subScenes.Remove(item);
            Game.Components.Remove(item);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public virtual void Destroy()
        {
            for (int i=0; i< subScenes.Count; i++)
            {
                this[i].Destroy();
            }
            subScenes.Clear();
            Content.Dispose();
            Game.Components.Remove(this);
        }
    }
}
