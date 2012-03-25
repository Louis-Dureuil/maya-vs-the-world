using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using FiatLux.Logic;
using Microsoft.Xna.Framework;

namespace FiatLux.Scenes
{
    /// <summary>
    /// Is shown during battle when a character is earning XP.
    /// Consists of the character's face, then a text indicating what how many and what 
    /// type of XP was earned, with a gauge that indicates the rate current XP/XP to next leveL.
    /// Message : +xx type XP
    /// XP types:
    /// 1. YELLOW: Base (any action/when getting initative)
    /// 2. GREEN: Class (any class command/command of another class raise another class's XP, but with a penalty)
    /// 3. RED: Weapon (weapon command)
    /// 4. BLUE: Skill (skill command)
    /// 5. PURPLE: Stat (specific)
    /// Maximum amount of gauges after one action:
    /// Base && Class && (Weapon || Skill) && Stat => 4
    /// Maximum amount of gauges after one reaction:
    /// Stat => 1
    /// Maximum of on-screen gauges => 8
    /// </summary>
    public class BattleExpGauge : Scene
    {
        Dictionary<ExpTypes, Color> ExpTypeColor = new Dictionary<ExpTypes, Color>() {
        {ExpTypes.Base, Color.Yellow},
        {ExpTypes.Class, Color.Green},
        {ExpTypes.Skill, Color.Blue},
        {ExpTypes.Stat, Color.Purple}
        };

        const float ACCELERATIONFACTOR = 2.0F;

        float acceleration = 1.0F;

        const float NOINCREASELIFETIME = 0.5F;
        const float INCREASELIFETIME = 0.25F;
        const float LVLIFETIME = 2.0F;
        const float STAYLIFETIME = 0.1F;
        const float BASELIFETIME = NOINCREASELIFETIME + INCREASELIFETIME + LVLIFETIME;
        Sprite Face;
        Bar Gauge;
        MessageWindow messageWindow;
        ExpTypes expType;
        int IncreasedValue;
        int currentValue;
        int oldValue;
        int oldLevel;
        int currentExp;
        Character chara;
        float lifeTime;
        int id = 0;

        public bool HasExpired
        {
            get { return lifeTime <= 0; }
        }

        public BattleExpGauge(Scene parent, Character chara, int IncreasedValue, ExpTypes expType, int id)
            : base(parent)
        {
            Face = new Sprite(this, new List<string>() { chara.faceName });
            Game.Components.Add(Face);
            Face.SetAnimation(chara.faceName);
            Gauge = new Bar(this, new Vector2(150, 2));
            Game.Components.Add(Gauge);
            Gauge.Position = new Vector2(16, 8);
            this.expType = expType;
            messageWindow = new MessageWindow(this, new Vector2(8, -16), GetXPText(chara, IncreasedValue, expType, id), MessageConfirmType.None);
            Game.Components.Add(messageWindow);
            messageWindow.WithFrame = false;
            this.IncreasedValue = IncreasedValue;
            this.chara = chara;
            Gauge.ValueColor = ExpTypeColor[expType];
            Gauge.BackgroundColor = Color.Black;
            currentExp = chara.GetExp(expType, id, true);
            Gauge.MaxValue = chara.GetMaxExp(expType, id, true);
            Gauge.Value = currentExp;
            this.lifeTime = BASELIFETIME;
            this.id = id;
            currentValue = IncreasedValue;
        }

        public override void Update(GameTime gameTime)
        {
            oldValue = currentValue;
            float FrameDuration = Shared.FrameDuration * acceleration;
            lifeTime -= FrameDuration;
            if (lifeTime > INCREASELIFETIME + LVLIFETIME)
            {
                oldLevel = chara.GetLevel(expType, id);
            }
            if (lifeTime <= INCREASELIFETIME + LVLIFETIME && lifeTime >= LVLIFETIME)
            {
                currentValue = (int)Math.Floor(IncreasedValue * (lifeTime - LVLIFETIME) / INCREASELIFETIME);
                if (currentValue < oldValue)
                    chara.AddExp(oldValue - currentValue, expType, id);
                currentExp = chara.GetExp(expType, id, true);
                Gauge.MaxValue = chara.GetMaxExp(expType, id, true);
                Gauge.Value = currentExp;
                messageWindow.Text = GetXPText(chara, currentValue, expType, id);
            }
            if (lifeTime < LVLIFETIME)
            {
                if (currentValue != 0)
                {
                    chara.AddExp(currentValue, expType, id);
                    currentValue = 0;
                    currentExp = chara.GetExp(expType, id, true);
                    Gauge.MaxValue = chara.GetMaxExp(expType, id, true);
                    Gauge.Value = currentExp;
                    messageWindow.Text = GetXPText(chara, currentValue, expType, id);
                }
                if (chara.GetLevel(expType, id) - oldLevel > 0)
                    messageWindow.Text = GetLvlText(chara, chara.GetLevel(expType, id) - oldLevel, expType, id);
                else if (lifeTime < LVLIFETIME - STAYLIFETIME)
                    lifeTime = 0;
            }
            if (lifeTime <= 0)
            {
                this.Enabled = false;
                this.Visible = false;
            }
            base.Update(gameTime);
        }

        string GetLvlText(Character chara, int IncreasedValue, ExpTypes expType, int id)
        {
            return GetTypeText(chara, expType, id) + " LV" + " +" + IncreasedValue.ToString() + " " + "(" + chara.GetLevel(expType, id).ToString() + ")!";
        }

        string GetTypeText(Character chara, ExpTypes expType, int id)
        {
            switch (expType)
            {
                case ExpTypes.Base:
                    return "Base";
                case ExpTypes.Class:
                    return GameData.classes[id].Name;
                case ExpTypes.Skill:
                    return GameData.commands[id].Name;
                case ExpTypes.Stat:
                    return Constants.STATNAMES[id];
                case ExpTypes.Weapon:
                    return "Weapon (NotImplemented)";
                default:
                    return "";
            }
        }

        string GetXPText(Character chara, int IncreasedValue, ExpTypes expType, int id)
        {
            string expTypeText = GetTypeText(chara, expType, id);
            
            return "+" + IncreasedValue + " " + expTypeText + " XP";
        }


        internal bool HasValue(Character chara, ExpTypes expType, int id)
        {
            return this.chara == chara && this.expType == expType && this.id == id;
        }

        internal void Increase(int IncreasedValue)
        {
            if (lifeTime > INCREASELIFETIME + LVLIFETIME)
            {
                this.IncreasedValue += IncreasedValue;
                messageWindow.Text = GetXPText(chara, this.IncreasedValue, expType, id);
            }
            if (lifeTime <= INCREASELIFETIME + LVLIFETIME && lifeTime >= LVLIFETIME)
            {
                this.IncreasedValue += currentValue + IncreasedValue;
                lifeTime = INCREASELIFETIME + LVLIFETIME;
            }
            if (lifeTime < LVLIFETIME)
            {
                this.IncreasedValue = IncreasedValue;
                lifeTime = INCREASELIFETIME + LVLIFETIME;
            }
        }

        public override void Destroy()
        {
            if (currentValue != 0)
                chara.AddExp(currentValue, expType, id);
            base.Destroy();
        }

        internal void Accelerate()
        {
            acceleration = ACCELERATIONFACTOR;
        }
    }

}
