/* Fiat Lux V0.4.44
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace FiatLux.Scenes
{
    public class BattleConfigurationMenuScene : Scene
    {
        public delegate void OnConfirmation(object sender, EventArgs e);
        public event OnConfirmation Confirmed;

        float tint = 0.0F;
        bool isTint = false;
        Sprite shape;
        BattleScene battleScene;
        TextWindow ConfigurationState;
        StringBuilder sb = new StringBuilder();
        int index = 0;
        SkillConfiguration skillConfiguration;
        Skill skill;
        int ConfigureIndex;

        public BattleConfigurationMenuScene(BattleScene parent)
            : base(parent)
        {
            battleScene = parent;
            ConfigureIndex = battleScene.CommandIndex;
            skillConfiguration = battleScene.FightersSkillConfiguration[battleScene.CommandIndex];
            skill = battleScene.FightersAction[battleScene.CommandIndex];
            if (skill.targetMode == TargetMode.Area)
            {
                shape = skill.Shape;
                shape.Parent = battleScene.camera;
                shape.Visible = true;
                LuxGame.IsMouseVisible = false;
                battleScene.camera.AddTarget(shape);
                shape.SpriteColor = Color.Blue;
                shape.HasAfterImage = false;
            }
        }



        public override void Initialize()
        {
            ConfigurationState = new TextWindow(this, Vector2.Zero);
            ConfigurationState.Game.Components.Add(ConfigurationState);
            base.Initialize();
        }

        public override void Destroy()
        {
            if (shape != null)
            {
                LuxGame.IsMouseVisible = true;
                battleScene.camera.RemoveTarget(shape);
                shape.Parent = battleScene;
                shape.Visible = false;
            }
            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            if (index >= skill.parametersNumber + skill.choiceParametersNumber + skill.shapeParametersNumber)
            {
                Confirmed(this, new EventArgs());
            }
            int cost = 0;
            sb.Clear();
            sb.Append(skill.Name);
            sb.AppendLine(" (" + skill.RecoveryTime.ToString() + ")");
            cost += (int)Math.Max(0,Math.Ceiling(skill.RecoveryTime));
            for (int i = 0; i < skill.parametersNumber; i++)
            {
                int paramCost = (int)Math.Max(0, Math.Ceiling((battleScene.Fighters[ConfigureIndex].Distance(battleScene.Fighters[skillConfiguration.parameters[i]].Position) - skill.Range) / battleScene.Fighters[ConfigureIndex].Speed));
                cost += paramCost;
                sb.Append(skill.parametersNames[i]);
                sb.Append(": ");
                if (battleScene.Fighters[skillConfiguration.parameters[i]] != null)
                {
                    sb.Append(battleScene.Fighters[skillConfiguration.parameters[i]].Name);
                }
                sb.AppendLine(" (" + paramCost.ToString() + ")");
            }

            for (int i = 0; i < skill.choiceParametersNumber; i++)
            {
                sb.Append(skill.parametersNames[i + skill.parametersNumber]);
                sb.Append(": ");
                sb.AppendLine(skillConfiguration.choiceParameters[i].ToString());
            }
            for (int i = 0; i < skill.shapeParametersNumber; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append("(" + skill.parametersNames[i + skill.parametersNumber + skill.choiceParametersNumber]);
                    sb.Append(": ");
                    sb.Append(skillConfiguration.shapeParameters[i].ToString());
                }
                else
                {
                    int shapeParamCost = 0;
                    sb.Append("; " + skill.parametersNames[i + skill.parametersNumber + skill.choiceParametersNumber]);
                    sb.Append(": ");
                    sb.Append(skillConfiguration.shapeParameters[i].ToString());
                    sb.Append(")");
                    shapeParamCost += (int)Math.Ceiling(battleScene.Fighters[ConfigureIndex].Distance(new Vector2(skillConfiguration.shapeParameters[i-1],skillConfiguration.shapeParameters[i]))/battleScene.Fighters[ConfigureIndex].Speed);
                    cost += shapeParamCost;
                    sb.AppendLine(" (" + shapeParamCost.ToString() + ")");
                }
                //sb.Append(skill.parametersNames[i + skill.parametersNumber + skill.choiceParametersNumber]);
                //sb.Append(": ");
                //sb.AppendLine(skillConfiguration.shapeParameters[i].ToString());
            }
            sb.AppendLine("Cost: " + cost.ToString() + " AP");

            if (skill.targetMode == TargetMode.Area)
            {
                float FrameDuration = Shared.FrameDuration;

                if (tint <= 0.5 && !isTint)
                {
                    tint += FrameDuration;
                }
                else if (!isTint)
                {
                    isTint = true;
                }
                else if (tint >= 0)
                {
                    tint -= FrameDuration;
                }
                else
                {
                    isTint = false;
                }
                if (cost - skill.RecoveryTime > battleScene.Fighters[ConfigureIndex].Stats["AP"])
                {
                    shape.SpriteColor = new Color(255, 0, 0, tint*2);
                }
                else if (cost > battleScene.Fighters[ConfigureIndex].Stats["AP"])
                {
                    shape.SpriteColor = new Color(255, 255, 0, tint*2);
                }
                else
                {
                    shape.SpriteColor = new Color(0, 0, 255, tint*2);
                }
            }

            ConfigurationState.Text = sb.ToString();
            ConfigurationState.Select(index + 1);

            UpdateConfigure();
            base.Update(gameTime);
        }

        void UpdateConfigure()
        {
            if (skill.targetMode == TargetMode.Area)
            {
                skillConfiguration.shapeParameters[0] = Input.CursorPosition.X - battleScene.camera.Position.X;
                skillConfiguration.shapeParameters[1] = Input.CursorPosition.Y - battleScene.camera.Position.Y;
                
                shape.Position = Input.CursorPosition - battleScene.camera.Position;
                if (Input.CursorValidated)
                {
                    index += 2;
                }
            }
        }

        
    }
}
