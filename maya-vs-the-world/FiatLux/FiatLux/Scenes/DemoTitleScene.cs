/* Fiat Lux V0.2.20
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using FiatLux.Logic;
using Microsoft.Xna.Framework.Audio;

namespace FiatLux.Scenes
{
    public class DemoTitleScene : Scene
    {
        MenuWindow choiceWindow;
        MessageWindow titleWindow;

        MenuWindow chooseMapWindow;

        SoundBank soundBank;
        WaveBank waveBank;

        Cue TitleMusic;

        public DemoTitleScene(LuxGame game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            soundBank = new SoundBank(LuxGame.engine, "Content/TitleMusicSoundBank.xsb");
            waveBank = new WaveBank(LuxGame.engine, "Content/Music.xwb");
            TitleMusic = soundBank.GetCue("trailer");
            TitleMusic.Play();
            base.LoadContent();
        }

        public override void Destroy()
        {
            if (TitleMusic != null)
                TitleMusic.Dispose();
            soundBank.Dispose();
            waveBank.Dispose();
            base.Destroy();
        }

        public override void Initialize()
        {
            choiceWindow = new MenuWindow(this, new Microsoft.Xna.Framework.Vector2(300, 175),
                Data.Local.MenuOptions.BattleDemo + "\n" + 
                Data.Local.MenuOptions.BattleDemo2P + "\n" +
                Data.Local.MenuOptions.Tutorial + "\n" +
                Data.Local.MenuOptions.SoundTest + "\n" +
                Data.Local.MenuOptions.MapCreator +"\n" +
                Data.Local.MenuOptions.TestMap +"\n" +
                Data.Local.MenuOptions.Quit);
            Game.Components.Add(choiceWindow);
            choiceWindow.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(choiceWindow_Confirmed);
            //choiceWindow.Disable(1);
            choiceWindow.horizontalAlignment = HorizontalAlignment.Center;
            //titleWindow = new MessageWindow(this, new Microsoft.Xna.Framework.Vector2(275, 0), "Fiat Lux Public Alpha 2\nV 0.4.38", MessageConfirmType.None);
            titleWindow = new MessageWindow(this, new Microsoft.Xna.Framework.Vector2(275, 0), "Fiat Lux Dev Version\nV 0.4.56", MessageConfirmType.None);
            Game.Components.Add(titleWindow);
            titleWindow.horizontalAlignment = HorizontalAlignment.Center;

            chooseMapWindow = new MenuWindow(this, new Microsoft.Xna.Framework.Vector2(0, 0), Data.Local.MenuOptions.Cancel);
            Game.Components.Add(chooseMapWindow);
            chooseMapWindow.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(chooseMapWindow_Confirmed);
            chooseMapWindow.Canceled += new MenuWindow.ChoiceConfirmedEventHandler(chooseMapWindow_Canceled);
            
            chooseMapWindow.Enabled = false;
            chooseMapWindow.Visible = false;
            //choiceWindow.Disable(4);
            //choiceWindow.Disable(5);
            base.Initialize();
        }


        void choiceWindow_Confirmed(object sender, MenuWindowEventArgs e)
        {
            if (e.Choice == Data.Local.MenuOptions.BattleDemo)
            {
                this.Destroy();
                Game.Components.Add(new Scenes.BattleTestScene(LuxGame));
                return;
            }
            if (e.Choice == Data.Local.MenuOptions.BattleDemo2P)
            {
                this.Destroy();
                Game.Components.Add(new Scenes.BattleTestScene2(LuxGame));
                return;
            }
            if (e.Choice == Data.Local.MenuOptions.Tutorial)
            {
                this.Destroy();
                Game.Components.Add(new Scenes.BattleTutorialScene(LuxGame));
                return;
            }
            if (e.Choice == Data.Local.MenuOptions.SoundTest)
            {
                this.Destroy();
                Game.Components.Add(new Scenes.SoundTestScene(LuxGame));
                return;
            }
            if (e.Choice == Data.Local.MenuOptions.MapCreator)
            {
                Game.Components.Add(new Scenes.MapCreatorScene(LuxGame));
            }
            if (e.Choice == Data.Local.MenuOptions.TestMap)
            {
                choiceWindow.Enabled = false;
                chooseMapWindow.Visible = true;
                chooseMapWindow.Enabled = true;
                chooseMapWindow.Text = MapManager.GetLoadableMaps("Data/Map/") + "Cancel";
                LuxGame.Tick();
                return;
            }
            if (e.Choice == Data.Local.MenuOptions.Quit)
            {
                Game.Exit();
            }
        }

        void chooseMapWindow_Canceled(object sender, MenuWindowEventArgs e)
        {
            choiceWindow.Enabled = true;
            chooseMapWindow.Enabled = false;
            chooseMapWindow.Visible = false;
        }

        void chooseMapWindow_Confirmed(object sender, MenuWindowEventArgs e)
        {
            if (e.Choice != Data.Local.MenuOptions.Cancel && !string.IsNullOrEmpty(e.Choice))
            {
                this.Destroy();
                GameDataManager.Initialize();
                Settings.Characters[0] = (Character)GameData.characters[0].GetCopy("");
                Settings.Characters[1] = (Character)GameData.characters[1].GetCopy("des villes");
                Settings.Characters[2] = (Character)GameData.characters[1].GetCopy("des bois");
                Settings.Characters[3] = (Character)GameData.characters[1].GetCopy("des forêts");

                Settings.Party[0] = 0;
                Settings.Party[1] = 1;
                Settings.Party[2] = 2;
                Settings.Party[3] = 3;
                Settings.PartySize = 4;
                Game.Components.Add(new Scenes.MapScene(LuxGame, e.Choice));
            }
            else if (e.Choice == Data.Local.MenuOptions.Cancel)
            {
                choiceWindow.Enabled = true;
                chooseMapWindow.Enabled = false;
                chooseMapWindow.Visible = false;
            }
        }
    }
}
