/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;

namespace FiatLux
{

    public class SkillEventArgs : EventArgs
    {
        public Scenes.BattleScene situation;
        public SkillConfiguration sconfig;
        public SkillEventArgs(Scenes.BattleScene situation, SkillConfiguration sconfig)
            : base()
        {
            this.situation = situation;
            this.sconfig = sconfig;
        }
    }
    /// <summary>
    /// A skill is a command that can be used during battles. Inherits from Command.
    /// </summary>
    public class Skill : Command
    {
        public delegate void OnSkillLaunching(object sender, SkillEventArgs sea);
        public event OnSkillLaunching SkillLaunching;
        public event OnSkillLaunching SkillLaunched;

        /// <summary>
        /// Describe the type of the skill.
        /// </summary>
        public enum SkillType
        {
            /// <summary>
            /// Physical Skills require physical contact with the target.
            /// A fighter needs to recover after using a physical skill.
            /// However, no cast is required to use a physical skill.
            /// </summary>
            Physical,
            /// <summary>
            /// Magical skills require the target to be in range.
            /// A fighter don't need to recover after using a magical skill.
            /// However, a cast is required prior the use of a magical skill.
            /// </summary>
            Magical,
            /// <summary>
            /// Special skills exhibit a more complex behaviour than magic's and physical's ones.
            /// A fighter may or may not have to recover after using a special skill.
            /// A cast may or may not be required prior the use of a special skill.
            /// Special skills may have no effective target.
            /// </summary>
            Special
        }
        const float Influence = 0.01F;
        public SkillType Type = new SkillType();
        public Logic.Prop AttackStats = new Logic.Prop(Constants.STATNAMES, Constants.STATCONVNAMES); // The influence, in %, of the stat on the damage
        public Logic.Prop DefenseStats = new Logic.Prop(Constants.STATNAMES, Constants.STATCONVNAMES);
        // In floating AP.
        public float CastTime = 1;
        public float RecoveryTime = 0.1F;
        public TargetMode targetMode = TargetMode.Single;
        public float Range = 45.0F;
        public float Strike = 60.0F;
        public float StrikeSpeed = 10.0F;
        public int BaseDamage = 20;
        public int BaseDefense = 15;
        public float Stun = 30.0F;
        public int Cost = 0;

        public int parametersNumber = 1; // Number of standard parameters.
        public int choiceParametersNumber = 0; // Number of choice parameters.
        public int shapeParametersNumber = 0; // Number of shape parameters.
        

        /// <summary>
        /// Name of all the parameters. Length must be equal to the sum 
        /// of parametersNumber's, choiceParametersNumber's and shapeParameterNumber's Lengthes.
        /// It must first contains the names of the standard parameters, then those of the choice
        /// parameters, and finally those of the shape parameters.
        /// </summary>
        public string[] parametersNames; 

        public int ParryRank = 0;
        public int DodgeRank = 0;

        public string shapeName;

        public Sprite Shape;
        public int CalculateDamage(Logic.Fighter attacker, Logic.Fighter defender)
        {
            int damage = BaseDamage;
            for (int i = 0; i < Constants.STATNUMBER; i++)
            {
                damage += BaseDamage * (int)(Influence * (float)AttackStats[i] * (float)attacker.Stats[i]);
                damage -= BaseDefense * (int)(Influence * (float)DefenseStats[i] * (float)defender.Stats[i]);
            }
            return Math.Max(0,damage);
        }

        public void Launch(Scenes.BattleScene situation, SkillConfiguration sconfig)
        {
            if (SkillLaunching != null)
                SkillLaunching(this, new SkillEventArgs(situation, sconfig));
            if (SkillLaunched != null)
                SkillLaunched(this, new SkillEventArgs(situation, sconfig));
        }

        internal void InitializeConfiguration(ref SkillConfiguration skillConfiguration)
        {
            if (skillConfiguration.parameters == null || skillConfiguration.parameters.Length != this.parametersNumber)
                skillConfiguration.parameters = new int[this.parametersNumber];

            if (skillConfiguration.choiceParameters == null || skillConfiguration.choiceParameters.Length != this.choiceParametersNumber)
                skillConfiguration.choiceParameters = new string[this.choiceParametersNumber];

            if (skillConfiguration.shapeParameters == null || skillConfiguration.shapeParameters.Length != this.shapeParametersNumber)
                skillConfiguration.shapeParameters = new float[this.shapeParametersNumber];
        }
    }

    /// <summary>
    /// A struct to record configuration information about a skill.
    /// </summary>
    public struct SkillConfiguration
    {
        /// <summary>
        /// The values of the "standard" parameters.
        /// A standard parameter is used to store a number, and is typically chosen
        /// with a menu or other windows.
        /// </summary>
        public int[] parameters;
        /// <summary>
        /// The values of the choice parameters.
        /// Usually chosen from a menu, it records the name of the shapes to use.
        /// </summary>
        public string[] choiceParameters;
        /// <summary>
        /// The values of the shape parameters.
        /// It records position and size informations about the shape that will
        /// be used to calculate the included targets.
        /// </summary>
        public float[] shapeParameters;

        //public int Target; // The ID of the Fighter concerned by the skill.
    }
    /// <summary>
    /// Indicates the way the targets are calculated.
    /// </summary>
    public enum TargetMode
    {
        /// <summary>
        /// Only affects the caster.
        /// </summary>
        Self, 
        /// <summary>
        /// Affects one chosen Target.
        /// </summary>
        Single,
        /// <summary>
        /// Affects a defined area.
        /// </summary>
        Area, 
        /// <summary>
        /// Affects all ennemies (here ennemies means "not the type of the caster")
        /// </summary>
        Ennemies,
        /// <summary>
        /// Affects all allies (ie "the type of the caster")
        /// </summary>
        Allies,
        /// <summary>
        /// Affects all Fighters on the Battleground.
        /// </summary>
        All 
    }
}
//Typical "area" skill type :
// Choose "area" location
// Choose "area" shape
// Game detects the included targets.
// The included targets are attacked.

//"Move" "area" skill :
// Choose "area" location
// Game detects whether it can be reached (pathfinding, Not Implemented yet)
// The caster moves to the selected area.
