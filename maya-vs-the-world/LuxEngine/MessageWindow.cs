/* LuxEngine V0.2.20
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    /// <summary>
    /// Indicates the way the message should be destroyed.
    /// </summary>
    public enum MessageConfirmType
    {
        /// <summary>
        /// The message will be displayed until it is manually destroyed.
        /// </summary>
        None,
        /// <summary>
        /// The message will be displayed until the player presses the confirm button.
        /// </summary>
        OnConfirm,
        /// <summary>
        /// The message will be displayed for a set amount of time, then automatically destroyed.
        /// </summary>
        Timed,
        /// <summary>
        /// The message will be displayed for a set amount of time the automatically destroyed, 
        /// and can also be destroyed by the player (by pressing the confirm button).
        /// </summary>
        OnConfirmAndTimed
    }

    public class MessageWindowEventArgs : EventArgs
    {
        public string message = "";
        public MessageConfirmType destroyedBy;
        public MessageWindowEventArgs(string message, MessageConfirmType destroyer)
        {
            this.message = message;
            destroyedBy = destroyer;
        }
    }

    /// <summary>
    /// A window that handles static messages. Inherits from TextWindow. 
    /// </summary>
    public class MessageWindow : TextWindow
    {
        public delegate void OnDestroy(object sender, MessageWindowEventArgs e);

        public event OnDestroy Destroying;

        MessageWindowEventArgs e;

        public Input.Action ConfirmAction = Input.Action.Confirm;

        public MessageConfirmType destroyType;
        float LifeTime = Shared.MESSAGELIFETIME;
        public MessageWindow(Scene parent, Vector2 Position, string Message, MessageConfirmType destroyType)
            : base(parent, Position)
        {
            this.destroyType = destroyType;
            base.Text = Message;
            e = new MessageWindowEventArgs(Message, destroyType); 
            Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (destroyType == MessageConfirmType.OnConfirmAndTimed || destroyType == MessageConfirmType.Timed)
            {
                //Frame duration, in s (precision 10^(-3) s.)
                float FrameDuration = Shared.FrameDuration;
           
                LifeTime -= FrameDuration;
                if (LifeTime <= 0)
                {
                    if (Destroying != null)
                    {
                        e.message = this.Text;
                        e.destroyedBy = MessageConfirmType.Timed;
                        Destroying(this, e);
                    }
                    this.Visible = false;
                    this.Enabled = false;
                    LifeTime = Shared.MESSAGELIFETIME;
                }
            }

            if (destroyType == MessageConfirmType.OnConfirm || destroyType == MessageConfirmType.OnConfirmAndTimed)
            {
                if (Input.isActionDone(ConfirmAction, false))
                {
                    if (Destroying != null)
                    {
                        e.message = this.Text;
                        e.destroyedBy = MessageConfirmType.OnConfirm;
                        Destroying(this, e);
                    }
                    this.Visible = false;
                    this.Enabled = false;
                    
                }
            }
            base.Update(gameTime);
        }
    }
}
