/* LuxEngine < V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace LuxEngine 
{
    /// <summary>
    /// Static class that handles Input.
    /// Provide abstraction for entry method (keyboard or gamepad),
    /// buffers, and key binding.
    /// </summary>
    static public class Input
    {
        class Buffer
        {
            float MaxLifeTime;
            int MaxSize;
            int CurrentButtonSize;
            int CurrentKeySize;
         
            Buttons[] buttons;
            float[] buttonsLifeTime;

            Keys[] keys;
            float[] keysLifeTime;

            bool[] isButtonActive;
            bool[] isKeyActive;

            internal Buffer(int MaxSize, float MaxLifeTime)
            {
                this.MaxSize = MaxSize;
                this.MaxLifeTime = MaxLifeTime;
                CurrentButtonSize = 0;
                CurrentKeySize = 0;
                buttons = new Buttons[MaxSize];
                isButtonActive = new bool[MaxSize];
                isKeyActive = new bool[MaxSize];

                buttonsLifeTime = new float[MaxSize];

                keys = new Keys[MaxSize];
                keysLifeTime = new float[MaxSize];
            }

            internal void Add(Buttons button, bool allowCopy)
            {
                if (allowCopy)
                {
                    buttons[CurrentButtonSize] = button;
                    buttonsLifeTime[CurrentButtonSize] = MaxLifeTime;
                    CurrentButtonSize = (CurrentButtonSize + 1) % MaxSize;
                }
                else
                {
                    bool isNew = true;
                    for (int i = 0; i < MaxSize; i++)
                    {
                        if (buttons[i] == button && buttonsLifeTime[i] > 0)
                        {
                            isButtonActive[i] = true;
                            isNew = false;
                        }
                    }
                    if (isNew)
                        Add(button, true);
                }
            }

            internal void Add(Keys key, bool allowCopy)
            {
                if (allowCopy)
                {
                    keys[CurrentKeySize] = key;
                    keysLifeTime[CurrentKeySize] = MaxLifeTime;
                    CurrentKeySize = (CurrentKeySize + 1) % MaxSize;
                    if (CurrentKeySize == 0 && keysLifeTime[0] > 0.0F)
                        throw new Exception("Buffer size overwhelmed!");
                }
                else
                {
                    bool isNew = true;
                    for (int i = 0; i < MaxSize; i++)
                    {
                        if (keys[i] == key && keysLifeTime[i] > 0)
                        {
                            isKeyActive[i] = true;
                            isNew = false;
                        }
                    }
                    if (isNew)
                        Add(key, true); 
                }
            }

            internal void Remove(Keys key)
            {
                for (int i = 0; i < MaxSize; i++)
                    if (keysLifeTime[i] > 0 && keys[i] == key)
                    {
                        keysLifeTime[i] = 0;
                    }
            }

            internal void Remove(Buttons button)
            {
                for (int i = 0; i < MaxSize; i++)
                    if (buttonsLifeTime[i] > 0 && buttons[i] == button)
                    {
                        buttonsLifeTime[i] = 0;
                    }
            }

            internal void Update(GameTime gameTime)
            {
                //Frame duration, in s (precision 10^(-3) s.)
                float FrameDuration = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;

                for (int i = 0; i < MaxSize; i++)
                {
                    if (buttonsLifeTime[i] > 0)
                        buttonsLifeTime[i] -= FrameDuration;
                    if (keysLifeTime[i] > 0)
                        keysLifeTime[i] -= FrameDuration;
                    isButtonActive[i] = gamePadState.IsButtonDown(buttons[i]);
                    isKeyActive[i] = keyboardState.IsKeyDown(keys[i]);
                } 
            }

            internal void CancelInput()
            {
                for (int i = 0; i < MaxSize; i++)
                {
                    isKeyActive[i] = keyboardState.IsKeyDown(keys[i]);
                    isButtonActive[i] = gamePadState.IsButtonDown(buttons[i]);
                }
            }

            internal bool Contains(Keys key)
            {
                for (int i = 0; i < MaxSize; i++)
                    if (keysLifeTime[i] > 0 && keys[i] == key)
                    {
                        return true;
                    }
                return false;
            }

            internal bool Contains(Buttons button)
            {
                for (int i = 0; i < MaxSize; i++)
                    if (buttonsLifeTime[i] > 0 && buttons[i] == button)
                    {
                      return true;
                    }
                return false;
            }

            internal bool ContainsActive(Buttons button)
            {
                for (int i = 0; i < MaxSize; i++)
                    if (buttons[i] == button && buttonsLifeTime[i] > 0 && isButtonActive[i])
                        {
                            return true;
                        }
                return false;
            
            }

            internal bool ContainsActive(Keys key)
            {
                for (int i = 0; i < MaxSize; i++)
                    if (keys[i] == key && keysLifeTime[i] > 0 && isKeyActive[i])
                    {
                     return true;
                    }
                return false;
           
            }
        }

        public enum Action
        {
            BattleMenuUp, // Go up in the battle menu.  
            BattleMenuLeft, // Go left in the battle menu.
            BattleMenuRight, // Go right in the battle menu.
            BattleMenuDown, // Go down in the battle menu.
            BattleConfirm, // Confirm an action in the battle menu.
            BattleSoftCancel, // Go to upper level in the battle menu.
            BattleCancel, // Go to root menu in the battle menu.
            Up, // Go up on explorations and menus.
            Left, // Go left on exploration and menus.
            Right, // Go right on exploration and menus.
            Down, // Go down on exploration and menus.
            Confirm, // Confirm a decision in menus.
            Cancel, // Cancel a decision in menus.
            ShowMenu, // Go/Leave the menu.
            Pause, // Pause the game.
            Debug //Show Debug Window.
        }

        static Dictionary<Action, Keys> KeyBinding = new Dictionary<Action, Keys>();
        static Dictionary<Action, Buttons> ButtonBinding = new Dictionary<Action, Buttons>();
        
        
        static KeyboardState keyboardState;
        static GamePadState gamePadState;

        static MouseState mouseState;

        static Buffer buffer;

        static public void Initialize()
        {
            // Initialize the controls.
            KeyBinding.Add(Action.Up, Keys.Up);
            KeyBinding.Add(Action.Left,Keys.Left);
            KeyBinding.Add(Action.Right, Keys.Right);
            KeyBinding.Add(Action.Down, Keys.Down);
            KeyBinding.Add(Action.Confirm, Keys.Enter);
            KeyBinding.Add(Action.Cancel, Keys.Escape);
            KeyBinding.Add(Action.Pause, Keys.P);
#if DEBUG
            KeyBinding.Add(Action.Debug, Keys.D);
#endif
            KeyBinding.Add(Action.ShowMenu, Keys.Escape);

            // Initialize the battle controls.
            KeyBinding.Add(Action.BattleMenuUp, Keys.Up);
            KeyBinding.Add(Action.BattleMenuLeft, Keys.Left);
            KeyBinding.Add(Action.BattleMenuRight,Keys.Right);
            KeyBinding.Add(Action.BattleMenuDown,Keys.Down);
            KeyBinding.Add(Action.BattleConfirm,Keys.Enter);
            KeyBinding.Add(Action.BattleCancel,Keys.Back);
            KeyBinding.Add(Action.BattleSoftCancel,Keys.RightShift);

            ButtonBinding.Add(Action.BattleCancel, Buttons.LeftShoulder);
            ButtonBinding.Add(Action.BattleConfirm, Buttons.RightTrigger);
            ButtonBinding.Add(Action.BattleMenuDown, Buttons.A);
            ButtonBinding.Add(Action.BattleMenuLeft, Buttons.X);
            ButtonBinding.Add(Action.BattleMenuRight, Buttons.B);
            ButtonBinding.Add(Action.BattleMenuUp, Buttons.Y);
            ButtonBinding.Add(Action.BattleSoftCancel, Buttons.LeftTrigger);
            ButtonBinding.Add(Action.Cancel, Buttons.Back);
            ButtonBinding.Add(Action.Confirm, Buttons.B);
            ButtonBinding.Add(Action.Down, Buttons.DPadDown);
            ButtonBinding.Add(Action.Left, Buttons.DPadLeft);
            ButtonBinding.Add(Action.Right, Buttons.DPadRight);
            ButtonBinding.Add(Action.ShowMenu, Buttons.Start);
            ButtonBinding.Add(Action.Up, Buttons.DPadUp);
            ButtonBinding.Add(Action.Pause, Buttons.Start);

            buffer = new Buffer(64, 1.0F);
        }

        /// <summary>
        /// Immediatly cancel all input triggered during the current frame.
        /// </summary>
        static public void CancelInput()
        {
            foreach (KeyValuePair<Action, Keys> actionKey in KeyBinding)
            {
                if (keyboardState.IsKeyDown(actionKey.Value))
                    buffer.Add(actionKey.Value, false);
                else
                    buffer.Remove(actionKey.Value);
            }

            foreach (KeyValuePair<Action, Buttons> actionButton in ButtonBinding)
            {
                if (gamePadState.IsButtonDown(actionButton.Value))
                    buffer.Add(actionButton.Value, false);
            }
            buffer.CancelInput();
        }

        static public void Update(GameTime GameTime)
        {
            
            foreach (KeyValuePair<Action, Keys> actionKey in KeyBinding)
            {
                if (keyboardState.IsKeyDown(actionKey.Value))
                    buffer.Add(actionKey.Value, false);
                else
                    buffer.Remove(actionKey.Value);
            }

            foreach (KeyValuePair<Action, Buttons> actionButton in ButtonBinding)
            {
                if (gamePadState.IsButtonDown(actionButton.Value))
                    buffer.Add(actionButton.Value, false);
                else
                    buffer.Remove(actionButton.Value);
            }

            buffer.Update(GameTime);

            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            mouseState = Mouse.GetState();

        }

        static public Vector2 CursorPosition
        {
            get
            {
                return new Vector2(mouseState.X, mouseState.Y);
            }
        }

        static public bool CursorValidated
        {
            get
            {
                return mouseState.LeftButton == ButtonState.Pressed;
            }
        }

        static public bool isActionDone(Action action, bool immediate)
        {
            bool inState = (KeyBinding.ContainsKey(action) && keyboardState.IsKeyDown(KeyBinding[action])) || (ButtonBinding.ContainsKey(action) && gamePadState.IsButtonDown(ButtonBinding[action]));
            bool inActiveBuffer = (KeyBinding.ContainsKey(action) && buffer.ContainsActive(KeyBinding[action])) || (ButtonBinding.ContainsKey(action) && buffer.ContainsActive(ButtonBinding[action]));
            if (!inActiveBuffer)
            {
                //
                int i=0;
                i++;
            }
            
            return inState && (!inActiveBuffer || immediate);
        }
    }
}
