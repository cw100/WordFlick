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
        public bool inSlot;
        public float timeSinceLastSlot;
        public Vector2 slotPostition;
        public Body tileBody;
        public Animation tileAnimation;
        public Vector2 velocity;
        public bool usable;
        SpriteFont font;
        public String letter;
        public int slotNum;

        public Tile( Texture2D sprite, String letter, SpriteFont font, Body tileBody)
        {
            timeSinceLastSlot = 0;
            slotNum = -1;
            inSlot = false;
            usable = false;
            this.tileBody = tileBody;
            this.font = font;
            this.letter = letter;
            tileAnimation = new Animation(sprite, tileBody.Position);
            velocity = new Vector2(0, 0);
        }
        
      
        public void Update(GameTime gameTime)
        {
            Vector2 lastPos = tileAnimation.Position;
          
            if (tileAnimation.Position.Y>500)
            {
                usable = true;
            }
            timeSinceLastSlot += gameTime.ElapsedGameTime.Milliseconds;
            if (slotPostition != new Vector2(0,0))
            {
                timeSinceLastSlot=0;
                Vector2 forceDirection = (slotPostition - tileAnimation.Position);
               
                    float distance = forceDirection.Length();
                    forceDirection.Normalize();

                    tileBody.ApplyForce(6*distance * forceDirection - 20 * tileBody.LinearVelocity);
                    float newAngle = -(tileBody.Rotation + tileBody.AngularVelocity/60);
                    while(newAngle<-Math.PI)
                    {
                        newAngle += 2 * (float)Math.PI;
                    }
                    while (newAngle > Math.PI)
                    {
                        newAngle -= 2 * (float)Math.PI;
                    }
                    
                    tileBody.ApplyTorque(tileBody.Inertia* newAngle*360);
                
            }
          
      
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
