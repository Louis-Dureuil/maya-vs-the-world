using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LuxEngine.Commands
{
    public class SayCommand : Command
    {
        private string message;
        private TimeSpan elapsedTime;
        private StringBuilder sb;
        private int index;
        private TextWindow tw;

        // number of letters per second.
        private const float displaySpeed = 4.2F;

        public SayCommand(Scene parent, string message) :
            base(parent)
        {
            this.message = message;
            elapsedTime = new TimeSpan();
            tw = new TextWindow(this, new Rectangle(100, 100, 300, 100));
            sb = new StringBuilder();
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Game.Components.Add(tw);
        }

        public override bool IsFinished
        {
            get
            {
                if (Input.isActionDone(Input.Action.Confirm, false))
                {
                    Input.CancelInput();
                    if (index == message.Length)
                    {
                        return true;
                    }
                    else
                    {
                        index = message.Length;
                        sb.Clear();
                        sb.Append(message);
                        return false;
                    }
                }
                return false;
            }
        }

        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            base.OnVisibleChanged(sender, args);
            tw.Visible = this.Visible;
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime = elapsedTime.Add(gameTime.ElapsedGameTime);
            int letterCount = Math.Min(message.Length, (int)(elapsedTime.TotalSeconds * displaySpeed));
            while (index < letterCount)
            {
                sb.Append(message[index]);
                index++;
            }   
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            tw.Text = sb.ToString();
        }
    }
}
