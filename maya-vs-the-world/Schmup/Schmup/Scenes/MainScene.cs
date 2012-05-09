using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;

namespace Schmup
{
    class MainScene : Scene
    {
        public MainScene(LuxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            List<string> skinName = new List<string>(1);
            skinName.Add("carre");
            // Les creations de sprite doivent être dans Initialize.
            Hero hero = new Hero(this.LuxGame, 1, 0, 0, null, 1);
            Sprite heroSprite = new Sprite(hero, skinName);
            hero.Skin = heroSprite;
            // Il faut ajouter au jeu les élements que vous créez.
            Game.Components.Add(hero);
            Game.Components.Add(heroSprite);
            // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.
            heroSprite.SetAnimation("carre");
        }
    }
}
