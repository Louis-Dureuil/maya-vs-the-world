using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;

namespace FiatLux.Scenes
{
    public class MapCreatorScene : Scene
    {
        public MapCreatorScene(LuxGame game) 
            : base(game)
        {
            Map myMap = new Map("backgroundM", "academie", 400, 400);
            MapManager.Save("Data/Map/myMap.FLMap", myMap);
        }
    }
}
