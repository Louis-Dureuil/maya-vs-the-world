/* Fiat Lux V0.4.32
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FiatLux.Scenes
{
    public class MapScene : Scene
    {
        const float PLAYERSPEED = 2 * 60;
        string mapName;
        Map map;

        MessageWindow DebugWindow;

        List<int> EnnemyID = new List<int>() { 0, 0, 0, 0 };

        Sprite background;
        Camera camera;

        Cue backgroundMusic;

        List<Sprite> sprites;

        BattleScene bs;

        Sprite playerSprite;
        private WaveBank MusicWaveBank;
        private SoundBank MusicSoundBank;

        public MapScene(LuxGame game, string mapName)
            : base(game)
        {
            this.mapName = mapName;
        }

        public MapScene(Scene parent, string mapName)
            : base(parent)
        {
            this.mapName = mapName;
        }

        protected override void LoadContent()
        {
            if (!string.IsNullOrEmpty(mapName))
            {
                MusicSoundBank = new SoundBank(LuxGame.engine, "Content/Music Sound Bank.xsb");
                MusicWaveBank = new WaveBank(LuxGame.engine, "Content/Music.xwb");
                map = MapManager.Load(mapName);
                
                if (!string.IsNullOrEmpty(map.BackgroundMusicName))
                {
                    backgroundMusic = MusicSoundBank.GetCue(map.BackgroundMusicName);
                    backgroundMusic.Play();
                }
                camera = new Camera(this, new Vector2(map.Width, map.Height));
                Game.Components.Add(camera);
                background = new Sprite(camera, new List<string>() { map.BackgroundName });
                Game.Components.Add(background);
                background.SetAnimation(map.BackgroundName);
                foreach (string spriteName in map.spritesNames)
                {
                    Sprite sp = new Sprite(camera, new List<string> { spriteName });
                    sprites.Add(sp);
                    Game.Components.Add(sp);
                    sprites[sprites.Count - 1].SetAnimation(spriteName);
                    sprites[sprites.Count - 1].DrawOrder++;
                }
                playerSprite = new Sprite(camera, new List<string>() { "Lucien" });
                Game.Components.Add(playerSprite);
                playerSprite.SetAnimation("Lucien");
                playerSprite.DrawOrder++;
                playerSprite.HasAfterImage = true;
                playerSprite.AfterImageColor = Color.Wheat;
                camera.Mode = CameraMode.FocusOnSingle;
                camera.AddTarget(playerSprite);
     
            }
            DebugWindow = new MessageWindow(this, Vector2.Zero, "", MessageConfirmType.None);
            Game.Components.Add(DebugWindow);
            DebugWindow.DrawOrder = 99;
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //Frame duration, in s (precision 10^(-3) s.)
            float FrameDuration = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;

            if (Input.isActionDone(Input.Action.Confirm, false))
            {
                bs = new BattleScene(this.LuxGame);
                this.Visible = false;
                this.Enabled = false;
                bs.Visible = true;
                bs.Enabled = true;
                backgroundMusic.Pause();
                bs.Initialized += new BattleScene.OnInitializing(bs_Initialized);
                bs.BattleEnded += new BattleScene.OnBattleEnding(bs_BattleEnded);
                bs.BattleEnding += new BattleScene.OnBattleEnding(bs_BattleEnding);
                Game.Components.Add(bs);
            }

            if (Input.isActionDone(Input.Action.ShowMenu, false))
            {
                this.Enabled = false;
                this.Visible = true;
                MainMenu mainMenu = new MainMenu(this);
                Game.Components.Add(mainMenu);
                mainMenu.Enabled = true;
                mainMenu.Visible = true;
                mainMenu.DrawOrder = 99;
            }

            if (Input.isActionDone(Input.Action.Left, true))
            {
                playerSprite.Position.X -= PLAYERSPEED * FrameDuration;
            }
            if (Input.isActionDone(Input.Action.Up, true))
            {
                playerSprite.Position.Y -= PLAYERSPEED * FrameDuration;
            }
            if (Input.isActionDone(Input.Action.Right, true))
            {
                playerSprite.Position.X += PLAYERSPEED *  FrameDuration;
            }
            if (Input.isActionDone(Input.Action.Down, true))
            {
                playerSprite.Position.Y += PLAYERSPEED * FrameDuration;
            }

            DebugWindow.Text = camera.Position.X.ToString() + "\n" + playerSprite.Position.X.ToString();

            base.Update(gameTime);
        }

        void bs_BattleEnding(object sender, EndBattleEventArgs e)
        {
            float TotalEnnemyMult = 0.0F;
            for (int i = 0; i < bs.Fighters.Length; i++)
            {
                if (bs.Fighters[i] is Logic.Ennemy)
                {
                    TotalEnnemyMult += ((Logic.Ennemy)bs.Fighters[i]).EnnemyMult;
                }
            }
            for (int i = 0; i < bs.Fighters.Length; i++)
            {
                if (bs.Fighters[i] != null && bs.Fighters[i] is Logic.Character && !bs.Fighters[i].isDead)
                    bs.AddExpGauge((Logic.Character)bs.Fighters[i], (int)Math.Ceiling(20 * TotalEnnemyMult), Logic.ExpTypes.Base, 0);
            }
            foreach (BattleExpGauge Gauge in bs.ExpGauges)
            {
                Gauge.Accelerate();
            }
        }

        void bs_BattleEnded(object sender, EndBattleEventArgs e)
        {
            bs.Destroy();
            bs = null;
            if (e.isVictory)
            {
                
                    this.Enabled = true;
                    this.Visible = true;
                    backgroundMusic.Resume();
                
                
            }
            else
            {
                this.Destroy();
                Game.Components.Add(new DemoTitleScene(this.LuxGame));
            }

        }

        void bs_Initialized(object sender, EventArgs e)
        {
            bs.camera.Position = this.camera.Position;
            bs.MusicCue = bs.MusicSoundBank.GetCue("battle_1");
            bs.background = new Sprite(bs.camera, new List<string>() { "backgroundM" });
            bs.background.Position = this.background.Position;
            Game.Components.Add(bs.background);
            bs.background.SetAnimation("backgroundM");
            //bs.background.Position = new Vector2(background.Width / 2 - 200, background.Height / 2 - 200);

            int j = 0;
            for (int i = 0; i < Settings.PartySize && i < Constants.BattleCharactersCapacity; i++)
            {
                bs.Fighters[i] = Settings.Characters[Settings.Party[i]];
                bs.Fighters[i].Sprite = new Sprite(bs.camera, new List<string>() { bs.Fighters[i].StanceSpriteName });
                
                Game.Components.Add(bs.Fighters[i].Sprite);
                bs.FightersFace[i] = new Sprite(bs.WoT[0], new List<string>() { bs.Fighters[i].faceName });
                bs.Fighters[i].Position = this.playerSprite.Position + Constants.ArrowStrategy[i];
                Game.Components.Add(bs.FightersFace[i]);

                bs.Fighters[i].Sprite.SetAnimation(bs.Fighters[i].StanceSpriteName);
                bs.FightersFace[i].SetAnimation(bs.Fighters[i].faceName);

                j = i;
            }
            j++;
            Vector2 EnnemyPosition = this.playerSprite.Position + new Vector2(400, 0);
            for (int i = 0; i < EnnemyID.Count && i < Constants.BattleEnnemiesCapacity; i++)
            {
                bs.Fighters[i + j] = (Logic.Ennemy)GameData.ennemies[EnnemyID[i]].GetCopy(i.ToString());
                bs.Fighters[i + j].Sprite = new Sprite(bs.camera, new List<string>() { bs.Fighters[i + j].StanceSpriteName });
                Game.Components.Add(bs.Fighters[i + j].Sprite);
                bs.FightersFace[i + j] = new Sprite(bs.WoT[0], new List<string>() { bs.Fighters[i + j].faceName });
                Game.Components.Add(bs.FightersFace[i + j]);
                bs.Fighters[i + j].Sprite.SetAnimation(bs.Fighters[i + j].StanceSpriteName);
                bs.FightersFace[i + j].SetAnimation(bs.Fighters[i + j].faceName);
                bs.Fighters[i + j].Position = EnnemyPosition + Constants.LineStrategy[i];
            }

        }
        public override void Destroy()
        {
            if (backgroundMusic != null)
                backgroundMusic.Stop(AudioStopOptions.Immediate);
            base.Destroy();
        }
    }
}
