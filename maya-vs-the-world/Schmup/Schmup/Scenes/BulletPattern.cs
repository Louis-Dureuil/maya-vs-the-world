using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class BulletPattern : MainScene
    {
        private uint nbTirs;
        private Vector2 debut;
        private Vector2 fin;

        public override void Initialize()
        {
            base.Initialize();
            // Euh... je gère pas les sprites correctement, je règlerai ça plus tard
            List<string> skinName = new List<string>(1);
            skinName.Add("carre");
            // Génération des nbTirs tirs
            // Allocation des données
            Vector2[] vect = new Vector2[nbTirs];
            Shot[] shot = new Shot[nbTirs];
            Sprite[] shotSprite = new Sprite[nbTirs];
            // Création des sprites et affectation des vitesses
            for (uint i = 0; i<nbTirs; i++)
            {
                vect[i] = new Vector2 (nbTirs-i)*debut+i*fin;
                shot[i] = new Shot(this.LuxGame, 1, null);
                shotSprite[i] = new Sprite(shot[i], skinName);
                shot[i].Speed = vect[i];
                shot[i].Position = this.Position;
                shot[i].Skin = shotSprite[i];
                //Ajout au jeu des éléments
                Game.Components.Add(shot[i]);
                Game.Components.Add(shotSprite[i]);
                shotSprite[i].SetAnimation("carre");
            }
        }


        public BulletPattern(LuxGame game, uint nbTirs, Vector2 debut, Vector2 fin)
            : base(game)
        {
            this.nbTirs = nbTirs;
            this.debut = debut;
            this.fin = fin;
        }
    }
}
