using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FLBS14.Battle.Interface
{
    /// <summary>
    /// Describes the ability of a battle element to move on the battlefield.
    /// </summary>
    public interface IMovable : IPositionable
    {
        Vector3 Velocity
        {
            get;
            set;
        }

        Vector3 Acceleration
        {
            get;
            set;
        }



        bool IsMoving { get; }
        bool CanMove { get; }
        bool IsAuto { get; }

        void OnMove();
    }
}
