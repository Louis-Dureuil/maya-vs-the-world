using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using FiatLux.Logic;

namespace FiatLux.Scenes
{
    public class BattleTestScene2 : BattleScene
    {
        public BattleTestScene2(LuxGame game)
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
                Lucien.maxBaseExp[i] = i* i * 50;
            }

            Character Rakun = new Character();
            Rakun.Name = "Rakun";
            Rakun.Stats["AP", true] = 50;
            Rakun.Stats["End", true] = 300;
            Rakun.Stats["MP", true] = 0;
            Rakun.Stats.ToMax("AP");
            Rakun.Stats.ToMax("MP");
            Rakun.Stats.ToMax("End");
            Rakun.Position = new Vector2(550, 250);
            Rakun.Sprite = new Sprite(camera, new List<string> { "RakunCôté"});
            Game.Components.Add(Rakun.Sprite);
            Rakun.faceName = "portraitRakun";

            for (int i = 0; i < Rakun.maxBaseExp.Length; i++)
            {
                Rakun.maxBaseExp[i] = i*i * 50;
            }


            for (int i = 0; i < Lucien.maxStatExp.GetLength(0); i++)
            {
                for (int j = 0; j < Lucien.maxStatExp.GetLength(1); j++)
                {

                    Lucien.maxStatExp[i, j] = j * j * 100;
                }
            }

            for (int i = 0; i < Rakun.maxStatExp.GetLength(0); i++)
            {
                for (int j = 0; j < Rakun.maxStatExp.GetLength(1); j++)
                {
                    Rakun.maxStatExp[i, j] = j * j * 100;
                }
            }

            Ennemy Master = new Ennemy();
            Master.Name = "Sergent A";
            Master.Stats["AP", true] = 100;
            Master.Stats["End", true] = 300;
            Master.Stats.ToMax("AP");
            Master.Stats.ToMax("End");
            Master.Position = new Vector2(650, 300);
            Master.Sprite = new Sprite(camera, new List<string> { "Sergent" });
            Game.Components.Add(Master.Sprite);
            Master.faceName = "portraitSergent";

            Ennemy Master2 = new Ennemy();
            Master2.Name = "Sergent B";
            Master2.Stats["AP", true] = 100;
            Master2.Stats["End", true] = 300;
            Master2.Stats.ToMax("AP");
            Master2.Stats.ToMax("End");
            Master2.Position = new Vector2(640, 200);
            Master2.Sprite = new Sprite(camera, new List<string> { "Sergent" });
            Game.Components.Add(Master2.Sprite);
            Master2.faceName = "portraitSergent";

            
            Master.Stats["PhyAtt"] = 45;
            Master2.Stats["PhyAtt"] = 45;
            Lucien.Stats["PhyAtt"] = 30;
            Rakun.Stats["PhyAtt"] = 25;
            Master.Stats["PhyDef"] = 20;
            Master2.Stats["PhyDef"] = 20;
            Lucien.Stats["PhyDef"] = 25;
            Rakun.Stats["PhyDef"] = 30;

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
            ((Skill)GameData.commands[2]).parametersNames = new string[1] { "Target" };
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
            ((Skill)GameData.commands[3]).AttackStats["PhyAtt"] = 10;
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

            GameData.commands[11].Name = "Tactics";
            GameData.commands[12] = new Skill();
            GameData.commands[12].ID = 12;
            GameData.commands[12].Name = "Move";
            ((Skill)GameData.commands[12]).Range = 0;
            ((Skill)GameData.commands[12]).RecoveryTime = 0;
            ((Skill)GameData.commands[12]).Shape = new Sprite(this, new List<string>() {"curseurzone"});
            Game.Components.Add(((Skill)GameData.commands[12]).Shape);
            ((Skill)GameData.commands[12]).Shape.SetAnimation("curseurzone");
            ((Skill)GameData.commands[12]).Shape.Width *= 2;
            ((Skill)GameData.commands[12]).Shape.Height *= 2;
            ((Skill)GameData.commands[12]).parametersNumber = 0;
            ((Skill)GameData.commands[12]).shapeParametersNumber = 2;
            ((Skill)GameData.commands[12]).parametersNames = new string[2] { "X", "Y" };
            ((Skill)GameData.commands[12]).targetMode = TargetMode.Area;
            ((Skill)GameData.commands[12]).Type = Skill.SkillType.Physical;

            GameData.commands[2].Parent = 1;
            GameData.commands[3].Parent = 1;

            GameData.commands[11].Parent = 6;
            GameData.commands[12].Parent = 6;

            GameData.commands[6].Children[3] = 11;
            GameData.commands[11].Children[3] = 12;

            GameData.commands[1].Children[0] = 2;
            GameData.commands[1].Children[1] = 3;

            Lucien.ActionCommands[0] = 1;
            Lucien.ActionCommands[1] = 4;
            Lucien.ActionCommands[2] = 5;
            Lucien.ActionCommands[3] = 6;

            Rakun.ActionCommands[0] = 1;
            Rakun.ActionCommands[1] = 4;
            Rakun.ActionCommands[2] = 5;
            Rakun.ActionCommands[3] = 6;

            Lucien.hasCommand[1] = true;
            Lucien.hasCommand[4] = true;
            Lucien.hasCommand[5] = true;
            Lucien.hasCommand[6] = true;
            Lucien.hasCommand[2] = true;
            Lucien.hasCommand[3] = true;
            Lucien.hasCommand[11] = true;
            Lucien.hasCommand[12] = true;

            Rakun.hasCommand[1] = true;
            Rakun.hasCommand[4] = true;
            Rakun.hasCommand[5] = true;
            Rakun.hasCommand[6] = true;
            Rakun.hasCommand[2] = true;
            Rakun.hasCommand[3] = true;
            Rakun.hasCommand[11] = true;
            Rakun.hasCommand[12] = true;


            Lucien.ReactionCommands[0] = 7;
            Lucien.ReactionCommands[1] = 8;
            Lucien.ReactionCommands[2] = 9;
            Lucien.ReactionCommands[3] = 10;

            Lucien.hasCommand[7] = true;
            Lucien.hasCommand[8] = true;
            Lucien.hasCommand[9] = true;
            Lucien.hasCommand[10] = true;

            Rakun.ReactionCommands[0] = 7;
            Rakun.ReactionCommands[1] = 8;
            Rakun.ReactionCommands[2] = 9;
            Rakun.ReactionCommands[3] = 10;

            Rakun.hasCommand[7] = true;
            Rakun.hasCommand[8] = true;
            Rakun.hasCommand[9] = true;
            Rakun.hasCommand[10] = true;


            Master.commands.Add(2);
            Master.commands.Add(3);

            Master2.commands.Add(2);
            Master2.commands.Add(3);

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

            Master2.AttackCue = "SergentAttack";
            Master2.DeathCue = "SergentDeath";
            Master2.VeryWoundCue = "SergentWound";
            Master2.WordCue = "SergentWord";
            Master2.WoundCue = "SergentWound";


            Fighters[0] = Lucien;
            Fighters[1] = Rakun;
            Fighters[2] = Master;
            Fighters[3] = Master2;

        

            FightersFace[0] = new Sprite(WoT[0], new List<string>() { Fighters[0].faceName });
            Game.Components.Add(FightersFace[0]);
            FightersFace[1] = new Sprite(WoT[0], new List<string>() { Fighters[1].faceName });
            Game.Components.Add(FightersFace[1]);
            FightersFace[2] = new Sprite(WoT[0], new List<string>() { Fighters[2].faceName });
            Game.Components.Add(FightersFace[2]);
            FightersFace[3] = new Sprite(WoT[0], new List<string>() { Fighters[3].faceName });
            Game.Components.Add(FightersFace[3]);


            FightersFace[0].SetAnimation(Fighters[0].faceName);
            FightersFace[1].SetAnimation(Fighters[1].faceName);
            FightersFace[2].SetAnimation(Fighters[2].faceName);
            FightersFace[3].SetAnimation(Fighters[3].faceName);


            Fighters[0].Sprite.SetAnimation("Lucien");
            Fighters[1].Sprite.SetAnimation("RakunCôté");

            Fighters[2].Sprite.SetAnimation("Sergent");
            Fighters[3].Sprite.SetAnimation("Sergent");
            Fighters[2].Sprite.effect = effect;
            Fighters[3].Sprite.effect = effect;
            background.effect = effect;

            PauseMenu.Confirmed += new MenuWindow.ChoiceConfirmedEventHandler(PauseMenu_Confirmed);

        }

        void PauseMenu_Confirmed(object sender, MenuWindowEventArgs e)
        {
            switch (e.Choice)
            {
                case "Retry":
                    Destroy();
                    Game.Components.Add(new BattleTestScene2(this.LuxGame));
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
