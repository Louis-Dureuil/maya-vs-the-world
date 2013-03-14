using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FLBS14.Battle.Interface
{
    /// <summary>
    /// Desribes the ability of an object to have and maintain a position.
    /// </summary>
    public interface IPositionable
    {
        Vector3 Position
        {
            get;
            set;
        }
    }
}
