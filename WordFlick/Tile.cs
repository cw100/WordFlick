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
    class Tile
    {
        public Body tileBody;
        public Animation tileAnimation;
        public Vector2 velocity;
        
        SpriteFont font;
        String letter;
        public Tile( Texture2D sprite, String letter, SpriteFont font, Body tileBody)
        {
            
            this.tileBody = tileBody;
            this.font = font;
            this.letter = letter;
            tileAnimation = new Animation(sprite, tileBody.Position);
            velocity = new Vector2(0, 0);
        }
        
      
        public void Update(GameTime gameTime)
        {
            
            tileAnimation.Position = ConvertUnits.ToDisplayUnits(tileBody.Position);
          
            tileAnimation.angle = tileBody.Rotation;
            tileAnimation.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            tileAnimation.Draw(spriteBatch);

            spriteBatch.DrawString(font, letter,  ConvertUnits.ToDisplayUnits(tileBody.Position),
                Color.Black, tileAnimation.angle, font.MeasureString(letter) / 2, 1.0f, SpriteEffects.None, 0.5f);

          
        }
    }
}
