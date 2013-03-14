using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Battle.Interface
{
    /// <summary>
    /// Describes the ability of a battle element to be selected as a target of an action.
    /// </summary>
    public interface ITargetable : IStatus, IPartyMember
    {
        bool IsTarget { get; }
        bool IsTargetOf(ICommander actor);
        bool IsTargetOf(Skill skill);

        Skill GetSkillFromActor(ICommander actor);
        ICommander GetActorFromSkill(Skill skill);

        /// <summary>
        /// Is triggered when the target is being targeted.
        /// </summary>
        /// <param name="targetingSkill">The skill that the actor uses to target the target.</param>
        /// <param name="actor">The actor that targets the target.</param>
        void OnTargeted(Skill targetingSkill, ICommander actor);

        /// <summary>
        /// Is triggered when it is time for the target to react to a skill.
        /// </summary>
        /// <param name="targetingSkill">The skill that the actor uses to target the target.</param>
        /// <param name="actor">The actor that targets the target.</param>
        void Reacting(Skill targetingSkill, ICommander actor);
        void Reacting(ICommander actor);
        void Reacting(Skill targetingSkill);

        /// <summary>
        /// Is triggered when the actor realise its action.
        /// </summary>
        /// <param name="targetingSkill">The skill that the actor uses to target the target.</param>
        /// <param name="actor">The actor that targets the target.</param>
        void OnImpact(Skill targetingSkill, ICommander actor);
        void OnImpact(ICommander actor);
        void OnImpact(Skill targetingSkill);

        /// <summary>
        /// Is triggered when the actor cancels its action. 
        /// </summary>
        /// <param name="targetingSkill">The skill that the actor uses to target the target.</param>
        /// <param name="actor">The actor that targets the target.</param>
        void OnCancel(Skill targetingSkill, ICommander actor);
        void OnCancel(Skill targetingSkill);
        void OnCancel(ICommander actor);
    }
}
