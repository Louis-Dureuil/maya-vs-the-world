/* Fiat Lux V0.2.14
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FiatLux.Logic;

namespace FiatLux.Scenes
{
    public class BattleEventArgs : EventArgs
    {
        public Fighter fighter;
        public BattleEventArgs(Fighter fighter)
        {
            this.fighter = fighter;
        }
    }
    public class EndBattleEventArgs : EventArgs
    {
        public bool isVictory;
        public EndBattleEventArgs(bool isVictory)
        {
            this.isVictory = isVictory;
        }
    }
    public class BattleScene : Scene
    {
        internal SoundBank MusicSoundBank;
        WaveBank MusicWaveBank;
        protected Effect effect;

        Texture2D pauseTexture;

        public BattleScene(LuxGame game)
            : base(game)
        {

        }
        public BattleScene(Scene parent)
            : base(parent)
        {
        }

        #region Constants
        const int ONSCREENXPGAUGES = 8;
        const int DIFFICULTY = 1;
        const float IDLESPEED = 20.0F;
       
        const float PLAYERSPEED = 0.20F; //2 AP in 10 seconds.
        const float FASTFORWARDSPEED = 60.0F;
        const float BEGINTURNDURATION = 1.0F;
        const float CONFIGUREDURATION = 15.0F; //Temporary Decision. Will later be skill-dependant
        const float DAMAGEDISPLAYDURATION = 2.0F;
        const float MAYREPEATWORD = 10.0F;
        const int REPEATWORDPROB = 30;
        const float PERFECTREACTIONTIMING = 0.5F;

        internal Sprite[] FightersFace = new Sprite[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];

        readonly float[,] criticEfficiencyTab = new float[5,5] { {0,0,0,0,0},{0.66F,0.5F,0.45F,0.38F,0.33F},{1.1F,1,0.95F,0.85F,0.7F},{1.6F,1.5F,1.45F,1.38F,1.33F},{2,2,2,2,2} } ;
        readonly int[,] criticProbTab = new int[5, 5] { {2, 5, 8, 10, 15}, {5, 10, 15, 18, 25}, {70, 70, 70, 70, 60}, {15, 10, 5, 2, 0}, {8, 5,2,0,0} };
        readonly string[] criticMessages = new string[5] { "Raté!", "Pas d'bol!", "", "Coup d'pot!", "Chance!" };

        public readonly int[,] reactionRankProbTab = new int[7, 5] { { 100, 90, 80, 70, 60 }, { 95, 85, 75, 65, 55 }, { 0, 90, 65, 55, 45 }, { 0, 0, 80, 35, 25 }, { 0, 0, 0, 70, 1 }, { 0, 0, 0, 0, 60 }, { 0, 0, 0, 0, 0 } };
        public readonly int[,] reactionRankMinProbTab = new int[7, 5] { { 90, 80, 65, 55, 40 }, { 80, 65, 50, 40, 30 }, { 0, 80, 50, 35, 20 }, { 0, 0, 65, 10, 1 }, { 0, 0, 0, 40, 1 }, { 0, 0, 0, 0, 25 }, { 0, 0, 0, 0, 0 } };
        public readonly int[,] reactionRankMaxProbTab = new int[7, 5] { { 110, 100, 95, 85, 80 }, { 110, 105, 100, 90, 80 }, { 0, 100, 80, 75, 70 }, { 0, 0, 95, 60, 49 }, { 0, 0, 0, 100, 1 }, { 0, 0, 0, 0, 95}, { 0, 0, 0, 0, 0 } }; 
        
        #endregion

        #region Delegate
        public delegate void OnInitializing(object sender, EventArgs e);

        public delegate void OnTurnEnding(object sender, EventArgs e);

        public delegate void OnFighterHavingInitiative(object sender, BattleEventArgs e);

        public delegate void OnTurnStarting(object sender, EventArgs e);

        public delegate void OnConfiguring(object sender, BattleEventArgs e);

        public delegate void OnAttacking(object sender, BattleEventArgs e);

        public delegate void OnPerformingAction(object sender, BattleEventArgs e);

        public delegate void OnReacting(object sender, BattleEventArgs e);

        public delegate void OnBattleEnding(object sender, EndBattleEventArgs e);

        #endregion

        #region Events
        public event OnConfiguring Configuring;
        public event OnConfiguring Configured;


        public event OnTurnEnding TurnEnding;
        public event OnTurnEnding TurnEnded;

        public event OnFighterHavingInitiative FighterHavingInitiative;
        public event OnFighterHavingInitiative FighterHasInitiative;


        public event OnReacting Reacting;
        public event OnReacting Reacted;


        public event OnPerformingAction PerformingAction;
        public event OnPerformingAction PerformedAction;

        public event OnInitializing Initializing;
        public event OnInitializing Initialized;

        public event OnTurnStarting TurnStarting;
        public event OnTurnStarting TurnStarted;

        public event OnAttacking Attacking;
        public event OnAttacking Attacked;

        public event OnBattleEnding BattleEnding;
        public event OnBattleEnding BattleEnded;

        #endregion

        #region Fields

        #region Scenes

        internal List<BattleExpGauge> ExpGauges = new List<BattleExpGauge>();

        protected Scenes.BattleMenuScene battleMenu;

        protected Scenes.BattleConfigurationMenuScene battleConfigurationMenu;

        internal Sprite background;

        internal Sprite[] WoT;

        internal Camera camera;

        #region Bars
        LuxEngine.Bar[] EndBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity];
        LuxEngine.Bar[] MPBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity];
        LuxEngine.Bar[] UnlimitedBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity];
        LuxEngine.Bar[] APBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity];
        LuxEngine.Bar[] EnnemyEndBars = new LuxEngine.Bar[Constants.EnnemiesCapacity];
        LuxEngine.Bar[] CastBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        #endregion

        #region Windows
        protected LuxEngine.TextWindow[] statusWindows = new LuxEngine.TextWindow[Constants.BattleCharactersCapacity] ;
        protected LuxEngine.TextWindow[] APWindows = new LuxEngine.TextWindow[Constants.BattleCharactersCapacity] ;
        protected LuxEngine.MessageWindow messageWindow;
        protected LuxEngine.MessageWindow TargetWindow;
        protected LuxEngine.MessageWindow[] AttackNameWindow = new LuxEngine.MessageWindow[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        LuxEngine.MessageWindow APWindow;
        LuxEngine.MessageWindow PauseWindow;
        protected LuxEngine.MenuWindow PauseMenu;
        #endregion

        #endregion

        #region Audio
        // Audio objects. Uncomment whereever music is supported.
        protected Cue WordCue;
        internal Cue MusicCue;
        protected Cue AttackerCue;
        protected Cue TargetCue;
        protected Cue OtherCue;
            

        #region AudioLevel

        protected bool MusicEnabled = true;
        bool SoundEnabled = true;
        /// <summary>
        /// 0: Enable dialog voices -- voices in the scenario
        /// 1: Enable ambiance voices -- voices when attacking/attacked
        /// 2: Enable custom voices -- voices that are specific to the current battle
        /// 3: Enable random voices -- voices that appear randomly
        /// </summary>
        protected bool[] VoiceEnabled = new bool[4] { false, true, true, true };  

        #endregion

        #endregion

        #region Display
        List<BattleDamageScene> DisplayDamageList = new List<BattleDamageScene>();
        protected string message = "";
        float CenterRotation = 0.0F;
        float Rotation = 0.0F;
        Sprite arrow;

        #endregion

        #region States and Switches
        WoTState woTState;
        public BattleState State;
        public BattleState resumePauseState;
        #endregion

        #region Logic
        internal float EnnemyMult;
        float customSpeed = IDLESPEED;
        protected float AP = 0.0F;
        float OldAP = 0.0F;
        internal int APRatio = 0;
        float Speed = 1.0F;
        float MaxAP = 1.0F; // Must be != 0.
        float WordTimer = 0.0F;
        #endregion

        #region Fighters
        //Fighters.
        internal Fighter[] Fighters = new Fighter[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        //Fighters State.
        public BattleFighterState[] FightersState = new BattleFighterState[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        //Track a duration for each fighter. Ie Command remaining time, preparation remaining time, recovery remaining time, ...
        public float[] FightersTime = new float[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        //Fighters Action ID
        public Skill[] FightersAction = new Skill[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        //Fighters Reaction
        public int[] FightersReaction = new int[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        //Keep track of the ID of the last Actions performed by the fighters.
        int[] FightersLastActionID = new int[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        int[] FightersLastActionCount = new int[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        // Track a duration for each fighter for the remaining reaction time.
        public float[] FightersReactionTime = new float[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        //Fighters Skill Configuration
        public SkillConfiguration[] FightersSkillConfiguration = new SkillConfiguration[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        public int[] FightersReactionRank = new int[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        public float[] FightersRotation = new float[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
        public int[] FightersBattleCommandCount = new int[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity]; //Reaction => Indicates the success prob
        int[] FightersOldAP = new int[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];

        #endregion

        #region Indexes
        //Keep Track of the fighter that is currently in command.
        public int CommandIndex = -1;

        //Keep Track of the fighter that is currently attacking.
        public int ActionIndex = -1;

        //Keep Track of the fighter that is currently reacting.
        public int ReactionIndex = -1;
        protected string pauseMenuChoices = "Resume\nQuit";
        #endregion

        #endregion

        #region Methods

        #region Initialize
       
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization logic here
            if (Initializing != null)
                Initializing(this, new EventArgs());

            Paused += new PausedEventHandler(BattleScene_Paused);
            UnPaused += new UnPausedEventHandler(BattleScene_UnPaused);

            camera = new Camera(this, Vector2.Zero);
            Game.Components.Add(camera);
            camera.DrawOrder = 0;
   
            WoT = new Sprite[2];
            WoT[0] = new Sprite(this, new List<string>() { "WoTAP_old" });
            Game.Components.Add(WoT[0]);
            WoT[0].DrawOrder = 99;
            WoT[1] = new Sprite(WoT[0], new List<string>() { "WoTCenterOrange" });
            Game.Components.Add(WoT[1]);

            WoT[0].SetAnimation("WoTAP_old");
            WoT[1].SetAnimation("WoTCenterOrange");

            WoT[0].Width /= 2.5F;
            WoT[0].Height /= 2.5F;
            WoT[1].Width /= 2.5F;
            WoT[1].Height /= 2.5F;

            messageWindow = new LuxEngine.MessageWindow(this, new Vector2(150,445),"",MessageConfirmType.Timed);
            Game.Components.Add(messageWindow);
            messageWindow.Visible = false;
            messageWindow.Enabled = false;
            messageWindow.DrawOrder = 5;
            APWindow = new MessageWindow(this, new Vector2(0, LuxGame.spriteBatch.GraphicsDevice.Viewport.Height - 160.0F), "AP:", MessageConfirmType.None);
            Game.Components.Add(APWindow);
            APWindow.DrawOrder = 5;
            TargetWindow = new MessageWindow(this, Vector2.Zero, "", MessageConfirmType.OnConfirm);
            TargetWindow.ConfirmAction = Input.Action.BattleConfirm;
            Game.Components.Add(TargetWindow);
            TargetWindow.DrawOrder = 5;

            CenterRotation = (float)Math.PI / 2;
            base.Initialize();

            PauseMenu = new MenuWindow(this, new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(pauseMenuChoices).X / 2 - LuxEngine.Shared.FRAMESIZE / 2, GraphicsDevice.Viewport.Height / 2 + spriteFont.MeasureString("Pause").Y + LuxEngine.Shared.FRAMESIZE * 2), pauseMenuChoices);
            Game.Components.Add(PauseMenu);
            PauseMenu.Visible = false;
            PauseMenu.Enabled = false;
            PauseMenu.DrawOrder = 101;
            PauseMenu.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(PauseMenu_Confirmed);
      
            if (Initialized != null)
                Initialized(this, new EventArgs());

            for (int i = 0; i < Fighters.Length; i++)
            {
                if (Fighters[i] != null)
                {
                    Fighters[i].Stats.ToMax("AP");
                    camera.AddTarget(Fighters[i].Sprite);
                    camera.Mode = CameraMode.Resize;
                    AttackNameWindow[i] = new MessageWindow(Fighters[i].Sprite, Vector2.Zero, "", MessageConfirmType.None);
                    Game.Components.Add(AttackNameWindow[i]);
                    AttackNameWindow[i].BackgroundColor = new Color(255, 255, 255, 10);
                    AttackNameWindow[i].Visible = false;
                    AttackNameWindow[i].WithFrame = false;
                    CastBars[i] = new Bar(Fighters[i].Sprite, new Vector2(100, 2));
                    Game.Components.Add(CastBars[i]);
                    CastBars[i].Position = new Vector2(-50, Fighters[i].Sprite.Height / 2 + 10);
                    CastBars[i].Visible = false;
                    if (Fighters[i] is Character)
                    {
                        statusWindows[i] = new LuxEngine.TextWindow(this, new Rectangle(150 + 155 * i, 0, 150, 90));
                        Game.Components.Add(statusWindows[i]);
                        statusWindows[i].Enabled = true;
                        statusWindows[i].Visible = true;
                        statusWindows[i].BackgroundColor = new Color(255, 255, 255, 125);
                        statusWindows[i].Text = "";
                        statusWindows[i].horizontalAlignment = LuxEngine.HorizontalAlignment.Center;
                        EndBars[i] = new LuxEngine.Bar(statusWindows[i], new Vector2(120, 5));
                        Game.Components.Add(EndBars[i]);
                        EndBars[i].ValueColor = Color.Green;
                        EndBars[i].MaxValue = Fighters[i].Stats["End", true];
                        EndBars[i].Value = Fighters[i].Stats["End"];

                        MPBars[i] = new LuxEngine.Bar(statusWindows[i], new Vector2(120, 5));
                        Game.Components.Add(MPBars[i]);
                        MPBars[i].ValueColor = Color.Blue;
                        MPBars[i].MaxValue = Fighters[i].Stats["MP", true];
                        MPBars[i].Value = Fighters[i].Stats["MP"];

                        UnlimitedBars[i] = new LuxEngine.Bar(statusWindows[i], new Vector2(120, 5));
                        Game.Components.Add(UnlimitedBars[i]);
                        UnlimitedBars[i].ValueColor = Color.Gold;
                        UnlimitedBars[i].MaxValue = Fighters[i].Stats["End", true];
                        UnlimitedBars[i].Value = Fighters[i].unlimitedGauge;

                        EndBars[i].Position = new Vector2(25, 41);
                        MPBars[i].Position = new Vector2(25, 61);
                        UnlimitedBars[i].Position = new Vector2(25, 81);

                        APWindows[i] = new LuxEngine.TextWindow(this, new Vector2(520, 348 + i * 32));
                        Game.Components.Add(APWindows[i]);
                        APWindows[i].BackgroundColor = new Color(255, 255, 255, 125);
                        APWindows[i].Enabled = true;
                        APWindows[i].Visible = true;
                        APBars[i] = new Bar(APWindows[i], new Vector2(10, 5));
                        Game.Components.Add(APBars[i]);

                        APBars[i].Position = new Vector2(6, 21);
                        APBars[i].ValueColor = Color.Red;
                        APBars[i].MaxValue = Fighters[i].Stats["AP", true];
                        APBars[i].Value = Fighters[i].Stats["AP"];
                    }
                }
            }      

            WoT[0].Position += new Vector2(0, GraphicsDevice.Viewport.Height);
            PauseWindow = new MessageWindow(this, new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString("Pause").X / 2 - LuxEngine.Shared.FRAMESIZE / 2, GraphicsDevice.Viewport.Height / 2), "Pause", MessageConfirmType.None);
            Game.Components.Add(PauseWindow);
            PauseWindow.Visible = false;
            PauseWindow.DrawOrder = 101;

            arrow = new Sprite(this, new List<string> { "arrow" });
            Game.Components.Add(arrow);
            arrow.SetAnimation("arrow");
            arrow.Visible = false;
            if (MusicEnabled && MusicCue != null)
            {
                MusicCue.Play();
            }

        }

        #region Events
        void PauseMenu_Confirmed(object sender, MenuWindowEventArgs e)
        {
            switch (e.Choice)
            {
                case "Resume":
                    TooglePause(this, new EventArgs());
                    break;
                case "Quit":
                    if (Parent != null)
                        parent.Destroy();
                    Destroy();
                    Game.Components.Add(new DemoTitleScene(LuxGame));
                    TooglePause(this, new EventArgs());
                    break;
            }
        }

        protected void BattleScene_UnPaused(object sender, EventArgs e)
        {
            State = resumePauseState;
            if (State == BattleState.Configuration && battleConfigurationMenu != null)
            {
                battleConfigurationMenu.Visible = true;
                battleConfigurationMenu.Enabled = true;
            }
            PauseWindow.Visible = false;
            PauseMenu.Enabled = false;
            PauseMenu.Visible = false;
            OtherCue = LuxGame.soundBank.GetCue("ConfirmDisabled");
            OtherCue.Play();
            if (!effect.IsDisposed)
            {
                effect.CurrentTechnique = effect.Techniques["Technique1"];
                effect.Parameters["fTimer"].SetValue(0);
            }
        }

        protected void BattleScene_Paused(object sender, EventArgs e)
        {
            if (battleConfigurationMenu != null)
            {
                battleConfigurationMenu.Visible = false;
                battleConfigurationMenu.Enabled = false;
            }
            foreach (BattleExpGauge expGauge in ExpGauges)
            {
                expGauge.Enabled = false;
            }
            resumePauseState = State;
            PauseWindow.Visible = true;
            PauseMenu.Enabled = true;
            PauseMenu.Visible = true;
            OtherCue = LuxGame.soundBank.GetCue("Confirm");
            OtherCue.Play();
            State = BattleState.Paused;
            effect.Parameters["iSeed"].SetValue(new Random().Next(0, 999));
            effect.Parameters["fNoiseAmount"].SetValue(0.05F);
            effect.Parameters["fTimer"].SetValue(0);
            effect.CurrentTechnique = effect.Techniques["Fog"];
        }
        #endregion
      
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            MusicSoundBank = new SoundBank(LuxGame.engine, "Content\\BattleMusicSoundBank.xsb");
            MusicWaveBank = new WaveBank(LuxGame.engine, "Content\\Music.xwb");
    
            pauseTexture = Content.Load<Texture2D>("ice");
            effect = Content.Load<Effect>("Distorsion");

            GraphicsDevice.Textures[1] = pauseTexture;
            // Create a new LuxGame.spriteBatch, which can be used to draw textures.
            //spriteFont = Content.Load<SpriteFont>("FontTest");
            spriteFont = Content.Load<SpriteFont>("DefaultSpriteFont");
            
            WordCue = Shared.Rand.Next(0,2) == 0 ? LuxGame.soundBank.GetCue("LucienIntro") : LuxGame.soundBank.GetCue("SergentIntro");
            if (VoiceEnabled[2])
                WordCue.Play();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            
        }
        
        #endregion

        #region Update
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {        
            //Update the text windows:
            UpdateTextWindows();

            APRatio = GetAPRatio();

            if (Input.isActionDone(Input.Action.Pause, false))
            {
                TooglePause(this, new EventArgs());
            }
            
            if (!isPaused)
            {
                //Update the XP Gauges
                UpdateExpGauges();
        
                UpdateWoT(gameTime);
                if (AP > 0)
                {
                    UpdateState();
                }
                else
                {
                    EndTurn();
                }
            }
            else
            {
                //Frame duration, in s (precision 10^(-3) s.)
                float FrameDuration = Shared.FrameDuration;
                //DIRTY: update the effect:
                effect.Parameters["fTimer"].SetValue(effect.Parameters["fTimer"].GetValueSingle() + FrameDuration /100);
            }
            base.Update(gameTime);
        }

        private int GetAPRatio()
        {
            int AP = 0;
            int FighterCount = 0;
            foreach (Fighter fighter in Fighters)
            {
                if (fighter != null && !fighter.isDead)
                {
                    AP += fighter.Stats["AP", true];
                    FighterCount++;
                }
            }
            AP = (int)Math.Ceiling((float)AP / (float)FighterCount);
            return AP;
        }

        private void UpdateTextWindows()
        {
            for (int i = 0; i < statusWindows.Length; i++)
            {
                if (statusWindows[i] != null)
                {
                    statusWindows[i].Text = Fighters[i].Name + "\n" + Fighters[i].Stats.ToString("End", true) + "\n" + Fighters[i].Stats.ToString("MP", false) + "\n" + "U.: " + Fighters[i].unlimitedGauge.ToString();
                }
                if (APWindows[i] != null)
                {
                    APWindows[i].Text = Fighters[i].Name + " " + Fighters[i].Stats.ToString("AP", true);
                    APWindows[i].TextColor = i == CommandIndex ? Color.Blue : i == ReactionIndex ? Color.Red : (FightersState[i] == BattleFighterState.Recovering ? Color.Gray : Color.White);
                    if (APBars[i] != null)
                        APBars[i].Size = new Vector2(spriteFont.MeasureString(APWindows[i].Text).X, 5);
                }
            }

            if (message == "")
            {
                messageWindow.Text = "";
                messageWindow.Enabled = false;
                messageWindow.Visible = false;
            }
            else if (message + "\r\n" != messageWindow.Text)
            {
                messageWindow.Text = message;
                messageWindow.Enabled = true;
                messageWindow.Visible = true;
            }
        }

        internal void UpdateExpGauges()
        {
            for (int i = 0; i < ExpGauges.Count; i++)
            {
                if (i < ONSCREENXPGAUGES)
                {
                    if (i == 0)
                    {
                        ExpGauges[i].Enabled = true;
                    }
                    else
                    {
                        ExpGauges[i].Enabled = false;
                    }
                    ExpGauges[i].Visible = true;
                    ExpGauges[i].Position = new Vector2(GraphicsDevice.Viewport.Width - 170, 120 + i * 24);
                    ExpGauges[i].DrawOrder = camera.DrawOrder + 20;
                }
                else
                {
                    ExpGauges[i].Enabled = false;
                    ExpGauges[i].Visible = false;
                }
                if (ExpGauges[i].HasExpired)
                {
                    ExpGauges[i].Visible = false;
                    ExpGauges[i].Enabled = false;
                    ExpGauges[i].Destroy();
                    ExpGauges.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateState()
        {
            switch (State)
            {
                case BattleState.Paused:
                    woTState = WoTState.Stopped;
                    break;
                case BattleState.WinBattle:
                    woTState = WoTState.Stopped;
                    EndBattle();
                    break;
                case BattleState.LoseBattle:
                    woTState = WoTState.Stopped;
                    EndBattle();
                    break;
                case BattleState.Idle:
                    bool isFastForward = true;
                    for (int i = 0; i < Fighters.Length; i++)
                    {
                        if (Fighters[i] != null)
                        {
                            if (FightersState[i] == BattleFighterState.Running)
                            {
                                isFastForward = false;
                            }
                            if (FightersState[i] == BattleFighterState.Command)
                            {
                                State = BattleState.Command;
                                if (FighterHavingInitiative != null)
                                    FighterHavingInitiative(this, new BattleEventArgs(Fighters[i]));
                            }
                            if (FightersState[i] == BattleFighterState.Attacking && State != BattleState.Command)
                                State = BattleState.Action;
                        }
                    }
                    if (isFastForward)
                        woTState = WoTState.FastForward;
                    else
                        woTState = WoTState.Idle;
                    break;
                case BattleState.StartTurn:
                    if (AP < MaxAP)
                        woTState = WoTState.Reverse;
                    else
                    {
                        State = BattleState.Idle;
                        if (TurnStarted != null)
                        {
                            TurnStarted(this, new EventArgs());
                        }
                    }
                    break;
                case BattleState.Command:
                    woTState = WoTState.Player;
                    if (CommandIndex < 0)
                    {
                        State = BattleState.Idle;
                        for (int i = 0; i < Fighters.Length; i++)
                        {
                            if (Fighters[i] != null && FightersState[i] == BattleFighterState.Command)
                            {
                                CommandIndex = i;
                                arrow.Parent = Fighters[i].Sprite;
                                arrow.SpriteColor = Color.Blue;
                                arrow.Visible = true;
                                arrow.DrawOrder = arrow.Parent.DrawOrder + 1;
                                arrow.Position = Vector2.Zero;
                                arrow.Position.Y -= ((Sprite)arrow.Parent).Height / 2 + 8;
                                if (Fighters[i] is Character)
                                {
                                    AddExpGauge((Character)(Fighters[i]), (int)Math.Ceiling(EnnemyMult), ExpTypes.Base, 0);
                                }
                                State = BattleState.Command;
                                if (FighterHavingInitiative != null)
                                    FighterHavingInitiative(this, new BattleEventArgs(Fighters[i]));
                                break;
                            }
                        }
                    }
                    else if (Fighters[CommandIndex] != null && FightersState[CommandIndex] == BattleFighterState.Command)
                    {
                        if (FightersTime[CommandIndex] >= 0.0F)
                        {
                            int index = CommandIndex;
                            UpdateCommand(false);
                            if (FighterHasInitiative != null)
                            {
                                FighterHasInitiative(this, new BattleEventArgs(Fighters[index]));
                            }
                        }
                        else
                        {
                            FightersState[CommandIndex] = Fighters[CommandIndex].Unlimited ? BattleFighterState.Active : BattleFighterState.Inactive;
                            CommandIndex = -1;
                            arrow.Visible = false;
                            battleMenu.Visible = false;
                            battleMenu.Enabled = false;
                        }
                    }

                    break;
                case BattleState.Configuration:
                    woTState = WoTState.Player;
                    if (CommandIndex < 0)
                    {
                        State = BattleState.Idle;
                        for (int i = 0; i < Fighters.Length; i++)
                        {
                            if (Fighters[i] != null && FightersState[i] == BattleFighterState.Configure)
                            {
                                CommandIndex = i;
                                State = BattleState.Configuration;
                                if (Configuring != null)
                                    Configuring(this, new BattleEventArgs(Fighters[i]));
                                break;
                            }
                        }
                    }
                    else if (Fighters[CommandIndex] != null && FightersState[CommandIndex] == BattleFighterState.Configure)
                    {
                        if (FightersTime[CommandIndex] >= 0.0F)
                        {
                            int index = CommandIndex;
                            UpdateConfigure();
                            if (Configured != null)
                                Configured(this, new BattleEventArgs(Fighters[index]));
                        }
                        else
                        {
                            FightersState[CommandIndex] = Fighters[CommandIndex].Unlimited ? BattleFighterState.Active : BattleFighterState.Inactive;
                            CommandIndex = -1;
                            arrow.Visible = false;
                            battleMenu.Enabled = false;
                            battleMenu.Visible = false;
                            battleConfigurationMenu.Enabled = false;
                            battleConfigurationMenu.Visible = false;
                            battleConfigurationMenu.Destroy();
                            if (TargetWindow != null)
                            {
                                TargetWindow.Enabled = false;
                                TargetWindow.Destroy();
                            }
                        }
                    }
                    break;
                case BattleState.Action:
                    {
                        woTState = WoTState.Player;
                        if (ActionIndex < 0)
                        {
                            State = BattleState.Idle;
                            for (int i = 0; i < Fighters.Length; i++)
                            {
                                if (Fighters[i] != null && FightersState[i] == BattleFighterState.Attacking)
                                {
                                    ActionIndex = i;
                                    if (FightersState[FightersSkillConfiguration[ActionIndex].parameters[0]] != BattleFighterState.Recovering
                                        && FightersState[FightersSkillConfiguration[ActionIndex].parameters[0]] != BattleFighterState.Casting
                                        && FightersState[FightersSkillConfiguration[ActionIndex].parameters[0]] != BattleFighterState.Disabled
                                        && !Fighters[FightersSkillConfiguration[ActionIndex].parameters[0]].isDead
                                        && !Fighters[FightersSkillConfiguration[ActionIndex].parameters[0]].isStun)
                                        FightersReactionTime[FightersSkillConfiguration[ActionIndex].parameters[0]] += Constants.COMMANDDURATION;
                                    {
                                        State = BattleState.Action;
                                        if (Attacking != null)
                                        { Attacking(this, new BattleEventArgs(Fighters[ActionIndex])); }
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (Fighters[ActionIndex].IsCloseTo(Fighters[FightersSkillConfiguration[ActionIndex].parameters[0]].Position, FightersAction[ActionIndex].Range))
                            {
                                int index = ActionIndex;
                                if (ReactionIndex < 0 && FightersReactionTime[FightersSkillConfiguration[ActionIndex].parameters[0]] > 0)
                                {
                                    ReactionIndex = FightersSkillConfiguration[ActionIndex].parameters[0]; // Seems pretty useless for now.
                                    // Will be better when the attacks will allow multiple targets.
                                    arrow.Parent = Fighters[ReactionIndex].Sprite;
                                    arrow.SpriteColor = Color.Red;
                                    arrow.Visible = true;
                                    arrow.DrawOrder = arrow.Parent.DrawOrder + 1;
                                    arrow.Position = Vector2.Zero;
                                    arrow.Position.Y -= ((Sprite)arrow.Parent).Height / 2 + 8;
                                    AttackNameWindow[ActionIndex].TextColor = Color.Red;
                                    AttackNameWindow[ActionIndex].DrawOrder = background.DrawOrder + 3;                                    
                                }
                                else if (ReactionIndex < 0) // If it remains < 0.
                                {
                                    // All the targets reacted.
                                    // Perform the attack if the Fighter is not stunned.
                                    if (!Fighters[ActionIndex].isStun)
                                    {
                                        AttackNameWindow[ActionIndex].Visible = false;
                                        PerformAction(ActionIndex);
                                    }
                                    else
                                        woTState = WoTState.Idle; // FIX: The WoT was slow w/ a stunned attacker.
                                }
                                else
                                {
                                    if (FightersReactionTime[ReactionIndex] > 0.0F)
                                    {
                                        if (Reacting != null)
                                            Reacting(this, new BattleEventArgs(Fighters[index]));
                                        UpdateCommand(true);
                                        if (Reacted != null)
                                            Reacted(this, new BattleEventArgs(Fighters[index]));
                                    }
                                    else
                                    {
                                        ReactionIndex = -1;
                                        arrow.Visible = false;
                                        battleMenu.Visible = false;
                                        battleMenu.Enabled = false;
                                    }
                                }
                                if (Attacked != null)
                                {
                                    Attacked(this, new BattleEventArgs(Fighters[index]));
                                }
                            }
                            else
                            {
                                FightersState[ActionIndex] = BattleFighterState.Running;
                                AttackNameWindow[ActionIndex].TextColor = Color.White;
                                AttackNameWindow[ActionIndex].DrawOrder = background.DrawOrder + 2;

                                ActionIndex = -1;

                                if (ReactionIndex > 0) FightersReactionTime[ReactionIndex] = 0.0F;
                                ReactionIndex = -1;
                                arrow.Visible = false;
                                battleMenu.Visible = false;
                                battleMenu.Enabled = false;
                            }
                        }
                        break;
                    }
            }
        }

        // 3 possible outputs:
        // A: The Fighter is fast enough to choose a command and start configuration.
        // B: Same as A, but with a magic: start casting.
        // C: The Fighter is slow and gets disabled.
        private void UpdateCommand(bool isReaction)
        {
            int index;
            if (isReaction)
                index = ReactionIndex;
            else
                index = CommandIndex;
            if (Fighters[index] is Ennemy)
            {
                if (!isReaction)
                {
                    FightersAction[CommandIndex] = (Skill)GameData.commands[((Ennemy)Fighters[CommandIndex]).MakeActionDecision(Fighters)];
                    FightersAction[CommandIndex].InitializeConfiguration(ref FightersSkillConfiguration[CommandIndex]);
                    if (FightersAction[CommandIndex].Type == Skill.SkillType.Physical || FightersAction[CommandIndex].Type == Skill.SkillType.Special)
                    {
                        FightersState[CommandIndex] = BattleFighterState.Configure;
                        State = BattleState.Configuration;
                        FightersTime[CommandIndex] = CONFIGUREDURATION;
                    }
                    else if (FightersAction[CommandIndex].Type == Skill.SkillType.Magical)
                    {
                        FightersState[CommandIndex] = BattleFighterState.Casting;
                        State = BattleState.Idle;
                        FightersTime[CommandIndex] = FightersAction[CommandIndex].CastTime;
                    }
                }
                else
                {
                    Random rand = Shared.Rand;
                    FightersReaction[index] = ((Ennemy)Fighters[index]).MakeReactionDecision(Fighters[ActionIndex], FightersAction[ActionIndex], FightersSkillConfiguration[ActionIndex]);
                    FightersReactionTime[index] = rand.Next(0, (int)Constants.COMMANDDURATION);
                    FightersReactionRank[index] = rand.Next(1, 6);

                    int ActionRank = 0;
                    int i = 0;
                    

                    if (FightersReaction[index] == 7)
                    {
                        i = 1;
                        ActionRank = Math.Min(FightersAction[ActionIndex].ParryRank, FightersAction[ActionIndex].DodgeRank);
                    }
                    if (FightersReaction[index] == 8)
                    {
                        i = 2;
                        ActionRank = FightersAction[ActionIndex].ParryRank;
                    }
                    if (FightersReaction[index] == 9)
                    {
                        ActionRank = FightersAction[ActionIndex].DodgeRank;
                        i = 3;
                    }
                    if (FightersReaction[index] == 10)
                    {
                        i = 4;
                        ActionRank = Math.Max(FightersAction[ActionIndex].DodgeRank, FightersAction[ActionIndex].ParryRank);
                    }

                    float realTime = Constants.COMMANDDURATION - PERFECTREACTIONTIMING * ActionRank;
                    double timeRatio = Math.Sqrt(FightersReactionTime[ReactionIndex] / realTime);
                    

                    if (i != 0 && realTime <= FightersReactionTime[ReactionIndex] && FightersReactionRank[ReactionIndex] == ActionRank)
                    {
                        FightersBattleCommandCount[ReactionIndex] = 100;
                    }
                    else if (i != 0)
                    {
                        int Count = 0;
                        // Pre vs. Speed
                        int Pre = Fighters[ActionIndex].Stats["Pre"];
                        int Speed = Fighters[ReactionIndex].Stats["Speed"];
                        // Energy vs. Per
                        int Energy = Fighters[ActionIndex].Stats["STR2"];
                        int Per = Fighters[ReactionIndex].Stats["Per"];


                        int ProbMin = reactionRankMinProbTab[ActionRank, FightersReactionRank[ReactionIndex] - 1];
                        int ProbMax = reactionRankMaxProbTab[ActionRank, FightersReactionRank[ReactionIndex] - 1];
                        if (i == 2)
                        {
                            Count = (int)Math.Ceiling(((ProbMin * Energy + ProbMax * Per) / (Energy + Per)) * timeRatio);
                        }
                        else if (i == 3)
                        {
                            Count = (int)Math.Ceiling(((ProbMin * Pre + ProbMax * Speed) / (Pre + Speed)) * timeRatio);
                        }
                        else if (i == 4)
                        {
                            Count = (int)Math.Ceiling((Math.Ceiling(((ProbMin * Energy * 3 + ProbMax * Per * 2) / ((Energy + Per) * 4)) * timeRatio) + Math.Ceiling(((ProbMin * Pre * 3 + ProbMax * Speed * 2) / ((Pre + Speed) * 4)) * timeRatio)) / 2);
                        }
                        if (Count >= 95)
                        {
                            FightersBattleCommandCount[ReactionIndex] = 95;
                        }
                        else
                        {
                            FightersBattleCommandCount[ReactionIndex] = Count;
                        }
                        if (i == 1)
                        {
                            FightersBattleCommandCount[ReactionIndex] = 100;
                        }
                    }
                    FightersReactionTime[index] = 0;
                }
            }
            else
            {
                if (battleMenu == null || !battleMenu.Enabled)
                {
                    if (battleMenu != null)
                    {
                        battleMenu.Destroy();
                    }
                    OtherCue = LuxGame.soundBank.GetCue("Select");
                    OtherCue.Play();
                    battleMenu = new Scenes.BattleMenuScene(this, Fighters[isReaction ? ReactionIndex : CommandIndex], isReaction);
                    Game.Components.Add(battleMenu);
                    battleMenu.Position = new Vector2(250, 380);
                    battleMenu.Enabled = true;
                    battleMenu.Visible = false;
                    battleMenu.DrawOrder = messageWindow.DrawOrder + 1;
                }
                else if (!battleMenu.Visible)
                    battleMenu.Visible = true;
            }
        }

        // 3 possible outputs, depending on the input:
        // A: The Fighter chose a physical Action, and is fast enough to finish configuration. The Fighter starts running.
        // B: The Fighter chose a magical Action, and is fast enough to finish configuration. The Fighter immediatly attacks.
        // C: The Fighter is slow and gets disabled.
        private void UpdateConfigure()
        {
            if (Fighters[CommandIndex] is Ennemy)
            {
                if (FightersAction[CommandIndex].Type == Skill.SkillType.Physical || FightersAction[CommandIndex].Type == Skill.SkillType.Special)
                {
                    FightersState[CommandIndex] = BattleFighterState.Running;
                    FightersSkillConfiguration[CommandIndex].parameters[0] = ((Ennemy)Fighters[CommandIndex]).ChooseSingleTarget(this);
                    AttackNameWindow[CommandIndex].Visible = true;
                    AttackNameWindow[CommandIndex].TextColor = Color.White;
                    AttackNameWindow[CommandIndex].DrawOrder = background.DrawOrder + 2;
                    AttackNameWindow[CommandIndex].Text = FightersAction[CommandIndex].Name;
                    AttackNameWindow[CommandIndex].Position.X = -spriteFont.MeasureString(FightersAction[CommandIndex].Name).X / 2 - LuxEngine.Shared.FRAMESIZE / 2;
                    AttackNameWindow[CommandIndex].Position.Y = -((Sprite)AttackNameWindow[CommandIndex].Parent).Height;            
                }
                else
                {
                    PerformAction(CommandIndex);
                    FightersState[CommandIndex] = Fighters[CommandIndex].Unlimited ? BattleFighterState.Active : BattleFighterState.Inactive;
                }
                State = BattleState.Idle;
                CommandIndex = -1;
                arrow.Visible = false;
            }
            else
            {
                if (battleConfigurationMenu == null || !battleConfigurationMenu.Enabled)
                {
                    if (battleConfigurationMenu != null)
                    {
                        battleConfigurationMenu.Destroy();
                    }
                    battleConfigurationMenu = new Scenes.BattleConfigurationMenuScene(this);
                    Game.Components.Add(battleConfigurationMenu);
                    battleConfigurationMenu.Confirmed += new BattleConfigurationMenuScene.OnConfirmation(battleConfigurationMenu_Confirmed);
                    battleConfigurationMenu.Enabled = true;
                    battleConfigurationMenu.Visible = false;
                    battleConfigurationMenu.DrawOrder = messageWindow.DrawOrder + 1;
                }
                else if (!battleConfigurationMenu.Visible)
                    battleConfigurationMenu.Visible = true;

                // set up the target.
                switch (FightersAction[CommandIndex].targetMode)
                {
                    case TargetMode.All:
                        // Nothing to do for the player, everyone is now a target.
                        // Fighters = Targets when multiple targets implemented.
                        break;
                    case TargetMode.Allies:
                        // foreach fighter in Fighters, if is Chara => Target.Add(fighter)
                        // will be done when multiple targets implemented.
                        break;
                    case TargetMode.Area:
                        // allow the player to set up where the area should be.
                        // Calculate the collision with the target.
                        // Calculate whether the target enters the area, with what intensity
                        // according to whether it is an ally or an ennemy.
                        // will be done when multiple targets implemented.
                        break;
                    case TargetMode.Ennemies:
                        // foreach fighter in Fighters, if is Ennemy => Target.Add(fighter)
                        // will be done when multipe targets implemented.
                        break;
                    case TargetMode.Self:
                        FightersSkillConfiguration[CommandIndex].parameters[0] = CommandIndex;
                        break;
                    case TargetMode.Single:
                        // Allow the player to choose the target ammong fighters.
                        if (!TargetWindow.Enabled)
                        {
                            // Default is the first Ennemy for an attack skill,
                            // And the first ally for a support skill.
                            // For now, it is the first ennemy.
                            int PreviousTarget = FightersSkillConfiguration[CommandIndex].parameters[0];
                            int CurrentTarget = FightersSkillConfiguration[CommandIndex].parameters[0];
                            if (Fighters[PreviousTarget] == null || Fighters[PreviousTarget] is Character
                                || Fighters[PreviousTarget].isDead)
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while ((Fighters[CurrentTarget] == null || Fighters[CurrentTarget] is Character || Fighters[CurrentTarget].isDead)  && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }
                            FightersSkillConfiguration[CommandIndex].parameters[0] = CurrentTarget;
                            TargetWindow.Parent = Fighters[CurrentTarget].Sprite;
                            TargetWindow.Text = Fighters[CurrentTarget].Name;
                            TargetWindow.Position = new Vector2(-spriteFont.MeasureString(Fighters[CurrentTarget].Name).X / 2 - LuxEngine.Shared.FRAMESIZE / 2, -Fighters[CurrentTarget].Sprite.Height);
                            TargetWindow.Enabled = true;
                            TargetWindow.Visible = true;
                        }
                        // Left-Right is used to change side (ie attacking allies or ennemies)
                        if (Input.isActionDone(Input.Action.BattleMenuLeft, false) || Input.isActionDone(Input.Action.BattleMenuRight, false))
                        {
                            int PreviousTarget = FightersSkillConfiguration[CommandIndex].parameters[0];
                            int CurrentTarget = FightersSkillConfiguration[CommandIndex].parameters[0];
                            if (Fighters[CurrentTarget] is Ennemy)
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget].isDead || Fighters[CurrentTarget] is Character && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }
                            else
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget].isDead || Fighters[CurrentTarget] is Ennemy && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }

                            FightersSkillConfiguration[CommandIndex].parameters[0] = CurrentTarget;
                            TargetWindow.Parent = Fighters[CurrentTarget].Sprite;
                            TargetWindow.Text = Fighters[CurrentTarget].Name;

                            OtherCue = LuxGame.soundBank.GetCue("Select");
                            OtherCue.Play();
               

                            TargetWindow.Position = new Vector2(-spriteFont.MeasureString(Fighters[CurrentTarget].Name).X / 2 - LuxEngine.Shared.FRAMESIZE / 2, -Fighters[CurrentTarget].Sprite.Height);
                            
                        }

                        // Up-Down is used to change target inside a side (attacking another ennemy among the ennemies)
                        if (Input.isActionDone(Input.Action.BattleMenuUp, false) || Input.isActionDone(Input.Action.BattleMenuDown, false))
                        {
                            int PreviousTarget = FightersSkillConfiguration[CommandIndex].parameters[0];
                            int CurrentTarget = FightersSkillConfiguration[CommandIndex].parameters[0];
                            if (Fighters[CurrentTarget] is Ennemy)
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget].isDead || Fighters[CurrentTarget] is Ennemy && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }
                            else
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget].isDead || Fighters[CurrentTarget] is Character && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }

                            OtherCue = LuxGame.soundBank.GetCue("Select");
                            OtherCue.Play();
               

                            FightersSkillConfiguration[CommandIndex].parameters[0] = CurrentTarget;
                            TargetWindow.Parent = Fighters[CurrentTarget].Sprite;
                            TargetWindow.Text = Fighters[CurrentTarget].Name;
                            TargetWindow.Position = new Vector2(-spriteFont.MeasureString(Fighters[CurrentTarget].Name).X / 2 - LuxEngine.Shared.FRAMESIZE / 2, -Fighters[CurrentTarget].Sprite.Height);
                        }
                        break;
                    default:
                        //Do nothing.
                        break;
                }

                // If Enter is pressed, then the Configure State is Over
                // The fighter may start running.
                if (Input.isActionDone(Input.Action.BattleConfirm, false))
                {
                    FightersState[CommandIndex] = BattleFighterState.Running;
                    AttackNameWindow[CommandIndex].TextColor = Color.White;
                    AttackNameWindow[CommandIndex].DrawOrder = background.DrawOrder + 2;

                    AttackNameWindow[CommandIndex].Visible = true;
                    AttackNameWindow[CommandIndex].Text = FightersAction[CommandIndex].Name;
                    AttackNameWindow[CommandIndex].Position.X = -spriteFont.MeasureString(FightersAction[CommandIndex].Name).X / 2 - LuxEngine.Shared.FRAMESIZE / 2;
                    AttackNameWindow[CommandIndex].Position.Y = -((Sprite)AttackNameWindow[CommandIndex].Parent).Height;            
                    CommandIndex = -1;
                    arrow.Visible = false;
                    OtherCue = LuxGame.soundBank.GetCue("Confirm");
                    OtherCue.Play();
               
                    battleMenu.Visible = false;
                    battleMenu.Enabled = false;
                    battleConfigurationMenu.Visible = false;
                    battleConfigurationMenu.Enabled = false;
                    this.Remove(battleMenu);
                    this.Remove(battleConfigurationMenu);
                }
            }
        }

        void battleConfigurationMenu_Confirmed(object sender, EventArgs e)
        {
            if (FightersAction[CommandIndex].Name == "Move") // !!! Very DIRTY! MUST be replaced with an event.
            {
                FightersState[CommandIndex] = BattleFighterState.Running;
                AttackNameWindow[CommandIndex].TextColor = Color.White;
                AttackNameWindow[CommandIndex].DrawOrder = background.DrawOrder + 2;
                AttackNameWindow[CommandIndex].Visible = true;
                AttackNameWindow[CommandIndex].Text = FightersAction[CommandIndex].Name;
                AttackNameWindow[CommandIndex].Position.X = -spriteFont.MeasureString(FightersAction[CommandIndex].Name).X / 2 - LuxEngine.Shared.FRAMESIZE / 2;
                AttackNameWindow[CommandIndex].Position.Y = -((Sprite)AttackNameWindow[CommandIndex].Parent).Height;
                CommandIndex = -1;
                arrow.Visible = false;
                OtherCue = LuxGame.soundBank.GetCue("Confirm");
                OtherCue.Play();

                battleMenu.Visible = false;
                battleMenu.Enabled = false;
                battleConfigurationMenu.Visible = false;
                battleConfigurationMenu.Enabled = false;
                battleConfigurationMenu.Destroy();
                this.Remove(battleMenu);
            }
        }

        /// <summary>
        /// Update the Wheel of Time AP amount and its rotation.
        /// </summary>
        /// <param name="gameTime">Provide a snapshot of timing values.</param>
        void UpdateWoT(GameTime gameTime)
        {
            //Frame duration, in s (precision 10^(-3) s.)
            float FrameDuration = Shared.FrameDuration;

            //Update the wordTimer
            if (VoiceEnabled[3])
            {
                WordTimer += FrameDuration;
                if (WordTimer > MAYREPEATWORD)
                {
                    WordTimer = 0.0F;
                    Random rand = Shared.Rand;
                    if (rand.Next(0, 100) < REPEATWORDPROB)
                    {
                        int id = rand.Next(0, Fighters.Length);
                        if (Fighters[id] != null && Fighters[id].WordCue != null)
                        {
                            WordCue = LuxGame.soundBank.GetCue(Fighters[id].WordCue);
                            WordCue.Play();
                        }
                    }
                }
            }
            
            //Choose the correct speed regarding the current state.
            switch (woTState)
            {
                case WoTState.Idle:
                    Speed = IDLESPEED;
                    break;
                case WoTState.Player:
                    Speed = PLAYERSPEED;
                    break;
                case WoTState.FastForward:
                    Speed = FASTFORWARDSPEED * MaxAP / 100;
                    break;
                case WoTState.Reverse:
                    Speed = -FASTFORWARDSPEED * MaxAP / 100;
                    break;
                case WoTState.Stopped:
                    Speed = 0.0F;
                    break;
                case WoTState.Custom:
                    Speed = customSpeed;
                    break;
            }

            //Decrease AP of 'Speed' each second.
            OldAP = AP;
            // MaxAP dependant formula AP = Math.Max(0, Math.Min(AP - Speed * FrameDuration * MaxAP / 100, MaxAP));
            AP = Math.Max(0, Math.Min(AP - Speed * FrameDuration, MaxAP));

            //Get the active fighters whose APs are between OldAP and AP.
            for (int i = 0; i < Fighters.Length; i++)
            {
                if (Fighters[i] != null && FightersState[i] == BattleFighterState.Active && Fighters[i].Stats["AP"] >= (int)Math.Floor(AP) && Fighters[i].Stats["AP"] <= (int)Math.Ceiling(OldAP))
                {
                    FightersState[i] = BattleFighterState.Command;
                    FightersTime[i] = Constants.COMMANDDURATION;
                    AP = (float)Fighters[i].Stats["AP"] ; 
                    //Try to fix bug:
                }
                // Ex : AP = 95, OldAP = 100. A1 = 96, A2 = 99.  => A1 et A2 passent à Command, et AP vaut 99.
            }

            // From Here, AP is definitive.

            bool anAllyAlive = false;
            bool anEnnemyAlive = false;
            EnnemyMult = 0.0F;
            
            // Update the AP of the Fighters moving with the Wheel and this the state problem that may occur.
            // Also update the time.
            for (int i = 0; i < Fighters.Length; i++)
            {
                if (Fighters[i] != null)
                {
                    if (Fighters[i] is Character)
                    {
                        EndBars[i].Value = Fighters[i].Stats["End"];
                        EndBars[i].MaxValue = Fighters[i].Stats["End", true];
                        MPBars[i].Value = Fighters[i].Stats["MP"];
                        MPBars[i].MaxValue = Fighters[i].Stats["MP", true];
                        UnlimitedBars[i].Value = Fighters[i].unlimitedGauge;
                        UnlimitedBars[i].MaxValue = Fighters[i].Stats["End", true];
                        APBars[i].Value = Fighters[i].Stats["AP"];
                        APBars[i].MaxValue = Fighters[i].Stats["AP", true];
                    }

                    FightersFace[i].Position = new Vector2(100 * (float)Math.Cos(-FightersRotation[i]), 100 * (float)Math.Sin(-FightersRotation[i]));
                    FightersFace[i].SpriteColor = FightersState[i] == BattleFighterState.Disabled ? Color.Black : (FightersState[i] == BattleFighterState.Inactive ? Color.Gray : (Fighters[i].Unlimited ? Color.Gold : (Fighters[i].isStun ? Color.Red : Color.White)));


                    Fighters[i].Sprite.SpriteColor = Fighters[i].isStun ? Color.Red : (Fighters[i].Unlimited ? Color.Yellow : Color.White);

                    Fighters[i].Update(OldAP - AP);
                    FightersRotation[i] = Math.Max(0, Math.Min((float)Math.PI / 2, (float)Math.PI / 4 + (float)Math.PI / 4 - CenterRotation + 2 * (Fighters[i].Stats["AP"] - (int)Math.Ceiling(AP)) / MaxAP * (float)Math.PI / 4));

                    if (!Fighters[i].isDead)
                        if (Fighters[i] is Character)
                            anAllyAlive = true;
                        else
                        {
                            anEnnemyAlive = true;
                            EnnemyMult += ((Ennemy)Fighters[i]).EnnemyMult;
                        }
                    else
                    {
                        if (Fighters[i] is Ennemy)
                            Fighters[i].Sprite.Visible = false;
                        FightersState[i] = BattleFighterState.Inactive;
                        AttackNameWindow[i].Visible = false;
                         
                    }

                    if (FightersState[i] == BattleFighterState.Inactive)
                    {
                        camera.RemoveTarget(Fighters[i].Sprite);
                    }

                    if (FightersReactionTime[i] > 0 && State != BattleState.Paused)
                        FightersReactionTime[i] -= FrameDuration;
                    if (FightersState[i] == BattleFighterState.Command && Fighters[i].Stats["AP"] < (int)Math.Ceiling(AP))
                        FightersState[i] = BattleFighterState.Active;
                    if (FightersState[i] == BattleFighterState.Casting || FightersState[i] == BattleFighterState.Command
                        || FightersState[i] == BattleFighterState.Configure || FightersState[i] == BattleFighterState.Running
                        || FightersState[i] == BattleFighterState.Recovering)
                    {
                        Fighters[i].Stats["AP"] = (int)Math.Ceiling(AP);
                        switch (FightersState[i])
                        {
                            case BattleFighterState.Casting:
                                if (!Fighters[i].isStun) FightersTime[i] -= OldAP - AP;
                                if (FightersTime[i] <= 0)
                                    FightersState[i] = BattleFighterState.Configure;
                                break;
                            case BattleFighterState.Command:
                                
                                if (CommandIndex == i && State != BattleState.Paused)
                                {
                                    FightersTime[i] -= FrameDuration;
                                    camera.AddTarget(Fighters[i].Sprite);
                                }
                                else
                                    Fighters[i].Stats["AP"] = (int)Math.Ceiling(OldAP);
                                break;
                            case BattleFighterState.Configure:
                                camera.AddTarget(Fighters[i].Sprite);
                                if (CommandIndex == i && State != BattleState.Paused) // Ajout... Correction de bug ?
                                    FightersTime[i] -= FrameDuration;
                                break;
                            case BattleFighterState.Running:    
                            // Move around depending on OldAP - AP.
                                if (!Fighters[i].isStun)
                                {
                                    if (FightersAction[i].targetMode == TargetMode.Single) //DIRTY DIRTY DIRTY!!!
                                    {
                                        if (Fighters[FightersSkillConfiguration[i].parameters[0]].isDead)
                                        {
                                            FightersState[i] = BattleFighterState.Active;
                                            continue;
                                        }
                                        Fighters[i].Move((OldAP - AP) * Fighters[i].Speed, Fighters[FightersSkillConfiguration[i].parameters[0]].Position, true);
                                        if (Fighters[i].IsCloseTo(Fighters[FightersSkillConfiguration[i].parameters[0]].Position, FightersAction[i].Range))
                                        {
                                            FightersState[i] = BattleFighterState.Attacking;
                                            Fighters[i].Stop();
                                        }
                                    }
                                    else 
                                    {
                                        Fighters[i].Move((OldAP - AP) * Fighters[i].Speed, new Vector2(FightersSkillConfiguration[i].shapeParameters[0], FightersSkillConfiguration[i].shapeParameters[1]), true);
                                        if (Fighters[i].IsCloseTo(new Vector2(FightersSkillConfiguration[i].shapeParameters[0], FightersSkillConfiguration[i].shapeParameters[1]), 0))
                                        {
                                            FightersState[i] = BattleFighterState.Inactive;
                                            Fighters[i].Stop();
                                        }
                                    }
                                }
                                break;
                            case BattleFighterState.Recovering:
                                if (!Fighters[i].isStun)
                                {
                                    FightersTime[i] -= OldAP - AP;
                                    CastBars[i].Value = (int)Math.Ceiling(FightersTime[i] * 100);
                                }
                                if (FightersTime[i] <= 0)
                                {
                                    FightersState[i] = Fighters[i].Unlimited ? BattleFighterState.Active : BattleFighterState.Inactive;
                                    CastBars[i].Visible = false;
                                }
                                break;
                            default:
                                // Do Nothing.
                                break;
                        }
                    }
                }
            }

            if (State != BattleState.WinBattle && State != BattleState.LoseBattle)
            {
                if (!anAllyAlive)
                {
                    if (BattleEnding != null)
                    {
                        BattleEnding(this, new EndBattleEventArgs(false));
                    }
                    State = BattleState.LoseBattle;
                }
                else if (!anEnnemyAlive)
                {
                    if (BattleEnding != null)
                    {
                        BattleEnding(this, new EndBattleEventArgs(true));
                    }
                    State = BattleState.WinBattle;
                }
            }
            //Update APWindow
            APWindow.Text = "AP: " + ((int)Math.Ceiling(AP)).ToString(); 

            //Calculate the rotation. 0 when MaxAP > AP > 7*MaxAP / 8, - 3 Pi/2 when 0 < AP < 1 MaxAP / 8
            if (AP > (7 * MaxAP / 8))
            {
                Rotation = 0;
                CenterRotation -= 8 * (AP - OldAP) / MaxAP * (float)Math.PI / 4;
            }
            else if (AP < (MaxAP / 8))
            {
                Rotation = -3 * (float)Math.PI / 2;
                CenterRotation -= 8 * (AP - OldAP) / MaxAP * (float)Math.PI / 4;
            }
            else
            {
                CenterRotation = (float)Math.PI / 4;
                Rotation -= (OldAP - AP) * (2 * (float)Math.PI) / MaxAP;
            }
            WoT[0].Rotation = Rotation;
            WoT[1].Rotation = CenterRotation;
        }

        #endregion

        #region End
 
        /// <summary>
        /// Performs the required operations to end the battle.
        /// </summary>
        private void EndBattle()
        {
            if (ExpGauges.Count == 0)
            {
                if (BattleEnded != null)
                {
                    //Game.Components.Add(new BattlePostScreenScene(this));
                    BattleEnded(this, new EndBattleEventArgs(State == BattleState.WinBattle));
                }
                else
                {
                    if (Input.isActionDone(Input.Action.BattleConfirm, false))
                    {
                        Game.Components.Add(new DemoTitleScene(this.LuxGame));
                        this.Destroy();
                        message = "";
                    }
                }
            }
                if (State == BattleState.WinBattle)
                    message = "You win!";
                else if (State == BattleState.LoseBattle)
                {
                    message = "You Lose!";
                    //Fighters[0].Sprite.SetAnimation("lucien_dead"); // WAY too DIRTY!
                }
            
        }

        /// <summary>
        /// Performs the required operations to end the turn.
        /// </summary>
        void EndTurn()
        {
            if (TurnEnding != null)
                TurnEnding(this, new EventArgs());

            woTState = WoTState.Stopped;
            //Clear the active Fighters.
            CommandIndex = -1;
            arrow.Visible = false;
            ActionIndex = -1;
            ReactionIndex = -1;
            if (battleMenu != null)
            {
                battleMenu.Enabled = false;
                battleMenu.Visible = false;
            }
            for (int i = 0; i < Fighters.Length; i++)
            {
                if (Fighters[i] != null)
                {
                    CastBars[i].Visible = false;
                    FightersTime[i] = 0;
                    AttackNameWindow[i].Visible = false;
                    if (Fighters[i] is Character && FightersOldAP[i] - Fighters[i].Stats["AP"] > 0)
                    {
                        AddExpGauge((Character)Fighters[i], FightersOldAP[i] - Fighters[i].Stats["AP"], ExpTypes.Stat, Fighters[i].Stats.IndexOf("AP"));
                    }
                    FightersOldAP[i] = Fighters[i].Stats["AP"];
                    if (Fighters[i].Unlimited && !(FightersState[i] == BattleFighterState.Inactive || FightersState[i]== BattleFighterState.Disabled))
                        Fighters[i].Unlimited = false;
                    // FIX: Unstun the stunned : 
                    if (Fighters[i].isStun)
                        Fighters[i].isStun = false;
                    // FIX: The non inactive fighters should have their APs set to 0.
                    if (FightersState[i] != BattleFighterState.Inactive)
                        Fighters[i].Stats["AP"] = 0;

                    // Revival of disabled characters.
                    if (FightersState[i] == BattleFighterState.Disabled)
                    {
                        Fighters[i].Stats["AP"] = Fighters[i].Stats["AP", true];
                    }

                    // Condition to become active.
                    if (Fighters[i].Stats["AP"] != 0 && !Fighters[i].isDead)
                        FightersState[i] = BattleFighterState.Active;


                    // Condition to become disabled.
                    if (Fighters[i].Stats["AP"] == 0)
                    {
                        FightersState[i] = BattleFighterState.Disabled;
                        Fighters[i].Stop();
                    }
                }
            }

            //If the Active Fighters list is not empty, calculate the MaxAP.
            if (FightersState.Contains(BattleFighterState.Active))
            {
                MaxAP = MaxFightersAP;
                //Set state to StartingTurn.
                State = BattleState.StartTurn;
                woTState = WoTState.Reverse;
                if (TurnEnded != null)
                    TurnEnded(this, new EventArgs());
                
            }
            else EndTurn();
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
        
        #endregion

        internal void AddExpGauge(Character chara, int IncreasedValue, ExpTypes expType, int id)
        {
            if (IncreasedValue != 0)
            {
                foreach (BattleExpGauge expGauge in ExpGauges)
                {
                    if (expGauge.HasValue(chara, expType, id))
                    {
                        expGauge.Increase(IncreasedValue);
                        return;
                    }
                }
                BattleExpGauge beg = new BattleExpGauge(this, chara, IncreasedValue, expType, id);
                ExpGauges.Add(beg);
                Game.Components.Add(beg);
            }
        }

        private void PerformAction(int CommandIndex)
        {
            if (PerformingAction != null)
                PerformingAction(this, new BattleEventArgs(Fighters[ActionIndex]));
            Random rand = Shared.Rand;
            string attackerOverHeadMessage = "";
            string targetOverHeadMessage = "";
            int index = 0;
            float e = 1; // L'efficacité.
            // Détermination du nombre d'attaques identiques successives
            if (FightersAction[ActionIndex].ID == FightersLastActionID[ActionIndex])
                FightersLastActionCount[ActionIndex]++;
            else
            {
                FightersLastActionCount[ActionIndex] = 1;
                FightersLastActionID[ActionIndex] = FightersAction[ActionIndex].ID;
            }
            if (FightersLastActionCount[ActionIndex] > 5)
                index = 4;
            else
                index = FightersLastActionCount[ActionIndex]-1;
            // Choix du critique en fonction.

            // Pour chaque cible :
            int target = FightersSkillConfiguration[ActionIndex].parameters[0];

                        int[] CriticWeight = new int[5];

            CriticWeight[0] = criticProbTab[0, index] * (Fighters[ActionIndex].Stats["Pre"] + Fighters[ActionIndex].Stats["STR2"]);
            CriticWeight[1] = criticProbTab[1, index] * (Fighters[ActionIndex].Stats["Pre"] + Fighters[ActionIndex].Stats["STR2"]);
            CriticWeight[2] = criticProbTab[2, index] * (Fighters[ActionIndex].Stats["Pre"] + Fighters[ActionIndex].Stats["STR2"] + Fighters[target].Stats["Speed"] + Fighters[target].Stats["Per"])/2;
            CriticWeight[3] = criticProbTab[3, index] * (Fighters[target].Stats["Speed"] + Fighters[target].Stats["Per"]); 
            CriticWeight[4] = criticProbTab[4, index] * (Fighters[target].Stats["Speed"] + Fighters[target].Stats["Per"]);
            int sum = 0;
            for (int i = 0; i < 5; i++)
            {
                sum += CriticWeight[i];
            }
            int p = rand.Next(0, sum);
            int s = 0;
            p++;
            for (int i = 0; i < 5; i++)
            {
                if (p >= s && p <= CriticWeight[i] + s)
                {
                    e *= criticEfficiencyTab[i, index];
                    if (i == 0)
                        targetOverHeadMessage = criticMessages[i];
                    else
                        attackerOverHeadMessage = criticMessages[i];
                    break;
                }
                else
                    s += CriticWeight[i];
            }
            if (Fighters[ActionIndex].GodMode)
            {
                e = 1;
                attackerOverHeadMessage = "";
                targetOverHeadMessage = "";
            }
            // Randomize from -10% to +10% :
            e = e + (float)rand.Next(-10, 11) / 100.0F * e;
            
            
            
            // Réussite de la réaction et modification de l'efficacité en fonction.
            int reactionRank = FightersReactionRank[target];
            int actionRank = 0;
            switch (FightersReaction[target])
            {
                case 7:
                    // Defend
                    // Comparaison du rang de la réaction avec le rang d'action correspondant.
                    e *= (float)(rand.Next(35, 66)) / 100.0F;
                    if (Fighters[target] is Character && Fighters[ActionIndex] is Ennemy)
                        AddExpGauge((Character)Fighters[target], (int)Math.Ceiling(60 * ((Ennemy)Fighters[ActionIndex]).EnnemyMult), ExpTypes.Stat, Fighters[target].Stats.IndexOf("Per"));
                    actionRank = Math.Min(FightersAction[ActionIndex].ParryRank, FightersAction[ActionIndex].DodgeRank);
                    if (reactionRank == actionRank)
                        e = (float)Math.Max(0, e - 0.1);
                    else if (reactionRank < actionRank)
                    {
                        e = (float)Math.Min(1, e + 0.1);
                    }
                    break;
                case 8:
                    // Parry
                    // Comparaison du rang de la réaction avec le rang d'action correspondant.
                    //actionRank = FightersAction[ActionIndex].ParryRank;
                    //if (rand.Next(0, 100) < reactionRankProbTab[actionRank,reactionRank-1])
                    if (rand.Next(0,100) < FightersBattleCommandCount[target] && !Fighters[ActionIndex].GodMode)
                    {
                        e *= 0;
                        if (Fighters[target] is Character && Fighters[ActionIndex] is Ennemy)
                            if (FightersAction[ActionIndex].Type == Skill.SkillType.Physical)
                                AddExpGauge((Character)Fighters[target], (int)Math.Ceiling(80 * ((Ennemy)Fighters[ActionIndex]).EnnemyMult), ExpTypes.Stat, Fighters[target].Stats.IndexOf("PhyDef"));
                            else if (FightersAction[ActionIndex].Type == Skill.SkillType.Magical)
                                AddExpGauge((Character)Fighters[target], (int)Math.Ceiling(80 * ((Ennemy)Fighters[ActionIndex]).EnnemyMult), ExpTypes.Stat, Fighters[target].Stats.IndexOf("MagDef"));
                        if (FightersBattleCommandCount[target] == 100)
                        {
                            targetOverHeadMessage = "Perfect Parry!";
                        }
                        else
                            targetOverHeadMessage = "Parry!";
                        OtherCue = LuxGame.soundBank.GetCue("Parry");
                        if (SoundEnabled)
                          OtherCue.Play();
                        Fighters[ActionIndex].Strike(Fighters[target].Position, 25.0F * reactionRank, 5.0F);
                        Fighters[ActionIndex].Stun(5.0F);
                    }
                    break;
                case 9:
                    // Dodge
                    // Comparaison du rang de la réaction avec le rang d'action correspondant.
                    //actionRank = FightersAction[ActionIndex].DodgeRank;
                    //if (rand.Next(0, 100) < reactionRankProbTab[actionRank, reactionRank-1])
                    if (rand.Next(0, 100) < FightersBattleCommandCount[target] && !Fighters[ActionIndex].GodMode)
                    {
                        if (Fighters[target] is Character && Fighters[ActionIndex] is Ennemy)
                            AddExpGauge((Character)Fighters[target], (int)Math.Ceiling(80 * ((Ennemy)Fighters[ActionIndex]).EnnemyMult), ExpTypes.Stat, Fighters[target].Stats.IndexOf("Speed"));
                        e *= 0;
                        if (FightersBattleCommandCount[target] == 100)
                        {
                            targetOverHeadMessage = "Perfect Dodge!";
                        }
                        else
                            targetOverHeadMessage = "Dodge!";
                        OtherCue = LuxGame.soundBank.GetCue("Dodge");
                        if (SoundEnabled)
                            OtherCue.Play();
                        Fighters[target].Strike(Fighters[ActionIndex].Position, 15.0F * reactionRank, 5.0F);
                    }
                    break;
                case 10:
                    // Counter
                    // Comparaison du rang de la réaction avec le rang d'action correspondant.
                    actionRank = Math.Max(FightersAction[ActionIndex].ParryRank,FightersAction[ActionIndex].DodgeRank);
                    //if (rand.Next(0, 100) < reactionRankProbTab[actionRank, reactionRank-1] / 2)
                    if (rand.Next(0,100) < FightersBattleCommandCount[target] && !Fighters[ActionIndex].GodMode)
                    {
                        e *= -1;
                        attackerOverHeadMessage = "Counter!";
                    }
                    break;
               default:
                    // Do nothing.
                    break;
            }
            if (actionRank >= reactionRank - 1)
            {
                message = "Reaction Rank too low! Reaction broken!";
            }
            else if (actionRank < reactionRank - 1)
            {
                message = "Reaction Rank too high! It is less effective!";
            }
            else
            {
                message = "";
            }
            FightersReaction[target] = 0;
               
            // Calcul de l'efficacité.
            // Application des effets (dommages, stun, strike, etc.)
            int AttackerIndex;
            int TargetIndex;
            if (e >= 0)
            {
                AttackerIndex = ActionIndex;
                TargetIndex = target;
            }
            else
            {
                e *= -0.75F;
                AttackerIndex = target;
                TargetIndex = ActionIndex;
            }
            if (Fighters[AttackerIndex].AttackCue != null)
            {
                AttackerCue = LuxGame.soundBank.GetCue(Fighters[AttackerIndex].AttackCue);
                if (VoiceEnabled[1])
                    AttackerCue.Play();
            }
            int Damage = 0;
            Damage = FightersAction[ActionIndex].CalculateDamage(Fighters[AttackerIndex], Fighters[TargetIndex]);
            Fighters[target].unlimitedGauge += (1 + rand.Next(0, 200) * Damage) / 200;
            Damage = (int)Math.Round(Damage * e);    
            if (e != 0)
            {
                Fighters[TargetIndex].Stun(FightersAction[ActionIndex].Stun * Math.Max(0, e));
                Fighters[TargetIndex].Strike(Fighters[AttackerIndex].Position, FightersAction[ActionIndex].Strike * Math.Max(e, 0), FightersAction[ActionIndex].StrikeSpeed * Math.Max(e, 0));
                Fighters[TargetIndex].Stats["End"] -= Damage;
                if (Fighters[TargetIndex].WoundCue != null && Fighters[TargetIndex].VeryWoundCue != null && Fighters[TargetIndex].DeathCue != null)
                {
                    if (Fighters[TargetIndex].Stats["End"] > (int)(Fighters[TargetIndex].Stats["End", true] * 30.0F / 100.0F) && Fighters[TargetIndex].WoundCue != null)
                        TargetCue = LuxGame.soundBank.GetCue(Fighters[TargetIndex].WoundCue);
                    else if (Fighters[TargetIndex].VeryWoundCue != null && Fighters[TargetIndex].DeathCue != null)
                        TargetCue = Fighters[TargetIndex].Stats["End"] == 0 ? LuxGame.soundBank.GetCue(Fighters[TargetIndex].DeathCue) : LuxGame.soundBank.GetCue(Fighters[TargetIndex].VeryWoundCue);
                    if (VoiceEnabled[1])
                        TargetCue.Play();
                }
            }  
            // Affichage des dommages :
            BattleDamageScene DamageDisplay = new BattleDamageScene(Fighters[AttackerIndex].Sprite, Fighters[TargetIndex].Sprite,Damage > 0 ? Damage : -Damage, (Damage > 0 ? Color.Red : Color.Green), attackerOverHeadMessage, targetOverHeadMessage);
            Game.Components.Add(DamageDisplay);
            
            DamageDisplay.Expired += new BattleDamageScene.expiredLifeTime(DamageDisplay_Expired);
            DisplayDamageList.Add(DamageDisplay);


            // Pour le lanceur :
            // Eventuelle application d'effets.
            // Chgt d'état.
            #region XP
            if (e != 0)
            {
                //XP : le lanceur est un personnage => il obtient de l'XP
                if (Fighters[AttackerIndex] is Character && Fighters[TargetIndex] is Ennemy)
                {
                    int exp = (int)Math.Ceiling(((Ennemy)Fighters[TargetIndex]).EnnemyMult * Math.Abs(e));
                    // Augmentation de l'XP de base
                    AddExpGauge((Character)Fighters[AttackerIndex], 8 * exp, ExpTypes.Base, 0);
                    
                    //Augmentation de l'XP de classe
                    //TODO pour le cas où l'attaque est de la classe en cours
                    AddExpGauge((Character)Fighters[AttackerIndex], 4 * exp, ExpTypes.Class, ((Character)Fighters[AttackerIndex]).ClassID);

                    // Prime si l'ennemi est éliminé
                    if (Fighters[TargetIndex].isDead)
                    {
                        AddExpGauge((Character)Fighters[AttackerIndex], 10 * (int)Math.Ceiling(((Ennemy)Fighters[TargetIndex]).EnnemyMult), ExpTypes.Base, 0);
                        AddExpGauge((Character)Fighters[AttackerIndex], 5 * (int)Math.Ceiling(((Ennemy)Fighters[TargetIndex]).EnnemyMult), ExpTypes.Class, ((Character)Fighters[AttackerIndex]).ClassID);
                    
                    }

                    // Augmentation de l'XP de classe si l'attaque est une action de classe
                    //TODO
                    if (AttackerIndex == ActionIndex)
                    {
                        // Augmentation de l'XP de skill si l'attaque est un skill.
                        AddExpGauge((Character)Fighters[AttackerIndex], (int)Math.Ceiling(4.0F * exp / FightersAction[ActionIndex].Rank), ExpTypes.Skill, FightersAction[ActionIndex].ID);
                        // Augmentation de l'XP de stat en fonction de la stat mise en jeu.
                        if (FightersAction[AttackerIndex].Type == Skill.SkillType.Physical)
                            AddExpGauge((Character)Fighters[AttackerIndex], Damage, ExpTypes.Stat, Fighters[AttackerIndex].Stats.IndexOf("PhyAtt"));
                        else if (FightersAction[AttackerIndex].Type == Skill.SkillType.Magical)
                            AddExpGauge((Character)Fighters[AttackerIndex], Damage, ExpTypes.Stat, Fighters[AttackerIndex].Stats.IndexOf("MagAtt"));
                    }
                    else
                    {
                        // Augmentation de l'XP de skill si l'attaque est un skill.
                        AddExpGauge((Character)Fighters[AttackerIndex], (int)Math.Ceiling((float)exp / GameData.commands[10].Rank), ExpTypes.Skill, 10);
                        // Augmentation de l'XP de stat en fonction de la stat mise en jeu.
                        
                    }

                    // Augmentation de l'XP de weapon si l'attaque met en jeu une arme
                    //TODO
                }
                else if (Fighters[AttackerIndex] is Ennemy && Fighters[TargetIndex] is Character)
                {
                    AddExpGauge((Character)Fighters[TargetIndex],Damage, ExpTypes.Stat, Fighters[TargetIndex].Stats.IndexOf("End"));
                }
            }
            #endregion
            FightersState[ActionIndex] = BattleFighterState.Recovering;
            FightersTime[ActionIndex] = (int)Math.Ceiling(FightersAction[ActionIndex].RecoveryTime*APRatio);
            CastBars[ActionIndex].Visible = true;
            CastBars[ActionIndex].ValueColor = Color.Red;
            CastBars[ActionIndex].MaxValue = (int)Math.Ceiling(100 * FightersTime[ActionIndex]);
            CastBars[ActionIndex].Value = CastBars[ActionIndex].MaxValue;
            FightersAction[ActionIndex] = null;
            if (Attacked != null)
                Attacked(this, new BattleEventArgs(Fighters[ActionIndex]));
            ActionIndex = -1;
            // Recovery.
        }

        void DamageDisplay_Expired(object sender, EventArgs e)
        {
            DisplayDamageList.Remove((BattleDamageScene)sender);
            ((Scene)sender).Destroy();
        }

        #region Aux

        internal List<FighterDist> FindTargets(Fighter Attacker, bool withAlly)
        {
            List<FighterDist> Targets = new List<FighterDist>();
            if (Attacker is Character)
            {
                for (int i = 0; i < Fighters.Length; i++)
                {
                    if (Fighters[i] != null && (Fighters[i] is Ennemy || withAlly))
                    {
                        FighterDist fighterdist = new FighterDist();
                        fighterdist.FighterID = i;
                        fighterdist.Fighter = Fighters[i]; 
                        fighterdist.Distance = Attacker.Distance(Fighters[i].Position);
                        Targets.Add(fighterdist);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Fighters.Length; i++)
                {
                    if (Fighters[i] != null && (Fighters[i] is Character || withAlly))
                    {
                        FighterDist fighterdist = new FighterDist();
                        fighterdist.FighterID = i;
                        fighterdist.Fighter = Fighters[i]; 
                        fighterdist.Distance = Attacker.Distance(Fighters[i].Position);
                        Targets.Add(fighterdist);
                    }
                }
            }
            Targets.Sort(new FighterDistComparer());
            return Targets;
        }

        /// <summary>
        /// Returns the shortest distance to a Target.
        /// </summary>
        /// <param name="Attacker">The fighter that tries to find the nearest target.</param>
        /// <param name="withAlly">Indicates whether allies should be counted into account.</param>
        /// <returns>A float indicating the shortest distance to a Target</returns>
        public float NearestTargetDistance(Fighter Attacker, bool withAlly)
        {
            float minDistance = -1.0F; // a negative minimal distance is a flag to indicate that it hasn't been 'initialized'.
            if (withAlly)
                return 0;
            if (Attacker is Character)
            {
                for (int i = 0; i < Fighters.Length; i++)
                {
                    if ( ((Fighters[i] != null && !Fighters[i].isDead &&
                        Attacker.Distance(Fighters[i].Position) <= minDistance) || minDistance < 0.0F ) && Fighters[i] is Ennemy)
                        minDistance = Attacker.Distance(Fighters[i].Position);
                }
            }
            else
            {
                for (int i = 0; i < Fighters.Length; i++)
                {
                    if (((Fighters[i] != null && !Fighters[i].isDead &&
                        Attacker.Distance(Fighters[i].Position) <= minDistance) || minDistance < 0.0F) && Fighters[i] is Character)
                        minDistance = Attacker.Distance(Fighters[i].Position);
                }
            }

            return (float)Math.Max(0, minDistance);
        }

        // Aux Property used everywhere
        public float MaxFightersAP
        {
            get
            {
                int Max = 0;
                for (int i = 0; i < Fighters.Length; i++)
                {
                    if (Fighters[i] != null && FightersState[i] == BattleFighterState.Active && Fighters[i].Stats["AP"] > Max)
                        Max = Fighters[i].Stats["AP"];
                }
                return Max;
            }
        }
        #endregion

        #endregion

        public override void Destroy()
        {
            MusicSoundBank.Dispose();
            MusicWaveBank.Dispose();

            base.Destroy();
        }


        
    }
    #region AuxTypes

    public enum BattleFighterState
    {
        Inactive,  // The state of a Fighter that already performed an action this turn. 
        // An incapacitated Fighter is automatically inactive until the turn next to the one the Fighter is Cured.
        Attacking, // The state of a Fighter that will soon perform an action.
        Active, // The state of a Fighter that can perform an action this turn.
        Command, // The state of a Fighter that can input Command.
        Casting, // The state of a Fighter that chose a Magic Command.
        Configure, // The state of Fighter that is Configuring the chosen Command. (after casting, but before running)
        Running, // The state of a Fighter that configured a Physical Command.
        Recovering, // The state of a Fighter that executed a Physical Command. The Fighter cannot perform a reaction while he remains on this state.
        Disabled // The state of a Fighter with 0 AP. The Character cannot perform an action nor a reaction this turn. His AP will be fully restored on next turn.
        // (Fighters with 0 MaxAP will be permanently disabled)

    }

    // State priority: StartTurn > Command > Configuration > Idle.
    public enum BattleState
    {
        Idle,
        StartTurn,
        Command,
        Configuration,
        Action,
        WinBattle,
        LoseBattle,
        Paused
    }

    // State priority: Stopped > Reverse > Player > Idle > FastForward 
    enum WoTState
    {
        Idle,
        Player,
        FastForward,
        Reverse,
        Stopped,
        Custom
    }

    public class FighterDist
    {
        public Fighter Fighter;
        public int FighterID;
        public float Distance;
    }

    public class FighterDistComparer : IComparer<FighterDist>
    {
        public int Compare(FighterDist x, FighterDist y)
        {
            return x.Distance.CompareTo(y.Distance);
        }
    }

    #endregion
    
}
