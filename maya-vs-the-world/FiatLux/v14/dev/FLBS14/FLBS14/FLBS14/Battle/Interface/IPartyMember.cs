using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Battle.Interface
{
    public interface IPartyMember
    {
        Party Party { get; set; }
        bool IsAllyOf(IPartyMember member);
        bool IsEnemyOf(IPartyMember member);
    }
}
