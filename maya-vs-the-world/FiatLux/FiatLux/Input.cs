using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace FiatLux 
{
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
                    isButtonActive[CurrentButtonSize] = true;
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
                            isNew = false;
                        }
                    }
                    if (isNew)
                        Add(key, true); 
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
                    isButtonActive[i] = false;
                    isKeyActive[i] = keyboardState.IsKeyDown(keys[i]);
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
                    if (buttons[i] == button && (buttonsLifeTime[i] > 0 && isButtonActive[i]))
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
            ShowMenu // Go/Leave the menu.
        }

        static Dictionary<Action, Keys> KeyBinding = new Dictionary<Action, Keys>();
        static Dictionary<Action, Buttons> ButtonBinding = new Dictionary<Action, Buttons>();
        
        
        static KeyboardState keyboardState;
        static GamePadState gamePadState;

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
           
            // Initialize the battle controls.
            KeyBinding.Add(Action.BattleMenuUp, Keys.Up);
            KeyBinding.Add(Action.BattleMenuLeft, Keys.Left);
            KeyBinding.Add(Action.BattleMenuRight,Keys.Right);
            KeyBinding.Add(Action.BattleMenuDown,Keys.Down);
            KeyBinding.Add(Action.BattleConfirm,Keys.Enter);
            KeyBinding.Add(Action.BattleCancel,Keys.Back);
            KeyBinding.Add(Action.BattleSoftCancel,Keys.RightShift);

            ButtonBinding.Add(Action.Cancel, Buttons.A); 
            ButtonBinding.Add(Action.BattleMenuUp, Buttons.DPadUp);

            buffer = new Buffer(64, 1.0F);
        }

        static public void Update(GameTime GameTime)
        {
            
            foreach (KeyValuePair<Action, Keys> actionKey in KeyBinding)
            {
                if (keyboardState.IsKeyDown(actionKey.Value))
                    buffer.Add(actionKey.Value, false);
            }

            foreach (KeyValuePair<Action, Buttons> actionButton in ButtonBinding)
            {
                if (gamePadState.IsButtonDown(actionButton.Value))
                    buffer.Add(actionButton.Value, false);
            }

            buffer.Update(GameTime);


            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);


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
