/* Fiat Lux V0.2.20
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework.Audio;

namespace FiatLux.Scenes
{
    class SoundTestScene : Scene
    {
        MenuWindow menuWindow;
        Cue musicCue;
        SoundBank MusicSoundBank;
        WaveBank MusicWaveBank;


        public SoundTestScene(LuxGame game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            MusicSoundBank = new SoundBank(LuxGame.engine, "Content/Music Sound Bank.xsb");
            MusicWaveBank = new WaveBank(LuxGame.engine, "Content/Music.xwb");
            base.LoadContent();
        }

        public override void Initialize()
        {
            menuWindow = new MenuWindow(this, new Microsoft.Xna.Framework.Vector2(300, 200),"academie\naztown\nbattle_1\necran_titre\nmap\ntrailer\nwind\nQuit");
            Game.Components.Add(menuWindow);
            menuWindow.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(menuWindow_Confirmed);
            base.Initialize();
        }

        public override void  Destroy()
        {
            MusicWaveBank.Dispose();
            MusicSoundBank.Dispose();
 	         base.Destroy();
        }

        void menuWindow_Confirmed(object sender, MenuWindowEventArgs e)
        {
            switch (e.Choice)
            {
                case "Quit":
                    if (musicCue != null)
                    {
                        musicCue.Stop(AudioStopOptions.Immediate);
                        musicCue.Dispose();
                    }
                    this.Destroy();
                    Game.Components.Add(new Scenes.DemoTitleScene(LuxGame));
                    break;
                default:
                    if (musicCue != null)
                    {
                        musicCue.Stop(AudioStopOptions.Immediate);
                        musicCue.Dispose();
                    }
                    musicCue = MusicSoundBank.GetCue(e.Choice);
                    musicCue.Play();
                    break;
            }
        }
    }
}
