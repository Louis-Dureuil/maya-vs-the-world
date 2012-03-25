/* LuxEngine V0.2.20
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace LuxEngine
{
    public class MenuWindowEventArgs : EventArgs
    {
        int index;
   
        string choice;
        public string Choice { get { return choice; } set { choice = value; } }
        public int Index { get { return index; } set { index = value; } }
        public MenuWindowEventArgs(int index)
        {
            this.index = index;
        }
        public MenuWindowEventArgs(int index, string choice)
        {
            this.index = index;
            this.choice = choice;
        }
    }

    /// <summary>
    /// A Window that handles basic selection menus. Inherits from TextWindow.
    /// </summary>
    public class MenuWindow : TextWindow
    {
        Cue MenuSound;

        public Input.Action ConfirmAction = Input.Action.Confirm;
        public Input.Action CancelAction = Input.Action.Cancel;

        public delegate void ChoiceConfirmedEventHandler(object sender, MenuWindowEventArgs e);
        public delegate void ChoiceCanceled(object sender, MenuWindowEventArgs e); 
        public event ChoiceConfirmedEventHandler Confirmed;
        public event ChoiceConfirmedEventHandler Canceled;
        int index = 0;
        public int Index { get { return index; } set { if (SelectedLines.Count > 0) { SelectedLines[index] = false; } index = value; if (SelectedLines.Count > 0) { SelectedLines[index] = true; } } }
        public MenuWindow(Scene parent, Vector2 Position,string Text)
            : base(parent, Position)
        {
            this.Text = Text;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            Index = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.textByLines.Count > 0)
            {
                index = index % textByLines.Count;
            }

            HandleInput();
            base.Update(gameTime);
        }

        void HandleInput()
        {
            if (SelectedLines.Count > index)
            {
                if (Input.isActionDone(ConfirmAction, false))
                {
                    if (!DisabledLines[Index])
                    {
                        Input.CancelInput();
                        MenuWindowEventArgs e = new MenuWindowEventArgs(Index);
                        e.Choice = textByLines[index];
                        MenuSound = LuxGame.soundBank.GetCue("Confirm");
                        MenuSound.Play();
                        if (Confirmed != null)
                            Confirmed(this, e);
                    }
                    else
                    {
                        MenuSound = LuxGame.soundBank.GetCue("ConfirmDisabled");
                        MenuSound.Play();
                    }
                }
               
                if (Input.isActionDone(Input.Action.Down, false))
                {
                    Index = (Index + 1) % SelectedLines.Count;
                    MenuSound = LuxGame.soundBank.GetCue("Select");
                    MenuSound.Play();
                }
                if (Input.isActionDone(Input.Action.Up, false))
                {
                    if (Index == 0)                    
                        Index = SelectedLines.Count - 1;
                    else
                    Index--;
                    MenuSound = LuxGame.soundBank.GetCue("Select");
                    MenuSound.Play();
                }
                if (Input.isActionDone(CancelAction, false))
                {
                    Input.CancelInput();
                    MenuWindowEventArgs e = new MenuWindowEventArgs(Index);
                    if (Canceled != null)
                        Canceled(this, e);
                }
            }
        }
    }
}
