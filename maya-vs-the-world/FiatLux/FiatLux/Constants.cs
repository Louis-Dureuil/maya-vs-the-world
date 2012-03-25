/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FiatLux
{
    /// <summary>
    /// Records all the game shared constants, that are defined before compilation and won't change at runtime. Contains only const.
    /// </summary>
    static public class Constants
    {
       
        // Maximum number of allies allowed on battles.
        public const int BattleCharactersCapacity = 4;

        // Maximum number of ennemies allowed on battles.
        public const int BattleEnnemiesCapacity = 4;

        // Maximum number of Characters allowed on battles.
        public const int PartyCapacity = 9;

        // Maximum amount of money that can be gained.
        public const int MaximumGold = 999999999;

        // Maximum number of Characters.
        public const int CharactersCapacity = 16;

        // Maximum number of ennemies.
        public const int EnnemiesCapacity = 300;

        // Maximum number of Commands.
        public const int CommandsCapacity = 700;

        // Maximum number of Items.
        public const int ItemsCapacity = 500;

        // Number of statistics.
        public const int STATNUMBER = 12;

        // Number of Caracteristics.
        public const int CARACNUMBER = 6;

        // Highest Level.
        public const int MAXIMUMLEVEL = 99;

        // Highest Skill Level.
        public const int MAXIMUMSKILLLEVEL = 8;

        // Highest Stat Level.
        public const int MAXIMUMSTATLEVEL = 99;

        // Maximum amount of base Exp.
        public const int MAXIMUMXP = 99999999;

        // Maximum amount of skill Exp.
        public const int MAXIMUMSKILLXP = 9999;

        // Maximum amount of stat Exp.
        public const int MAXIMUMSTATXP = 999999;

        // Statistics Name
        public static readonly string[] STATNAMES = new string[STATNUMBER] {"PhyAtt", "STR2", "End", "PhyDef", "AP", "Speed", "Pre", "Per", "MP", "MagAtt", "MagDef", "Con"}; 
        // Statistics Convenient Name
        public static readonly string[] STATCONVNAMES = new string[STATNUMBER] {Data.Local.StatNames.PhyAtt,
            Data.Local.StatNames.STR2,
            Data.Local.StatNames.End,
            Data.Local.StatNames.PhyDef,
            Data.Local.StatNames.AP,
            Data.Local.StatNames.Spe,
            Data.Local.StatNames.Pre,
            Data.Local.StatNames.Per,
            Data.Local.StatNames.MP,
            Data.Local.StatNames.MagAtt,
            Data.Local.StatNames.MagDef,
            Data.Local.StatNames.Con}
            ;

        // Statistics Long Name
        public static readonly string[] STATLONGNAMES = new string[STATNUMBER] 
        {
            Data.Local.StatLongNames.PhyAtt,
            Data.Local.StatLongNames.STR2,
            Data.Local.StatLongNames.End,
            Data.Local.StatLongNames.PhyDef,
            Data.Local.StatLongNames.AP,
            Data.Local.StatLongNames.Spe,
            Data.Local.StatLongNames.Pre,
            Data.Local.StatLongNames.Per,
            Data.Local.StatLongNames.MP,
            Data.Local.StatLongNames.MagAtt,
            Data.Local.StatLongNames.MagDef,
            Data.Local.StatLongNames.Con
        };

        public const int CARACTERWIDTH = 9;


        // Statistics Base value
        public static readonly int[] STATBASE = new int[STATNUMBER] { 25, 25, 250, 25, 50, 25, 25, 25, 50, 25, 25, 25 };

        // Statistics Base Raise value
        public static readonly int[] STATMULT = new int[STATNUMBER] { 5, 5, 50, 5, 25, 5, 5, 5, 25, 5, 5, 5 };

        public const int BASEINFLUENCE = 10;

        public static readonly int[] STATTOCARAC = new int[STATNUMBER] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 };

        public const int CARACINFLUENCE = 5;

        // Caracteristics Name
        public static readonly string[] CARACNAMES = new string[CARACNUMBER] { "STR", "VIT", "AGI", "DEX", "INT", "SPI" };

        public const int MAXIMUMCLASSXP = 99999;

        public static readonly string[] CARACCONVNAMES = new string[CARACNUMBER]
        {
            Data.Local.CaracNames.STR,
            Data.Local.CaracNames.VIT,
            Data.Local.CaracNames.AGI,
            Data.Local.CaracNames.DEX,
            Data.Local.CaracNames.INT,
            Data.Local.CaracNames.SPI
        };

    

        // Have Statistics Maximum?
        public static readonly Dictionary<string, bool> STATSHAVEMAX = new Dictionary<string, bool>() { { "PhyAtt", false }, { "STR2", false }, { "End", true }, { "PhyDef", false }, { "AP", true }, { "Speed", false }, { "Pre", false }, { "Per", false }, { "MP", true }, { "MagAttack", false }, { "MagDefense", false }, { "Con", false } };

        public const int ClassNumber = 28;

        public const float COMMANDDURATION = 10.0F;

        public const string DEFAULTLOCALCODE = "EN-en";

        public const int GAMEDATAVERSION = 1;

        #region Strategy
        #region Character Strategy
        public static readonly Vector2[] ArrowStrategy = new Vector2[BattleCharactersCapacity] {
        new Vector2(0, 0),
        new Vector2(-100, 0),
        new Vector2(-100, -100),
        new Vector2(-100, 100)
        };
        #endregion
        #region Ennemy Strategy
        public static readonly Vector2[] LineStrategy = new Vector2[BattleEnnemiesCapacity] {
        new Vector2(0, 50),
        new Vector2(0, -50),
        new Vector2(0, 100),
        new Vector2(0, -100)
        };
        public const int MaximumClassLevel = 30;
        #endregion
        #endregion
    }
}
