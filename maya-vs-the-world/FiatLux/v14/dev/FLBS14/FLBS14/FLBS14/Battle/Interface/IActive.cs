using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Battle.Interface
{
    /// <summary>
    /// Describes the ability of a battle element to react to the flow of time on the battlefield.
    /// </summary>
    public interface IActive
    {
        bool IsActive { get; }
        bool IsDisabled { get; }
        int Initiative { get; }
        float Ap { get; }

        void OnActive();
        void OnTurnStart(float avgInitiative);
        void OnTurnEnd();
    }
}
