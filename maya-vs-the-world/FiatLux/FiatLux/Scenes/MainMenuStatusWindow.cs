using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FiatLux.Logic;

namespace FiatLux.Scenes
{
    public class MainMenuStatusWindow : Window
    {
        const int VERTICALOFFSET = 10;
        const int HORIZONTALOFFSET = 10;
        const int VERTICALSPACING = 16;
        const int HORIZONTALSPACING = 300;
        const int SMALLHORIZONTALSPACING = 150;
        const int MINIHORIZONTALSPACING = 75;
        const int CARACTERWIDTH = Constants.CARACTERWIDTH;

        #region WithTextArea
        TextArea textArea; 
        #endregion

        int line = 0;
        int column = 0;
        Character chara;
        MenuWindow CaracteristicsMenu;

        Bar EndBar;
        Bar MPBar;
        Bar APBar;
        Bar BaseExpBar;
        Bar ClassExpBar;
        Bar[] StatExpBars = new Bar[Constants.STATNUMBER];

        public MainMenuStatusWindow(Scene parent, int CharacterID)
            : base(parent, new Rectangle(100, 0, 650, 300))
        {
            chara = Settings.Characters[CharacterID];
            #region textArea
           
            textArea = new TextArea(this, 650, 300, OnOvertaking.ShowNormally);
            textArea.Position = new Vector2(Shared.FRAMESIZE*2, Shared.FRAMESIZE*2);

            #region Defining Lines
            textArea.AddLine(0); //0
            textArea.AddLine(VERTICALSPACING); //1
            textArea.AddLine(VERTICALSPACING*2); //2
            textArea.AddLine(VERTICALSPACING); //3
            textArea.AddLine(VERTICALSPACING); //4
            textArea.AddLine(VERTICALSPACING); //5
            textArea.AddLine(VERTICALSPACING); //6
            textArea.AddLine(VERTICALSPACING*2); //7
            textArea.AddLine(VERTICALSPACING); //8
            textArea.AddLine(VERTICALSPACING); //9
            textArea.AddLine(VERTICALSPACING); //10
            textArea.AddLine(VERTICALSPACING); //11
            textArea.AddLine(VERTICALSPACING); //12
            textArea.AddLine(VERTICALSPACING); //13
            textArea.AddLine(VERTICALSPACING); //14
            textArea.AddLine(VERTICALSPACING); //15
            #endregion

            #region Defining Rows
            textArea.AddRowToLine(0, 0);
            textArea.AddRowToLine(17 * CARACTERWIDTH, 0);
            textArea.AddRowToLine(11 * CARACTERWIDTH, 0);
            textArea.AddRowToLine(2*CARACTERWIDTH, 0);
            textArea.AddRowToLine(13 * CARACTERWIDTH, 0);
            textArea.AddRowToLine(2*CARACTERWIDTH, 0);
            textArea.AddRowToLine(14 * CARACTERWIDTH, 0);
            
            textArea.AddRowToLine(0, 1);
            textArea.AddRowToLine(17*CARACTERWIDTH, 1);
            textArea.AddRowToLine(11*CARACTERWIDTH, 1);
            textArea.AddRowToLine(2*CARACTERWIDTH, 1);
            textArea.AddRowToLine(13*CARACTERWIDTH, 1);
            textArea.AddRowToLine(2*CARACTERWIDTH, 1);
            textArea.AddRowToLine(14*CARACTERWIDTH, 1); 
 
            textArea.AddRowToLine(0, 2);
            textArea.AddRowToLine(52 * CARACTERWIDTH, 2);
 
            textArea.AddRowToLine(0, 3);
            textArea.AddRowToLine(17 * CARACTERWIDTH, 3);
            textArea.AddRowToLine(14 * CARACTERWIDTH, 3);
            textArea.AddRowToLine(7 * CARACTERWIDTH, 3);
            textArea.AddRowToLine(8 * CARACTERWIDTH, 3);
            textArea.AddRowToLine(10 * CARACTERWIDTH, 3);  
            

            textArea.AddRowToLine(0, 4);
            textArea.AddRowToLine(23 * CARACTERWIDTH, 4);
            textArea.AddRowToLine(0, 4);
            textArea.AddRowToLine(6 * CARACTERWIDTH, 4);
            textArea.AddRowToLine(6 * CARACTERWIDTH, 4);
            textArea.AddRowToLine(5 * CARACTERWIDTH, 4);
            textArea.AddRowToLine(8 * CARACTERWIDTH, 4);
            textArea.AddRowToLine(8 * CARACTERWIDTH, 4);  

            textArea.AddRowToLine(0, 5);
            textArea.AddRowToLine(23 * CARACTERWIDTH, 5);
            textArea.AddRowToLine(0, 5);
            textArea.AddRowToLine(6 * CARACTERWIDTH, 5);
            textArea.AddRowToLine(6 * CARACTERWIDTH, 5);
            textArea.AddRowToLine(5 * CARACTERWIDTH, 5);
            textArea.AddRowToLine(8 * CARACTERWIDTH, 5);

            textArea.AddRowToLine(0, 6);
            textArea.AddRowToLine(23 * CARACTERWIDTH, 6);
            textArea.AddRowToLine(0, 6);
            textArea.AddRowToLine(6 * CARACTERWIDTH, 6);
            textArea.AddRowToLine(6 * CARACTERWIDTH, 6);
            textArea.AddRowToLine(5 * CARACTERWIDTH, 6);
            textArea.AddRowToLine(8 * CARACTERWIDTH, 6);


            for (int i = 7; i <= 15; i++)
            {
                textArea.AddRowToLine(0, i);
                textArea.AddRowToLine(22*CARACTERWIDTH, i);
                textArea.AddRowToLine(13*CARACTERWIDTH, i);
                textArea.AddRowToLine(5 * CARACTERWIDTH, i);
                textArea.AddRowToLine(8 * CARACTERWIDTH, i);
            }
            textArea.AddRowToLine(8 * CARACTERWIDTH, 15);
            #endregion

            Game.Components.Add(textArea);


            
            #endregion
        
            EndBar = new Bar(this, new Vector2(100, 4));
            Game.Components.Add(EndBar);
            MPBar = new Bar(this, new Vector2(100, 4));
            Game.Components.Add(MPBar);
            APBar = new Bar(this, new Vector2(100, 4));
            Game.Components.Add(APBar);
            BaseExpBar = new Bar(this, new Vector2(260, 2));
            Game.Components.Add(BaseExpBar);
            ClassExpBar = new Bar(this, new Vector2(260, 2));
            Game.Components.Add(ClassExpBar);
            for (int i = 0; i < Constants.STATNUMBER; i++)
            {
                StatExpBars[i] = new Bar(this, new Vector2(160, 2));
                Game.Components.Add(StatExpBars[i]);
            }

            InitializeCaracMenu();
        }

        void InitializeCaracMenu()
        {
            if (CaracteristicsMenu != null)
            {
                CaracteristicsMenu.Destroy();
            }
            string s = "";
            for (int i = 0; i < Constants.CARACNUMBER; i++)
            {
                s += Constants.CARACCONVNAMES[i] + " " + chara.Caracteristics[i].ToString() + "+" + chara.ClassCarac[i].ToString() + "\n";
            }


            CaracteristicsMenu = new MenuWindow(this, new Vector2(0, 0), s);
            Game.Components.Add(CaracteristicsMenu);
            for (int i = 0; i < Constants.CARACNUMBER; i++)
            {
                if (!chara.CanIncreaseCarac(i))
                {
                    CaracteristicsMenu.Disable(i);
                }
            }
            CaracteristicsMenu.Enabled = false;
            CaracteristicsMenu.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(CaracteristicsMenu_Confirmed);
        }

        void CaracteristicsMenu_Confirmed(object sender, MenuWindowEventArgs e)
        {
            chara.Caracteristics[e.Index] += 1;
            chara.UpdateStats();
            int currentIndex = CaracteristicsMenu.Index;
            InitializeCaracMenu();
            CaracteristicsMenu.Enabled = true;
            CaracteristicsMenu.Index = currentIndex;
        }

        

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
   

            line = 0;
            column = 0;
            textArea.TextAtCell(line, column).Message = chara.Name;
            
            column++;
            textArea.TextAtCell(line, column).Message = "Base  Lv.";
            column++;
            textArea.TextAtCell(line, column).Message = chara.GetLevel(ExpTypes.Base, 0).ToString();
            textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
                    
            column++;
            BaseExpBar.Position = textArea.CellPosition(line, column) + new Vector2(2*Shared.FRAMESIZE, 1.5F * VERTICALSPACING + 2);
            BaseExpBar.MaxValue = chara.GetMaxExp(ExpTypes.Base, 0, true);
            BaseExpBar.Value = chara.GetExp(ExpTypes.Base, 0, true);
            BaseExpBar.ValueColor = Color.Yellow;
            
            textArea.TextAtCell(line, column).Message = "Exp";
            column++;
            textArea.TextAtCell(line, column).Message = chara.GetExp(ExpTypes.Base, 0, false).ToString();
            textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;            
            column++;

            textArea.TextAtCell(line, column).Message = "Next";
            column++;
            textArea.TextAtCell(line, column).Message = (chara.GetMaxExp(ExpTypes.Base, 0, false) - chara.GetExp(ExpTypes.Base, 0, false)).ToString();
            textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
            
            line++;
            column = 0;

            textArea.TextAtCell(line, column).Message = GameData.classes[chara.ClassID].Name;
            column++;
            textArea.TextAtCell(line, column).Message = "Class Lv. ";
            column++;
            textArea.TextAtCell(line, column).Message = chara.GetLevel(ExpTypes.Class, chara.ClassID).ToString();
            textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
            column++;
            ClassExpBar.Position = textArea.CellPosition(line, column) + new Vector2(2 * Shared.FRAMESIZE, 1.5F * VERTICALSPACING + 2);
            ClassExpBar.MaxValue = chara.GetMaxExp(ExpTypes.Class, chara.ClassID, true);
            ClassExpBar.Value = chara.GetExp(ExpTypes.Class, chara.ClassID, true);
            ClassExpBar.ValueColor = Color.Green;
            

            textArea.TextAtCell(line, column).Message ="Exp ";
            column++;
            textArea.TextAtCell(line, column).Message = chara.GetExp(ExpTypes.Class, chara.ClassID, false).ToString();
            textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
            column++;
            textArea.TextAtCell(line, column).Message = "Next ";
            column++;
            textArea.TextAtCell(line, column).Message = (chara.GetMaxExp(ExpTypes.Class, chara.ClassID, false)-chara.GetExp(ExpTypes.Class, chara.ClassID, false)).ToString();
            textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
            column = 0;
            line++;
            textArea.TextAtCell(line, column).Message = "Statistics";
            column++;
            textArea.TextAtCell(line, column).Message = "Caracteristics";
            line++;
            column = 0;
            textArea.TextAtCell(line, column).Message = "Name";
            column++;
            textArea.TextAtCell(line, column).Message = "Value/Maxi.";
            column++;
            textArea.TextAtCell(line, column).Message = "Level";
            column++;
            textArea.TextAtCell(line, column).Message = "Exp";
            column++;
            textArea.TextAtCell(line, column).Message = "Next";
            column++;
            textArea.TextAtCell(line, column).Message = "CAR " + chara.CaracPoints.ToString();

            line++;
            column = 0;
            CaracteristicsMenu.Position = textArea.CellPosition(line, textArea.LastRow(line)) + new Vector2(Shared.FRAMESIZE * 2, Shared.FRAMESIZE * 2);

            for (int i = 0; i < Constants.STATNUMBER; i++)
            {
                if (chara.Stats.HaveMax(i))
                {
                    if (CaracteristicsMenu.Enabled && i / 2 == CaracteristicsMenu.Index)
                    {
                        textArea.TextAtCell(line, column).Color = Color.Green;
                    }
                    else
                        textArea.TextAtCell(line, column).Color = Color.White;
        
                    textArea.TextAtCell(line, column).Message = Constants.STATLONGNAMES[i];
                    column++;
                    
                    if (i == chara.Stats.IndexOf("End"))
                    {
                        EndBar.Position = textArea.CellPosition(line, column) + new Vector2(-4*CARACTERWIDTH, 1.5F*VERTICALSPACING+2);
                        EndBar.MaxValue = chara.Stats["End", true];
                        EndBar.Value = chara.Stats["End"];
                        EndBar.ValueColor = Color.Green;
                        EndBar.Visible = true;
                    }
                    if (i == chara.Stats.IndexOf("MP"))
                    {
                        MPBar.Position = textArea.CellPosition(line, column) + new Vector2(-4*CARACTERWIDTH, 1.5F*VERTICALSPACING+2);
                        MPBar.MaxValue = chara.Stats["MP", true];
                        MPBar.Value = chara.Stats["MP"];
                        MPBar.ValueColor = Color.Blue;
                    }
                    if (i == chara.Stats.IndexOf("AP"))
                    {
                        APBar.Position = textArea.CellPosition(line, column) + new Vector2(-4*CARACTERWIDTH, 1.5F*VERTICALSPACING+2);
                        APBar.MaxValue = chara.Stats["AP", true];
                        APBar.Value = chara.Stats["AP"];
                        APBar.ValueColor = Color.Red;
                    }
                    textArea.TextAtCell(line, column).Message = chara.Stats[i].ToString();
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
                    column++;
                    textArea.TextAtCell(line, column).Message = "/";
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;
                    column++;
                    if (CaracteristicsMenu.Enabled && i / 2 == CaracteristicsMenu.Index)
                    {
                        textArea.TextAtCell(line, column).Color = Color.Green;
                        textArea.TextAtCell(line, column).Message = chara.PreviewStat(1, i).ToString();
                    }
                    else
                    {
                        textArea.TextAtCell(line, column).Message = chara.Stats[i, true].ToString();
                        textArea.TextAtCell(line, column).Color = Color.White;
                    }
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;

                    column++;
                    StatExpBars[i].Position = textArea.CellPosition(line, column) + new Vector2(-2*CARACTERWIDTH, 1.5F*VERTICALSPACING+2);
                    StatExpBars[i].MaxValue = chara.GetMaxExp(ExpTypes.Stat, i, true);
                    StatExpBars[i].Value = chara.GetExp(ExpTypes.Stat, i, true);
                    StatExpBars[i].ValueColor = Color.Purple;

                    textArea.TextAtCell(line, column).Message = chara.GetLevel(ExpTypes.Stat, i).ToString();
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;

                    column++;
                    textArea.TextAtCell(line, column).Message = chara.GetExp(ExpTypes.Stat, i, false).ToString();
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;

                    column++;
                    textArea.TextAtCell(line, column).Message = (chara.GetMaxExp(ExpTypes.Stat, i, false) - chara.GetExp(ExpTypes.Stat, i, false)).ToString();
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;
                    
                    column = 0;
                    line++;
                }
            }

            for (int i = 0; i < Constants.STATNUMBER; i++)
            {
                if (!chara.Stats.HaveMax(i))
                {
                    if (CaracteristicsMenu.Enabled && i / 2 == CaracteristicsMenu.Index)
                    {
                        textArea.TextAtCell(line, column).Color = Color.Green;
                    }
                    else
                        textArea.TextAtCell(line, column).Color = Color.White;
        
                    textArea.TextAtCell(line, column).Message = Constants.STATLONGNAMES[i];
                    column++;
                    if (CaracteristicsMenu.Enabled && i / 2 == CaracteristicsMenu.Index)
                    {
                        textArea.TextAtCell(line, column).Color = Color.Green;
                        textArea.TextAtCell(line, column).Message = chara.PreviewStat(1, i).ToString();
                    }
                    else
                    {
                        textArea.TextAtCell(line, column).Message = chara.Stats[i].ToString();
                        textArea.TextAtCell(line, column).Color = Color.White;
                    }
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;

                    column++;
                    StatExpBars[i].Position = textArea.CellPosition(line, column) + new Vector2(-2 * CARACTERWIDTH, 1.5F * VERTICALSPACING + 2);
                    StatExpBars[i].MaxValue = chara.GetMaxExp(ExpTypes.Stat, i, true);
                    StatExpBars[i].Value = chara.GetExp(ExpTypes.Stat, i, true);
                    StatExpBars[i].ValueColor = Color.Purple;

                    textArea.TextAtCell(line, column).Message = chara.GetLevel(ExpTypes.Stat, i).ToString();
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
                    column++;
                    textArea.TextAtCell(line, column).Message = chara.GetExp(ExpTypes.Stat, i, false).ToString();
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;
                    column++;
                    textArea.TextAtCell(line, column).Message = (chara.GetMaxExp(ExpTypes.Stat, i, false) - chara.GetExp(ExpTypes.Stat, i, false)).ToString();
                    textArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;
                    line++;
                    column = 0;
                }
            }
            
        }
    }
}
