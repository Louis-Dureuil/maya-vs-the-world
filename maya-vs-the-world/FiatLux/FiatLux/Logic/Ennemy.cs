/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
namespace FiatLux.Logic
{
    public class Ennemy : Fighter
    {
        Bar LifeBar;

        public float EnnemyMult = 1.0F;

        public List<int> commands = new List<int>();

        public override bool HasCommand(int Index)
        {
            return commands.Contains(Index);
        }

        public override bool CanUseCommand(int Index)
        {
            return HasCommand(Index) && this.Stats["MP"] >= ((Skill)GameData.commands[Index]).Cost;
        }

        public override void Update(float gameTime)
        {
            if (LifeBar == null)
            {
                LifeBar = new Bar(this.Sprite, new Vector2(100, 5));
                LifeBar.Game.Components.Add(LifeBar);
                LifeBar.ValueColor = Color.Green;
            }
            else
            {
                LifeBar.Position = new Vector2(-50, Sprite.Height / 2+5);
                LifeBar.MaxValue = this.Stats["End", true];
                LifeBar.Value = this.Stats["End"];
            }
            base.Update(gameTime);
        }

        internal int MakeActionDecision(Fighter[] Fighters)
        {
            Random rand = LuxEngine.Shared.Rand;
            int[] weight = new int[2] { 50, 30 };
            if (this.Stats["AP"] < 30)
                weight[1] += 30;
            int s = weight[0] + weight[1];
            int r =rand.Next(0,s);
            s = weight[0];
            if (r < s && CanUseCommand(2))
            {
                return 2;
            }
            s += weight[1];
            if (r < s && CanUseCommand(1))
                return 3;
            return 3;
        }

        internal int MakeReactionDecision(Fighter fighter, Skill skill, SkillConfiguration skillConfiguration)
        {
            Random rand = LuxEngine.Shared.Rand;
            int p = rand.Next(1, 101);
            if (p <= 40)
                return 7;
            else if (p <= 65)
                return 8;
            else if (p <= 90)
                return 9;
            else
                return 10;
        }

        internal int ChooseSingleTarget(Scenes.BattleScene situation)
        {
            float WeightSum = 0;
            Random rand = LuxEngine.Shared.Rand;
            List<Scenes.FighterDist> Targets = situation.FindTargets(this, false);
            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i].Fighter.isDead)
                {
                    Targets.RemoveAt(i);
                    i--;
                }
                else if (Targets[i].Distance != 0)
                    WeightSum += 1 / Targets[i].Distance;
                else
                    return i;
            }


            for (int i = 0; i < Targets.Count; i++)
            {
                if ((float)rand.Next(0, 100) < 100.0F / (WeightSum * Targets[i].Distance))
                {
                    return Targets[i].FighterID;
                }
                else
                    WeightSum -= 1 / Targets[i].Distance;
            }

            throw new Exception("No possible target");

        }

        
    }
}
