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

namespace Sunday_Bloody_Sunday
{
    class Map
    {
        // FIELDS
        Rectangle MapTexture;
        public List<Spawn> spawns;
        public Spawn_Items spawn_items;

        public List<Items> liste_box; //Liste Items
        public List<Items> liste_box2; //Liste Items secondaire, utilisée pour nettoyer la mémoire

        public List<DestructibleItems> liste_barrel; //Liste barrel
        public List<DestructibleItems> liste_barrel2; //Liste barrel secondaire

        public List<ExplosionParticule> liste_explosions; //Liste particules d'explosion
        public List<ExplosionParticule> liste_explosions2; //Liste particules d'explosion secondaire

        public List<Player> liste_joueurs = new List<Player>(); //Liste joueur
        public List<Player> liste_joueurs2 = new List<Player>(); //Liste joueurs secondaire

        public List<IA> liste_ia; //Liste des IA
        public List<IA> liste_ia2; //Liste IA secondaire

        public List<Projectile> liste_projectile = new List<Projectile>(); //Liste des Projectiles
        public List<Projectile> liste_projectile2 = new List<Projectile>(); //Liste Projectiles secondaire
        Projectile balle;
        IA ia;
        Texture2D explosionTexture;
        PhysicsEngine map_physique;
        private Rectangle futur_rectangle; //Rectangle utilisé por stocker des données
        int compteur = 0;
        Random rand = new Random();
        Sound moteur_son = new Sound();
        bool etape1 = false;
        bool etape2 = false;
        public bool game_over = false;
        Param_Map parametre;


        // CONSTRUCTOR
        public Map(Param_Map parametre)
        {
            this.parametre = parametre;

            MapTexture = new Rectangle(0, 0, parametre.hauteur * 16, (parametre.largeur - 1) * 16);
            this.map_physique = new PhysicsEngine(parametre.liste,parametre.liste_projectile);
            this.liste_ia = new List<IA>();
            this.liste_joueurs.Add(new Player(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.N, Keys.P, Keys.Enter, Ressources.Player1, parametre.x,parametre.y));

            //HEALTH + AMMO BOXES
            this.liste_box = new List<Items>();
            this.liste_box2 = new List<Items>();

            //EXPLOSIVE BOXES
            this.liste_barrel = new List<DestructibleItems>();
            this.liste_barrel = parametre.liste_barrel;

            //EXPLOSION PARTICULE
            this.liste_explosions = new List<ExplosionParticule>();
            this.liste_explosions2 = new List<ExplosionParticule>();
            this.explosionTexture = Ressources.ExplosionParticule;

            this.spawns = parametre.liste_spawn;
            this.spawn_items = parametre.liste_caisses;

        }


        // METHODS
        //Verifie la possibilité des actions de l'IA
        //Ces deux actions sont similaires à celles du héros
        //Gère le deplacement de l'ia
        public void pathfing(ref string action, Player joueur)
        {

            if (ia.compteur_path % 30 == 0)
            {
                ia.compteur_path = 0;
                Random rand = new Random();
                ia.ia_dir = rand.Next(0, 2);
            }

            ia.compteur_path++;

            if (ia.ia_dir == 0)
            {
                if (joueur.PlayerTexture.X < this.ia.IATexture.X)
                {
                    action = "left";
                    if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                         && !(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas())))
                    {
                    }
                    else
                    {
                        if (joueur.PlayerTexture.Y < this.ia.IATexture.Y)
                        {
                            action = "up";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                             && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                        else if (joueur.PlayerTexture.Y > this.ia.IATexture.Y)
                        {
                            action = "down";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas()))
                             && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                    }
                }
                else if (joueur.PlayerTexture.X > this.ia.IATexture.X)
                {
                    action = "right";
                    if (!(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut()))
                         && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                    {
                    }
                    else
                    {
                        action = "up";
                        if (joueur.PlayerTexture.Y < this.ia.IATexture.Y)
                        {

                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                             && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                        else if (joueur.PlayerTexture.Y > this.ia.IATexture.Y)
                        {
                            action = "down";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas()))
                             && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                    }
                }
                else if (joueur.PlayerTexture.Y < this.ia.IATexture.Y)
                {
                    action = "up";

                    if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                     && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut())))
                    {
                    }
                    else
                    {
                        if (joueur.PlayerTexture.X < this.ia.IATexture.X)
                        {
                            action = "left";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                                 && !(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                        else if (joueur.PlayerTexture.X > this.ia.IATexture.X)
                        {
                            action = "right";
                            if (!(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut()))
                                 && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                    }
                }
                else if (joueur.PlayerTexture.Y > this.ia.IATexture.Y)
                {
                    action = "down";
                    if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas()))
                     && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                    {
                    }
                    else
                    {
                        if (joueur.PlayerTexture.X < this.ia.IATexture.X)
                        {
                            action = "left";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                                 && !(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                        else if (joueur.PlayerTexture.X > this.ia.IATexture.X)
                        {
                            action = "right";
                            if (!(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut()))
                                 && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }

                    }
                }
            }
            else
            {

                if (joueur.PlayerTexture.Y < this.ia.IATexture.Y)
                {
                    action = "up";

                    if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                     && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut())))
                    {
                    }
                    else
                    {
                        if (joueur.PlayerTexture.X < this.ia.IATexture.X)
                        {
                            action = "left";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                                 && !(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                        else if (joueur.PlayerTexture.X > this.ia.IATexture.X)
                        {
                            action = "right";
                            if (!(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut()))
                                 && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                    }
                }
                else if (joueur.PlayerTexture.Y > this.ia.IATexture.Y)
                {
                    action = "down";
                    if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas()))
                     && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                    {
                    }
                    else
                    {
                        if (joueur.PlayerTexture.X < this.ia.IATexture.X)
                        {
                            action = "left";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                                 && !(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                        else if (joueur.PlayerTexture.X > this.ia.IATexture.X)
                        {
                            action = "right";
                            if (!(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut()))
                                 && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }

                    }
                }
                else if (joueur.PlayerTexture.X < this.ia.IATexture.X)
                {
                    action = "left";
                    if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                         && !(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas())))
                    {
                    }
                    else
                    {
                        if (joueur.PlayerTexture.Y < this.ia.IATexture.Y)
                        {
                            action = "up";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                             && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                        else if (joueur.PlayerTexture.Y > this.ia.IATexture.Y)
                        {
                            action = "down";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas()))
                             && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                    }
                }
                else if (joueur.PlayerTexture.X > this.ia.IATexture.X)
                {
                    action = "right";
                    if (!(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut()))
                         && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                    {
                    }
                    else
                    {
                        action = "up";
                        if (joueur.PlayerTexture.Y < this.ia.IATexture.Y)
                        {

                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_haut()))
                             && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_haut())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                        else if (joueur.PlayerTexture.Y > this.ia.IATexture.Y)
                        {
                            action = "down";
                            if (!(map_physique.mur(this.ia.futur_position_X_gauche(), this.ia.futur_position_Y_bas()))
                             && !(map_physique.mur(this.ia.futur_position_X_droite(), this.ia.futur_position_Y_bas())))
                            {
                            }
                            else
                            {
                                action = "";
                            }
                        }
                    }
                }
            }

        }



        //Gère le raffraichissement de la liste d'IA
        public void update_ia()
        {
            foreach (IA ia in liste_ia) //Pour chaque IA de la liste
            {
                if (ia.Health > 0)
                {
                    ia.est_update = true; //Précise que l'on travaille sur la liste
                    this.ia = ia;
                    float distance = 10000;
                    Vector2 vector_ia = new Vector2(ia.IATexture.X, ia.IATexture.Y);
                    Player joueur_cible = new Player(Keys.Up, Keys.Down, Keys.Right, Keys.Left, Keys.S, Keys.P, Keys.Enter, Ressources.Player1,parametre.x,parametre.y);
                    Vector2 vector_joueur = new Vector2();
                    foreach (Player joueur in liste_joueurs)
                    {
                        joueur_cible = joueur;
                        vector_joueur = new Vector2(joueur_cible.PlayerTexture.X, joueur_cible.PlayerTexture.Y);
                    }

                    Vector2.Distance(ref vector_ia, ref vector_joueur, out distance);
                    foreach (Player joueur in liste_joueurs)
                    {
                        vector_joueur = new Vector2(joueur.PlayerTexture.X, joueur.PlayerTexture.Y);
                        float distance_2;
                        Vector2.Distance(ref vector_ia, ref vector_joueur, out distance_2);
                        if (distance_2 < distance)
                        {
                            distance = distance_2;
                            joueur_cible = joueur;
                        }
                    }
                    //L'ensemble des commandes précédentes définissent quel héros est la cible, ici le plus proche
                    /*
                    Node départ = new Node();
                    départ.x = (this.ia.IATexture.X + 8) / 16;
                    départ.y = (this.ia.IATexture.Y + 8) / 16;
                    Node arrivée = new Node();
                    arrivée.x = (joueur_cible.PlayerTexture.X + 8) / 16;
                    arrivée.y = (joueur_cible.PlayerTexture.Y + 8) / 16;

                    bool[,] map = map_physique.map();*/
                    /*
                    this.ia.action = Pathfinding.pathfind(map, départ, arrivée);
                    */
                    pathfing(ref this.ia.action, joueur_cible);
                    //Trouve quelle action va faire l'IA
                    ia.action_ia(ia, joueur_cible, liste_barrel, map_physique, liste_ia, liste_joueurs); //Verifie la possibilité de réalisation des actions
                    ia.Update(); //Met à jour l'IA
                    ia.attaque_ia(liste_joueurs);
                    ia.est_update = false; //Désactive l'update de l'IA
                }
                else
                {
                    ia.en_vie = false; //Tue l'IA
                }
            }
            if (compteur > 180 && etape1) //Ajout de nouvelles IA a la map
            {

                int choix = rand.Next(spawns.Count);
                Spawn spawn = spawns.ElementAt(choix);
                choix = rand.Next(spawn.créatures.Count);

                IA ia = spawn.créatures.ElementAt(choix);
                IA ia_ = new IA(ia.IATexture.X, ia.IATexture.Y, ia.id_son, ia.id_texture, ia.Health, ia.Damage);


                float distance = 10000;
                Vector2 vector_ia = new Vector2(ia_.IATexture.X, ia_.IATexture.Y);
                Player joueur_cible = new Player(Keys.Up, Keys.Down, Keys.Right, Keys.Left, Keys.S, Keys.P, Keys.Enter, Ressources.Player1,parametre.x,parametre.y);
                Vector2 vector_joueur = new Vector2();
                foreach (Player joueur in liste_joueurs)
                {
                    joueur_cible = joueur;
                    vector_joueur = new Vector2(joueur_cible.PlayerTexture.X, joueur_cible.PlayerTexture.Y);
                }

                Vector2.Distance(ref vector_ia, ref vector_joueur, out distance);
                foreach (Player joueur in liste_joueurs)
                {
                    vector_joueur = new Vector2(joueur.PlayerTexture.X, joueur.PlayerTexture.Y);
                    float distance_2;
                    Vector2.Distance(ref vector_ia, ref vector_joueur, out distance_2);
                    if (distance_2 < distance)
                    {
                        distance = distance_2;
                    }
                }
                if (distance < 400)
                {
                    bool test = true;
                    foreach (IA ia__ in liste_ia)
                    {
                        if (ia__.IATexture.Intersects(ia_.IATexture))
                        {
                            test = false;
                        }
                    }
                    if (test)
                    {
                        liste_ia.Add(ia_);
                        moteur_son.PlayPika();
                        compteur = 0;
                    }
                }
            }
            compteur = compteur + 1;

            liste_ia2 = new List<IA>(); //Recopie la liste d'IA encore en vie dans une nouvelle liste
            foreach (IA ia in liste_ia)
            {
                if (ia.en_vie)
                    liste_ia2.Add(ia);
            }
            liste_ia = liste_ia2; //Vide la liste secondaire dans la premiere
        }

        public void update_projectiles(KeyboardState keyboard)
        {
            foreach (Projectile balle in liste_projectile)
            {
                balle.init = 0;
            }

            foreach (Projectile balle in liste_projectile)
            {
                while ((balle.init < balle.projectileMoveSpeed) && balle.isVisible)
                {
                    balle.collision_entite_balle(liste_ia); //Collision entres balles et entité
                    balle.collision_balle(map_physique); //Collision entre balle et mur
                    collision_barrel_balle(balle);
                    balle.init++; //On bouge la balle d'une case
                }
            }

            foreach (Player joueur in liste_joueurs) //On ajoute des balles en fonction des touches et du refroidissement
            {
                if ((keyboard.IsKeyDown(joueur.Tire)) && joueur.refroidissement >= 12 && etape1 && joueur.Ammo > 0)
                {
                    balle = new Projectile(Ressources.Projectile, (int)joueur.centre().X, (int)joueur.centre().Y, 10, joueur.Direction, 50);
                    liste_projectile.Add(balle);
                    joueur.refroidissement = 0;
                    joueur.Ammo = joueur.Ammo - 1;
                    moteur_son.PlayTire();

                }
                joueur.refroidissement++;
            }

            liste_projectile2 = new List<Projectile>(); //On nettoie la liste, comme avec les IA
            foreach (Projectile balle in liste_projectile)
            {
                if (balle.isVisible)
                    liste_projectile2.Add(balle);
            }
            liste_projectile = liste_projectile2;
        }

        public void update_Box()
        {
            foreach (Items box in liste_box)
            {
                box.Update(liste_joueurs);
            }
            liste_box2 = new List<Items>();
            foreach (Items box in liste_box)
            {
                if (box.isVisible)
                {
                    liste_box2.Add(box);
                }
            }
            liste_box = liste_box2;

            if (compteur > 180) //Ajout de nouveaux "barrels" a la map
            {
                Items box;
                int spawn = rand.Next(spawn_items.emplacement.Count);
                int texture = rand.Next(2);
                Vector2 emplacement = spawn_items.emplacement.ElementAt(spawn);
                bool test = true;
                if (texture == 0)
                {
                    box = new Items((int)emplacement.X, (int)emplacement.Y, "health");
                    foreach (Items box_ in liste_box)
                    {
                        if ((box_.HealthBoxTexture.Intersects(box.HealthBoxTexture)) && test)
                        {
                            test = false;
                        }
                    }
                    if (test)
                        liste_box.Add(box);
                }
                else
                {

                    box = new Items((int)emplacement.X, (int)emplacement.Y, "ammo");
                    foreach (Items box_ in liste_box)
                    {
                        if ((box_.HealthBoxTexture.Intersects(box.HealthBoxTexture)) && test)
                        {
                            test = false;
                        }
                    }
                    if (test)
                        liste_box.Add(box);
                }
                if (test)
                {
                    moteur_son.PlayPop();

                    compteur = 0;
                }
            }

            compteur++;
        }

        public void update_Bomb(List<Player> liste_joueurs, KeyboardState keyboard)
        {
            foreach (Player joueur in liste_joueurs)
            {
                if (keyboard.IsKeyDown(joueur.poser) && joueur.bomb > 0)
                {
                    AddBomb(joueur.PlayerTexture.X, joueur.PlayerTexture.Y, joueur.activer);
                    joueur.bomb--;
                }
            }

            foreach (DestructibleItems bomb in liste_barrel)
            {
                if (bomb.type == "bomb" && keyboard.IsKeyDown(bomb.boum))
                {
                    AddExplosion(new Vector2(bomb.BombTexture.X + 8, bomb.BombTexture.Y + 8), bomb.Aire_barrel.X - 16, bomb.Aire_barrel.Y - 16, 48);
                    moteur_son.PlayExplosionEffect();
                    bomb.isVisible = false;
                }
            }

        }

        public void update_Barrel(KeyboardState keyboard)
        {
            foreach (DestructibleItems barrel in liste_barrel)
            {
                barrel.Update(liste_joueurs, keyboard, liste_barrel);
            }
            liste_barrel2 = new List<DestructibleItems>();
            foreach (DestructibleItems barrel in liste_barrel)
            {
                if (barrel.isVisible)
                {
                    liste_barrel2.Add(barrel);
                }
            }
            liste_barrel = liste_barrel2;
        }

        public void collision_barrel_balle(Projectile balle) //S'occupe de la collision des balles avec les "barrels"
        {
            futur_rectangle = balle.rectangle();
            bool test = true;
            foreach (DestructibleItems barrel in liste_barrel) //Vérifie pour chaque "barrels"
            {
                if ((test)) //Permet de casser la boucle dès qu'un "barrel" est touché
                {
                    if (futur_rectangle.Intersects(barrel.Aire_barrel) && barrel.type == "barrel") //Si la HitBox du projectile est en contact avec celle du "barrel", alors (...)
                    {
                        balle.isVisible = false; //La balle n'existe plus
                        barrel.isVisible = false; //Le "barrel" n'existe plus
                        AddExplosion(new Vector2(barrel.Aire_barrel.X + 8, barrel.Aire_barrel.Y + 8), barrel.Aire_barrel.X - 16, barrel.Aire_barrel.Y - 16, 48);
                        moteur_son.PlayExplosionEffect();
                        test = false; //On casse le si
                    }
                }
            }
        }

        public void AddExplosion(Vector2 position, int x, int y, int largeur)
        {
            ExplosionParticule explosion = new ExplosionParticule();
            explosion.Initialize(Ressources.ExplosionParticule, position, 134, 134, 12, 45, Color.White, 1f, false, x, y, largeur);
            liste_explosions.Add(explosion);
        }

        public void update_explosions(GameTime gameTime)
        {
            liste_explosions2 = new List<ExplosionParticule>();
            foreach (ExplosionParticule explosion in liste_explosions)
            {
                explosion.Update(gameTime, liste_joueurs, liste_ia, liste_barrel);
                if (explosion.Active == true)
                {
                    liste_explosions2.Add(explosion);
                }
            }

            liste_explosions = liste_explosions2;
        }

        public void AddBomb(int x, int y, Keys activer)
        {
            DestructibleItems bomb = new DestructibleItems(x, y, "bomb", activer);
            liste_barrel.Add(bomb);
        }

        //Gère l'affichage de la liste d'IA
        public void swap(ref IA a, ref IA b)
        {
            IA c = a;
            a = b;
            b = c;
        }

        public int minimum(IA[] tableau, int d, int f)
        {
            int pos = d;
            int i = d + 1;
            while (i < tableau.Length)
            {
                if (tableau[i].IATexture.Y < tableau[pos].IATexture.Y)
                {
                    pos = i;
                }
                i++;
            }

            return pos;
        }

        public void tri(ref IA[] tableau)
        {
            int i = 0;
            while (i < tableau.Length)
            {
                swap(ref tableau[i], ref tableau[minimum(tableau, i, tableau.Length)]);
                i++;
            }
        }

        public void draw_ordre(SpriteBatch spriteBatch)
        {
            IA[] tableau_ia = new IA[liste_ia.Count];
            liste_ia.CopyTo(tableau_ia);
            tri(ref tableau_ia);
            foreach (Player joueur in liste_joueurs)
            {
                joueur.est_afficher = false;
            }
            foreach (IA ia in tableau_ia)
            {
                foreach (Player joueur in liste_joueurs)
                {
                    if ((ia.IATexture.Y > joueur.PlayerTexture.Y) && !joueur.est_afficher)
                    {
                        joueur.Draw(spriteBatch, MapTexture);
                        joueur.est_afficher = true;

                    }

                    ia.Draw(spriteBatch, MapTexture);
                }


            }
            foreach (Player joueur in liste_joueurs)
            {
                if (!joueur.est_afficher)
                {
                    joueur.Draw(spriteBatch, MapTexture);
                    joueur.est_afficher = true;

                }
            }
        }

        public void update_player(KeyboardState keyboard, MouseState mouse)
        {

            // Update l'objet joueur contenu par la map
            foreach (Player joueur in liste_joueurs)
            {
                joueur.Update(mouse, keyboard);
                joueur.action_hero(map_physique, liste_ia, liste_barrel);
                if (joueur.Health > 0)
                {
                    liste_joueurs2.Add(joueur);
                }
            }
            liste_joueurs = liste_joueurs2;
            liste_joueurs2 = new List<Player>();

            if (keyboard.IsKeyDown(Keys.D1) && !etape1)
            {
                etape1 = true;
            }

            if (keyboard.IsKeyDown(Keys.D2) && !etape2)
            {
                etape2 = true;
                this.liste_joueurs.Add(new Player(Keys.Z, Keys.S, Keys.Q, Keys.D, Keys.A, Keys.E, Keys.R, Ressources.Player2,parametre.x,parametre.y));/*
                this.liste_joueurs.Add(new Player(Keys.NumPad8, Keys.NumPad5, Keys.NumPad4, Keys.NumPad6, Keys.NumPad7, Ressources.Player3));*/
            }
            if (liste_joueurs.Count == 0) //Si il n'y a plus de joueurs
            {
                game_over = true;
            }
        }

        // UPDATE & DRAW
        public void Update(MouseState mouse, KeyboardState keyboard, GameTime gameTime)
        {
            update_ia();
            update_Box();
            update_Barrel(keyboard);
            update_explosions(gameTime);
            update_projectiles(keyboard);
            update_player(keyboard, mouse);
            update_Bomb(liste_joueurs, keyboard);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int x = 0;
            int y = 0;
            foreach (Player joueur in liste_joueurs)
            {
                x = joueur.PlayerTexture.X;
                y = joueur.PlayerTexture.Y;
            }
            MapTexture.X  = 400 - x;
            MapTexture.Y = 240 - y;
            if (parametre.texture_map == 0)
            {
                spriteBatch.Draw(Ressources.Map, this.MapTexture, Color.CadetBlue);
            }
            else
            {
                spriteBatch.Draw(Ressources.Map02, this.MapTexture, Color.CadetBlue);
            }
            foreach (Items box in liste_box)
            {
                box.Draw(spriteBatch, MapTexture);
            }
            foreach (DestructibleItems barrel in liste_barrel)
            {
                barrel.Draw(spriteBatch, MapTexture);
            }
            draw_ordre(spriteBatch);
            foreach (Projectile projectile in liste_projectile)
            {
                projectile.Draw(spriteBatch, MapTexture);
            }
            foreach (ExplosionParticule explosion in liste_explosions)
            {
                explosion.Draw(spriteBatch, MapTexture);
            }
        }
    }
}
