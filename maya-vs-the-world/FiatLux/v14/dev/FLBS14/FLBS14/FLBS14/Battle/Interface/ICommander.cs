using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Battle.Interface
{
    /// <summary>
    /// Describes the ability of an object to issue, prepare and perform commands.
    /// </summary>
    public interface ICommander : IStatus, IPartyMember
    {
        List<ITargetable> Targets { get; }
        IPositionable Impact { get; } 
        bool HasSkill { get; }
        bool CanPrepareSkill { get; }
        bool IsSkillMagic { get; }
        Skill Skill { get; }
        float PreparationTime { get; }
        float CastTime { get; }
        float HRecoveryTime { get; }
        float RecoveryTime { get; }
        float SpellTime { get; }

        void OnChoosing();
        void OnChosen();

        void OnPreparing();

        void OnPerformed();

        void OnRecovery();
        void OnCast();
        void OnSpell();

        void Cancel();
    }
}
