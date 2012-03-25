/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiatLux
{
    public class Command
    {
        public int[] Children = new int[4]; // The ID of the 4 command's children. ID = 0 for invalid commands.
        public int Parent = 0; // The Parent Command's ID.
        public int ID = 0; // FOR NOW, ID BUILT IN IN CLASSES. ID = 0 => Invalid Command.
        public string Name = "";
        public int[] maxExp = new int[Constants.MAXIMUMSKILLLEVEL] { 0, 10, 100, 500, 1000, 2500, 5000, 8000 };
        public int Rank = 1;
    }
}
