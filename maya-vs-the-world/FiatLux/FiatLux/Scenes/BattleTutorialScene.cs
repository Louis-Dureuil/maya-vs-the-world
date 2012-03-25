/* Fiat Lux V0.2.21
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
using FiatLux.Logic;


namespace FiatLux.Scenes
{
    public class BattleTutorialScene : BattleScene
    {
  
        SoundBank TutorialSoundBank;
        WaveBank TutorialWaveBank;
        Cue TutorialVoice; 
        MessageWindow TutorialMessageWindow;
        string TutorialMessage;
        bool[] tutorialConditions = new bool[92]
        {
            true, //0
            true, //1
            true, //2
            true, //3
            true, //4
            true, //5
            true, //6
            true, //7
            true, //8
            true, //9
            true, //10
            true, //11
            false, //12
            true, //13
            true, //14
            true, //15
            true, //16
            true, //17
            true, //18,
            true, //19
            true, //20
            false, //21
            true, //22
            true, //23
            true, //24,
            true, //25
            true, //26
            false, //27
            true, //28
            true, //29
            true, //30
            true, //31
            true, //32
            true, //33
            false, //34
            true, //35
            true, //36
            false, //37
            true, //38
            true, //39
            true, //40
            true, //41
            true, //42
            true, //43
            true, //44
            false, //45
            true, //46
            false, //47
            true, //48
            false, //49
            true, //50
            true, //51
            true, //52
            true, //53
            true, //54
            true, //55
            false, //56
            true, //57
            true, //58
            true, //59
            true, //60
            true, //61
            true, //62
            true, //63
            true, //64
            false, //65
            true, //66
            false, //67
            true, //68
            true, //69
            true, //70
            false, //71
            true, //72
            true, //73
            true, //74
            true, //75
            true, //76
            true, //77
            true, //78
            true, //79
            true, //80
            true, //81
            true, //82
            true, //83
            true, //84
            true, //85
            true, //86
            true, //87
            true, //88
            false, //89
            true, //90
            true, //91
        };
        
        readonly string[] tutorialMessages = new string[92]
        { 
            "Hey! Welcome on this tutorial! Press Enter to continue.", //0
            "Let's have a look at the user interface first.", //1
            "What you can see here is your character...", //2
            "...and this is your ennemy.\nYou have to defeat him to win the demo.", //3
            "The green bar here indicates how many endurance he still has.\nWhen he has no more endurance, he is defeated.", //4
            "Here is your own endurance.\nIf you have no more, you lose,\nso be careful.", //5
            "There are you Magic Points,\nbut they aren't very useful in the demo,\nif you ask me.", //6
            "There is your 'Unlimited Gauge.',\nWe'll see how it works later.", //7
            "Here is the Wheel of Time.\nIt's related to how much AP\nyou and your ennemy have.", //8
            "AP stands for 'Action Points',\nbut we'll cover this later.", //9
            "Here is your AP bar though.", //10
            "Ok, Let's start the battle.\nPress the P key to pause the game at any moment.\nAre you ready? (press Enter when you are)", //11
            "Here we go!", //12
            "Seen what happened?", //13
            "The wheel, well, turned!\nThis indicates the beginning of a new Turn.", //14
            "The number here shows the current AP of the wheel.", //15
            "At the beginning of a turn, it is set to the highest\nvalue between your and your ennemy's AP.", //16
            "The wheel of Time indicates who will have the initiative\nand when.", //17
            "The fighters that already have the initiative this turn\nare shown left to the arrow, those that are pointed\nby the arrow currently have the initiative, and those that are\n right to the arrow will have it later on this turn.", //18
            "Here for instance, your ennemy has probably 100 AP\nwhich is more than you, and he'll have immediatly the initiative.", //19
            "He will probably use it to attack you.", //20
            "See? He did'nt even let me finish.", //21
            "But in between you've got the initiative\nand a battle menu appeared!", //22
            "As you can see, you have 10\nseconds to make your choice.\nHowever, the timer is disabled\n for educational purposes.", //23
            "To select a command, just use the directionnal arrows\nin the direction of the command you want to select.", //24
            "If you make a mistake, you can cancel it by pressing Right Shift.\nOr press Back to go back to the 'Action' Menu.", //25
            "You can also give up your initiative by pressing Escape,\nexcepted in the tutorial.", //26
            "Select 'Attacks' for now.", //27
            "The battle menu now shows the subcommands of the 'Attacks' command", //28
            "All battle commands have up to four associated subcommands,", //29
            "A subcommand can be another command or a skill.", //30
            "For now, you only have two skills.", //31
            "I know it's pretty lame, but at least it'll be easy for the tutorial.", //32
            "Each skill has properties, like the amount of damage it deals,\nits range, or the number of AP you need to recover after using it.", //33
            "Select the 'Quick Attack' skill.", //34
            "Since it's a skill, it doesn't expand subcommands.", //35
            "Instead, it shows how many AP you need to recover after using the skill\nand how many AP you need to reach the nearest target.", //36
            "Select the 'Strong Attack' skill.", //37
            "'Strong Attack' needs more AP to recover\nand has a bigger range, thus need less AP to reach the nearest target.", //38
            "Press Enter to choose the 'Strong Attack' skill.",//39
            "Hurray, you've now entered the configuration phase.", //40
            "The configuration phase depends on the skill you've chosen.", //41
            "Here, since 'Strong Attack' is pretty basic\nyou only have to choose your target.", //42
            "Keep in mind that you are\nstill timed during this phase.", //43
            "You can select your target with the directional arrows,\nbut since you've already selected your ennemy,\ngo ahead and press Enter.", //44
            "There you are, your character will run toward the target.", //45
            "And when you're in range, let's see what happens!", //46
            "There you are you've hit your ennemy!", //47
            "But he still have to attack you.", //48
            "Here he comes.", //49
            "Each time an ennemy is about to attack you,\nyou have the possibility to oppose a reaction.", //50
            "There is four kinds of reactions:", //51
            "Defend will reduce the damage you take\nand will always work. It's the 'default' solution.", //52
            "Parry and Dodge will let you cancel your ennemy's attack,\nbut they won't work if you don't choose the right grade", //53
            "Each attack has parry and dodge grades", //54
            "You have to guess, then to memorize theses grades\nin order to choose the correct one when you have to react\nto the attack.", //55
            "Select 'Parry'.", //56
            "The grade you've chosen is symbolized by the number of stars\nshown next to the reaction you've chosen.", //57
            "You can raise it by selecting the reaction again,\nand you can decrease it by pressing R. Shift.", //58
            "Since this is a demo, you don't have to guess\nthe attack's Grade:", //59
            "the success rate of your reaction is shown next to the stars.", //60
            "So just choose the grade in order to have the higher success rate", //61
            "Success rate also decreases with time, so don't be too slow!", //62
            "The last available reaction is 'Counter',\n which allows you to cancel the attack of your ennemy\nand to attack in return.", //63
            "But it has the lowest rate of success,\nso be careful when using it.", //64
            "For now, select 'Parry' with the third\nreaction Grade (press Left twice).", //65
            "Good. Press Enter to confirm your choice.", //66
            "Nice, you succeeded!", //67
            "As you can see you take no damage.", //68
            "Keep in mind that using the good reaction in battle\nis a key concept if you want to have a long life.", //69
            "Once all fighters attacked, the turn gets over.", //70
            "Here, seen?", //71
            "A new turn can begin!", //72
            "But you and your ennemy used a few AP, so the max is no longer 100", //73
            "At this point, you may wonder what happens when you have no more AP?", //74
            "Well, first, you cannot attack for a turn,\nwhich can be pretty annoying\nif you're only one slash away from your\nennemy's death.", //75
            "Moreover, you cannot oppose any reaction,\nwhich means that you are doomed\nto take any attack from your ennemy hundred percents!", //76
            "Ouch. So save your AP.", //77
            "After a turn with 0 AP you get all your AP back though.", //78
            "Oh, and on a side note:\nafter an attack you use AP to recover, remember?\nWhen it happens, you also can't oppose a reaction.", //79
            "So plan your attacks well!", //80
            "One last thing before you go on a real fight.", // 81
            "Remember the 'Unlimited Gauge'?", //82
            "When you made a reaction it filled a little.", //83
            "It fills when you make reactions\nor when you take damage.", // 84
            "What happens when it's full?", //85
            "Let's cheat a little, OK?", //86
            "You entered the 'Unlimited Mode'.\n In this mode, you're able to enter\n commands as long as you've still have AP.", //87
            "So the more AP you have when you enter it, the better!", //88
            "Let's have fun! Use all your AP!", //89
            "You're done!", //90
            "This is the end of this tutorial! You can replay it by\nchoosing 'Battle Tutorial' in the main menu\nPress Enter to go to the main menu." //91
        };
        readonly string[] tutorialSpecialMessages = new string[6]
        {
             "The game is paused. To continue the Tutorial, press P or choose 'Resume'.", //0
              "Here we go! Press P to pause the game again.", //1
              "To continue the tutorial, press P to unpause the game first!", //2
              "Nah! You must select 'Attack'. Press Return or R.Shift to cancel!", //3
              "No more unlimited mode!", //4
              "Ennemy defeated!" //5
        };

        int tutorialIndex = 0;

        Sprite Pointer;

        public BattleTutorialScene(Scene parent)
            : base(parent)
        {
        }

        public BattleTutorialScene(LuxGame game)
            : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (TutorialMessage != null)
                TutorialMessageWindow.Text = TutorialMessage;
            TutorialMessageWindow.Visible = TutorialMessageWindow.Text != "";
            TutorialMessageWindow.Enabled = true;
            if (tutorialIndex == 67 && AP <= 5)
            {
                State = BattleState.Paused;
                tutorialIndex++;
                TutorialMessage = tutorialMessages[tutorialIndex];
                if (TutorialVoice != null && TutorialVoice.IsPlaying)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
            }
            base.Update(gameTime);
        }

        public override void Initialize()
        {
            Initializing += new OnInitializing(BattleTutorialScene_Initializing);
            Initialized += new OnInitializing(BattleTutorialScene_Initialized);
            TurnEnded += new OnTurnEnding(BattleTutorialScene_TurnEnded);
            TurnStarted += new OnTurnStarting(BattleTutorialScene_TurnStarted);
            FighterHasInitiative += new OnFighterHavingInitiative(BattleTutorialScene_FighterHasInitiative);
            Configured += new OnConfiguring(BattleTutorialScene_Configured);
            Attacked += new OnAttacking(BattleTutorialScene_Attacked);
            Attacking += new OnAttacking(BattleTutorialScene_Attacking);
            Paused += new PausedEventHandler(BattleTutorialScene_Paused);
            UnPaused += new UnPausedEventHandler(BattleTutorialScene_UnPaused);
            Reacted += new OnReacting(BattleTutorialScene_Reacted);
            PerformedAction += new OnPerformingAction(BattleTutorialScene_PerformedAction);
            base.Initialize();
        }

        void BattleTutorialScene_PerformedAction(object sender, BattleEventArgs e)
        {
            if (e.fighter == Fighters[1])
                Fighters[0].Stats.ToMax("End");
        }

        void BattleTutorialScene_Reacted(object sender, BattleEventArgs e)
        {
            if (e.fighter is Ennemy && tutorialIndex == 49)
            {
                
                State = BattleState.Paused;
                battleMenu.Visible = true;
                battleMenu.Enabled = false;
                battleMenu.Selecting += new BattleMenuScene.OnSelecting(battleMenu_Selecting);
                battleMenu.ReactionRankSelected += new BattleMenuScene.OnReactionRankSelecting(battleMenu_ReactionRankSelected);
                tutorialIndex++;
                TutorialMessage = tutorialMessages[tutorialIndex];
                if (TutorialVoice != null && TutorialVoice.IsPlaying)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
            }
        }

        void battleMenu_ReactionRankSelected(object sender, ReactionRankSelectEventArgs e)
        {
            if (tutorialIndex == 65 && e.rank == 3)
            {
                FightersReactionTime[0] = Constants.COMMANDDURATION;
                tutorialIndex++;
                
                TutorialMessage = tutorialMessages[tutorialIndex];
                if (TutorialVoice != null && TutorialVoice.IsPlaying)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
            }
            if (tutorialIndex == 66)
            {
                if (e.rank != 3)
                {
                    tutorialConditions[66] = false;
                    FightersReactionTime[0] = 0;  
                }
                else
                {
                    tutorialConditions[66] = true;
                    FightersReactionTime[0] = Constants.COMMANDDURATION;
                }
            }
        }

        void BattleTutorialScene_Attacking(object sender, BattleEventArgs e)
        {
            if (e.fighter is Ennemy && tutorialIndex == 47)
            {
                State = BattleState.Paused;
                tutorialIndex++;
                TutorialMessage = tutorialMessages[tutorialIndex];
                if (TutorialVoice != null && TutorialVoice.IsPlaying)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
            }
        }



        void BattleTutorialScene_Attacked(object sender, BattleEventArgs e)
        {
            if (e.fighter is Character && tutorialIndex == 45)
            {
                State = BattleState.Paused;
                tutorialIndex++;
                TutorialMessage = tutorialMessages[tutorialIndex];
                if (TutorialVoice != null && TutorialVoice.IsPlaying)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
            }

        }

        void BattleTutorialScene_Configured(object sender, BattleEventArgs e)
        {
            if (e.fighter is Character && tutorialIndex == 40)
            { State = BattleState.Paused;
            TargetWindow.Enabled = false;
            }
        }

        void BattleTutorialScene_FighterHasInitiative(object sender, BattleEventArgs e)
        {
            if (e.fighter is Character && tutorialIndex == 21)
            {
                State = BattleState.Paused;
                battleMenu.Visible = true;
                battleMenu.Enabled = false;
                battleMenu.Selecting += new BattleMenuScene.OnSelecting(battleMenu_Selecting);
                tutorialIndex++;
                if (TutorialVoice != null)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
                TutorialMessage = tutorialMessages[tutorialIndex];
                Pointer.Position.X = battleMenu.Position.X + Pointer.Width / 2 + 100;
                Pointer.Position.Y = battleMenu.Position.Y - Pointer.Height / 2 - spriteFont.MeasureString("Attacks").Y - LuxEngine.Shared.FRAMESIZE /2;
                Pointer.Visible = true;
                TutorialMessageWindow.Position.X = Pointer.Position.X - TutorialMessageWindow.WindowRectangle.Width /2;
                TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height - Pointer.Height / 2;
            }
        }

        void battleMenu_Selecting(object sender, MenuWindowEventArgs e)
        {
            if (tutorialIndex == 56 && GameData.commands[e.Index].Name == "Parry")
            {
                battleMenu.Enabled = false;
                tutorialIndex++;
                TutorialMessage = tutorialMessages[tutorialIndex];
                if (TutorialVoice != null && TutorialVoice.IsPlaying)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
            }
           
            if (tutorialIndex ==27)
            switch (GameData.commands[e.Index].Name)
            {
                case "Attacks":
                    message = "";
                    tutorialIndex++;
                    if (TutorialVoice != null)
                    {
                        TutorialVoice.Stop(AudioStopOptions.Immediate);
                        TutorialVoice.Dispose();
                    }
                    TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex+1).ToString());
                    TutorialVoice.Play();
                    TutorialMessage = tutorialMessages[tutorialIndex];
                    TutorialMessageWindow.Position.X -= 200;
                    battleMenu.Enabled = false;
                    Pointer.Visible = false;
                    break;
                default:
                    message = tutorialSpecialMessages[3];
                    break;
            }
            if (tutorialIndex == 34 && GameData.commands[e.Index].Name == "Quick Attack")
            {
                tutorialIndex++;
                if (TutorialVoice != null)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
                TutorialMessage = tutorialMessages[tutorialIndex];
                battleMenu.Enabled = false;
            }
            if (tutorialIndex == 37 && GameData.commands[e.Index].Name == "Strong Attack")
            {
                
                Pointer.Position.Y += 85;
                Pointer.Rotation = 5*(float)Math.PI / 6;
                      
                tutorialIndex++;
                if (TutorialVoice != null)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                TutorialVoice.Play();
                TutorialMessage = tutorialMessages[tutorialIndex];
                battleMenu.Enabled = false;
            }
        }

        void BattleTutorialScene_Initializing(object sender, EventArgs e)
        {
            this.VoiceEnabled[1] = false;
            this.VoiceEnabled[2] = false;
            this.VoiceEnabled[3] = false;

            pauseMenuChoices = "Resume\nRestart Tutorial\nStart a real battle!\nGo to Sound Test\nQuit";
        }

        void BattleTutorialScene_TurnStarted(object sender, EventArgs e)
        {
            State = BattleState.Paused;
            tutorialIndex++;
            TutorialMessage = tutorialMessages[tutorialIndex];
            Pointer.Rotation = (float)Math.PI / 4;
            Pointer.Position.X = this.WoT[0].Position.X + WoT[0].Width / 3;
            Pointer.Position.Y = this.WoT[0].Position.Y - WoT[0].Height / 2.5F;
            TutorialMessageWindow.Position.X = Pointer.Position.X + Pointer.Width / 2;
            TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height - Pointer.Height / 2;
            Pointer.Visible = true;
            if (TutorialVoice != null)
            {
                TutorialVoice.Stop(AudioStopOptions.Immediate);
                TutorialVoice.Dispose();
            }
            TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
            TutorialVoice.Play();
        }

        void TutorialMessageWindow_Destroying(object sender, MessageWindowEventArgs e)
        {
            if (!isPaused && TutorialMessageWindow.Enabled && tutorialConditions[tutorialIndex])
            {
                switch (tutorialIndex)
                {
                    case 0:
                        TutorialMessageWindow.Position = new Vector2(306, 0);
                        break;
                    case 1:

                        Pointer.Visible = true;
                        Pointer.Rotation = 5 * (float)Math.PI / 6;
                        Pointer.Position.X = Fighters[0].Position.X + Fighters[0].Sprite.Width;
                        Pointer.Position.Y = Fighters[0].Position.Y + Fighters[0].Sprite.Height;
                        TutorialMessageWindow.Position.X = Pointer.Position.X - 100;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y + Pointer.Height / 2;

                        break;
                    case 2:
                        Pointer.Rotation = -(float)Math.PI / 6;
                        Pointer.Position.X = Fighters[1].Position.X - Fighters[1].Sprite.Width;
                        Pointer.Position.Y = Fighters[1].Position.Y - Fighters[1].Sprite.Height;
                        TutorialMessageWindow.Position.X = Pointer.Position.X - 300;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - 1.5F * Pointer.Height;

                        break;
                    case 3:
                        Pointer.Rotation = -(float)Math.PI / 2;
                        Pointer.Position.X = Fighters[1].Position.X - Fighters[1].Sprite.Width - Pointer.Width;
                        Pointer.Position.Y = Fighters[1].Position.Y + Fighters[1].Sprite.Height / 2 + 10;
                        TutorialMessageWindow.Position.X = Pointer.Position.X - 300;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y;
                        break;
                    case 4:
                        Pointer.Rotation = (float)Math.PI / 2;
                        Pointer.Position.X = this.statusWindows[0].Position.X + 160 + Pointer.Width / 2;
                        Pointer.Position.Y = statusWindows[0].Position.Y + LuxEngine.Shared.FRAMESIZE / 2 + spriteFont.MeasureString("Lucien").Y * 2;
                        TutorialMessageWindow.Position.X = Pointer.Position.X + Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - Pointer.Height / 2;
                        break;
                    case 5:
                        Pointer.Rotation = (float)Math.PI / 2;
                        Pointer.Position.X = this.statusWindows[0].Position.X + 160 + Pointer.Width / 2;
                        Pointer.Position.Y = statusWindows[0].Position.Y + LuxEngine.Shared.FRAMESIZE / 2 + spriteFont.MeasureString("Lucien").Y * 3;
                        TutorialMessageWindow.Position.X = Pointer.Position.X + Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - Pointer.Height / 2;
                        break;
                    case 6:
                        Pointer.Rotation = (float)Math.PI / 2;
                        Pointer.Position.X = this.statusWindows[0].Position.X + 160 + Pointer.Width / 2;
                        Pointer.Position.Y = statusWindows[0].Position.Y + LuxEngine.Shared.FRAMESIZE / 2 + spriteFont.MeasureString("Lucien").Y * 4;
                        TutorialMessageWindow.Position.X = Pointer.Position.X + Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - Pointer.Height / 2;
                        break;
                    case 7:
                        Pointer.Rotation = (float)Math.PI / 4;
                        Pointer.Position.X = this.WoT[0].Position.X + WoT[0].Width / 3;
                        Pointer.Position.Y = this.WoT[0].Position.Y - WoT[0].Height / 2.5F;
                        TutorialMessageWindow.Position.X = Pointer.Position.X + Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height - Pointer.Height / 2;
                        break;
                    case 8:
                        Pointer.Visible = false;
                        break;
                    case 9:
                        Pointer.Visible = true;
                        Pointer.Rotation = -(float)Math.PI / 2;
                        Pointer.Position.X = this.APWindows[0].Position.X - Pointer.Width / 2;
                        Pointer.Position.Y = APWindows[0].Position.Y + LuxEngine.Shared.FRAMESIZE + spriteFont.MeasureString("Lucien").Y;
                        TutorialMessageWindow.Position.X = Pointer.Position.X - TutorialMessageWindow.WindowRectangle.Width - Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height / 2;
                        break;
                    case 10:
                        Pointer.Visible = false;
                        TutorialMessageWindow.Position.X = GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(tutorialMessages[tutorialIndex + 1]).X / 2;
                        TutorialMessageWindow.Position.Y = GraphicsDevice.Viewport.Height / 2 - spriteFont.MeasureString(tutorialMessages[tutorialIndex + 1]).Y / 2;
                        break;
                    case 11:
                        TutorialMessageWindow.Position.X = GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(tutorialMessages[tutorialIndex + 1]).X / 2;
                        TutorialMessageWindow.Position.Y = GraphicsDevice.Viewport.Height / 2 - spriteFont.MeasureString(tutorialMessages[tutorialIndex + 1]).Y / 2;
                        State = BattleState.StartTurn;
                        break;
                    case 20:
                        Pointer.Visible = false;
                        State = BattleState.Idle;
                        break;
                    case 22:
                        Pointer.Rotation = (float)Math.PI / 2;
                        Pointer.Position.X += 96;
                        Pointer.Position.Y += 96;
                        TutorialMessageWindow.Position.X = Pointer.Position.X + Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height;
                        break;
                    case 23:
                        TutorialMessageWindow.Position.X = GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(tutorialMessages[tutorialIndex + 1]).X / 2;
                        TutorialMessageWindow.Position.Y = GraphicsDevice.Viewport.Height / 2 - spriteFont.MeasureString(tutorialMessages[tutorialIndex + 1]).Y / 2;
                        Pointer.Visible = false;
                        Pointer.Position.X = battleMenu.Position.X + Pointer.Width / 2 + 100;
                        Pointer.Position.Y = battleMenu.Position.Y - Pointer.Height / 2 - spriteFont.MeasureString("Attacks").Y - LuxEngine.Shared.FRAMESIZE / 2;

                        break;

                    case 26:
                        battleMenu.Enabled = true;
                        Pointer.Visible = true;
                        Pointer.Rotation = (float)Math.PI / 6;
                        TutorialMessageWindow.Position.X = Pointer.Position.X;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height - Pointer.Height / 2;
                        break;
                    case 33:
                        Pointer.Visible = true;
                        ((Character)Fighters[0]).DisabledCommands.Add(3);
                        battleMenu.Enabled = true;
                        break;
                    case 35:
                        Pointer.Rotation = 0;
                        Pointer.Position.X -= 100;
                        Pointer.Position.Y -= spriteFont.MeasureString("32").Y + Pointer.Height / 3;
                        TutorialMessageWindow.Position.X = Pointer.Position.X - TutorialMessageWindow.WindowRectangle.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height * 3;
                        break;
                    case 36:
                        Pointer.Position.X -= 100;
                        Pointer.Position.Y += 75;
                        Pointer.Rotation = (float)Math.PI / 6;
                        TutorialMessageWindow.Position.X = Pointer.Position.X;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height - Pointer.Height / 2;

                        ((Character)Fighters[0]).DisabledCommands.Remove(3);
                        ((Character)Fighters[0]).DisabledCommands.Add(2);
                        battleMenu.Enabled = true;
                        break;
                    case 39:
                        battleMenu.Enabled = true;
                        break;
                    case 41:
                        Pointer.Rotation = 0;
                        Pointer.Position.X = TargetWindow.WindowRectangle.X + TargetWindow.WindowRectangle.Width / 2;
                        Pointer.Position.Y = TargetWindow.WindowRectangle.Y - Pointer.Height / 2;
                        TutorialMessageWindow.Position.X = Pointer.Position.X - 200;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - 100;
                        break;
                    case 42:
                        Pointer.Rotation = (float)Math.PI / 2;
                        Pointer.Position.X = battleMenu.Position.X + 230;
                        Pointer.Position.Y = battleMenu.Position.Y + 45;
                        TutorialMessageWindow.Position.X = Pointer.Position.X + Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - 10;

                        break;
                    case 43:
                        Pointer.Rotation = 0;
                        Pointer.Position.X = TargetWindow.WindowRectangle.X + TargetWindow.WindowRectangle.Width / 2;
                        Pointer.Position.Y = TargetWindow.WindowRectangle.Y - Pointer.Height / 2;
                        TutorialMessageWindow.Position.X = Pointer.Position.X - 200;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - 100;
                        break;
                    case 44:
                        Pointer.Visible = false;
                        State = BattleState.Configuration;
                        TargetWindow.Enabled = true;
                        break;
                    case 46:
                        State = BattleState.Action;
                        break;
                    case 48:
                        State = BattleState.Idle;
                        break;
                    case 55:
                        ((Character)Fighters[0]).DisabledCommands.Add(7);
                        ((Character)Fighters[0]).DisabledCommands.Add(9);
                        ((Character)Fighters[0]).DisabledCommands.Add(10);
                        battleMenu.Enabled = true;
                        break;
                    case 64:
                        battleMenu.Enabled = true;
                        break;
                    case 66:
                        State = BattleState.Action;
                        break;
                    case 70:
                        State = BattleState.Idle;
                        break;
                    case 81:
                        Pointer.Rotation = (float)Math.PI / 2;
                        Pointer.Position.X = this.statusWindows[0].Position.X + 160 + Pointer.Width / 2;
                        Pointer.Position.Y = statusWindows[0].Position.Y + LuxEngine.Shared.FRAMESIZE / 2 + spriteFont.MeasureString("Lucien").Y * 4;
                        TutorialMessageWindow.Position.X = Pointer.Position.X + Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - Pointer.Height / 2;
                        break;
                    case 86:
                        Fighters[0].Unlimited = true;
                        break;
                    case 87:
                        Pointer.Visible = true;
                        Pointer.Rotation = -(float)Math.PI / 2;
                        Pointer.Position.X = this.APWindows[0].Position.X - Pointer.Width / 2;
                        Pointer.Position.Y = APWindows[0].Position.Y + LuxEngine.Shared.FRAMESIZE + spriteFont.MeasureString("Lucien").Y;
                        TutorialMessageWindow.Position.X = Pointer.Position.X - TutorialMessageWindow.WindowRectangle.Width - Pointer.Width / 2;
                        TutorialMessageWindow.Position.Y = Pointer.Position.Y - TutorialMessageWindow.WindowRectangle.Height / 2;
                        break;
                    case 88:
                        Pointer.Visible = false;
                        State = BattleState.Idle;
                        TutorialMessageWindow.Position = new Vector2(306, 0);
                        ((Character)Fighters[0]).DisabledCommands.Clear();
                        break;
                    case 91:
                        new Scenes.DemoTitleScene(LuxGame);
                        this.Destroy();
                        break;
                }
                tutorialIndex++;
                if (TutorialVoice != null && TutorialVoice.IsPlaying)
                {
                    TutorialVoice.Stop(AudioStopOptions.Immediate);
                    TutorialVoice.Dispose();
                }
                if (!TutorialSoundBank.IsDisposed)
                {
                    TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
                    TutorialVoice.Play();

                    TutorialMessage = tutorialMessages[tutorialIndex];
                }

            }
            else
            {
                if (isPaused)
                    message = tutorialSpecialMessages[2];
                TutorialMessageWindow.Enabled = true;
            }

        }

        void BattleTutorialScene_UnPaused(object sender, EventArgs e)
        {
            message = tutorialSpecialMessages[1];
            TutorialMessageWindow.Enabled = false;
            if (TutorialVoice != null && TutorialVoice.IsPaused)
                TutorialVoice.Resume();
        }

        void BattleTutorialScene_Paused(object sender, EventArgs e)
        {
            message = tutorialSpecialMessages[0];
            if (TutorialVoice != null && TutorialVoice.IsPlaying)
                TutorialVoice.Pause();
        }

        void BattleTutorialScene_TurnEnded(object sender, EventArgs e)
        {
            if (tutorialIndex == 0)
            {
                this.TutorialMessageWindow.Destroying += new MessageWindow.OnDestroy(TutorialMessageWindow_Destroying);
                this.TutorialMessageWindow.destroyType = MessageConfirmType.OnConfirm;

                TutorialMessage = tutorialMessages[0];
                State = BattleState.Paused;
            }
        }
        public override void Destroy()
        {
            if (TutorialVoice != null)
                TutorialVoice.Dispose();
            TutorialWaveBank.Dispose();
            TutorialSoundBank.Dispose();
            LuxGame.engine.GetCategory("Music").SetVolume(1.0F);
            base.Destroy();
        }

        
        void BattleTutorialScene_Initialized(object sender, EventArgs e)
        {
            MusicCue = MusicSoundBank.GetCue("ecran_titre");
            LuxGame.engine.GetCategory("Music").SetVolume(0.3F);

            TutorialMessageWindow = new MessageWindow(this, new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(tutorialMessages[0]).X / 2, GraphicsDevice.Viewport.Height / 2), "", MessageConfirmType.None);

            Game.Components.Add(TutorialMessageWindow);
            TutorialMessageWindow.DrawOrder = 100;
            TutorialMessageWindow.BackgroundColor = new Color(255, 255, 255, 140);
            TutorialMessageWindow.Enabled = true;

            PauseMenu.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(PauseMenu_Confirmed);

            Pointer = new Sprite(this, new List<string> { "fleche" });
            Game.Components.Add(Pointer);
            Pointer.SetAnimation("fleche");
            Pointer.Visible = false;
            Pointer.Width /= 2;
            Pointer.Height /= 2;
            Pointer.DrawOrder = 100;
            Pointer.Position = new Vector2(100, 100);
            background = new Sprite(camera, new List<string>() { "backgroundM" });
            Game.Components.Add(background);
            background.SetAnimation("backgroundM");
            background.Position = new Vector2(background.Width / 2 - 200, background.Height / 2 - 200);

            //Initialize Fighters
            Character Lucien = new Character();
            Lucien.Name = "Lucien";
            Lucien.Stats["AP", true] = 75;
            Lucien.Stats["End", true] = 255;
            Lucien.Stats["MP", true] = 25;
            Lucien.Stats.ToMax("AP");
            Lucien.Stats.ToMax("MP");
            Lucien.Stats.ToMax("End");
            Lucien.Position = new Vector2(150, 150);
            Lucien.Sprite = new Sprite(camera, new List<string> { "Lucien", "lucien_dead" });
            Game.Components.Add(Lucien.Sprite);
            Lucien.faceName = "portraitLucien";

            Ennemy Master = new Ennemy();
            Master.Name = "Sergent";
            Master.Stats["AP", true] = 100;
            Master.Stats["End", true] = 300;
            Master.Stats.ToMax("AP");
            Master.Stats.ToMax("End");
            Master.Position = new Vector2(650, 300);
            Master.Sprite = new Sprite(camera, new List<string> { "Sergent" });
            Game.Components.Add(Master.Sprite);
            Master.faceName = "portraitSergent";

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
            ((Skill)GameData.commands[3]).RecoveryTime += 10.0F;
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

            Lucien.hasCommand[1] = true;
            Lucien.hasCommand[4] = true;
            Lucien.hasCommand[5] = true;
            Lucien.hasCommand[6] = true;
            Lucien.hasCommand[2] = true;
            Lucien.hasCommand[3] = true;


            Lucien.ReactionCommands[0] = 7;
            Lucien.ReactionCommands[1] = 8;
            Lucien.ReactionCommands[2] = 9;
            Lucien.ReactionCommands[3] = 10;

            Lucien.hasCommand[7] = true;
            Lucien.hasCommand[8] = true;
            Lucien.hasCommand[9] = true;
            Lucien.hasCommand[10] = true;
            Lucien.GodMode = true;
            Master.commands.Add(3);


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

            TutorialVoice = TutorialSoundBank.GetCue((tutorialIndex + 1).ToString());
            TutorialVoice.Play();

            Fighters[0] = Lucien;
            Fighters[1] = Master;

            FightersFace[0] = new Sprite(WoT[0], new List<string>() { Fighters[0].faceName });
            Game.Components.Add(FightersFace[0]);
            FightersFace[1] = new Sprite(WoT[0], new List<string>() { Fighters[1].faceName });
            Game.Components.Add(FightersFace[1]);

            FightersFace[0].SetAnimation(Fighters[0].faceName);
            FightersFace[1].SetAnimation(Fighters[1].faceName);

            Fighters[0].Sprite.SetAnimation("Lucien");
            Fighters[1].Sprite.SetAnimation("Sergent");
            Fighters[1].Sprite.effect = effect;
            background.effect = effect;
        }

        void PauseMenu_Confirmed(object sender, MenuWindowEventArgs e)
        {
            switch (e.Choice)
            {
                case "Restart Tutorial":
                    TooglePause(this, new EventArgs());
                  Destroy();
                  Game.Components.Add(new BattleTutorialScene(this.LuxGame));
                  break;
                case "Start a real battle!":
                    TooglePause(this, new EventArgs());
 
                    Destroy();
                    Game.Components.Add(new BattleTestScene(this.LuxGame));

                    break;
                case "Go to Sound Test":
                    TooglePause(this, new EventArgs());
   
                    Destroy();
                    Game.Components.Add(new SoundTestScene(this.LuxGame));

                    break;
            }
        }
        protected override void LoadContent()
        {
            TutorialSoundBank = new SoundBank(LuxGame.engine, "Content/Tutorial Sound Bank.xsb");
            TutorialWaveBank = new WaveBank(LuxGame.engine, "Content/TutorialVoices.xwb");
            base.LoadContent();
        }
    }
}
