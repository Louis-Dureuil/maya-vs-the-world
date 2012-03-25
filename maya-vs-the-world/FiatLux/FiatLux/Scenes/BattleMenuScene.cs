/* Fiat Lux V0.2.14
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiatLux.Logic;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace FiatLux.Scenes
{
    public class ReactionRankSelectEventArgs : EventArgs
    {
        public int rank;
        public ReactionRankSelectEventArgs(int rank)
        {
            this.rank = rank;
        }
    }
    public class BattleMenuScene : Scene
    {
        public delegate void OnReactionRankSelecting(object sender, ReactionRankSelectEventArgs e);
        public event OnReactionRankSelecting ReactionRankSelected;
        const float CONFIGUREDURATION = 15.0F; //Temporary Decision. Will later be skill-dependant
        const float PERFECTREACTIONTIMING = 0.5F;
        BattleScene BattleParent;

        int[] skillCost = new int[4];
        int[] nearestTargetCost = new int[4];

        bool oldVisible = false;

        Cue systemCue;

        Texture2D disabledstar;
        Texture2D enabledstar;

        int selected = 0;
        TextWindow[] MenuEntries; // 5 entries: 0 => Parent Command. 1..4 => Current Command ID on branches U-L-R-D.
        TextWindow TimeWindow;


        int Selected { get { return selected; } set { MenuEntries[selected].TextColor = Color.White; selected = value; MenuEntries[selected].TextColor = Color.Blue; } } // The Command currently selected. 0 is invalid.

        bool[] disabledCommands = new bool[4];
            
        int[] Commands = new int[5];

        public bool isReaction;
        public Fighter fighter;

        public BattleMenuScene(BattleScene parent, Fighter fighter, bool isReaction)
            : base(parent)
        {
            this.isReaction = isReaction;
            this.fighter = fighter;
            this.Paused += new PausedEventHandler(BattleMenuScene_Paused);
            this.UnPaused += new UnPausedEventHandler(BattleMenuScene_UnPaused);
            this.BattleParent = parent;

            Initialize();
        }

        public delegate void OnSelecting(object sender, MenuWindowEventArgs e);

        public event OnSelecting Selecting;

        public override void Initialize()
        {
            int HOffset = 128;
            int VOffset = 25;
            int Hs = LuxEngine.Shared.FRAMESIZE;
            int Vs = LuxEngine.Shared.FRAMESIZE;
            if (MenuEntries == null)
            {
                TimeWindow = new TextWindow(this, new Vector2(135, 31));
                Game.Components.Add(TimeWindow);
                MenuEntries = new TextWindow[5];
                MenuEntries[0] = new TextWindow(this, new Rectangle(0, 0, HOffset, VOffset));
                MenuEntries[1] = new TextWindow(this, new Rectangle(0, -(VOffset + Vs), HOffset, VOffset));
                MenuEntries[2] = new TextWindow(this, new Rectangle(-(HOffset + Hs), 0, HOffset, VOffset));
                MenuEntries[3] = new TextWindow(this, new Rectangle(HOffset + Hs, 0, HOffset, VOffset));
                MenuEntries[4] = new TextWindow(this, new Rectangle(0, VOffset + Vs, HOffset, VOffset));
                for (int i = 0; i<MenuEntries.Length;i++)
                    Game.Components.Add(MenuEntries[i]);
                MenuEntries[0].horizontalAlignment = HorizontalAlignment.Center;
                MenuEntries[1].horizontalAlignment = HorizontalAlignment.Center;
                MenuEntries[2].horizontalAlignment = HorizontalAlignment.Right;
                MenuEntries[4].horizontalAlignment = HorizontalAlignment.Center;    
            }
            
            if (fighter != null)
            {
                if (isReaction)
                {
                    Commands[0] = 0;
                    Commands[1] = fighter.ReactionCommands[0];
                    Commands[2] = fighter.ReactionCommands[1];
                    Commands[3] = fighter.ReactionCommands[2];
                    Commands[4] = fighter.ReactionCommands[3];
                }
                else
                {
                    Commands[0] = 0;
                    Commands[1] = fighter.ActionCommands[0];
                    Commands[2] = fighter.ActionCommands[1];
                    Commands[3] = fighter.ActionCommands[2];
                    Commands[4] = fighter.ActionCommands[3];
                }
            }
            Selected = 0;


            base.Initialize();
        }

        void BattleMenuScene_UnPaused(object sender, EventArgs e)
        {
            this.Visible = oldVisible;    
        }

        void BattleMenuScene_Paused(object sender, EventArgs e)
        {
            oldVisible = Visible;
            this.Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isPaused)
            {
                disabledCommands[0] = !fighter.CanUseCommand(Commands[1]);
                disabledCommands[1] = !fighter.CanUseCommand(Commands[2]);
                disabledCommands[2] = !fighter.CanUseCommand(Commands[3]);
                disabledCommands[3] = !fighter.CanUseCommand(Commands[4]);
                if (Selected > 0 && disabledCommands[Selected-1])
                    Selected = 0;
                if (isReaction)
                    UpdateReactionProb();
                UpdateMenu();
            }
            base.Update(gameTime);
        }

        private void UpdateReactionProb()
        {           
                int ActionRank = 0;
                int i = 0;
                if (Selected == 1)
                {
                    i = 1;
                    ActionRank = Math.Min(((BattleScene)parent).FightersAction[((BattleScene)parent).ActionIndex].ParryRank, ((BattleScene)parent).FightersAction[((BattleScene)parent).ActionIndex].DodgeRank);
                }
                if (Selected == 2)
                {
                    i = 2;
                    ActionRank = ((BattleScene)parent).FightersAction[((BattleScene)parent).ActionIndex].ParryRank;
                }
                if (Selected == 3)
                {
                    ActionRank = ((BattleScene)parent).FightersAction[((BattleScene)parent).ActionIndex].DodgeRank;
                    i = 3;
                }
                if (Selected == 4)
                {
                    i = 4;
                    ActionRank = Math.Max(((BattleScene)parent).FightersAction[((BattleScene)parent).ActionIndex].DodgeRank, ((BattleScene)parent).FightersAction[((BattleScene)parent).ActionIndex].ParryRank);
                }

                float realTime = Constants.COMMANDDURATION - PERFECTREACTIONTIMING * ActionRank;
                if (i != 0 && realTime <= BattleParent.FightersReactionTime[BattleParent.ReactionIndex] && BattleParent.FightersReactionRank[BattleParent.ReactionIndex] == ActionRank)
                {
                    BattleParent.FightersBattleCommandCount[BattleParent.ReactionIndex] = 100;
                }
                else if (i != 0)
                {
                    int Count = 0;
                    // Pre vs. Speed
                    int Pre = BattleParent.Fighters[BattleParent.ActionIndex].Stats["Pre"];
                    int Speed = BattleParent.Fighters[BattleParent.ReactionIndex].Stats["Speed"];
                    // Energy vs. Per
                    int Energy = BattleParent.Fighters[BattleParent.ActionIndex].Stats["STR2"];
                    int Per = BattleParent.Fighters[BattleParent.ReactionIndex].Stats["Per"];
                    double timeRatio = Math.Sqrt(BattleParent.FightersReactionTime[BattleParent.ReactionIndex] / realTime);
                    int ProbMin = BattleParent.reactionRankMinProbTab[ActionRank, BattleParent.FightersReactionRank[BattleParent.ReactionIndex] - 1];
                    int ProbMax = BattleParent.reactionRankMaxProbTab[ActionRank, BattleParent.FightersReactionRank[BattleParent.ReactionIndex] - 1];
                    if (i == 2)
                    {
                        Count = (int)Math.Ceiling( ((ProbMin * Energy + ProbMax * Per)/ (Energy + Per)) *  timeRatio);
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
                        BattleParent.FightersBattleCommandCount[BattleParent.ReactionIndex] = 95;
                    }
                    else
                    {
                        BattleParent.FightersBattleCommandCount[BattleParent.ReactionIndex] = Count;
                    }
                    if (i == 1)
                    {
                        BattleParent.FightersBattleCommandCount[BattleParent.ReactionIndex] = 100;
                    }
                }
                //if (i != 0)
                //{
                ////    //BattleCommandCount = COMMANDDURATION - PERFECTREACTIONTIMING <= FightersReactionTime[ReactionIndex] && FightersReactionRank[ReactionIndex] == ActionRank ? 100 : (int)Math.Ceiling(reactionRankProbTab[ActionRank, FightersReactionRank[ReactionIndex] - 1] / (i == 4 ? 2 : 1) * (FightersReactionTime[ReactionIndex]) / (COMMANDDURATION - PERFECTREACTIONTIMING));
                ////    //BattleCommandCount = COMMANDDURATION - PERFECTREACTIONTIMING <= FightersReactionTime[ReactionIndex] && FightersReactionRank[ReactionIndex] == ActionRank ? 100 : (int)Math.Ceiling(reactionRankProbTab[ActionRank, FightersReactionRank[ReactionIndex] - 1] / (i == 4 ? 2 : 1) * Math.Pow((FightersReactionTime[ReactionIndex]) / (COMMANDDURATION - PERFECTREACTIONTIMING), 2));
                //    ((BattleScene)parent).FightersBattleCommandCount[((BattleScene)parent).ReactionIndex] = Constants.COMMANDDURATION - PERFECTREACTIONTIMING <= ((BattleScene)parent).FightersReactionTime[((BattleScene)parent).ReactionIndex] && ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] == ActionRank ? 100 : (int)Math.Ceiling(((BattleScene)parent).reactionRankProbTab[ActionRank, ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] - 1] / (i == 4 ? 2 : 1) * Math.Sqrt((((BattleScene)parent).FightersReactionTime[((BattleScene)parent).ReactionIndex]) / (Constants.COMMANDDURATION - PERFECTREACTIONTIMING)));
                //}
            }
        

        void UpdateMenu()
        {
            int SkillID = Commands[Selected];
            if (isReaction)
            {
                if (Input.isActionDone(Input.Action.BattleCancel, false))
                {
                    if (Selecting != null)
                    {
                        Selecting(this, new MenuWindowEventArgs(Commands[0]));
                    }
                    if (ReactionRankSelected != null)
                            ReactionRankSelected(this, new ReactionRankSelectEventArgs(0));
                    Selected = 0;
                }
                if (Input.isActionDone(Input.Action.BattleConfirm, false))
                {
                    if (Selected != 0)
                    {
                        ((BattleScene)parent).FightersReaction[((BattleScene)parent).ReactionIndex] = SkillID;
                        ((BattleScene)parent).FightersReactionTime[((BattleScene)parent).ReactionIndex] = 0;
                        systemCue = LuxGame.soundBank.GetCue("Confirm");
                        systemCue.Play();
                    }
                }
                if (Input.isActionDone(Input.Action.BattleMenuDown, false))
                {
                    if (Selected == 4)
                    {
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] = Math.Min(5, ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] + 1);
                        if (ReactionRankSelected != null)
                            ReactionRankSelected(this, new ReactionRankSelectEventArgs(((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex]));
                    }
                    else if (!disabledCommands[3])
                    {
                        systemCue = LuxGame.soundBank.GetCue("Select");
                        systemCue.Play();
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[4]));
                        }
                        Selected = 4;
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] = 1;
                    }
                }
                if (Input.isActionDone(Input.Action.BattleMenuLeft, false))
                {
                    if (Selected == 2)
                    {
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] = Math.Min(5, ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] + 1);
                        if (ReactionRankSelected != null)
                            ReactionRankSelected(this, new ReactionRankSelectEventArgs(((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex]));
                    }
                    else if (!disabledCommands[1])
                    {
                        systemCue = LuxGame.soundBank.GetCue("Select");
                        systemCue.Play();

                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[2]));
                        }
                        Selected = 2;
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] = 1;
                    }
               
                }

                if (Input.isActionDone(Input.Action.BattleMenuRight, false))
                {
                    if (Selected == 3)
                    {
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] = Math.Min(5, ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] + 1);
                        if (ReactionRankSelected != null)
                            ReactionRankSelected(this, new ReactionRankSelectEventArgs(((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex]));
                    }
                    else if (!disabledCommands[2])
                    {
                        systemCue = LuxGame.soundBank.GetCue("Select");
                        systemCue.Play();
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[3]));
                        }
                        Selected = 3;
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] = 1;
                    }
               
                }
                if (Input.isActionDone(Input.Action.BattleMenuUp, false))
                {
                    if (Selected == 1)
                    {
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] = Math.Min(5, ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] + 1);
                        if (ReactionRankSelected != null)
                            ReactionRankSelected(this, new ReactionRankSelectEventArgs(((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex]));
                    }
                    else if (!disabledCommands[0])
                    {
                        systemCue = LuxGame.soundBank.GetCue("Select");
                        systemCue.Play();
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[1]));
                        }
                        Selected = 1;
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] = 1;
                    }
               
                }
                if (Input.isActionDone(Input.Action.BattleSoftCancel, false))
                {
                    if (((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex] == 1)
                    {
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[0]));
                        }
                        if (ReactionRankSelected != null)
                            ReactionRankSelected(this, new ReactionRankSelectEventArgs(0));
       
                        Selected = 0;
                    }
                    else
                    {
                        ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex]--;
                        if (ReactionRankSelected != null)
                            ReactionRankSelected(this, new ReactionRankSelectEventArgs(((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex]));
       
                    }
                }
            }
            else
            {
                if (Input.isActionDone(Input.Action.Cancel, false))
                {
                    if (fighter.Unlimited)
                        fighter.unlimitedGauge /= 2;
                    else
                        ((BattleScene)parent).FightersTime[((BattleScene)parent).CommandIndex] = 0;
                }
                if (Input.isActionDone(Input.Action.BattleCancel, false))
                {
                    this.Initialize();
                }
                else if (Input.isActionDone(Input.Action.BattleConfirm, false))
                {
                    if (GameData.commands[Commands[Selected]] is Skill)
                    {

                        ((BattleScene)parent).FightersAction[((BattleScene)parent).CommandIndex] = (Skill)GameData.commands[SkillID];
                        ((BattleScene)parent).FightersAction[((BattleScene)parent).CommandIndex].InitializeConfiguration(ref ((BattleScene)parent).FightersSkillConfiguration[((BattleScene)parent).CommandIndex]);
                        if (((BattleScene)parent).FightersAction[((BattleScene)parent).CommandIndex].Type == Skill.SkillType.Physical)
                        {
                            ((BattleScene)parent).FightersState[((BattleScene)parent).CommandIndex] = BattleFighterState.Configure;
                            ((BattleScene)parent).State = BattleState.Configuration;
                            ((BattleScene)parent).FightersTime[((BattleScene)parent).CommandIndex] = CONFIGUREDURATION;
                        }
                        else if (BattleParent.FightersAction[BattleParent.CommandIndex].Type == Skill.SkillType.Magical)
                        {
                            ((BattleScene)parent).FightersState[((BattleScene)parent).CommandIndex] = BattleFighterState.Casting;
                            ((BattleScene)parent).State = BattleState.Idle;
                            ((BattleScene)parent).FightersTime[((BattleScene)parent).CommandIndex] = ((BattleScene)parent).FightersAction[((BattleScene)parent).CommandIndex].CastTime;
                        }
                        else
                        {
                            BattleParent.FightersState[BattleParent.CommandIndex] = BattleFighterState.Inactive;
                            BattleParent.State = BattleState.Configuration;
                            BattleParent.FightersAction[BattleParent.CommandIndex].Launch(BattleParent,BattleParent.FightersSkillConfiguration[BattleParent.CommandIndex]);
                            BattleParent.CommandIndex = -1;
                            this.Visible = false;
                        }
                        this.Enabled = false;
                        systemCue = LuxGame.soundBank.GetCue("Confirm");
                        systemCue.Play();
                    }
                    else
                    {
                        systemCue = LuxGame.soundBank.GetCue("ConfirmDisabled");
                        systemCue.Play();
                    }
                }
                else if (Input.isActionDone(Input.Action.BattleMenuDown, false))
                {
                    if (Commands[4] != 0)
                    {
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[4]));
                        }
                        ChangeRoot(4);
                    }
                    systemCue = LuxGame.soundBank.GetCue("Select");
                    systemCue.Play();
               
                }
                else if (Input.isActionDone(Input.Action.BattleMenuLeft, false))
                {
                    if (Commands[2] != 0)
                    {
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[2]));
                        }
                        ChangeRoot(2);
                    }
                    systemCue = LuxGame.soundBank.GetCue("Select");
                    systemCue.Play();
               
                }
                else if (Input.isActionDone(Input.Action.BattleMenuRight, false))
                {
                    if (Commands[3] != 0)
                    {
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[3]));
                        }
                        ChangeRoot(3);
                    }
                    systemCue = LuxGame.soundBank.GetCue("Select");
                    systemCue.Play();
               
                }

                else if (Input.isActionDone(Input.Action.BattleMenuUp, false))
                {
                    if (Commands[1] != 0)
                    {
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[1]));
                        }
                        ChangeRoot(1);
                    }
                    systemCue = LuxGame.soundBank.GetCue("Select");
                    systemCue.Play();
                }
                else if (Input.isActionDone(Input.Action.BattleSoftCancel, false))
                {
                    if (Selected != 0)
                        Selected = 0;
                    else
                    {
                        if (Selecting != null)
                        {
                            Selecting(this, new MenuWindowEventArgs(Commands[0]));
                        }
                        ChangeRoot(0);
                    }
                }
            }
        }
  

        // Aux Method for UpdateCommand.
        void ChangeRoot(int newRoot)
        {

            int SkillID;
            if (newRoot == 0)
                SkillID = GameData.commands[Commands[0]].Parent;
            else
                SkillID = Commands[newRoot];
            if (GameData.commands[SkillID] is Skill)
            {
                if (!disabledCommands[newRoot-1])
                    Selected = newRoot;
            }
            else
            {
                Selected = 0;
                Commands[0] = SkillID;
                if (SkillID == 0)
                {
                    Commands[1] = fighter.HasCommand(fighter.ActionCommands[0]) ? fighter.ActionCommands[0] : 0;
                    Commands[2] = fighter.HasCommand(fighter.ActionCommands[1]) ? fighter.ActionCommands[1] : 0;
                    Commands[3] = fighter.HasCommand(fighter.ActionCommands[2]) ? fighter.ActionCommands[2] : 0;
                    Commands[4] = fighter.HasCommand(fighter.ActionCommands[3]) ? fighter.ActionCommands[3] : 0;
                    disabledCommands[0] = !fighter.CanUseCommand(fighter.ActionCommands[0]);
                    disabledCommands[1] = !fighter.CanUseCommand(fighter.ActionCommands[1]);
                    disabledCommands[2] = !fighter.CanUseCommand(fighter.ActionCommands[2]);
                    disabledCommands[3] = !fighter.CanUseCommand(fighter.ActionCommands[3]);
                }
                else
                {
                    Commands[1] = fighter.HasCommand(GameData.commands[SkillID].Children[0]) ? GameData.commands[SkillID].Children[0] : 0;
                    Commands[2] = fighter.HasCommand(GameData.commands[SkillID].Children[1]) ? GameData.commands[SkillID].Children[1] : 0;
                    Commands[3] = fighter.HasCommand(GameData.commands[SkillID].Children[2]) ? GameData.commands[SkillID].Children[2] : 0;
                    Commands[4] = fighter.HasCommand(GameData.commands[SkillID].Children[3]) ? GameData.commands[SkillID].Children[3] : 0;
                    disabledCommands[0] = !fighter.CanUseCommand(GameData.commands[SkillID].Children[0]);
                    disabledCommands[1] = !fighter.CanUseCommand(GameData.commands[SkillID].Children[1]);
                    disabledCommands[2] = !fighter.CanUseCommand(GameData.commands[SkillID].Children[2]);
                    disabledCommands[3] = !fighter.CanUseCommand(GameData.commands[SkillID].Children[3]);
                    for (int i = 0; i < skillCost.Length; i++)
                    {
                        if (GameData.commands[Commands[i]] is Skill)
                        {
                            skillCost[i-1] = (int)Math.Max(0,Math.Ceiling(((Skill)GameData.commands[Commands[i]]).RecoveryTime * ((BattleScene)Parent).APRatio));
                            nearestTargetCost[i-1] = (int)Math.Max(0,Math.Ceiling((((BattleScene)Parent).NearestTargetDistance(fighter, false) - ((Skill)GameData.commands[Commands[i]]).Range) / fighter.Speed));
                        }
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            spriteFont = this.Game.Content.Load<SpriteFont>("FontTest");
            //spriteFont = this.Game.Content.Load<SpriteFont>("DefaultSpriteFont");
            enabledstar = this.Game.Content.Load<Texture2D>("etoileactive");
            disabledstar = this.Game.Content.Load<Texture2D>("etoiledesactive");
          
            base.LoadContent();
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            for (int i = 0; i < MenuEntries.Length; i++)
            {
                MenuEntries[i].Text = GameData.commands[Commands[i]].Name;
                if (i != 0 && i != Selected)
                {
                    if (disabledCommands[i-1])
                        MenuEntries[i].TextColor = Color.Gray;
                    else if (!isReaction)
                    {
                        if (GameData.commands[Commands[i]] is Skill)
                            if (nearestTargetCost[i - 1] >= fighter.Stats["AP"])
                                MenuEntries[i].TextColor = Color.Red;
                            else if (nearestTargetCost[i - 1] + skillCost[i - 1] >= fighter.Stats["AP"])
                                MenuEntries[i].TextColor = Color.Yellow;
                            else
                                MenuEntries[i].TextColor = Color.White;
                        else
                            MenuEntries[i].TextColor = Color.White;
                    }
                }
            }
            if (MenuEntries[0].Text == "")
                MenuEntries[0].Text = (isReaction ? "Reaction" : "Action");
            TimeWindow.Text = !isReaction ? "Time " + (Math.Ceiling(((BattleScene)parent).FightersTime[((BattleScene)parent).CommandIndex])).ToString() : "Time " + (Math.Ceiling(((BattleScene)parent).FightersReactionTime[((BattleScene)parent).ReactionIndex])).ToString();
            TimeWindow.TextColor = ((BattleScene)parent).CommandIndex >= 0 ? (((BattleScene)parent).FightersTime[((BattleScene)parent).CommandIndex] > Constants.COMMANDDURATION / 3.0F ? Color.White : Color.Red) : (((BattleScene)parent).FightersReactionTime[((BattleScene)parent).ReactionIndex] > Constants.COMMANDDURATION / 3.0F ? Color.White : Color.Red);

            LuxGame.spriteBatch.Begin();
            if (Selected != 0)
            {
                Vector2 starPosition = Vector2.Zero;
                if (Selected == 1)
                    starPosition = new Vector2(0, -60);

                if (Selected == 2)
                    starPosition = new Vector2(-128, 30);

                if (Selected == 3)
                    starPosition = new Vector2(128, -30);

                if (Selected == 4)
                    starPosition = new Vector2(0, 60);

                if (isReaction)
                {
                    for (int i = 5; i >= ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex]; i--)
                        LuxGame.spriteBatch.Draw(disabledstar, Position + starPosition + new Vector2(16 * i, 0), Color.White);

                    for (int i = ((BattleScene)parent).FightersReactionRank[((BattleScene)parent).ReactionIndex]; i > 0; i--)
                        LuxGame.spriteBatch.Draw(enabledstar, Position + starPosition + new Vector2(16 * i, 0), Color.White);

                    LuxGame.spriteBatch.DrawString(spriteFont, ((BattleScene)parent).FightersBattleCommandCount[((BattleScene)parent).ReactionIndex].ToString() + "%", Position + starPosition, Color.White);
                }
                else
                {
                    LuxGame.spriteBatch.DrawString(spriteFont, skillCost[Selected-1].ToString() + "+" + nearestTargetCost[Selected-1].ToString(), Position + starPosition, Color.White);
                }
            }
            LuxGame.spriteBatch.End();
        }
    }
}
