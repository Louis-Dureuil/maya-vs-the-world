using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Battle
{
    enum FighterState
    {
        Inactive,
        Resting,
        Disabled,
        Active,
        Casting,
        UsingSpell,
        PreparingAction,
        PerformingAction,
        HeavyRecovery,
        Recovery
    }
}
