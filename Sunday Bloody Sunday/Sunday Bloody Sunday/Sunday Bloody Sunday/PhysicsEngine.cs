﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Sunday_Bloody_Sunday
{
    public class PhysicsEngine
    {
        // Tableau des booléens, le 100*100 est purement arbitraire, il devrait correspondre à la longueur/largeur de la map
        private bool[,] liste;
        private bool[,] liste_projectile;

        public PhysicsEngine(bool[,] liste, bool[,] liste_projectile)
        {
            this.liste = liste;
            this.liste_projectile = liste_projectile;
        }

        // Teste si la zone est franchissable (false) ou infranchissable (true) à l'aide du tableau de bool, si x <= 0 ou y <= 0, on est hors de la map
        public bool mur(int x, int y)
        {

            try
            {
                return this.liste[x / 16, y / 16];
            }
            catch
            {
                return true;
            }
            /*
            if ((x <= 0) || (y <= 0) || (x >= 800) ||(y >=1600))
                return true;
            else
                return this.liste[x / 16, y / 16]; */
        }

        public bool mur_projectile(int x, int y)
        {
            
            try
            {
                return this.liste_projectile[x / 16, y / 16];
            }
            catch
            {
                return true;
            }
            /*
            if ((x < 0) || (y < 0) || (x >= 800) ||(y >=480))
                return true;
            else
                return this.liste[x / 16, y / 16];*/
        }

        public bool[,] map()
        {
            return liste;
        }

        public bool[,] map_projectile()
        {
            return liste_projectile;
        }
    }
}
