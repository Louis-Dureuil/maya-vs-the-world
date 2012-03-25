/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Text;
using FiatLux.Logic;

namespace FiatLux
{
    /// <summary>
    /// Records all game Settings and the Party informations. This is meant to change during the game, but must have a global scope.
    /// </summary>
    static public class Settings
    {
        // Current array of the ID of the Actors that are contained in the party.
        static public int[] Party = new int[Constants.PartyCapacity];

        static public int PartySize = 0;

        // Current state of the characters.
        static public Character[] Characters = new Character[Constants.CharactersCapacity];

        static int gold = 0;

        // Current Gold amount.
        static public int Gold
        {
            get { return gold; }
            set
            {
                gold = Math.Max(Math.Min(value, Constants.MaximumGold), 0);
            }
        }
    }
}
