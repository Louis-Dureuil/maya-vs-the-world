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
using Microsoft.Xna.Framework.Graphics;

namespace FiatLux.Scenes
{
    public class BattleDamageScene : Scene
    {
        const float DAMAGELIFETIME = 2.0F;
        struct DamageStruct
        {
            internal float lifeTime; // The duration during which the damage must be displayed.
         }
        public delegate void expiredLifeTime(object sender, EventArgs e);
        public event expiredLifeTime Expired;
        Sprite Attacker;
        Sprite Target;
        MessageWindow attackerInfo;
        MessageWindow targetInfo;
        MessageWindow targetDamage;
        Color color;
        string attackerOverHeadMessage;
        string targetOverHeadMessage;
        int damage;

        DamageStruct damageStruct;
        public BattleDamageScene(Sprite Attacker, Sprite Target, int damage, Color color, string attackerOverHeadMessage, string targetOverHeadMessage)
            : base(Attacker)
        {
            this.Attacker = Attacker;
            this.Target = Target;
            this.color = color;
            this.attackerOverHeadMessage = attackerOverHeadMessage;
            this.targetOverHeadMessage = targetOverHeadMessage;
            this.damage = damage;
        }

        protected override void LoadContent()
        {
            spriteFont = Content.Load<SpriteFont>("FontTest");
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
            attackerInfo = new MessageWindow(Attacker, new Vector2(-spriteFont.MeasureString(attackerOverHeadMessage).X / 2, -Attacker.Height), attackerOverHeadMessage, MessageConfirmType.None);
            Game.Components.Add(attackerInfo);
            attackerInfo.WithFrame = false;
            attackerInfo.TextColor = color;

            targetInfo = new MessageWindow(Target, new Vector2(-spriteFont.MeasureString(targetOverHeadMessage).X / 2, -Target.Height), targetOverHeadMessage, MessageConfirmType.None);
            Game.Components.Add(targetInfo);
            targetInfo.WithFrame = false;
            targetInfo.TextColor = color;

            targetDamage = new MessageWindow(Target, new Vector2(-spriteFont.MeasureString(damage.ToString()).X, Target.Height / 2), damage.ToString(), MessageConfirmType.None);
            Game.Components.Add(targetDamage);
            targetDamage.WithFrame = false;
            targetDamage.TextColor = color;
            damageStruct = new DamageStruct();
            damageStruct.lifeTime = DAMAGELIFETIME; 
        }

        public override void Update(GameTime gameTime)
        {
            //Frame duration, in s (precision 10^(-3) s.)
            float FrameDuration = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;

            base.Update(gameTime);
            if (!isPaused)
            damageStruct.lifeTime -= FrameDuration;
            if (damageStruct.lifeTime <= 0)
                Expired(this, new EventArgs());
        }

        public override void Destroy()
        {
            targetInfo.Destroy();
            attackerInfo.Destroy();
            targetDamage.Destroy();
            base.Destroy();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
