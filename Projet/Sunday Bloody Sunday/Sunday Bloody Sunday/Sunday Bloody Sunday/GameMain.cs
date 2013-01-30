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
    class GameMain
    {
        // FIELDS
        public Map MainMap;
        HUD MainHUD;


        // CONCSTRUCTOR
        public GameMain()
        {
            this.MainMap = new Map(new PhysicsEngine());
            this.MainHUD = new HUD();
        }


        // METHODS


        // UPDATE & DRAW
        public void Update(MouseState mouse, KeyboardState keyboard)
        {
                MainMap.Update(mouse, keyboard);
                MainHUD.Update(keyboard, MainMap.joueurs);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            MainMap.Draw(spriteBatch);
            MainHUD.Draw(spriteBatch, spriteFont);
        }
    }
}
