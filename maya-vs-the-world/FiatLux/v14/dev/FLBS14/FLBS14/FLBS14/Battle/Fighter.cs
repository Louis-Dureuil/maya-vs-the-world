using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLBS14.Battle.Interface;
using Microsoft.Xna.Framework;

namespace FLBS14.Battle
{
    class Fighter : IActive, IMovable, ITargetable, ICommander
    {
        


        #region public

        #region Properties
        public FighterState State
        {
            get { return state; }
        }

        public Color StateColor
        {
            get { return Fighter.stateColors[state]; }
        }
        #endregion

        #region ctr
        public Fighter(BattleSituation situation, Party party)
        {
            this.situation = situation;
            this.party = party;
        }
        #endregion

        #endregion

        #region Interfaces

        #region IActive
        public bool IsActive
        {
            get { return this.ap >= situation.Ap && state != FighterState.Resting && End.Val != 0; }
        }

        public void OnActive()
        {
            float apFlow = situation.ApFlow;
            switch (state)
            {
                case FighterState.Active:
                    situation.Speed = GameConstants.Speed["Interactive"];
                    // AP Flow
                    // Choose Command
                    if (!this.HasSkill)
                    {
                        this.OnChoosing();
                    }
                    else
                    {
                        // Cannot prepare skill, move.
                        if (CanPrepareSkill)
                        {
                            state = FighterState.PreparingAction;
                            return;
                        }
                        this.MoveTo(Impact);
                    }
                    break;
                case FighterState.Casting:
                    // AP Flow
                    // Do Cast
                    OnCast(); 
                    break;
                case FighterState.Disabled:
                    // NO AP FLOW
                    // Be Disabled
                    return;
                case FighterState.HeavyRecovery:
                    // Ap flow
                    // Recover heavily
                    OnRecovery();
                    break;
                case FighterState.Inactive:
                    // NO AP FLOW
                    // Wait for your turn
                    // If you're here, that probably means you're active.
                    state = FighterState.Active;
                    return;
                case FighterState.PerformingAction:
                    // Ap Flow
                    // Performs an action
                    OnPerformed();
                    state = FighterState.HeavyRecovery;
                    break;
                case FighterState.PreparingAction:
                    // Ap Flow
                    // Prepare an action
                    OnPreparing();
                    break;
                case FighterState.Recovery:
                    // Ap Flow
                    // Recover from an action
                    OnRecovery();
                    break;
                case FighterState.Resting:
                    // No Ap Flow
                    // Rest till the end of turn.
                    return;
                case FighterState.UsingSpell:
                    // Ap Flow
                    // Use a spell
                    OnSpell();
                    break;
            }
            this.ap = situation.Ap;
        }

        public void OnTurnStart(float avgInitiative)
        {
            if (state != FighterState.Disabled)
            {
                float rel = (Initiative - avgInitiative) / Math.Max(Initiative, avgInitiative);
                float avgScore = GameConstants.BasAp + GameConstants.ModAp * rel;
                float avgBonus = 0;
                if (rel > 0)
                {
                    avgBonus = ((GameConstants.MaxAp) / 20) * (float)Math.Sqrt(rel);
                }
                this.ap += avgScore + avgBonus + (avgScore * GameCommon.Rand.Next(-5, 6) + avgBonus * GameCommon.Rand.Next(-10, 11)) /  100.0F;
                this.ap = Math.Max(GameConstants.MinAp, Math.Min(this.ap, GameConstants.MaxAp));
                this.state = FighterState.Inactive;
            }
            else
            {
                this.ap = 0;
            }
        }

        public void OnTurnEnd()
        {
            Cancel();
            if (state == FighterState.Disabled)
            {
                state = FighterState.Resting;
            }
            if (state != FighterState.Resting)
            {
                state = FighterState.Disabled;
            }
        }

        public bool IsDisabled
        {
            get { return state == FighterState.Disabled; }
        }

        public int Initiative
        {
            get { return (int)Init; }
        }

        public float Ap
        {
            get { return this.ap; }
        }

        #endregion

        #region IMovable
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        public Vector3 Acceleration
        {
            get
            {
                return acceleration;
            }
            set
            {
                acceleration = value;
            }
        }

        public bool IsMoving
        {
            get { return velocity != Vector3.Zero; }
        }

        public bool CanMove
        {
            get { return canMove; }
        }

        public bool IsAuto
        {
            get { return false; }
        }

        public void OnMove()
        {
            // DO physics stuff.
            Position += Velocity * situation.ApFlow;
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public void MoveTo(IPositionable destination)
        {
            // Set Destination
            // Finds path
            // Move along the path.
        }

        #endregion

        #region ITargetable
        public bool IsTarget
        {
            get { return targetingSkills.Count != 0; }
        }

        public bool IsTargetOf(ICommander actor)
        {
            return targetingSkills.ContainsKey(actor);
        }

        public bool IsTargetOf(Skill skill)
        {
            return targetingSkills.ContainsValue(skill);
        }

        public Skill GetSkillFromActor(ICommander actor)
        {
            return targetingSkills[actor];
        }

        public ICommander GetActorFromSkill(Skill skill)
        {
            return targetingActors[skill];
        }

        public void OnTargeted(Skill targetingSkill, ICommander actor)
        {
            targetingActors.Add(targetingSkill, actor);
            targetingSkills.Add(actor, targetingSkill);
        }

        public void Reacting(Skill targetingSkill, ICommander actor)
        {
            throw new NotImplementedException();
        }

        public void Reacting(ICommander actor)
        {
            throw new NotImplementedException();
        }

        public void Reacting(Skill targetingSkill)
        {
            throw new NotImplementedException();
        }

        public void OnImpact(Skill targetingSkill, ICommander actor)
        {
            this.End.Val -= Math.Max(1, ((int)(targetingSkill.Strength * actor.AttPhy.Val) - this.DefPhy.Val));
            targetingActors.Remove(targetingSkill);
            targetingSkills.Remove(actor);
        }

        public void OnImpact(ICommander actor)
        {
            OnImpact(targetingSkills[actor], actor);
        }

        public void OnImpact(Skill targetingSkill)
        {
            OnImpact(targetingSkill, targetingActors[targetingSkill]);
        }

        public void OnCancel(Skill targetingSkill, ICommander actor)
        {
            targetingSkills.Remove(actor);
            targetingActors.Remove(targetingSkill);
        }

        public void OnCancel(Skill targetingSkill)
        {
            OnCancel(targetingSkill, targetingActors[targetingSkill]);
        }

        public void OnCancel(ICommander actor)
        {
            OnCancel(targetingSkills[actor], actor);
        }
        #endregion

        #region ICommander
        public bool HasSkill
        {
            get { return skill != null; }
        }

        public Skill Skill
        {
            get { return skill; }
        }

        public float PreparationTime
        {
            get { return preparationTime; }
        }

        public float CastTime
        {
            get { return castTime; }
        }

        public float HRecoveryTime
        {
            get { return hRecoveryTime; }
        }

        public float RecoveryTime
        {
            get { return recoveryTime; }
        }

        public float SpellTime
        {
            get { return spellTime; }
        }

        public void OnChoosing()
        {
            //throw new NotImplementedException();
            int chooseCommand = GameCommon.Rand.Next(0, 100);
            if (chooseCommand == 0)
            {
                if (ap < 100)
                {
                    decision["Rest"] = 30 + (100 - (int)ap)*2;
                }
                else
                {
                    decision["Rest"] = ((500 - (int)ap) / 30); 
                }
                if (ap > 500)
                {
                    decision["Rest"] = 1;
                }
                if (Mp.Val < (Mp.MaxVal * 20) / 100)
                {
                    decision["Magic"] = 5;
                }
                else
                {
                    decision["Magic"] = 20 + ((Mp.Val * 100) / Mp.MaxVal) / 5;
                }
                if (Mp.Val < 5)
                {
                    decision["Magic"] = 0;
                }
                decision.SelectNextCommand();
                string command = decision.Command;
                switch (command)
                {
                    case "Rest":
                        state = FighterState.Resting;
                        break;
                    case "VeryLight":
                        state = FighterState.PreparingAction;
                        preparationTime = 4;
                        hRecoveryTime = 9;
                        recoveryTime = 17;
                        skill = new Skill("VeryLight", 0.5F, false);
                        break;
                    case "Light":
                        state = FighterState.PreparingAction;
                        preparationTime = 5;
                        hRecoveryTime = 12;
                        recoveryTime = 20;
                        skill = new Skill("Light", 0.8F, false);
                        break;
                    case "Normal":
                        state = FighterState.PreparingAction;
                        preparationTime = 6;
                        hRecoveryTime = 14;
                        recoveryTime = 30;
                        skill = new Skill("Normal", 1.2F, false);
                        break;
                    case "Strong":
                        state = FighterState.PreparingAction;
                        preparationTime = 10;
                        hRecoveryTime = 20;
                        recoveryTime = 45;
                        skill = new Skill("Strong", 1.5F, false);
                        break;
                    case "VeryStrong":
                        state = FighterState.PreparingAction;
                        preparationTime = 20;
                        hRecoveryTime = 45;
                        recoveryTime = 90;
                        skill = new Skill("VeryStrong", 2.0F, false);
                        break;
                    case "Magic":
                        state = FighterState.Casting;
                        castTime = 30;
                        spellTime = 20;
                        skill = new Skill("Magic", 1.0F, true);
                        break;
                    default:
                        break;
                }
                OnChosen();
            }
        }

        public void OnChosen()
        {
            if (this.HasSkill)
            {
                ITargetable target = (ITargetable)Party.EnemyParties[0].Members[0];
                target.OnTargeted(skill, this);
                targets.Add(target);
            }
        }

        public void OnPreparing()
        {
            preparationTime -= situation.ApFlow;
            if (preparationTime <= 0)
            {
                state = FighterState.PerformingAction;
            }
        }

        public void OnPerformed()
        {
            if (skill.IsMagic)
            {
                Mp.Val -= 5;
            }
            foreach (ITargetable target in Targets)
            {
                target.OnImpact(this);
            }
            targets.Clear();
            state = FighterState.HeavyRecovery;
            skill = null;
        }

        public void OnRecovery()
        {
            if (state == FighterState.HeavyRecovery)
            {
                hRecoveryTime -= situation.ApFlow;

                if (HRecoveryTime <= 0)
                {
                    recoveryTime -= -HRecoveryTime;
                    state = FighterState.Recovery;
                }
            }
            else
            {
                recoveryTime -= situation.ApFlow;

                if (recoveryTime <= 0)
                {
                    state = FighterState.Active;
                }
            }
        }

        public List<ITargetable> Targets
        {
            get { return targets; }
        }

        public IPositionable Impact
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanPrepareSkill
        {
            get { return true; }
        }

        public bool IsSkillMagic
        {
            get { return skill.IsMagic; }
        }

        public void OnCast()
        {
            castTime -= situation.ApFlow;
            if (CastTime <= 0)
            {
                state = FighterState.UsingSpell;
            }
        }

        public void OnSpell()
        {
            // Do the magic.
            spellTime -= situation.ApFlow;
            if (SpellTime <= 0)
            {
                state = FighterState.Active;
                // Remove magic.
            }
        }

        public void Cancel()
        {
            foreach (ITargetable target in targets)
            {
                target.OnCancel(this);
            }
            targets.Clear();
            skill = null;
        }
        #endregion

        #region IStatus
        public Dictionary<string, Stat> Status
        {
            get { return status; }
        }

        public Stat AttPhy
        {
            get { return status["AttPhy"]; }
        }

        public Stat AttMag
        {
            get { return status["AttMag"]; }
        }

        public Stat DefPhy
        {
            get { return status["DefPhy"]; }
        }

        public Stat DefMag
        {
            get { return status["DefMag"]; }
        }

        public Stat PaiPhy
        {
            get { return status[" PaiPhy"]; }
        }

        public Stat PaiMag
        {
            get { return status["PaiMag"]; }
        }

        public Stat ResPhy
        {
            get { return status["ResPhy"]; }
        }

        public Stat ResMag
        {
            get { return status["ResMag"]; }
        }

        public Stat DisPlus
        {
            get { return status["DisPlus"]; }
        }

        public Stat DisMinus
        {
            get { return status["DisMinus"]; }
        }

        public Stat End
        {
            get { return status["End"]; }
        }

        public Stat Init
        {
            get { return status["Init"]; }
        }

        public Stat Mp
        {
            get { return status["Mp"]; }
        }

        public Stat HitP
        {
            get { return status["HitP"]; }
        }

        public Stat FleP
        {
            get { return status["FleP"]; }
        }

        public Stat ParP
        {
            get { return status["ParP"]; }
        }

        public Stat DodP
        {
            get { return status["DodP"]; }
        }

        public Stat SpeP
        {
            get { return status["SpeP"]; }
        }

        public Stat Thre
        {
            get { return status["Thre"]; }
        }

        public Stat Rec
        {
            get { return status["Rec"]; }
        }

        public Stat CastSp
        {
            get { return status["CastSp"]; }
        }

        public Stat Luck
        {
            get { return status["Luck"]; }
        }
        #endregion

        #region IPartyMember
        public Party Party
        {
            get { return party; }
            set { party = value; }
        }

        public bool IsAllyOf(IPartyMember member)
        {
            return member.Party == Party || Party.FriendlyParties.Contains(member.Party);
        }

        public bool IsEnemyOf(IPartyMember member)
        {
            return member.Party != Party && Party.EnemyParties.Contains(member.Party);
        }
        #endregion

        #endregion

        #region private

        #region Fields
        // A fighter is aware of the current battle situation.
        private BattleSituation situation;
        private Party party;
        // Current state of the fighter.
        private FighterState state;
        private float ap;
        #region Stats
        private Dictionary<string, Stat> status = new Dictionary<string, Stat>()
            {
                { "AttPhy", new Stat("AttPhy", "Physical Attack")},
            { "AttMag", new Stat("AttMag", "Magical Attack") },
            { "DefPhy", new Stat("DefPhy", "Physical Defense")},
            { "DefMag", new Stat("DefMag", "Magical Defense")},
            { "PaiPhy", new Stat("PaiPhy", "Physical Pain")},
            { "PaiMag", new Stat("PaiMag", "Magical Pain")},
            { "ResPhy", new Stat("ResPhy", "Physical Resistance")},
            { "ResMag", new Stat("ResMag", "Magical Resistance")},
            { "DisPlus", new Stat("DisPlus", "Distance +")},
            { "DisMinus", new Stat("DisMinus", "Distance -")},
            { "End", new Stat("End", "Endurance")},
            { "Init", new Stat("Init", "Initiative") },
            { "Mp", new Stat("Mp", "Magic points")},
            { "HitP", new Stat("HitP", "Hit %")},
            { "FleP", new Stat("FleP", "Flee %")},
            { "ParP", new Stat("ParP", "Parry %")},
            { "DodP", new Stat("DodP", "Dodge %")},
            { "SpeP", new Stat("SpeP", "Speed %")},
            { "Thre", new Stat("Thre", "Threshold")},
            { "Rec", new Stat("Rec", "Recovery")},
            { "CastSpe", new Stat("CastSpe", "Cast Speed")},
            { "Luck", new Stat("Luck", "Luck")} 
            };
        #endregion

        Skill skill;

        List<ITargetable> targets = new List<ITargetable>();

        #region Space
        Vector3 position = new Vector3();
        Vector3 velocity = new Vector3();
        Vector3 acceleration = new Vector3();
        bool canMove = false;
        #endregion

        #region Time
        float hRecoveryTime;
        float recoveryTime;
        float preparationTime;
        float castTime;
        float spellTime;
        #endregion

        #region Target
        Dictionary<ICommander, Skill> targetingSkills = new Dictionary<ICommander,Skill>();
        Dictionary<Skill, ICommander> targetingActors = new Dictionary<Skill, ICommander>();
        #endregion

        #region static
        private DecisionTree decision = new DecisionTree()
            {
                { "NoCommand", 5},
                { "VeryLight", 30},
                { "Light", 20},
                { "Normal", 15},
                { "Strong", 10},
                { "VeryStrong", 5},
                {"Rest", 10},
                {"Magic", 20}
            };
        private static Dictionary<FighterState, Color> stateColors = new Dictionary<FighterState, Color>()
        {
            { FighterState.Active, Color.Green },
            { FighterState.Casting, Color.Purple },
            { FighterState.Disabled, Color.DarkGray },
            { FighterState.HeavyRecovery, Color.Red },
            { FighterState.Inactive, Color.Gray },
            { FighterState.PerformingAction, Color.White },
            { FighterState.PreparingAction, Color.Orange },
            { FighterState.Recovery, Color.Yellow },
            { FighterState.Resting, new Color(0.2F, 0.2F, 0.2F, 0.2F) },
            { FighterState.UsingSpell, Color.Blue }
        };
        #endregion
        #endregion

        #endregion
    }
}