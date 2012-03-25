/* LuxEngine V0.4.56
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
    
    public enum OnOvertaking
    {
        /// <summary>
        /// Allow the text to overflow the TextArea.
        /// </summary>
        ShowNormally,
        /// <summary>
        /// The text will be scaled to fit horizontally
        /// </summary>
        Scale,
        /// <summary>
        /// Trigger the Overtaking event (custom behaviour)
        /// </summary>
        Report
    }
    public class TextArea : Scene
    {
        /// <summary>
        /// Contains the original Position of the Rows.
        /// </summary>
        List<List<int>> Rows;
        List<int> Lines;
        List<List<Text>> TextContent;

        public override void Initialize()
        {
            SuppressOvertaking();
            AlignContent();
            base.Initialize();
        }

        public Vector2 CellPosition(int line, int column)
        {
            return new Vector2(Rows[line][column], Lines[line]);
        }

        public int LastLine
        {
            get { return Lines.Count - 1; }
        }

        public int LastRow(int line)
        {
            return Rows[line].Count - 1;
        }

        public void AddLine(int offset)
        {
            if (Lines.Count != 0)
                Lines.Add(Lines[Lines.Count - 1] + offset);
            else
                Lines.Add(offset);
            Rows.Add(new List<int>());
            TextContent.Add(new List<Text>());    
        }

        public void AddRowToLine(int offset, int Line)
        {
            if (Rows[Line].Count != 0)
                Rows[Line].Add(Rows[Line][Rows[Line].Count - 1] + offset);
            else
                Rows[Line].Add(offset);
            Text textToAdd = new Text(this, "DefaultSpriteFont");
            TextContent[Line].Add(textToAdd);
            Game.Components.Add(textToAdd);
        }

        public Text TextAtCell(int Line, int Row)
        {
            return TextContent[Line][Row];
        }

        public OnOvertaking overBehaviour;
        public Vector2 Size;
        public TextArea(Scene parent, int Width, int Height, OnOvertaking overBehaviour)
            : base(parent)
        {
            Lines = new List<int>();
            Rows = new List<List<int>>();
            TextContent = new List<List<Text>>();
            Size = new Vector2(Width, Height);
            this.overBehaviour = overBehaviour;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            SuppressOvertaking();
            AlignContent();
         
            base.Draw(gameTime);
        }
        void AlignContent()
        {
            for (int i = 0; i < Lines.Count; i++)
                for (int j = 0; j < Rows[i].Count; j++)
                    TextContent[i][j].Position = new Vector2(Rows[i][j], Lines[i]);
        }
        void SuppressOvertaking()
        {

            for (int i = 0; i < Lines.Count; i++)
                for (int j = 0; j < Rows[i].Count; j++)
                    if (IsOvertaking(i, j))
                        TreatOvertaking(i, j);
                    else
                    {
                        if (overBehaviour == OnOvertaking.Scale)
                            TextContent[i][j].Scale = 1;
                    }
               
        }

        bool IsOvertaking(int i, int j)
        {
            bool Overtook = false;
            int Up = i == 0 ? 0 : Lines[i - 1];
            int Left = j == 0 ? 0 : Rows[i][j - 1];
            int Right = j == Rows[i].Count - 1 ? (int)Size.X : Rows[i][j + 1];
            int Down = i == Lines.Count - 1 ? (int)Size.Y : Lines[i + 1];
            Text t = TextContent[i][j];
            // Overtaking Left.
            if ((t.HorizontalAlignment == HorizontalAlignment.Right && (Rows[i][j] - t.Size.X < Left))
                || (t.HorizontalAlignment == HorizontalAlignment.Center && (Rows[i][j] - t.Size.X / 2 < Left)))
                Overtook = true;
            // Overtaking Right.
            if ((t.HorizontalAlignment == HorizontalAlignment.Left && (Rows[i][j] + t.Size.X > Right))
                || (t.HorizontalAlignment == HorizontalAlignment.Center && (Rows[i][j] + t.Size.X / 2 > Right)))
                Overtook = true;
            // Overtaking Up.
            if ((t.VerticalAlignment == VerticalAlignment.Down && (Lines[i] - t.Size.Y < Up))
                || (t.VerticalAlignment == VerticalAlignment.Center && (Lines[i] - t.Size.Y / 2 < Up)))
                Overtook = true;
            // Overtaking Down.
            if ((t.VerticalAlignment == VerticalAlignment.Up && (Lines[i] + t.Size.Y > Down))
                || (t.VerticalAlignment == VerticalAlignment.Center && (Lines[i] + t.Size.Y / 2 > Down)))
                Overtook = true;
            return Overtook;
        }

        void TreatOvertaking(int i, int j)
        {
            switch (overBehaviour)
            {
                case OnOvertaking.Report:
                    break;
                case OnOvertaking.Scale:
                    ScaleContent(i, j);
                    break;
                case OnOvertaking.ShowNormally:
                    break;
            }
        }

        void ScaleContent(int i, int j)
        {
            Text t = TextContent[i][j];
            int Up = i == 0 ? 0 : Lines[i - 1];
            int Left = j == 0 ? 0 : Rows[i][j - 1];
            int Right = j == Rows[i].Count - 1 ? (int)Size.X : Rows[i][j + 1];
            int Down = i == Lines.Count - 1 ? (int)Size.Y : Lines[i + 1];
            float ScaleUp = t.VerticalAlignment == VerticalAlignment.Up ? 1.0F : t.VerticalAlignment == VerticalAlignment.Down ?
                CalculateScale(- t.Size.Y, Up, Lines[i]) : CalculateScale(- t.Size.Y / 2, Up, Lines[i]);
            float ScaleDown = t.VerticalAlignment == VerticalAlignment.Up ? CalculateScale(t.Size.Y, Down, Lines[i]) :
                t.VerticalAlignment == VerticalAlignment.Down ? 1.0F : CalculateScale(t.Size.Y / 2, Down, Lines[i]);
            float ScaleRight = t.HorizontalAlignment == HorizontalAlignment.Left ? CalculateScale(t.Size.X, Right, Rows[i][j])
                : t.HorizontalAlignment == HorizontalAlignment.Center ? CalculateScale(t.Size.X/2, Right, Rows[i][j]) : 1.0F;
            float ScaleLeft = t.HorizontalAlignment == HorizontalAlignment.Left ? 1.0F : t.HorizontalAlignment == HorizontalAlignment.Center ?
                CalculateScale(-t.Size.X / 2, Left, Rows[i][j]) : CalculateScale(-t.Size.X, Left, Rows[i][j]);
            t.Scale = (float)Math.Min(Math.Min(ScaleDown, ScaleUp), Math.Min(ScaleLeft, ScaleRight));
        }

        float CalculateScale(float Size, int Border, int Origin)
        {
           return (float)(Border - Origin)/Size; 
        }
    }
}
