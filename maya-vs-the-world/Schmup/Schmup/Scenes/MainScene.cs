using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

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
            Vector2 vect = new Vector2(1,1);
            Vector2 vect2 = new Vector2(1,0);
            // Les creations de sprite doivent être dans Initialize.
            BulletPattern bPatternTest = new BulletPattern(this.LuxGame, 1, vect, vect);
            Hero hero = new Hero(this.LuxGame, 1, 0, 0, null, 5);
            Sprite heroSprite = new Sprite(hero, skinName);
            Shot shot = new Shot(this.LuxGame, 1, null);
            Sprite shotSprite = new Sprite(shot, skinName);
            shot.Speed = vect;
            shot.Skin = shotSprite;
            hero.Skin = heroSprite;
            // Il faut ajouter au jeu les élements que vous créez.
            Game.Components.Add(hero);
            Game.Components.Add(heroSprite);
            //Game.Components.Add(shot);
            //Game.Components.Add(shotSprite);
            // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.
            heroSprite.SetAnimation("carre");
            shotSprite.SetAnimation("carre");
        }
    }
}
