/* Fiat Lux V0.1.2
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

namespace FiatLux
{
    public static class GameData
    {
        public static Character[] characters = new Character[Constants.CharactersCapacity];
        public static Ennemy[] ennemies = new Ennemy[Constants.EnnemiesCapacity];
        public static Command[] commands = new Command[Constants.CommandsCapacity];
        public static string SystemLocalCode = "FR-fr";

        static string h = "Fiat Lux Game Data file Version ";
        static int v = Constants.GAMEDATAVERSION;
        public static Class[] classes = new Class[Constants.ClassNumber];

        public static string GetHeader
        {
            get
            {
                return h + v.ToString();
            }
        }
    }
    public static class GameDataManager
    {
        /// <summary>
        /// Initialize the game's Data with default parameters.
        /// </summary>
        public static void Initialize()
        {
            // Initialize commands
            for (int i = 0; i < Constants.CommandsCapacity; i++)
            {
                GameData.commands[i] = new Command();
                GameData.commands[i].ID = i;
            }
            GameData.commands[0].Name = "";
            GameData.commands[1].Name = Data.Local.Skills.Attacks;
            GameData.commands[2] = new Skill();
            GameData.commands[3] = new Skill();
            GameData.commands[2].ID = 2;
            GameData.commands[3].ID = 3;
            GameData.commands[2].Name = Data.Local.Skills.QuickAttack;
            ((Skill)GameData.commands[2]).AttackStats["PhyAtt"] = 8;
            ((Skill)GameData.commands[2]).parametersNames = new string[1] { "Target" };
            ((Skill)GameData.commands[2]).DefenseStats["PhyDef"] = 5;
            ((Skill)GameData.commands[2]).DodgeRank = 3;
            ((Skill)GameData.commands[2]).ParryRank = 1;
            GameData.commands[3].Name = Data.Local.Skills.StrongAttack;

            ((Skill)GameData.commands[3]).parametersNames = new string[1] { "Target" };
            ((Skill)GameData.commands[3]).Range += 10.0F;
            ((Skill)GameData.commands[3]).Strike += 20.0F;
            ((Skill)GameData.commands[3]).StrikeSpeed += 10.0F;
            ((Skill)GameData.commands[3]).Stun += 20.0F;
            ((Skill)GameData.commands[3]).RecoveryTime = 0.4F;
            ((Skill)GameData.commands[3]).BaseDamage += 5;
            ((Skill)GameData.commands[3]).ParryRank = 3;
            ((Skill)GameData.commands[3]).DodgeRank = 1;
            ((Skill)GameData.commands[3]).AttackStats["PhyAtt"] = 10;
            ((Skill)GameData.commands[3]).DefenseStats["PhyDef"] = 5;
            GameData.commands[4].Name = Data.Local.Skills.Magic;
            GameData.commands[5].Name = Data.Local.Skills.Summon;
            GameData.commands[6].Name = Data.Local.Skills.Special;

            GameData.commands[13] = new Skill();
            Skill skill = (Skill)GameData.commands[13];
            skill.Name = Data.Local.Skills.Fire;
            skill.ID = 13;
            skill.AttackStats["MagAtt"] = 10;
            skill.DefenseStats["MagDef"] = 5;
            skill.CastTime = 0.25F;
            skill.Cost = 10;
            skill.targetMode = TargetMode.Single;
            skill.Type = Skill.SkillType.Magical;

            GameData.commands[7] = new Reaction();
            GameData.commands[7].Name = Data.Local.Skills.Defend;
            GameData.commands[8] = new Reaction();
            GameData.commands[8].Name = Data.Local.Skills.Parry;
            GameData.commands[9] = new Reaction();
            GameData.commands[9].Name = Data.Local.Skills.Dodge;
            GameData.commands[10] = new Reaction();
            GameData.commands[10].Name = Data.Local.Skills.Counter;

            GameData.commands[11].Name = Data.Local.Skills.Tactics;
            GameData.commands[12] = new Skill();
            GameData.commands[12].ID = 12;
            GameData.commands[12].Name = Data.Local.Skills.Move;
            ((Skill)GameData.commands[12]).Range = 0;
            ((Skill)GameData.commands[12]).RecoveryTime = 0;
            ((Skill)GameData.commands[12]).shapeName = "curseurzone";
            ((Skill)GameData.commands[12]).parametersNumber = 0;
            ((Skill)GameData.commands[12]).shapeParametersNumber = 2;
            ((Skill)GameData.commands[12]).parametersNames = new string[2] { "X", "Y" };
            ((Skill)GameData.commands[12]).targetMode = TargetMode.Area;
            ((Skill)GameData.commands[12]).Type = Skill.SkillType.Physical;


            GameData.commands[14] = new Skill();
            skill = (Skill)GameData.commands[14];
            skill.Name = Data.Local.Skills.Rest;
            skill.ID = 14;
            skill.Type = Skill.SkillType.Special;
            skill.targetMode = TargetMode.Self;

            skill.SkillLaunched += new Skill.OnSkillLaunching(Rest_SkillLaunched);

            GameData.commands[2].Parent = 1;
            GameData.commands[3].Parent = 1;

            GameData.commands[11].Parent = 6;
            GameData.commands[12].Parent = 11;

            GameData.commands[14].Parent = 6;

            GameData.commands[6].Children[3] = 11;
            GameData.commands[11].Children[3] = 12;
            GameData.commands[11].Children[1] = 14;

            GameData.commands[1].Children[0] = 2;
            GameData.commands[1].Children[1] = 3;
           
            
            // Initialize ennemies
            Ennemy Master = new Ennemy();
            Master.Name = Data.Local.Ennemies.Sergeant;
            Master.Stats["AP", true] = 100;
            Master.Stats["End", true] = 400;
            Master.Stats.ToMax("AP");
            Master.Stats.ToMax("End");
            Master.Position = new Vector2(650, 300);
            Master.StanceSpriteName = "Sergent";
            Master.faceName = "portraitSergent";
            Master.EnnemyMult = 2.5F;

            Master.Stats["PhyAtt"] = 50;
            Master.Stats["PhyDef"] = 30;
            Master.Stats["STR2"] = 40;
            Master.Stats["Pre"] = 40;

            Master.commands.Add(2);
            Master.commands.Add(3);

            Master.AttackCue = "SergentAttack";
            Master.DeathCue = "SergentDeath";
            Master.VeryWoundCue = "SergentWound";
            Master.WordCue = "SergentWord";
            Master.WoundCue = "SergentWound";

            GameData.ennemies[0] = Master;

            // Initialize classes //TODO: Weapons & Commands.

            for (int i = 0; i < Constants.ClassNumber; i++)
            {
                GameData.classes[i] = new Class();
                GameData.classes[i].MaximumXP = new int[Constants.MaximumClassLevel*2];
                for (int j = 1; j < Constants.MaximumClassLevel * 2; j++)
                    GameData.classes[i].MaximumXP[j] = (GameData.classes[i].MaximumXP[j - 1] + 50 * j + 50 * j * j);            
            }
            // #0 Student
            GameData.classes[0].Name = Data.Local.Classes.Student;
            GameData.classes[0].CaracRaise = new int[Constants.CARACNUMBER] { 0, 1, 2, 3, 4, 5 };
            GameData.classes[0].CommandID = 0;
            GameData.classes[0].Rank = 1;
            GameData.classes[0].QuestItemID = 0;

            // #1 Warrior
            GameData.classes[1].Name = Data.Local.Classes.Warrior;
            GameData.classes[1].CaracRaise = new int[1] { 0 };
            GameData.classes[1].CommandID = 10;
            GameData.classes[1].Rank = 1;
            GameData.classes[1].QuestItemID = 1;

            // #2 Guardian
            GameData.classes[2].Name = Data.Local.Classes.Guardian;
            GameData.classes[2].CaracRaise = new int[1] { 1 };
            GameData.classes[2].CommandID = 11;
            GameData.classes[2].Rank = 1;
            GameData.classes[2].QuestItemID = 2;

            // #3 Wanderer
            GameData.classes[3].Name = Data.Local.Classes.Wanderer;
            GameData.classes[3].CaracRaise = new int[1] { 2 };
            GameData.classes[3].CommandID = 12;
            GameData.classes[3].Rank = 1;
            GameData.classes[3].QuestItemID = 3;

            // #4 Archer
            GameData.classes[4].Name = Data.Local.Classes.Archer;
            GameData.classes[4].CaracRaise = new int[1] { 3 };
            GameData.classes[4].CommandID = 13;
            GameData.classes[4].Rank = 1;
            GameData.classes[4].QuestItemID = 4;

            // #5 Wizard
            GameData.classes[5].Name = Data.Local.Classes.Wizard;
            GameData.classes[5].CaracRaise = new int[1] { 4 };
            GameData.classes[5].CommandID = 14;
            GameData.classes[5].Rank = 1;
            GameData.classes[5].QuestItemID = 5;

            // #6 Scholar
            GameData.classes[6].Name = Data.Local.Classes.Scholar;
            GameData.classes[6].CaracRaise = new int[1] { 5 };
            GameData.classes[6].CommandID = 15;
            GameData.classes[6].Rank = 1;
            GameData.classes[6].QuestItemID = 6;

            // #7 Brute
            GameData.classes[7].Name = Data.Local.Classes.Brute;
            GameData.classes[7].CaracRaise = new int[1] { 0 };
            GameData.classes[7].CommandID = 16;
            GameData.classes[7].Rank = 2;
            GameData.classes[7].QuestItemID = 7;

            // #8 Survivor
            GameData.classes[8].Name = Data.Local.Classes.Survivor;
            GameData.classes[8].CaracRaise = new int[1] { 1 };
            GameData.classes[8].CommandID = 17;
            GameData.classes[8].Rank = 2;
            GameData.classes[8].QuestItemID = 8;

            // #9 Acrobat
            GameData.classes[9].Name = Data.Local.Classes.Acrobat;
            GameData.classes[9].CaracRaise = new int[1] { 2 };
            GameData.classes[9].CommandID = 18;
            GameData.classes[9].Rank = 2;
            GameData.classes[9].QuestItemID = 9;

            // #10 Gunner
            GameData.classes[10].Name = Data.Local.Classes.Gunner;
            GameData.classes[10].CaracRaise = new int[1] { 3 };
            GameData.classes[10].CommandID = 19;
            GameData.classes[10].Rank = 2;
            
            // #11 Sorcerer
            GameData.classes[11].Name = Data.Local.Classes.Sorcerer;
            GameData.classes[11].CaracRaise = new int[1] { 4 };
            GameData.classes[11].CommandID = 20;
            GameData.classes[11].Rank = 2;
            
            // #12 Sage
            GameData.classes[12].Name = Data.Local.Classes.Sage;
            GameData.classes[12].CaracRaise = new int[1] { 5 };
            GameData.classes[12].CommandID = 21;
            GameData.classes[12].Rank = 2;

            // #13 Swordman
            GameData.classes[13].Name = Data.Local.Classes.Swordman;
            GameData.classes[13].CaracRaise = new int[2] { 0,1 };
            GameData.classes[13].CommandID = 22;
            GameData.classes[13].Rank = 2;

            // #14 Fencer
            GameData.classes[14].Name = Data.Local.Classes.Fencer;
            GameData.classes[14].CaracRaise = new int[2] { 0, 2 };
            GameData.classes[14].CommandID = 23;
            GameData.classes[14].Rank = 2;

            // #15 Lancer
            GameData.classes[15].Name = Data.Local.Classes.Lancer;
            GameData.classes[15].CaracRaise = new int[2] { 0, 3 };
            GameData.classes[15].CommandID = 24;
            GameData.classes[15].Rank = 2;

            // #16 Arcanist
            GameData.classes[16].Name = Data.Local.Classes.Arcanist;
            GameData.classes[16].CaracRaise = new int[2] { 0, 4 };
            GameData.classes[16].CommandID = 25;
            GameData.classes[16].Rank = 2;

            // #17 Monk
            GameData.classes[17].Name = Data.Local.Classes.Monk;
            GameData.classes[17].CaracRaise = new int[2] { 0, 5 };
            GameData.classes[17].CommandID = 26;
            GameData.classes[17].Rank = 2;

            // #18 Assassin
            GameData.classes[18].Name = Data.Local.Classes.Assassin;
            GameData.classes[18].CaracRaise = new int[2] { 1, 2 };
            GameData.classes[18].CommandID = 27;
            GameData.classes[18].Rank = 2;

            // #19 Freebooter
            GameData.classes[19].Name = Data.Local.Classes.Freebooter;
            GameData.classes[19].CaracRaise = new int[2] { 1, 3 };
            GameData.classes[19].CommandID = 28;
            GameData.classes[19].Rank = 2;

            // #20 Shaman
            GameData.classes[20].Name = Data.Local.Classes.Shaman;
            GameData.classes[20].CaracRaise = new int[2] { 1, 4 };
            GameData.classes[20].CommandID = 29;
            GameData.classes[20].Rank = 2;

            // #21 Summoner
            GameData.classes[21].Name = Data.Local.Classes.Summoner;
            GameData.classes[21].CaracRaise = new int[2] { 1, 5 };
            GameData.classes[21].CommandID = 30;
            GameData.classes[21].Rank = 2;

            // #22 Ninja
            GameData.classes[22].Name = Data.Local.Classes.Ninja;
            GameData.classes[22].CaracRaise = new int[2] { 2, 3 };
            GameData.classes[22].CommandID = 31;
            GameData.classes[22].Rank = 2;

            // #23 Bard
            GameData.classes[23].Name = Data.Local.Classes.Bard;
            GameData.classes[23].CaracRaise = new int[2] { 2, 4 };
            GameData.classes[23].CommandID = 32;
            GameData.classes[23].Rank = 2;

            // #24 Healer
            GameData.classes[24].Name = Data.Local.Classes.Healer;
            GameData.classes[24].CaracRaise = new int[2] { 2, 5 };
            GameData.classes[24].CommandID = 33;
            GameData.classes[24].Rank = 2;

            // #25 Archer-Wizard
            GameData.classes[25].Name = Data.Local.Classes.ArcherWizard;
            GameData.classes[25].CaracRaise = new int[2] { 3, 4 };
            GameData.classes[25].CommandID = 34;
            GameData.classes[25].Rank = 2;

            // #26 Tactician
            GameData.classes[26].Name = Data.Local.Classes.Tactician;
            GameData.classes[26].CaracRaise = new int[2] { 3, 5 };
            GameData.classes[26].CommandID = 35;
            GameData.classes[26].Rank = 2;

            // #27 Sage
            GameData.classes[27].Name = Data.Local.Classes.Sage;
            GameData.classes[27].CaracRaise = new int[2] { 4, 5 };
            GameData.classes[27].CommandID = 36;
            GameData.classes[27].Rank = 2;

            // Initialize characters
            Character Lucien = new Character();
            Lucien.Name = Data.Local.Chara.Lucien;
            Lucien.Position = new Vector2(150, 150);
            Lucien.StanceSpriteName = "Lucien";
            Lucien.faceName = "portraitLucien";
            
           
            Lucien.StatisticsIncreasePercent[0] = Constants.STATMULT[0];
            Lucien.StatisticsIncreasePercent[1] = Constants.STATMULT[1];
            Lucien.StatisticsIncreasePercent[2] = Constants.STATMULT[2];
            Lucien.StatisticsIncreasePercent[3] = Constants.STATMULT[3];
            Lucien.StatisticsIncreasePercent[4] = Constants.STATMULT[4];
            Lucien.StatisticsIncreasePercent[5] = Constants.STATMULT[5];
            Lucien.StatisticsIncreasePercent[6] = Constants.STATMULT[6];
            Lucien.StatisticsIncreasePercent[7] = Constants.STATMULT[7];
            Lucien.StatisticsIncreasePercent[8] = Constants.STATMULT[8];
            Lucien.StatisticsIncreasePercent[9] = Constants.STATMULT[9];
            Lucien.StatisticsIncreasePercent[10] = Constants.STATMULT[10];
            Lucien.StatisticsIncreasePercent[11] = Constants.STATMULT[11];



            for (int i = 1; i < Lucien.maxBaseExp.Length; i++)
            {
                Lucien.maxBaseExp[i] = Math.Min(Constants.MAXIMUMXP, Lucien.maxBaseExp[i - 1] + (int)Math.Ceiling(15 * Math.Pow(2, (double)(i - 1) / (double)5)) + 60 * i*i + 25 * i);
            }
            Lucien.maxBaseExp[98] = Constants.MAXIMUMXP;
            for (int i = 0; i < Lucien.maxStatExp.GetLength(0); i++)
            {
                for (int j = 0; j < Lucien.maxStatExp.GetLength(1); j++)
                {

                    Lucien.maxStatExp[i, j] = j * j * 100;
                }
            }

            Character Rakun = new Character();
            Rakun.Name = "Rakun";
            Rakun.Position = new Vector2(550, 250);
            Rakun.StanceSpriteName = "RakunCôté";
            Rakun.faceName = "portraitRakun";

            Rakun.StatisticsIncreasePercent[0] = 4;
            Rakun.StatisticsIncreasePercent[1] = 5;
            Rakun.StatisticsIncreasePercent[2] = 50;
            Rakun.StatisticsIncreasePercent[3] = 6;
            Rakun.StatisticsIncreasePercent[4] = 25;
            Rakun.StatisticsIncreasePercent[5] = 6;
            Rakun.StatisticsIncreasePercent[6] = 4;
            Rakun.StatisticsIncreasePercent[7] = 5;
            Rakun.StatisticsIncreasePercent[8] = 30;
            Rakun.StatisticsIncreasePercent[9] = 4;
            Rakun.StatisticsIncreasePercent[10] = 5;
            Rakun.StatisticsIncreasePercent[11] = 5;

            for (int i = 1; i < Rakun.maxBaseExp.Length; i++)
            {
                Rakun.maxBaseExp[i] = Lucien.maxBaseExp[i];
            }

            for (int i = 0; i < Rakun.maxStatExp.GetLength(0); i++)
            {
                for (int j = 0; j < Rakun.maxStatExp.GetLength(1); j++)
                {
                    Rakun.maxStatExp[i, j] = j * j * 100;
                }
            }


            Lucien.Level = 1;
            Rakun.Level = 1;

            Lucien.UpdateStats();
            Rakun.UpdateStats();

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
            Lucien.hasCommand[14] = true;

            Rakun.hasCommand[1] = true;
            Rakun.hasCommand[4] = true;
            Rakun.hasCommand[5] = true;
            Rakun.hasCommand[6] = true;
            Rakun.hasCommand[2] = true;
            Rakun.hasCommand[3] = true;
            Rakun.hasCommand[11] = true;
            Rakun.hasCommand[12] = true;
            Rakun.hasCommand[14] = true;
      
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

            Lucien.AttackCue = "LucienAttack";
            Lucien.DeathCue = "LucienDeath";
            Lucien.VeryWoundCue = "LucienVeryWound";
            Lucien.WordCue = "LucienWord";
            Lucien.WoundCue = "LucienWound";

            for (int i = 0; i < Constants.STATNUMBER; i++)
            {
                Lucien.AddExp(0, ExpTypes.Stat, i);
                Rakun.AddExp(0, ExpTypes.Stat, i);
            }
            for (int i = 0; i < Constants.ClassNumber; i++)
            {
                Lucien.AddExp(0, ExpTypes.Class, i);
                Rakun.AddExp(0, ExpTypes.Class, i);
            }
            Lucien.Heal();
            Rakun.Heal();
            GameData.characters[0] = Lucien;
            GameData.characters[1] = Rakun;
        }

        static void Rest_SkillLaunched(object sender, SkillEventArgs sea)
        {
            sea.situation.Fighters[sea.situation.CommandIndex].Stats.ToMax("AP");
            if (sea.situation.Fighters[sea.situation.CommandIndex].Unlimited)
            {
                sea.situation.Fighters[sea.situation.CommandIndex].Unlimited = false;
            }
        }

        /// <summary>
        /// Load the game's Data from a file 
        /// </summary>
        /// <param name="filename">Name of the file to load from</param>
        public static void Load(string filename)
        {
        }

        /// <summary>
        /// Save the game's current Data 
        /// </summary>
        /// <param name="filename">Name of the file to load from</param>
        public static void Save(string filename)
        {
        }
    }
}
