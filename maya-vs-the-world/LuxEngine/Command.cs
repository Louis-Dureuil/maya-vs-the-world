using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public abstract class Command : Scene
    {
        public Command(Scene parent) : base(parent)
        {
        }

        abstract public bool IsFinished { get; }
    }
}
