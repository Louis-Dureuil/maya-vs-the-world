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
            // GROSSE MODIFICATION
            // A ENLEVER DES QUE POSSIBLE
            // Euh... je gère pas les sprites correctement, je règlerai ça plus tard
            // Génération des nbTirs tirs
            // Allocation des données
            Vector2 debut = new Vector2 (1,1);
            Vector2 fin = new Vector2 (1,0);
            Vector2[] vectar = new Vector2[5];
            Shot[] shotar = new Shot[5];
            Sprite[] shotSpritear = new Sprite[5];
            // Création des sprites et affectation des vitesses
            for (uint i = 0; i<5; i++)
            {
                vectar[i] = new Vector2 (5-i)*debut+i*fin;
                shotar[i] = new Shot(this.LuxGame, 1, null);
                shotSpritear[i] = new Sprite(shotar[i], skinName);
                shotar[i].Speed = vectar[i];
                shotar[i].Position = this.Position;
                shotar[i].Skin = shotSpritear[i];
                //Ajout au jeu des éléments
                Game.Components.Add(shotar[i]);
                Game.Components.Add(shotSpritear[i]);
                shotSpritear[i].SetAnimation("carre");
            }
            // FIN DE LA MODIF
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
