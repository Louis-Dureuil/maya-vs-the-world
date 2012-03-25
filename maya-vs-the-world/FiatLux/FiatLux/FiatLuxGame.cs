using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FiatLux.Logic;
namespace FiatLux
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FiatLuxGame : Game
    {
        #region Constants
        const int DIFFICULTY = 1;
        const float IDLESPEED = 15.0F;
        const float PLAYERSPEED = 0.20F; //2 AP in 10 seconds.
        const float FASTFORWARDSPEED = 40.0F;
        const float BEGINTURNDURATION = 2.0F;
        const float COMMANDDURATION = 15.0F;
        const float CONFIGUREDURATION = 5.0F; //Temporary Decision. Will later be skill-dependant
        const float BASESPEED = 10.0F;
        const float DAMAGEDISPLAYDURATION = 2.0F;
        const float MAYREPEATWORD = 10.0F;
        const int REPEATWORDPROB = 30;
        const float PERFECTREACTIONTIMING = 0.5F;
      


        readonly float[,] criticEfficiencyTab = new float[5,5] { {0,0,0,0,0},{0.66F,0.5F,0.45F,0.38F,0.33F},{1.1F,1,0.95F,0.85F,0.7F},{1.6F,1.5F,1.45F,1.38F,1.33F},{2,2,2,2,2} } ;
        readonly int[,] criticProbTab = new int[5, 5] { {2, 5, 8, 10, 15}, {5, 10, 15, 18, 25}, {70, 70, 70, 70, 60}, {15, 10, 5, 2, 0}, {8, 5,2,0,0} };
        readonly string[] criticMessage = new string[5] { "Raté!", "Pas d'bol!", "", "Coup d'pot!", "Chance!" };

        public readonly int[,] reactionRankProbTab = new int[7, 5] { { 100, 90, 80, 70, 60 }, { 95, 85, 75, 65, 55 }, { 0, 90, 65, 55, 45 }, { 0, 0, 80, 35, 25 }, { 0, 0, 0, 70, 1 }, { 0, 0, 0, 0, 60 }, { 0, 0, 0, 0, 0 } }; 
        
        #endregion

        struct DamageStruct
        {
            internal int index; //The index of the fighter that took damage.
            internal int damage; // The amount of damage taken.
            internal float lifeTime; // The duration during which the damage must be displayed.
            internal Color color; // The color of the displayed text.
            internal string criticMessage; // An additionnal text.
        }

        #region Fields

        #region Scenes

        Scenes.BattleMenuScene battleMenu;

        #region Bars
        LuxEngine.Bar[] EndBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity];
        LuxEngine.Bar[] MPBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity];
        LuxEngine.Bar[] UnlimitedBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity];
        LuxEngine.Bar[] APBars = new LuxEngine.Bar[Constants.BattleCharactersCapacity];
        LuxEngine.Bar[] EnnemyEndBars = new LuxEngine.Bar[Constants.EnnemiesCapacity];
        LuxEngine.Bar APBar;
        #endregion

        #region Windows
        LuxEngine.TextWindow[] statusWindows = new LuxEngine.TextWindow[Constants.BattleCharactersCapacity] ;
        LuxEngine.TextWindow[] APWindows = new LuxEngine.TextWindow[Constants.BattleCharactersCapacity] ;
        LuxEngine.TextWindow messageWindow;
        #endregion

        #endregion

        #region Graphics
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        
        
        // Graphical objects
        Texture2D background;
        Texture2D Ennemy;
        Texture2D Ally;
        Texture2D WoTAP;
        Texture2D WoTCenter;
        SpriteFont spriteFont;
        #endregion

        #region Audio
        // Audio objects. Uncomment whereever music is supported.
        AudioEngine engine;
        SoundBank MusicSoundBank;
        WaveBank MusicWaveBank;
        SoundBank soundBank;
        WaveBank waveBank;
        Cue WordCue;

        #region AudioLevel

        bool MusicEnabled = true;
        bool SoundEnabled = true;
        /// <summary>
        /// 0: Enable dialog voices -- voices in the scenario
        /// 1: Enable ambiance voices -- voices when attacking/attacked
        /// 2: Enable custom voices -- voices that are specific to the current battle
        /// 3: Enable random voices -- voices that appear randomly
        /// </summary>
        bool[] VoiceEnabled = new bool[4] { false, true, true, true };  

        #endregion

        #endregion

        #region Display
        List<DamageStruct> DisplayDamageList = new List<DamageStruct>();
        string message = "";
        float CenterRotation = 0.0F;
        float Rotation = 0.0F;
            
        #region Menus
      
        #region TargetMenu
        // Target Menu
        bool TargetVisible = false; // Whether the current selected target should be drawn.
        #endregion
        
        #endregion

        #endregion

        #region States and Switches
        WoTState woTState;
        public BattleState State;
        #endregion

        #region Logic
        float customSpeed = IDLESPEED;
        float AP = 0.0F;
        float OldAP = 0.0F;
        float Speed = 1.0F;
        float MaxAP = 1.0F; // Must be != 0.
        float WordTimer = 0.0F;
        #endregion

        #region Fighters
        //Fighters.
        Fighter[] Fighters = new Fighter[Constants.BattleCharactersCapacity + Constants.BattleEnnemiesCapacity];
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
        
        #endregion

        #region Indexes
        //Keep Track of the fighter that is currently in command.
        public int CommandIndex = -1;

        //Keep Track of the fighter that is currently attacking.
        public int ActionIndex = -1;

        //Keep Track of the fighter that is currently reacting.
        public int ReactionIndex = -1;
        #endregion

        #endregion

        #region Methods

        #region Initialize

        public FiatLuxGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.GraphicsProfile = GraphicsProfile.Reach;
            // TODO: Add your initialization logic here
            // Initialize audio objects.
            //graphics.ToggleFullScreen();
			//uncomment when the .wav are available for compilation (on pengouin-fixe)
			engine = new AudioEngine("Content\\fiatluxsound.xgs");
            MusicSoundBank = new SoundBank(engine, "Content\\Music Sound Bank.xsb");
            MusicWaveBank = new WaveBank(engine, "Content\\Music.xwb");
            soundBank = new SoundBank(engine, "Content\\Sound Bank.xsb");
            waveBank = new WaveBank(engine, "Content\\Sound.xwb");
          
            //Initialize Input
            Input.Initialize();

            //Initialize Fighters
            Character Lucien = new Character();
            Lucien.Name = "Lucien";
            Lucien.Stats["AP",true] = 75;
            Lucien.Stats["End",true] = 255;
            Lucien.Stats["MP", true] = 25;
            Lucien.Stats.ToMax("AP");
            Lucien.Stats.ToMax("MP");
            Lucien.Stats.ToMax("End");
            Lucien.Position = new Vector2(100, 100);

            Ennemy Master = new Ennemy();
            Master.Name = "Sergent";
            Master.Stats["AP",true] = 100;
            Master.Stats["End",true] = 300;
            Master.Stats.ToMax("AP");
            Master.Stats.ToMax("End");
            Master.Position = new Vector2(600, 250);

            Master.Stats["PhyAtt"] = 35;
            Lucien.Stats["PhyAtt"] = 30;
            Master.Stats["PhyDef"] = 15;
            Lucien.Stats["PhyDef"] = 15; 

            // Initialize Skills
            for (int i = 0; i < Constants.CommandsCapacity; i++)
            {
                GameData.commands[i] = new Command();
                GameData.commands[i].ID = i;
            }
            GameData.commands[0].Name = "";
            GameData.commands[1].Name = "Attacks";
            GameData.commands[2] = new Skill();
            GameData.commands[3] = new Skill();
            GameData.commands[2].ID = 2;
            GameData.commands[3].ID = 3;
            GameData.commands[2].Name = "Quick Attack";
            ((Skill)GameData.commands[2]).AttackStats["PhyAtt"] = 8;
            ((Skill)GameData.commands[2]).DefenseStats["PhyDef"] = 10;
            ((Skill)GameData.commands[2]).DodgeRank = 3;
            ((Skill)GameData.commands[2]).ParryRank = 1;
            GameData.commands[3].Name = "Strong Attack";
            ((Skill)GameData.commands[3]).Range += 10.0F;
            ((Skill)GameData.commands[3]).Strike += 20.0F;
            ((Skill)GameData.commands[3]).StrikeSpeed += 10.0F;
            ((Skill)GameData.commands[3]).Stun += 20.0F;
            ((Skill)GameData.commands[3]).RecoveryTime += 20.0F;
            ((Skill)GameData.commands[3]).BaseDamage += 5;
            ((Skill)GameData.commands[3]).ParryRank = 3;
            ((Skill)GameData.commands[3]).DodgeRank = 1;
            ((Skill)GameData.commands[3]).AttackStats["PhyAtt"] = 12;
            ((Skill)GameData.commands[3]).DefenseStats["PhyDef"] = 10;    
            GameData.commands[4].Name = "Magic";
            GameData.commands[5].Name = "Summon";
            GameData.commands[6].Name = "Special";

            GameData.commands[7] = new Reaction();
            GameData.commands[7].Name = "Defend";
            GameData.commands[8] = new Reaction();
            GameData.commands[8].Name = "Parry";
            GameData.commands[9] = new Reaction();
            GameData.commands[9].Name = "Dodge";
            GameData.commands[10] = new Reaction();
            GameData.commands[10].Name = "Counter";

            GameData.commands[2].Parent = 1;
            GameData.commands[3].Parent = 1;

            GameData.commands[1].Children[0] = 2;
            GameData.commands[1].Children[1] = 3;

            Lucien.ActionCommands[0] = 1;
            Lucien.ActionCommands[1] = 4;
            Lucien.ActionCommands[2] = 5;
            Lucien.ActionCommands[3] = 6;

            Lucien.ReactionCommands[0] = 7;
            Lucien.ReactionCommands[1] = 8;
            Lucien.ReactionCommands[2] = 9;
            Lucien.ReactionCommands[3] = 10;

            //Audio Cue.
            Lucien.AttackCue = "LucienAttack";
            Lucien.DeathCue = "LucienDeath";
            Lucien.VeryWoundCue = "LucienVeryWound";
            Lucien.WordCue = "LucienWord";
            Lucien.WoundCue = "LucienWound";

            Master.AttackCue = "SergentAttack";
            Master.DeathCue = "SergentDeath";
            Master.VeryWoundCue = "SergentWound";
            Master.WordCue = "SergentWord";
            Master.WoundCue = "SergentWound";

            Fighters[0] = Lucien;
            Fighters[1] = Master;
            for (int i = 0; i < statusWindows.Length; i++)
            {
                if (Fighters[i] != null && Fighters[i] is Character)
                {
                    statusWindows[i] = new LuxEngine.TextWindow(this, new Rectangle(150 + 150 * i, 0, 150, 90));
                    statusWindows[i].Enabled = true;
                    statusWindows[i].Visible = true;
                    statusWindows[i].BackgroundColor = new Color(255, 255, 255, 80);
                    statusWindows[i].Text = "";
                    statusWindows[i].horizontalAlignment = LuxEngine.HorizontalAlignment.Center;
                    EndBars[i] = new LuxEngine.Bar(statusWindows[i], new Vector2(100, 5));
                    EndBars[i].ValueColor = Color.Green;
                    EndBars[i].MaxValue = Fighters[i].Stats["End", true];
                    EndBars[i].Value = Fighters[i].Stats["End"];

                    MPBars[i] = new LuxEngine.Bar(statusWindows[i], new Vector2(100, 5));
                    MPBars[i].ValueColor = Color.Blue;
                    MPBars[i].MaxValue = Fighters[i].Stats["MP", true];
                    MPBars[i].Value = Fighters[i].Stats["MP"];

                    UnlimitedBars[i] = new LuxEngine.Bar(statusWindows[i], new Vector2(100, 5));
                    UnlimitedBars[i].ValueColor = Color.Gold;
                    UnlimitedBars[i].MaxValue = Fighters[i].Stats["End", true];
                    UnlimitedBars[i].Value = Fighters[i].unlimitedGauge;
                    
                    statusWindows[i].Add(EndBars[i]);
                    statusWindows[i].Add(MPBars[i]);
                    statusWindows[i].Add(UnlimitedBars[i]);

                    EndBars[i].Position = new Vector2(10, 30);
                    MPBars[i].Position = new Vector2(10, 40);
                    UnlimitedBars[i].Position = new Vector2(10, 50);
                    this.Components.Add(statusWindows[i]);

                    APWindows[i] = new LuxEngine.TextWindow(this, new Vector2(520, 340));
                    APWindows[i].Enabled = true;
                    APWindows[i].Visible = true;
                    this.Components.Add(APWindows[i]);
                }
            }

            APBar = new LuxEngine.Bar(this, new Vector2(100, 10));
            APBar.ValueColor = Color.Green;
            this.Components.Add(APBar);

            messageWindow = new LuxEngine.TextWindow(this, new Vector2(150,445));
            messageWindow.Visible = false;
            this.Components.Add(messageWindow);
            CenterRotation = (float)Math.PI / 2;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = this.Content.Load<SpriteFont>("FontTest");
            Ennemy = this.Content.Load<Texture2D>("Sergent");
            Ally = this.Content.Load<Texture2D>("Lucien");
            WoTAP = this.Content.Load<Texture2D>("WoTAP_old");
            WoTCenter = this.Content.Load<Texture2D>("WoTCenterOrange");
            background = this.Content.Load<Texture2D>("backgroundM");
            Cue cue = MusicSoundBank.GetCue("battle_1");
            if (MusicEnabled)
                cue.Play();
            Cue sound = new Random().Next(0,2) == 0 ? soundBank.GetCue("LucienIntro") : soundBank.GetCue("SergentIntro");
            if (VoiceEnabled[2])
                sound.Play();
            APBar.Position = new Vector2(0, spriteBatch.GraphicsDevice.Viewport.Height - 150.0F);
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
        protected override void Update(GameTime gameTime)
        {
            // Update Input.
            Input.Update(gameTime);

            //Update the text windows:
            for (int i = 0; i < statusWindows.Length; i++)
            {
                if (statusWindows[i] != null)
                {
                    statusWindows[i].Text = Fighters[i].Name + "\n" + Fighters[i].Stats.ToString("End", true) + "\n" + Fighters[i].Stats.ToString("MP", false) + "\n" + "U.: " + Fighters[i].unlimitedGauge.ToString();
                }
                if (APWindows[i] != null)
                {
                    APWindows[i].Text = Fighters[i].Name + " " + Fighters[i].Stats.ToString("AP", true) + " " + FightersState[i].ToString();
                    APWindows[i].TextColor = FightersState[i] == BattleFighterState.Command ? Color.Blue : Color.White;
                }
            }

            if (message != messageWindow.Text)
            {
                messageWindow.Text = message;
                messageWindow.Enabled = messageWindow.Text != "";
                messageWindow.Visible = messageWindow.Text != "";
            }
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            UpdateWoT(gameTime);
            if (AP > 0)
            {
                UpdateState();
            }
            else
            {
                EndTurn();
            }
            base.Update(gameTime);
        }

        private void UpdateState()
        {
            switch (State)
            {
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
                            if (FightersState[i] == BattleFighterState.Active || FightersState[i] == BattleFighterState.Running ||
                                FightersState[i] == BattleFighterState.Casting || FightersState[i] == BattleFighterState.Recovering)
                            {
                                isFastForward = false;
                            }
                            if (FightersState[i] == BattleFighterState.Command)
                                State = BattleState.Command;
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
                                State = BattleState.Command;
                                break;
                            }
                        }
             
                    }
                    else if (Fighters[CommandIndex] != null && FightersState[CommandIndex] == BattleFighterState.Command)
                    {
                        if (FightersTime[CommandIndex] >= 0.0F)
                            UpdateCommand(false);
                        else
                        {
                            FightersState[CommandIndex] = Fighters[CommandIndex].Unlimited ? BattleFighterState.Active : BattleFighterState.Inactive;
                            CommandIndex = -1;
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
                                break;
                            }
                        }
                    }
                    else if (Fighters[CommandIndex] != null && FightersState[CommandIndex] == BattleFighterState.Configure)
                    {
                        if (FightersTime[CommandIndex] >= 0.0F)
                            UpdateConfigure();
                        else
                        {
                            FightersState[CommandIndex] = Fighters[CommandIndex].Unlimited ? BattleFighterState.Active : BattleFighterState.Inactive;
                            CommandIndex = -1;
                            battleMenu.Enabled = false;
                            battleMenu.Visible = false;
                            TargetVisible = false;
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
                                    if (FightersState[FightersSkillConfiguration[ActionIndex].Target] != BattleFighterState.Recovering
                                        && FightersState[FightersSkillConfiguration[ActionIndex].Target] != BattleFighterState.Casting
                                        && FightersState[FightersSkillConfiguration[ActionIndex].Target] != BattleFighterState.Disabled)
                                        FightersReactionTime[FightersSkillConfiguration[ActionIndex].Target] += COMMANDDURATION;
                                    State = BattleState.Action;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (Fighters[ActionIndex].IsCloseTo(Fighters[FightersSkillConfiguration[ActionIndex].Target].Position, FightersAction[ActionIndex].Range))
                            {

                                if (ReactionIndex < 0 && FightersReactionTime[FightersSkillConfiguration[ActionIndex].Target] > 0)
                                    ReactionIndex = FightersSkillConfiguration[ActionIndex].Target; // Seems pretty useless for now.
                                // Will be better when the attacks will allow multiple targets.
                                else if (ReactionIndex < 0) // If it remains < 0.
                                {
                                    // All the targets reacted.
                                    // Perform the attack if the Fighter is not stunned.
                                    if (!Fighters[ActionIndex].isStun)
                                        PerformAction(ActionIndex);
                                    else
                                        woTState = WoTState.Idle; // FIX: The WoT was slow w/ a stunned attacker.
                                }
                                else
                                {
                                    if (FightersReactionTime[ReactionIndex] > 0.0F)
                                    {
                                        UpdateCommand(true);
                                    }
                                    else
                                    {
                                        ReactionIndex = -1;
                                        battleMenu.Visible = false;
                                        battleMenu.Enabled = false;
                                    }
                                }

                            }
                            else
                            {
                                FightersState[ActionIndex] = BattleFighterState.Running;
                                ActionIndex = -1;
                                if (ReactionIndex > 0) FightersReactionTime[ReactionIndex] = 0.0F;
                                ReactionIndex = -1;
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

                    if (FightersAction[CommandIndex].Type == Skill.SkillType.Physical)
                    {
                        FightersState[CommandIndex] = BattleFighterState.Configure;
                        State = BattleState.Configuration;
                        FightersTime[CommandIndex] = CONFIGUREDURATION;
                    }
                    else
                    {
                        FightersState[CommandIndex] = BattleFighterState.Casting;
                        State = BattleState.Idle;
                        FightersTime[CommandIndex] = FightersAction[CommandIndex].CastTime;
                    }
                }
                else
                {
                    Random rand = new Random();
                    FightersReaction[index] = ((Ennemy)Fighters[index]).MakeReactionDecision(Fighters[ActionIndex], FightersAction[ActionIndex], FightersSkillConfiguration[ActionIndex]);
                    FightersReactionTime[index] = 0;
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

                    FightersBattleCommandCount[index] = COMMANDDURATION - PERFECTREACTIONTIMING <= FightersReactionTime[index] && FightersReactionRank[index] == ActionRank ? 100 : (int)Math.Ceiling((reactionRankProbTab[ActionRank, FightersReactionRank[ReactionIndex] - 1] / (i == 4 ? 2 : 1) * rand.Next(0,100)) / 100.0F); 
             
                }
            }
            else
            {
                if (battleMenu == null || !battleMenu.Enabled)
                {
                    if (battleMenu != null)
                    {
                        this.Components.Remove(battleMenu);
                    }
                    battleMenu = new Scenes.BattleMenuScene(this, Fighters[isReaction ? ReactionIndex : CommandIndex], isReaction);
                    battleMenu.Enabled = true;
                    battleMenu.Visible = true;
                    battleMenu.Position = new Vector2(250, 380);
                    this.Components.Add(battleMenu);
                }
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
                if (FightersAction[CommandIndex].Type == Skill.SkillType.Physical)
                {
                    FightersState[CommandIndex] = BattleFighterState.Running;
                }
                else
                {
                    PerformAction(CommandIndex);
                    FightersState[CommandIndex] = Fighters[CommandIndex].Unlimited ? BattleFighterState.Active : BattleFighterState.Inactive;
                }
                State = BattleState.Idle;
                CommandIndex = -1;
            }
            else
            {
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
                        FightersSkillConfiguration[CommandIndex].Target = CommandIndex;
                        break;
                    case TargetMode.Single:
                        // Allow the player to choose the target ammong fighters.
                        if (!TargetVisible)
                        {
                            // Default is the first Ennemy for an attack skill,
                            // And the first ally for a support skill.
                            // For now, it is the first ennemy.
                            int PreviousTarget = FightersSkillConfiguration[CommandIndex].Target;
                            int CurrentTarget = FightersSkillConfiguration[CommandIndex].Target;
                            if (Fighters[PreviousTarget] == null || Fighters[PreviousTarget] is Character)
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget] is Character && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }
                            TargetVisible = true;
                            FightersSkillConfiguration[CommandIndex].Target = CurrentTarget;
                        }
                        // Left-Right is used to change side (ie attacking allies or ennemies)
                        if (Input.isActionDone(Input.Action.BattleMenuLeft, false) || Input.isActionDone(Input.Action.BattleMenuRight, false))
                        {
                            int PreviousTarget = FightersSkillConfiguration[CommandIndex].Target;
                            int CurrentTarget = FightersSkillConfiguration[CommandIndex].Target;
                            if (Fighters[CurrentTarget] is Ennemy)
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget] is Character && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }
                            else
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget] is Ennemy && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }

                            FightersSkillConfiguration[CommandIndex].Target = CurrentTarget;
                        }



                        // Up-Down is used to change target inside a side (attacking another ennemy among the ennemies)
                        if (Input.isActionDone(Input.Action.BattleMenuUp, false) || Input.isActionDone(Input.Action.BattleMenuDown, false))
                        {
                            int PreviousTarget = FightersSkillConfiguration[CommandIndex].Target;
                            int CurrentTarget = FightersSkillConfiguration[CommandIndex].Target;
                            if (Fighters[CurrentTarget] is Ennemy)
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget] is Ennemy && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }
                            else
                            {
                                CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                                while (Fighters[CurrentTarget] == null || Fighters[CurrentTarget] is Character && CurrentTarget != PreviousTarget)
                                    CurrentTarget = (CurrentTarget + 1) % Fighters.Length;
                            }

                            FightersSkillConfiguration[CommandIndex].Target = CurrentTarget;
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
                    CommandIndex = -1;
                    battleMenu.Visible = false;
                    battleMenu.Enabled = false;
                    this.Components.Remove(battleMenu);
                    TargetVisible = false;
                }
            }
        }

        /// <summary>
        /// Update the Wheel of Time AP amount and its rotation.
        /// </summary>
        /// <param name="gameTime">Provide a snapshot of timing values.</param>
        void UpdateWoT(GameTime gameTime)
        {
            //Frame duration, in s (precision 10^(-3) s.)
            float FrameDuration = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;

            //Update the wordTimer
            if (VoiceEnabled[3])
            {
                WordTimer += FrameDuration;
                if (WordTimer > MAYREPEATWORD)
                {
                    WordTimer = 0.0F;
                    Random rand = new Random();
                    if (rand.Next(0, 100) < REPEATWORDPROB)
                    {
                        WordCue = soundBank.GetCue(rand.Next(0, 2) == 0 ? Fighters[0].WordCue : Fighters[1].WordCue);
                        WordCue.Play();
                    }
                }
            }
            // Update the damage drawing.
            for (int i = 0; i < DisplayDamageList.Count; i++)
            {
                DamageStruct displayDamage = DisplayDamageList[i];
                displayDamage.lifeTime -= FrameDuration;
                DisplayDamageList[i] = displayDamage;
                if (displayDamage.lifeTime < 0)
                    DisplayDamageList.RemoveAt(i);
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
                    Speed = FASTFORWARDSPEED;
                    break;
                case WoTState.Reverse:
                    Speed = -FASTFORWARDSPEED;
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
            AP = Math.Max(0, Math.Min(AP - Speed * FrameDuration * MaxAP / 100, MaxAP));

            //Get the active fighters whose APs are between OldAP and AP.
            for (int i = 0; i < Fighters.Length; i++)
            {
                if (Fighters[i] != null && FightersState[i] == BattleFighterState.Active && Fighters[i].Stats["AP"] >= (int)Math.Ceiling(AP) && Fighters[i].Stats["AP"] <= (int)Math.Ceiling(OldAP))
                {
                    FightersState[i] = BattleFighterState.Command;
                    FightersTime[i] = COMMANDDURATION;
                    //AP = (float)Fighters[i].AP + 0.50F ; ??
                }
                // Ex : AP = 95, OldAP = 100. A1 = 96, A2 = 99.  => A1 et A2 passent à Command, et AP vaut 99.
            }

            // From Here, AP is definitive.

            bool anAllyAlive = false;
            bool anEnnemyAlive = false;

            // Update the AP of the Fighters moving with the Wheel and this the state problem that may occur.
            // Also update the time.
            for (int i = 0; i < Fighters.Length; i++)
            {
                if (Fighters[i] != null)
                {
                    if (Fighters[i] is Character)
                    {
                        EndBars[i].Value = Fighters[i].Stats["End"];
                        MPBars[i].Value = Fighters[i].Stats["MP"];
                        UnlimitedBars[i].Value = Fighters[i].unlimitedGauge;
                    }

                    Fighters[i].Update(OldAP - AP);
                    FightersRotation[i] = Math.Max(0, Math.Min((float)Math.PI / 2, (float)Math.PI / 4 + (float)Math.PI / 4 - CenterRotation + 2 * (Fighters[i].Stats["AP"] - (int)Math.Ceiling(AP)) / MaxAP * (float)Math.PI / 4));

                    if (!Fighters[i].isDead)
                        if (Fighters[i] is Character)
                            anAllyAlive = true;
                        else
                            anEnnemyAlive = true;

                    if (FightersReactionTime[i] > 0)
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
                                if (CommandIndex == i)
                                    FightersTime[i] -= FrameDuration;
                                else
                                    Fighters[i].Stats["AP"] = (int)Math.Ceiling(OldAP);
                                break;
                            case BattleFighterState.Configure:
                                FightersTime[i] -= FrameDuration;
                                break;
                            case BattleFighterState.Running:
                                // Move around depending on OldAP - AP.
                                if (!Fighters[i].isStun)
                                {
                                    Fighters[i].Move((OldAP - AP) * BASESPEED, Fighters[FightersSkillConfiguration[i].Target].Position, true);
                                    if (Fighters[i].IsCloseTo(Fighters[FightersSkillConfiguration[i].Target].Position, FightersAction[i].Range))
                                    {
                                        FightersState[i] = BattleFighterState.Attacking;
                                        Fighters[i].Stop();
                                    }
                                }
                                break;
                            case BattleFighterState.Recovering:
                                if (!Fighters[i].isStun) FightersTime[i] -= OldAP - AP;
                                if (FightersTime[i] <= 0)
                                    FightersState[i] = Fighters[i].Unlimited ? BattleFighterState.Active : BattleFighterState.Inactive;
                                break;
                            default:
                                // Do Nothing.
                                break;
                        }
                    }
                }
            }

            if (!anAllyAlive)
                State = BattleState.LoseBattle;
            else if (!anEnnemyAlive)
                State = BattleState.WinBattle;


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

            // Update the APBar
            APBar.MaxValue = (int)Math.Ceiling(MaxAP);
            APBar.Value = (int)Math.Ceiling(AP);
        }

        #endregion

        #region End
 
        /// <summary>
        /// Performs the required operations to end the battle.
        /// </summary>
        private void EndBattle()
        {
            if (Input.isActionDone(Input.Action.BattleConfirm, false))
            {
                this.Exit();
                message = "";
            }
            if (State == BattleState.WinBattle)
                message = "You win!";
            else if (State == BattleState.LoseBattle)
                message = "You Lose!";
        }

        /// <summary>
        /// Performs the required operations to end the turn.
        /// </summary>
        void EndTurn()
        {
            woTState = WoTState.Stopped;
            //Clear the active Fighters.
            CommandIndex = -1;
            ActionIndex = -1;
            ReactionIndex = -1;
            for (int i = 0; i < Fighters.Length; i++)
            {
                if (Fighters[i] != null)
                {
                    if (Fighters[i].Unlimited && !(FightersState[i] == BattleFighterState.Inactive))
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
                    if (Fighters[i].Stats["AP"] != 0)
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
            }
            else EndTurn();
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BlanchedAlmond);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(Ally, new Rectangle((int)Fighters[0].Position.X, (int)Fighters[0].Position.Y, 32, 64), Fighters[0].isStun ? Color.Red : (Fighters[0].Unlimited ? Color.Gold : Color.White));
            spriteBatch.Draw(Ennemy, new Rectangle((int)Fighters[1].Position.X, (int)Fighters[1].Position.Y, 32, 64), Fighters[1].isStun ? Color.Red : (Fighters[1].Unlimited ? Color.Gold : Color.White));

            spriteBatch.Draw(WoTAP, new Rectangle(0, spriteBatch.GraphicsDevice.Viewport.Height, 453, 340), new Rectangle(0, 0, 1024, 768), Color.White, Rotation, new Vector2(512.0F, 768.0F / 2), SpriteEffects.None, 0.0F);
            spriteBatch.Draw(WoTCenter, new Rectangle(0, spriteBatch.GraphicsDevice.Viewport.Height, 453, 340), new Rectangle(0, 0, 1024, 768), Color.White, CenterRotation, new Vector2(512.0F, 768.0F / 2), SpriteEffects.None, 0.0F);

            spriteBatch.Draw(Ally, new Rectangle((int)(Math.Round(100 * Math.Cos(FightersRotation[0]))), spriteBatch.GraphicsDevice.Viewport.Height - (int)(Math.Round(100 * Math.Sin(FightersRotation[0]))), 24, 24), new Rectangle(4, 0, 24, 24), FightersState[0] == BattleFighterState.Disabled ? Color.Black : (FightersState[0] == BattleFighterState.Inactive ? Color.Gray : (Fighters[0].Unlimited ? Color.Gold : (Fighters[0].isStun ? Color.Red : (FightersState[0] == BattleFighterState.Recovering ? Color.DarkGray : Color.White)))), 0.0F, new Vector2(12, 12), SpriteEffects.None, 1);
            spriteBatch.Draw(Ennemy, new Rectangle((int)(Math.Round(100 * Math.Cos(FightersRotation[1]))), spriteBatch.GraphicsDevice.Viewport.Height - (int)(Math.Round(100 * Math.Sin(FightersRotation[1]))), 24, 24), new Rectangle(4, 0, 24, 24), FightersState[1] == BattleFighterState.Disabled ? Color.Black : (FightersState[1] == BattleFighterState.Inactive ? Color.Gray : (Fighters[1].Unlimited ? Color.Gold : (Fighters[1].isStun ? Color.Red : Color.White))), 0.0F, new Vector2(12, 12), SpriteEffects.None, 1);

            spriteBatch.DrawString(spriteFont, ((int)(AP / MaxAP * 100)).ToString() + "%", new Vector2(100.0F * (float)Math.Sin(CenterRotation), spriteBatch.GraphicsDevice.Viewport.Height - 110.0F * (float)Math.Cos(CenterRotation) - 10.0F), Color.White);
            spriteBatch.DrawString(spriteFont, ((int)Math.Min(Math.Max((AP + MaxAP / 8) / MaxAP * 100, 25), 100)).ToString() + "%", new Vector2(0, spriteBatch.GraphicsDevice.Viewport.Height - 120.0F), Color.Blue);
            spriteBatch.DrawString(spriteFont, ((int)Math.Max(Math.Min((AP - MaxAP / 8) / MaxAP * 100, 75), 0)).ToString() + "%", new Vector2(100.0F, spriteBatch.GraphicsDevice.Viewport.Height - 15.0F), Color.Red);
            spriteBatch.DrawString(spriteFont, ((int)Math.Ceiling(AP)).ToString() + "/" + ((int)Math.Ceiling(MaxAP)).ToString(), new Vector2(150.0F, spriteBatch.GraphicsDevice.Viewport.Height - 120.0F), Color.Green);

            spriteBatch.DrawString(spriteFont, Fighters[1].Name + " " + Fighters[1].Stats.ToString("AP", true) + " " + FightersState[1].ToString(), new Vector2(530.0F, 390.0F), FightersState[1] == BattleFighterState.Command ? Color.Blue : Color.White);

            spriteBatch.DrawString(spriteFont, Fighters[1].Name + "\n" + Fighters[1].Stats.ToString("End", true) + "\n" + Fighters[1].Stats.ToString("MP", false) + "\n" + "U.: " + Fighters[1].unlimitedGauge.ToString(), new Vector2(350.0F, 0.0F), Color.White);

            
            // Draw Target "Menu"
            if (TargetVisible)
            {
                spriteBatch.DrawString(spriteFont, "Target", Fighters[FightersSkillConfiguration[CommandIndex].Target].Position - new Vector2(0.0F, 30.0F), Color.Blue);
            }

            // Draw Action Name
            if (ActionIndex >= 0)
            {
                spriteBatch.DrawString(spriteFont, FightersAction[ActionIndex].Name, Fighters[ActionIndex].Position - new Vector2(spriteFont.MeasureString(FightersAction[ActionIndex].Name).X / 2, 30.0F), Color.White);
            }

            // DrawDamage :
            foreach (DamageStruct damage in DisplayDamageList)
            {
                spriteBatch.DrawString(spriteFont, damage.criticMessage, Fighters[damage.index].Position - new Vector2(0.0F, 30.0F), damage.color);
                spriteBatch.DrawString(spriteFont, damage.damage.ToString(), Fighters[damage.index].Position - new Vector2(-8.0F, -64.0F), damage.color);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        #endregion

        private void PerformAction(int CommandIndex)
        {
            Cue AttackerCue;
            Cue TargetCue;
            Cue OtherCue;
            Random rand = new Random();
            DamageStruct DamageDisplay = new DamageStruct();
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
            int p = rand.Next(0, 99);
            p++;
            int s = 0;
            for (int j = 0; j < 5; j++)
            {
                if (p >= s && p <= (criticProbTab[j, index] + s))
                {
                    e *= criticEfficiencyTab[j, index];
                    DamageDisplay.criticMessage = criticMessage[j];
                    break;
                }
                else
                    s += criticProbTab[j, index];
            }
            
            // Randomize from -10% to +10% :
            e = e + (float)rand.Next(-10, 11) / 100.0F * e;
            // Pour chaque cible :
            int target = FightersSkillConfiguration[ActionIndex].Target;
            // Réussite de la réaction et modification de l'efficacité en fonction.
            int reactionRank = FightersReactionRank[target];
            int actionRank = 0;
            switch (FightersReaction[target])
            {
                case 7:
                    // Defend
                    // Comparaison du rang de la réaction avec le rang d'action correspondant.
                    e *= (float)(rand.Next(35, 66)) / 100.0F;
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
                    if (rand.Next(0,100) < FightersBattleCommandCount[target])
                    {
                        e *= 0;
                        DamageDisplay.criticMessage = "Parry!";
                        OtherCue = soundBank.GetCue("Parry");
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
                    if (rand.Next(0,100) < FightersBattleCommandCount[target])
                    {
                        e *= 0;
                        DamageDisplay.criticMessage = "Dodge!";
                        OtherCue = soundBank.GetCue("Dodge");
                        if (SoundEnabled)
                            OtherCue.Play();
                        Fighters[target].Strike(Fighters[ActionIndex].Position, 15.0F * reactionRank, 5.0F);
                    }
                    break;
                case 10:
                    // Parry
                    // Comparaison du rang de la réaction avec le rang d'action correspondant.
                    actionRank = Math.Max(FightersAction[ActionIndex].ParryRank,FightersAction[ActionIndex].DodgeRank);
                    //if (rand.Next(0, 100) < reactionRankProbTab[actionRank, reactionRank-1] / 2)
                    if (rand.Next(0,100) < FightersBattleCommandCount[target])
                    {
                        e *= -1;
                        DamageDisplay.criticMessage = "Counter!";
                    }
                    break;
               default:
                    // Do nothing.
                    break;
            }
            if (actionRank > reactionRank - 1)
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
            AttackerCue = soundBank.GetCue(Fighters[AttackerIndex].AttackCue) ;
            if (VoiceEnabled[1])
              AttackerCue.Play();
            int Damage = 0;
            Damage = FightersAction[ActionIndex].CalculateDamage(Fighters[AttackerIndex], Fighters[TargetIndex]);
            Fighters[target].unlimitedGauge += (1 + rand.Next(0, 200) * Damage) / 200;
            Damage = (int)Math.Round(Damage * e);    
            if (e != 0)
            {
                Fighters[TargetIndex].Stun(FightersAction[ActionIndex].Stun * Math.Max(0, e));
                Fighters[TargetIndex].Strike(Fighters[AttackerIndex].Position, FightersAction[ActionIndex].Strike * Math.Max(e, 0), FightersAction[ActionIndex].StrikeSpeed * Math.Max(e, 0));
                Fighters[TargetIndex].Stats["End"] -= Damage;
                if (Fighters[TargetIndex].Stats["End"] > (int)(Fighters[TargetIndex].Stats["End", true] * 30.0F / 100.0F))
                    TargetCue = soundBank.GetCue(Fighters[TargetIndex].WoundCue);
                else
                    TargetCue = Fighters[TargetIndex].Stats["End"] == 0 ? soundBank.GetCue(Fighters[TargetIndex].DeathCue) : soundBank.GetCue(Fighters[TargetIndex].VeryWoundCue);
                if (VoiceEnabled[1])
                    TargetCue.Play();
            }  
            // Affichage des dommages :
            DamageDisplay.damage = Damage > 0 ? Damage : - Damage;
            DamageDisplay.index = TargetIndex;
            DamageDisplay.lifeTime = DAMAGEDISPLAYDURATION;
            DamageDisplay.color = (Damage > 0 ? Color.Red : Color.Green);
            DisplayDamageList.Add(DamageDisplay);
           

            // Pour le lanceur :
            // Eventuelle application d'effets.
            // Chgt d'état.
            FightersState[ActionIndex] = BattleFighterState.Recovering;
            FightersTime[ActionIndex] = FightersAction[ActionIndex].RecoveryTime;
            FightersAction[ActionIndex] = null;
            ActionIndex = -1;
            // Recovery.
        }

        #region Aux

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
        LoseBattle
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

    #endregion
}