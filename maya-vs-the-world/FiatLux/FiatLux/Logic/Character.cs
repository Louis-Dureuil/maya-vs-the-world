/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiatLux.Logic
{
    public class Character : Fighter
    {
        #region Consts

        #endregion
     
        public bool[] hasCommand = new bool[Constants.CommandsCapacity];

        int classID;

        public override float Speed
        {
            get
            {
                return base.Speed + Stats["Speed"] / 100;
            }
        }

        // All level types:
        // 1. Base
        int level = 0;
        // 2. Class
        int[] classLevel = new int[Constants.ClassNumber];
        // 3. Skill
        int[] skillLevel = new int[GameData.commands.Length];
        // 4. Weapon
        // 5. Stat
        int[] statLevel = new int[Constants.STATNUMBER];
       
        // All XP Types:
        // 1. Base
        int baseExp = 0;
        // 2. Class
        int[] classExp = new int[Constants.ClassNumber];
        // 3. Skill
        int[] skillExp = new int[GameData.commands.Length];
        // 4. Weapons (NOT IMPLEMENTED YET)
        int[] weaponExp = new int[1];
        // 5. Stat
        int[] statExp = new int[Constants.STATNUMBER];


        // All level public properties type.
        public int Level
        {
            get { return level; }
            set
            {
                level = Math.Max(0, Math.Min(Constants.MAXIMUMLEVEL, value));
                UpdateStats();
            }
        }

        public int CaracPoints
        {
            get
            {
                int used = 0;
                for (int i = 0; i < Constants.CARACNUMBER; i++)
                {
                    used += this.Caracteristics[i];
                }
                return level * 5 - used;
            }
        }

        public bool CanIncreaseCarac(int index)
        {
            return Caracteristics[index] < 2*level && CaracPoints > 0 && Caracteristics[index] < 99;
        }

        public bool CanIncreaseCarac(string index)
        {
            return Caracteristics[index] < 2 * level && CaracPoints > 0 && Caracteristics[index] < 99;
        }

        public int[] ClassCarac
        {
            get
            {
                int[] CaracBonus = new int[Constants.CARACNUMBER];

                // Calculating CaracBonus.
                for (int i = 0; i < classLevel[classID]; i++)
                {
                    int bonus = i % GameData.classes[classID].CaracRaise.Length;
                    CaracBonus[bonus]++;
                }
                return CaracBonus;
            }
        }

        public int ClassID
        {
            get { return classID; }
        }

        public void ChangeClass(int targetClass)
        {
            if (CanChangeClass(targetClass))
            {
                classID = targetClass;
                ActionCommands[1] = GameData.classes[targetClass].CommandID;
                if (ActionCommands[1] == ActionCommands[2])
                    ActionCommands[2] = 0;
            }
        }

        public bool CanChangeClass(int targetClass)
        {
            return false;
        }

        /// <summary>
        /// Update Stats according to the following formula:
        /// stat = baseStat + equipment + bonus
        /// where baseStat is a value depending on the level, classLevel, statLevel and Caracs.
        /// where equipment is a value depending on the equipped items.
        /// where bonus is a value calculated as a constant or depending of baseStat, according 
        /// to the character trait's and other modifications.
        /// </summary>
        public void UpdateStats()
        {
            
            for (int i = 0; i < Constants.STATNUMBER; i++)
            {
                
                // Calculating baseStat
                int baseStat = (int)Math.Ceiling(StatisticsIncreasePercent[i]*(100+Constants.BASEINFLUENCE+statLevel[i])
                    * level * 0.01F);

                baseStat += (int)Math.Ceiling(baseStat * Constants.CARACINFLUENCE * 
                    (Caracteristics[Constants.STATTOCARAC[i]]+ClassCarac[Constants.STATTOCARAC[i]]) * 0.01F);
                baseStat += Constants.STATBASE[i];
                //TODO: Calculating equipment.
                int equipment = 0;

                //TODO: Calculating bonus.
                int bonus = 0;

                if (Stats.HaveMax(i))
                {
                    float ratio = 1.0F;
                    int oldStat = Stats[i, true];
                    Stats[i, true] = baseStat + equipment + bonus;
                    ratio = (float)Stats[i] / (float)oldStat;
                    if (i != Stats.IndexOf("AP"))
                        Stats[i] = (int)Math.Round(Stats[i, true]*ratio);
                }
                else
                    Stats[i] = baseStat + equipment + bonus;
            }
        }

        public void Heal()
        {
            for (int i = 0; i < Constants.STATNUMBER; i++)
            {
                if (Stats.HaveMax(i))
                    Stats.ToMax(i);
            }
        }

        // Maximum XP required to get to the next level:
        // 1. Base. PUBLIC for now (DIRTY)
        public int[] maxBaseExp = new int[Constants.MAXIMUMLEVEL];
        // 2. Class
        // => See in GameData[id].maxExp
        // 3. Skill
        // => See in GameData[id].maxExp.
        // 4. Weapons
        // 5. Stat. PUBLIC for now (DIRTY)
        public int[,] maxStatExp = new int[Constants.STATNUMBER, Constants.MAXIMUMSTATLEVEL];

        /// <summary>
        /// Set a character's XP.
        /// </summary>
        /// <param name="expType">The type of XP to be modified</param>
        /// <param name="id">When relevant, the id of the modified type's XP (irrelevant for base)</param>
        public void SetExp(int value, ExpTypes expType, int id)
        {
            switch (expType)
            {
                case ExpTypes.Base:
                    baseExp = Math.Max(0, Math.Min(Constants.MAXIMUMXP, value));
                    while (Level < Constants.MAXIMUMLEVEL && baseExp >= maxBaseExp[Level])
                    {
                        Level++;
                    }
                    break;
                case ExpTypes.Class:
                    classExp[id] = Math.Max(0, Math.Min(Constants.MAXIMUMCLASSXP, value));
                    while (classLevel[id] < Constants.MaximumClassLevel * GameData.classes[id].Rank
                        && classExp[id] >= GameData.classes[id].MaximumXP[classLevel[id]])
                    {
                        classLevel[id]++;
                        UpdateStats();
                    }
                    break;
                case ExpTypes.Skill:
                    skillExp[id] = Math.Max(0, Math.Min(Constants.MAXIMUMSKILLXP, value));
                    while (skillLevel[id] < Constants.MAXIMUMSKILLLEVEL && skillExp[id] >= GameData.commands[id].maxExp[skillLevel[id]])
                    {
                        skillLevel[id]++;
                        UpdateStats();
                    }
                    break;
                case ExpTypes.Stat:
                    statExp[id] = Math.Max(0, Math.Min(Constants.MAXIMUMSTATXP, value));
                    while (statLevel[id] < Constants.MAXIMUMSTATLEVEL && statExp[id] >= maxStatExp[id, statLevel[id]])
                    {
                        statLevel[id]++;
                    }
                    break;
                case ExpTypes.Weapon:
                    throw new NotImplementedException("The weapons aren't ready yet!");
            }
        }

        /// <summary>
        /// Get a character's XP.
        /// </summary>
        /// <param name="expType">The XP type.</param>
        /// <param name="id">When relevant, the type's id (irrelevant for base XP).</param>
        /// <param name="sinceLastLevel">if true, returns value since the last level</param>
        /// <returns>The character's XP</returns>
        public int GetExp(ExpTypes expType, int id, bool sinceLastLevel)
        {
            switch (expType)
            {
                case ExpTypes.Base:
                    if (sinceLastLevel && level != 0)
                        return baseExp - maxBaseExp[level-1];
                    else
                        return baseExp;
                case ExpTypes.Class:
                    if (sinceLastLevel && classLevel[id] != 0)
                        return classExp[id] - GameData.classes[id].MaximumXP[classLevel[id] - 1];
                    else
                        return classExp[id];
                case ExpTypes.Skill:
                    if (sinceLastLevel && skillLevel[id] != 0)
                        return skillExp[id] - GameData.commands[id].maxExp[skillLevel[id]-1];
                    else
                        return skillExp[id];
                case ExpTypes.Stat:
                    if (sinceLastLevel && statLevel[id] != 0)
                        return statExp[id] - maxStatExp[id, statLevel[id]-1];
                    else
                        return statExp[id];
                case ExpTypes.Weapon:
                    throw new NotImplementedException("Weapons not ready yet!");
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Set a character's XP to the formula:
        /// XP += value 
        /// </summary>
        /// <param name="value">the value to be added</param>
        /// <param name="expType">the XP type</param>
        /// <param name="id">the type's id</param>
        /// <remarks>Can also be used to perfom substractions</remarks>
        public void AddExp(int value, ExpTypes expType, int id)
        {
            int totalValue = value + GetExp(expType, id, false);
            SetExp(totalValue, expType, id);
        }

        /// <summary>
        /// Set a character's XP to the formula:
        /// XP *= value
        /// </summary>
        /// <param name="value">the value to multiplicate</param>
        /// <param name="expType">the XP type</param>
        /// <param name="id">the type's id</param>
        /// <remarks>Can also be used to perform divisions</remarks>
        public void MultExp(int value, ExpTypes expType, int id)
        {
            int totalValue = GetExp(expType, id, false) * value;
            SetExp(totalValue, expType, id);
        }

        /// <summary>
        /// Get a character's Max XP.
        /// </summary>
        /// <param name="expType">the XP type.</param>
        /// <param name="id">the type's id</param>
        /// <param name="toNextLevel">if true, returns needed XP to the next Level.</param>
        public int GetMaxExp(ExpTypes expType, int id, bool sinceLastLevel)
        {
            switch (expType)
            {
                case ExpTypes.Base:
                    if (level < Constants.MAXIMUMLEVEL)
                    {
                        if (sinceLastLevel && level >= 1)
                        {
                            return maxBaseExp[level] - maxBaseExp[level - 1];
                        }
                        else
                            return maxBaseExp[level];
                    }
                    else
                        return baseExp;
                case ExpTypes.Class:
                    if (classLevel[id] < Constants.MaximumClassLevel * GameData.classes[id].Rank)
                    {
                        if (sinceLastLevel && classLevel[id] >= 1)
                        {
                            return GameData.classes[id].MaximumXP[classLevel[id]] - GameData.classes[id].MaximumXP[classLevel[id] - 1];
                        }
                        else
                            return GameData.classes[id].MaximumXP[classLevel[id]];
                    }
                    else
                        return classExp[id];
                case ExpTypes.Skill:
                    if (skillLevel[id] < Constants.MAXIMUMSKILLLEVEL)
                    {
                        if (sinceLastLevel && skillLevel[id] >= 1)
                            return GameData.commands[id].maxExp[skillLevel[id]] - skillExp[id];
                        else
                            return GameData.commands[id].maxExp[skillLevel[id]];
                    }
                    else
                        return skillExp[id];
                case ExpTypes.Stat:
                    if (statLevel[id] < Constants.MAXIMUMSTATLEVEL)
                    {
                        if (sinceLastLevel && statLevel[id] >= 1)
                            return maxStatExp[id, statLevel[id]] - maxStatExp[id,statLevel[id]-1];
                        else
                            return maxStatExp[id, statLevel[id]];
                    }
                    else
                        return statExp[id];
                case ExpTypes.Weapon:
                    throw new NotImplementedException("Weapons not supported yet!");
                default:
                    return 0;
            }
        }

        public int GetLevel(ExpTypes expType, int id)
        {
            switch (expType)
            {
                case ExpTypes.Base:
                    return Level;
                case ExpTypes.Class:
                    return classLevel[id];
                case ExpTypes.Skill:
                    return skillLevel[id];
                case ExpTypes.Stat:
                    return statLevel[id];
                case ExpTypes.Weapon:
                    throw new NotImplementedException();
                default:
                    return 0;
            }
        }

        public List<int> DisabledCommands = new List<int>();

        public override bool HasCommand(int Index)
        {
            return hasCommand[Index];
        }

        public override bool CanUseCommand(int Index)
        {
            return HasCommand(Index) && !DisabledCommands.Contains(Index);
        }

        public Prop Caracteristics = new Prop(Constants.CARACNAMES, Constants.CARACCONVNAMES);
        public Prop StatisticsIncreasePercent = new Prop(Constants.STATNAMES, Constants.STATCONVNAMES);
        int EquippedWeapon;

        public int PreviewStat(int increaseValue, int stat)
        {
            // Calculating baseStat
            int baseStat = (int)Math.Ceiling(StatisticsIncreasePercent[stat] * (100 + Constants.BASEINFLUENCE + statLevel[stat])
                * level * 0.01F);
            baseStat += (int)Math.Ceiling(baseStat * Constants.CARACINFLUENCE *
                ((Caracteristics[Constants.STATTOCARAC[stat]]+ClassCarac[Constants.STATTOCARAC[stat]])+increaseValue) * 0.01F);
            baseStat += Constants.STATBASE[stat];
            //TODO: Calculating equipment.
            int equipment = 0;

            //TODO: Calculating bonus.
            int bonus = 0;
            return baseStat + equipment + bonus;
        }

        public override Fighter GetCopy(string order)
        {
            Character copy = (Character)base.GetCopy(order);
            copy.Caracteristics = new Prop(Constants.CARACNAMES, Constants.CARACCONVNAMES);
            return copy;
        }
    }

    #region AuxTypes
    public enum ExpTypes
    {
        /// <summary>
        /// Base XP used to increase Level (up to 99).
        /// Specific to a character.
        /// Raised by: any Action; getting Initiative
        /// Raise formula: base * EnnemyMult
        /// where base=1 if getting Initiative
        /// where base=8 if performing an action.
        /// where EnnemyMult is specific to the ennemies in battle
        /// </summary>
        Base,
        /// <summary>
        /// Class XP used to increase class Level (up to 50) and acquire Commands.
        /// Specific to a character and to its current class.
        /// Raised by: Class Action
        /// Raise formula: 2*(Rank+1)*EnnemyMult
        /// where Rank is the Action rank
        /// where EnnemyMult is specific to the ennemies in battle
        /// </summary>
        Class,
        /// <summary>
        /// Weapon XP used to increase Weapon Level (up to 16).
        /// Specific to the character and to its current weapon type.
        /// Raised by: Weapon Action (eg. Attacks)
        /// Raise formula: (Rank+1)*EnnemyMult
        /// where Rank is the current weapon rank
        /// where EnnemyMult is specific to the ennemies in battle
        /// </summary>
        Weapon,
        /// <summary>
        /// Skill XP used to increase Skill Level (up to 8).
        /// Specific to the character and to the used Skill.
        /// Raised by: Use of the Skill
        /// Raise formula: 4*(SkillLvl+1)*EnnemyMult / SkillRank
        /// where SkillLvl is the used Lvl of the skill
        /// where EnnemyMult is specific to the ennemies in battle
        /// where SkillRank is the skill rank
        /// </summary>
        Skill,
        /// <summary>
        /// Stat XP used to increase specific stats of the character.
        /// Specific to the character and to the relevant stat.
        /// Raised by: specific condition.
        /// Raise formula: specific.
        /// </summary>
        Stat
    }
    #endregion

    
}
