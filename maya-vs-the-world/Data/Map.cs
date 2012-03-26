using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    private class Tile
    {
        /// <summary>
        /// Relative or absolute path to the file
        /// </summary>
        private string location;

        private int xSrc;
        private int ySrc;
        private int widthSrc;
        private int heightSrc;

        private float xDest;
        private float yDest;

        private float widthDest;
        private float heightDest;

        private float angle;

        private int z; // Distance, for parallax.
        private int drawOrder;
 
    }

    private enum InstructionMnemonic
    {
        Nop
    }

    private class Instruction
    {
        private InstructionMnemonic mnemonic;
        private List<Object> args;
    }

    private class TileEvent : Tile
    {
        private List<Instruction> instructions;
    }

	public class Map
	{
        private List<Tile> tiles;
	}
}
