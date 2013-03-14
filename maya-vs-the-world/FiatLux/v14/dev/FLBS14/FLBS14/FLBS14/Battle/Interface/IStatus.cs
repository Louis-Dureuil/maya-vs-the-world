using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Battle.Interface
{
    public interface IStatus
    {
        Dictionary<string, Stat> Status { get; }

        Stat AttPhy { get; }
        Stat AttMag { get; }
        Stat DefPhy { get; }
        Stat DefMag { get; }
        Stat PaiPhy { get; }
        Stat PaiMag { get; }
        Stat ResPhy { get; }
        Stat ResMag { get; }
        Stat DisPlus { get; }
        Stat DisMinus { get; }
        Stat End { get; }
        Stat Init { get; }
        Stat Mp { get; }
        Stat HitP { get; }
        Stat FleP { get; }
        Stat ParP { get; }
        Stat DodP { get; }
        Stat SpeP { get; }
        Stat Thre { get; }
        Stat Rec { get; }
        Stat CastSp { get; }
        Stat Luck { get; }

    }
}
