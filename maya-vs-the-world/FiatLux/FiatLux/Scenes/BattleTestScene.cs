using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using FiatLux.Logic;

namespace FiatLux.Scenes
{
    public class BattleTestScene : BattleScene
    {
        public BattleTestScene(LuxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            Initializing += new OnInitializing(BattleTestScene_Initializing);
            Initialized += new OnInitializing(BattleTestScene_Initialized);
           
            base.Initialize();
        }

        void BattleTestScene_Initializing(object sender, EventArgs e)
        {
            pauseMenuChoices = "Resume\nRetry\nView Tutorial\nGo to Sound Test\nQuit";
        }

        void BattleTestScene_Initialized(object sender, EventArgs e)
        {
            MusicCue = MusicSoundBank.GetCue("battle_1");

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
            for (int i = 0; i < Lucien.maxBaseExp.Length; i++)
            {
                Lucien.maxBaseExp[i] = i*i * 10;
            }

            for (int i = 0; i < Lucien.maxStatExp.GetLength(0); i++)
            {
                for (int j = 0; j < Lucien.maxStatExp.GetLength(1); j++)
                {
                    Lucien.maxStatExp[i, j] = j * j * 100;
                }
            }

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
            ((Skill)GameData.commands[2]).parametersNames = new string[1] { "Target" };
            ((Skill)GameData.commands[2]).AttackStats["PhyAtt"] = 8;
            ((Skill)GameData.commands[2]).DefenseStats["PhyDef"] = 10;
            ((Skill)GameData.commands[2]).DodgeRank = 3;
            ((Skill)GameData.commands[2]).ParryRank = 1;
            GameData.commands[3].Name = "Strong Attack";
            ((Skill)GameData.commands[3]).parametersNames = new string[1] { "Target" };
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

            Master.commands.Add(2);
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

            PauseMenu.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(PauseMenu_Confirmed);

        }

        void PauseMenu_Confirmed(object sender, MenuWindowEventArgs e)
        {
            switch (e.Choice)
            {
                case "Retry":
                    Destroy();
                    Game.Components.Add(new BattleTestScene(this.LuxGame));
                    TooglePause(this, new EventArgs());
                    break;
                case "Go to Sound Test":
                    Destroy();
                    Game.Components.Add(new SoundTestScene(this.LuxGame));
                    TooglePause(this, new EventArgs());
                    break;
                case "View Tutorial":
                    Destroy();
                    Game.Components.Add(new BattleTutorialScene(this.LuxGame));
                    TooglePause(this, new EventArgs());
                    break;
            }
        }
    }
}
