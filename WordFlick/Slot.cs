using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;

namespace WordFlick
{
    class Slot
    {
        public Vector2 position;
        public bool full;
        public Animation slotAnimation;
        public String letter;
           public Slot( Texture2D sprite, Vector2 position)
            {
                
            full = false;
            this.position = position;
            slotAnimation = new Animation(sprite, position);
           }
        public void Update(GameTime gameTime)
           {
               slotAnimation.Update(gameTime);
           }
        public void Draw(SpriteBatch spriteBatch)
        {
            slotAnimation.Draw(spriteBatch);
        }
    }
}
