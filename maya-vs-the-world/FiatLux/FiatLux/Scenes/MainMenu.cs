using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using FiatLux.Logic;

namespace FiatLux.Scenes
{
    public class MainMenu : Scene
    {
        TextWindow[] StatusWindow = new TextWindow[Constants.CharactersCapacity];
        Bar[] EndBars = new Bar[Constants.CharactersCapacity];
        Bar[] MPBars = new Bar[Constants.CharactersCapacity];

        Bar[] BaseExpBars = new Bar[Constants.CharactersCapacity];
        Bar[] ClassExpBars = new Bar[Constants.CharactersCapacity];
        int Index = 0;
        MenuWindow SelectionWindow;

        StringBuilder sb = new StringBuilder();

        Window subWindow;

        MenuWindow Menu;
        MenuWindow subMenu;
        public MainMenu(Scene parent)
            : base(parent)
        {
        }

        public override void Initialize()
        {
           
            for (int i = 0; i < StatusWindow.Length; i++)
            {
                StatusWindow[i] = new TextWindow(this, new Rectangle(100 + i * 155, 0, 150, 150));
#region Initializing TextArea
                StatusWindow[i].TextArea.AddLine(0);
                StatusWindow[i].TextArea.AddLine(20);
                StatusWindow[i].TextArea.AddLine(20);
                StatusWindow[i].TextArea.AddLine(20);
                StatusWindow[i].TextArea.AddLine(20);
                StatusWindow[i].TextArea.AddLine(20);
                #region Initializing Rows
                StatusWindow[i].TextArea.AddRowToLine(75,0);

                StatusWindow[i].TextArea.AddRowToLine(75, 1);

                StatusWindow[i].TextArea.AddRowToLine(0, 2);
                StatusWindow[i].TextArea.AddRowToLine(7*Constants.CARACTERWIDTH, 2);
                StatusWindow[i].TextArea.AddRowToLine(0, 2);
                StatusWindow[i].TextArea.AddRowToLine(Constants.CARACTERWIDTH/2 + 2*Constants.CARACTERWIDTH, 2);

                StatusWindow[i].TextArea.AddRowToLine(75, 3);

                StatusWindow[i].TextArea.AddRowToLine(0, 4);
                StatusWindow[i].TextArea.AddRowToLine(9*Constants.CARACTERWIDTH, 4);
                StatusWindow[i].TextArea.AddRowToLine(0, 4);
                StatusWindow[i].TextArea.AddRowToLine(5 * Constants.CARACTERWIDTH, 4);

                StatusWindow[i].TextArea.AddRowToLine(0, 5);
                StatusWindow[i].TextArea.AddRowToLine(9 * Constants.CARACTERWIDTH, 5);
                StatusWindow[i].TextArea.AddRowToLine(0, 5);
                StatusWindow[i].TextArea.AddRowToLine(5 * Constants.CARACTERWIDTH, 5);
                
                #endregion
#endregion

                Game.Components.Add(StatusWindow[i]);
                if (i >= Settings.PartySize)
                    StatusWindow[i].Visible = false;
                StatusWindow[i].horizontalAlignment = HorizontalAlignment.Center;
            }

            for (int i = 0; i < EndBars.Length; i++)
            {
                EndBars[i] = new Bar(StatusWindow[i], new Vector2(130, 4));
                Game.Components.Add(EndBars[i]);                
                EndBars[i].Position = new Vector2(10, 101);
                EndBars[i].ValueColor = Color.Green;
            }

            for (int i = 0; i < MPBars.Length; i++)
            {
                MPBars[i] = new Bar(StatusWindow[i], new Vector2(130, 4));
                Game.Components.Add(MPBars[i]);                
                MPBars[i].Position = new Vector2(10, 122);
                MPBars[i].ValueColor = Color.Blue;
            }

            for (int i = 0; i < BaseExpBars.Length; i++)
            {
                BaseExpBars[i] = new Bar(StatusWindow[i], new Vector2(130, 2));
                Game.Components.Add(BaseExpBars[i]);
                BaseExpBars[i].Position = new Vector2(10, 61);
                BaseExpBars[i].ValueColor = Color.Yellow;
            }

            for (int i = 0; i < ClassExpBars.Length; i++)
            {
                ClassExpBars[i] = new Bar(StatusWindow[i], new Vector2(130, 2));
                Game.Components.Add(ClassExpBars[i]);                
                ClassExpBars[i].Position = new Vector2(10, 63);
                ClassExpBars[i].ValueColor = Color.Green;
            }
            Menu = new MenuWindow(this, Vector2.Zero, "Inventory\nEquipment\nStatus\nClass\nTactics\nExit\nQuit");
            Game.Components.Add(Menu);
            Menu.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(Menu_Confirmed);
            Menu.Canceled += new MenuWindow.ChoiceConfirmedEventHandler(Menu_Canceled);
            Menu.horizontalAlignment = HorizontalAlignment.Center;
            Menu.Disable(0);
            Menu.Disable(1);
            Menu.Disable(3);
            Menu.Disable(4);
            base.Initialize();
        }

        void Menu_Canceled(object sender, MenuWindowEventArgs e)
        {
            parent.Enabled = true;
            this.Destroy();
                
        }

        void ShowBars()
        {
            for (int i = 0; i < EndBars.Length && i < Settings.PartySize; i++)
            {
                StatusWindow[i].TextArea.Visible = true;

                EndBars[i].Visible = true;
            }

            for (int i = 0; i < MPBars.Length && i < Settings.PartySize; i++)
            {
                MPBars[i].Visible = true;
            }

            for (int i = 0; i < BaseExpBars.Length && i < Settings.PartySize; i++)
            {
                BaseExpBars[i].Visible = true;
            }

            for (int i = 0; i < ClassExpBars.Length && i < Settings.PartySize; i++)
            {
                ClassExpBars[i].Visible = true;
            }
        }

        void HideBars()
        {
            for (int i = 0; i < EndBars.Length; i++)
            {
                EndBars[i].Visible = false;
                StatusWindow[i].TextArea.Visible = false;
            }

            for (int i = 0; i < MPBars.Length; i++)
            {
                MPBars[i].Visible = false;
            }

            for (int i = 0; i < BaseExpBars.Length; i++)
            {
                BaseExpBars[i].Visible = false;
            }

            for (int i = 0; i < ClassExpBars.Length; i++)
            {
                ClassExpBars[i].Visible = false;
            }
        }

        void Menu_Confirmed(object sender, MenuWindowEventArgs e)
        {
            switch (e.Choice)
            {
                case "Inventory":
                    break;
                case "Equipment":
                    break;
                case "Status":
                    Menu.Enabled = false;
                    SelectionWindow = new MenuWindow(this, new Vector2(100 + Index*150, 0), Settings.Characters[Settings.Party[Index]].Name);
                    Game.Components.Add(SelectionWindow);
             
                    SelectionWindow.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(SelectionWindow_Status_Confirmed);
                    SelectionWindow.Canceled += new MenuWindow.ChoiceConfirmedEventHandler(SelectionWindow_Canceled);
                    break;
                case "Class":
                    break;
                case "Tactics":
                    break;
                case "Exit":
                    parent.Enabled = true;
                    this.Destroy();
                    break;
                case "Quit":
                    parent.Destroy();
                    Game.Components.Add(new DemoTitleScene(this.LuxGame));
                    break;
            }
        }

        void SelectionWindow_Canceled(object sender, MenuWindowEventArgs e)
        {
            SelectionWindow.Destroy();
            SelectionWindow = null;
            Menu.Enabled = true;
        }

        void SelectionWindow_Status_Confirmed(object sender, MenuWindowEventArgs e)
        {
            SelectionWindow.Destroy();
            SelectionWindow = null;
            subMenu = new MenuWindow(this, new Vector2(Shared.FRAMESIZE, Shared.FRAMESIZE + (Menu.Index+1) * 20), "Change Class\nAllocate\nView\nBack");
            Game.Components.Add(subMenu);
            subMenu.horizontalAlignment = HorizontalAlignment.Center;
            subMenu.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(subMenu_Confirmed);
            subMenu.Canceled += new MenuWindow.ChoiceConfirmedEventHandler(subMenu_Canceled);
            subWindow = new MainMenuStatusWindow(this, Settings.Party[Index]);
            Game.Components.Add(subWindow);
            subWindow.Enabled = false;
            HideBars();
        }

        void subMenu_Canceled(object sender, MenuWindowEventArgs e)
        {
            subWindow.Destroy();
            subWindow = null;
            subMenu.Destroy();
            subMenu = null;
            ShowBars();
            Menu.Enabled = true;
        }

        void subMenu_Confirmed(object sender, MenuWindowEventArgs e)
        {
            switch (e.Choice)
            {
                case "Back":
                    subMenu_Canceled(sender, e);
                    break;
                case "View":
                    break;
                case "Allocate":
                    subWindow.Enabled = true;
                    subMenu.Enabled = false;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HandleInput();
            Menu.DrawOrder = this.DrawOrder + 1;
            if (SelectionWindow != null)
            {
                SelectionWindow.DrawOrder = this.DrawOrder + 4;
                SelectionWindow.Position = new Vector2(100 + Index * 150, 0);
               SelectionWindow.Text = Settings.Characters[Settings.Party[Index]].Name;
            }
            for (int i = 0; i < Settings.PartySize;i++ )
            {
                if (Settings.Characters[Settings.Party[i]] != null)
                {
                    StatusWindow[i].TextArea.DrawOrder = this.DrawOrder + 5;
                    for (int j = 0; j <= StatusWindow[i].TextArea.LastLine; j++)
                        for (int k = 0; k <= StatusWindow[i].TextArea.LastRow(j); k++)
                            StatusWindow[i].TextArea.TextAtCell(j, k).DrawOrder = this.DrawOrder + 4; 
                    StatusWindow[i].DrawOrder = this.DrawOrder + 1;
                    EndBars[i].DrawOrder = StatusWindow[i].DrawOrder + 1;
                    MPBars[i].DrawOrder = StatusWindow[i].DrawOrder + 1;
                    BaseExpBars[i].DrawOrder = StatusWindow[i].DrawOrder + 1;
                    ClassExpBars[i].DrawOrder = StatusWindow[i].DrawOrder + 1;
                    Character chara = Settings.Characters[Settings.Party[i]];
                    int line = 0;
                    int column = 0;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = chara.Name;
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;
                    line++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = GameData.classes[chara.ClassID].Name;
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;
                    line++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = "Level ";
                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = chara.GetLevel(ExpTypes.Base, 0).ToString();
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
                    
                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = "/";
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;

                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = chara.GetLevel(ExpTypes.Class, chara.ClassID).ToString();
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;

                    line++;
                    column = 0;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = "Good";
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;

                    line++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = Data.Local.StatNames.End;
                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = chara.Stats["End"].ToString();
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;
                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = "/";
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;

                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = chara.Stats["End", true].ToString();
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;

                    line++;
                    column=0;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = Data.Local.StatNames.MP;
                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = chara.Stats["MP"].ToString();
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;

                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = "/";
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Center;

                    column++;
                    StatusWindow[i].TextArea.TextAtCell(line, column).Message = chara.Stats["MP", true].ToString();
                    StatusWindow[i].TextArea.TextAtCell(line, column).HorizontalAlignment = HorizontalAlignment.Right;

                    line++;
                    column = 0;
                    StatusWindow[i].Text = sb.ToString(); 
                    EndBars[i].MaxValue = chara.Stats["End", true];
                    EndBars[i].Value = chara.Stats["End"];
                    MPBars[i].MaxValue = chara.Stats["MP", true];
                    MPBars[i].Value = chara.Stats["MP"];
                    BaseExpBars[i].MaxValue = chara.GetMaxExp(ExpTypes.Base, 0, true);
                    BaseExpBars[i].Value = chara.GetExp(ExpTypes.Base, 0, true);
                    ClassExpBars[i].MaxValue = chara.GetMaxExp(ExpTypes.Class, chara.ClassID, true);
                    ClassExpBars[i].Value = chara.GetExp(ExpTypes.Class, chara.ClassID, true);
                    
                }
            }
        }

        private void HandleInput()
        {
            if (SelectionWindow != null && (Input.isActionDone(Input.Action.Right, false)
                || Input.isActionDone(Input.Action.Down, false)))
            {
                Index = (Index + 1) % Settings.PartySize;
            }
            if (SelectionWindow != null && (Input.isActionDone(Input.Action.Left, false)
                || Input.isActionDone(Input.Action.Up, false)))
            {
                Index--;
                if (Index < 0)
                    Index = Settings.PartySize - 1;
            }
            if (subWindow != null && Input.isActionDone(Input.Action.Cancel, false) && subWindow.Enabled)
            {
                subWindow.Enabled = false;
                subMenu.Enabled = true;
            }
        }
    }
}
