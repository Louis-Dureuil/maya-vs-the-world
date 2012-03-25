/* LuxEngine V0.2.13
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace LuxEngine
{
    /// <summary>
    /// Indicates the horizontal alignment of the text.
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// The text will be aligned to the left. (default)
        /// </summary>
        Left,
        /// <summary>
        /// The text will be centered.
        /// </summary>
        Center,
        /// <summary>
        /// The text will be aligned to the right.
        /// </summary>
        Right
    }
    /// <summary>
    /// Indicates the vertical alignment of the text.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// The text will be aligned to the top (default).
        /// </summary>
        Up,
        /// <summary>
        /// The text will be centered.
        /// </summary>
        Center,
        /// <summary>
        /// The text will be aligned to the bottom.
        /// </summary>
        Down
    }

    /// <summary>
    /// A window that contains text. Inherits from Window.
    /// </summary>
    public class TextWindow : Window
    {
        float Timer = 0.0F;
        bool selecting = true;
        bool disabling = true;
        const float TINTDURATION = 0.5F;
        Color SelectingColor = Shared.SelectedColor;
        Color DisablingColor = Shared.DisabledColor;
        public Color TextColor = Color.White;
        StringBuilder sb = new StringBuilder();

        protected List<bool> SelectedLines = new List<bool>();

        protected List<bool> DisabledLines = new List<bool>();

        //new System.
        public TextArea TextArea;

        /// <summary>
        /// For compatibility.
        /// </summary>
        protected List<string> textByLines = new List<string>();
        public string Text
        {
            get
            {
                sb.Clear();
                foreach (string line in textByLines)
                {
                    sb.AppendLine(line);
                }
                return sb.ToString();
            }
            set
            {
                textByLines.Clear();
                DisabledLines.Clear();
                SelectedLines.Clear();
                StringReader sr = new StringReader(value);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    textByLines.Add(line);
                    DisabledLines.Add(false);
                    SelectedLines.Add(false);
                }
            }
        }
        string oldText = "";
        public HorizontalAlignment horizontalAlignment;
        public VerticalAlignment verticalAlignment;
        public bool AutoResize = true;
        
        public TextWindow(LuxGame game, Vector2 position)
            : base(game, new Rectangle((int)position.X, (int)position.Y,0, 0))
        {
            TextArea = new TextArea(this, 0, 0, OnOvertaking.ShowNormally);
            TextArea.Position = new Vector2(Shared.FRAMESIZE, Shared.FRAMESIZE);
            Game.Components.Add(TextArea);

        }

        public TextWindow(LuxGame game, Rectangle windowRectangle)
            : base(game, windowRectangle)
        {
            TextArea = new TextArea(this, windowRectangle.Width - 2*Shared.FRAMESIZE, windowRectangle.Height - 2*Shared.FRAMESIZE, OnOvertaking.ShowNormally);
            TextArea.Position = new Vector2(Shared.FRAMESIZE, Shared.FRAMESIZE);
            Game.Components.Add(TextArea);

            AutoResize = false;
        }

        public TextWindow(Scene parent, Vector2 position)
            : base(parent, new Rectangle((int)position.X, (int)position.Y, 0, 0))
        {
            TextArea = new TextArea(this, 0, 0, OnOvertaking.ShowNormally);
            TextArea.Position = new Vector2(Shared.FRAMESIZE, Shared.FRAMESIZE);
            Game.Components.Add(TextArea);

        }

        public TextWindow(Scene parent, Rectangle windowRectangle)
            : base(parent, windowRectangle)
        {
            TextArea = new TextArea(this, windowRectangle.Width - 2 * Shared.FRAMESIZE, windowRectangle.Height - 2 * Shared.FRAMESIZE, OnOvertaking.ShowNormally);
            TextArea.Position = new Vector2(Shared.FRAMESIZE, Shared.FRAMESIZE);
            Game.Components.Add(TextArea);
            AutoResize = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public void Disable(int index)
        {
            if (DisabledLines.Count > index)
                DisabledLines[index] = true;
        }

        public void Enable(int index)
        {
            if (DisabledLines.Count > index)
                DisabledLines[index] = false;
        }

        public void Select(int index)
        {
            if (SelectedLines.Count > index)
                SelectedLines[index] = true;
        }

        public void Unselect(int index)
        {
            if (SelectedLines.Count > index)
                SelectedLines[index] = false;
        }

        public override void Update(GameTime gameTime)
        {
            //Frame duration, in s (precision 10^(-3) s.)
            float FrameDuration = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;

            Timer += FrameDuration;
            if (Timer >= TINTDURATION)
            {
                Timer = 0;
                selecting = !(selecting && SelectingColor == Color.White) || (!selecting && SelectingColor == Shared.SelectedColor);
                disabling = !(disabling && DisablingColor == Color.White) || (!disabling && DisablingColor == Shared.DisabledColor);
                SelectingColor.A = (byte)Math.Max((byte)255, SelectingColor.A + (selecting ? (byte)1 : -1));
                SelectingColor.B = (byte)Math.Max((byte)255, SelectingColor.B + (selecting ? (byte)1 : -1));
                SelectingColor.G = (byte)Math.Max((byte)255, SelectingColor.G + (selecting ? (byte)1 : -1));
                SelectingColor.R = (byte)Math.Max((byte)255, SelectingColor.R + (selecting ? (byte)1 : -1));
                DisablingColor.A = (byte)Math.Max((byte)255, DisablingColor.A + (disabling ? (byte)1 : -1));
                DisablingColor.B = (byte)Math.Max((byte)255, DisablingColor.B + (disabling ? (byte)1 : -1));
                DisablingColor.G = (byte)Math.Max((byte)255, DisablingColor.G + (disabling ? (byte)1 : -1));
                DisablingColor.R = (byte)Math.Max((byte)255, DisablingColor.R + (disabling ? (byte)1 : -1));
                
            }

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (oldText != Text)
            {
                if (AutoResize)
                {
                    int oldWidth = WindowRectangle.Width;
                    int oldHeight = WindowRectangle.Height;
                    WindowRectangle.Width = (int)Math.Ceiling(spriteFont.MeasureString(Text).X) + Shared.FRAMESIZE;
                    WindowRectangle.Height = Shared.FRAMESIZE;
                    for (int i = 0; i < textByLines.Count; i++)
                    {
                        WindowRectangle.Height += (int)Math.Ceiling(spriteFont.MeasureString(textByLines[i]).Y);
                    }
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Center:
                            WindowRectangle.X += (oldWidth - WindowRectangle.Width) / 2;
                            break;
                        case HorizontalAlignment.Right:
                            WindowRectangle.X += (oldWidth - WindowRectangle.Width);
                            break;
                        default:
                            break;
                    }
                    switch (verticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            WindowRectangle.Y += (oldHeight - WindowRectangle.Height) / 2;
                            break;
                        case VerticalAlignment.Down:
                            WindowRectangle.Y += (oldHeight - WindowRectangle.Height);
                            break;
                        default:
                            break;
                    }
                }
                oldText = Text;
            }
          
            base.Draw(gameTime);
            LuxGame.spriteBatch.Begin();
            for (int i=0;i < textByLines.Count;i++)
            {               
                Vector2 Offset = new Vector2(horizontalAlignment == HorizontalAlignment.Center ? (WindowRectangle.Width - spriteFont.MeasureString(textByLines[i]).X) / 2  + Shared.FRAMESIZE /2 : (horizontalAlignment == HorizontalAlignment.Right ? WindowRectangle.Width - spriteFont.MeasureString(textByLines[i]).X - Shared.FRAMESIZE : 0) + Shared.FRAMESIZE, verticalAlignment == VerticalAlignment.Center ? (WindowRectangle.Height - spriteFont.MeasureString(Text).Y) / 2 : (verticalAlignment == VerticalAlignment.Down ? WindowRectangle.Height - spriteFont.MeasureString(Text).Y : 0) + Shared.FRAMESIZE);
                LuxGame.spriteBatch.DrawString(spriteFont, textByLines[i], new Vector2(WindowRectangle.X, WindowRectangle.Y) + Offset + i * new Vector2(0, spriteFont.MeasureString(textByLines[i]).Y), SelectedLines[i] ? (DisabledLines[i] ? DisablingColor : SelectingColor) : (DisabledLines[i] ? Shared.DisabledColor : TextColor));
            }

            LuxGame.spriteBatch.End();
        }
    }
}
