using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Controllers;
using FarseerPhysics;
using System.IO;
namespace WordFlick
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        int score;
        MouseState mouseState;
        FixedMouseJoint mouseJoint;
        World world;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D lineTex;
        Texture2D slotTex;
        List<Slot> slots;
        List<Tile> tiles;
        SpriteFont tileFont;

        SpriteFont wordFont;
        Texture2D tileTex;
        Random random;
        static public int windowHeight = 800;
        static public int windowWidth = 600;
        float width = ConvertUnits.ToSimUnits(windowWidth);
        string[] words;
        float height = ConvertUnits.ToSimUnits(windowHeight); 
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            words = File.ReadAllLines("dictionary.txt");
          
             this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;   
            graphics.ApplyChanges();
            tiles = new List<Tile>();
            slots = new List<Slot>();
            random = new Random();
            base.Initialize();
            GenerateTiles(30);
            GenerateSlots(5);
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            if (world == null)
            {
                world = new World(new Vector2(0, 5));
                
            }
            else
            {
                world.Clear();
            }
         
            Body screenLeft = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(0, 0),
                    ConvertUnits.ToSimUnits(0, windowHeight));
            screenLeft.BodyType = BodyType.Static;
            Body screenRight = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(windowWidth, 0),
                    ConvertUnits.ToSimUnits(windowWidth, windowHeight));
            screenRight.BodyType = BodyType.Static;
            Body screenBottom = BodyFactory.CreateEdge(world, ConvertUnits.ToSimUnits(0, windowHeight),
                    ConvertUnits.ToSimUnits(windowWidth, windowHeight));
            screenBottom.BodyType = BodyType.Static;

            slotTex = Content.Load<Texture2D>("slot");
           


            spriteBatch = new SpriteBatch(GraphicsDevice);
            lineTex = new Texture2D(GraphicsDevice, 1, 1);
            lineTex.SetData<Color>(new Color[] { Color.White });
            
            tileTex = Content.Load<Texture2D>("Tile");
            tileFont = Content.Load<SpriteFont>("Arial");
            wordFont = Content.Load<SpriteFont>("ArialBig");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        public char GetLetter()
        {
           
            int index = random.Next(0, 26);
            char letter = (char)('A' + index);
            return letter;
        }
        public void GenerateTiles(int max)
        {
            for (int i = 0; i < max; i++)
            {
               
                int xPos = random.Next(tileTex.Width / 2, windowWidth - tileTex.Width / 2);
                int yPos = random.Next(tileTex.Height*3);
                Body tileBody = BodyFactory.CreateRectangle(world,  ConvertUnits.ToSimUnits(tileTex.Width-1),
                    ConvertUnits.ToSimUnits(tileTex.Height-1), 5.0f);
                tileBody.BodyType = BodyType.Dynamic;
                tileBody.Position = ConvertUnits.ToSimUnits(xPos, yPos);
                Tile tile = new Tile(tileTex, GetLetter().ToString(), tileFont, tileBody);
                tiles.Add(tile);
            }
        }
        public void GenerateSlots(int max)
        {
            for (int i = 0; i < max; i++)
            {
                Vector2 pos = new Vector2(50 + 70 * i, 400);
                
                Slot slot = new Slot(slotTex,pos);
                slots.Add(slot);

            }
        }
        String currentWord;
        bool correctWord;
        public void TestLetters()
        {
            textColor = Color.Black;
            currentWord = "";
             correctWord = false;
            foreach(Slot slot in slots)
            {
                if (slot.letter != null)
                {
                    currentWord += slot.letter.ToLower();
                }
            

            }
            if (currentWord != "" && currentWord.Length > 1)
            {
                int index = Array.IndexOf(words, currentWord);
                if (index > 0)
                {
                   
                    correctWord = true;
                   
                    textColor = Color.Green;
                }
            }
        }
        public void SubmitWord()
        {
            if (correctWord == true)
            {


                score += currentWord.Length;
                for (int i = 0; i < tiles.Count; i++)
                {
                    if (tiles[i].inSlot == true)
                    {
                        world.RemoveBody(tiles[i].tileBody);
                        tiles.RemoveAt(i);
                    }
                }
                foreach (Slot slot in slots)
                {
                    slot.letter = "";
                    slot.full = false;
                }
            }
        }
        public void TileSlotCollision()
        {
            foreach(Tile tile in tiles)
            {
                if (tile.usable)
                {
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i].full != true&& tile.inSlot!=true)
                        {
                            if (tile.tileAnimation.hitBox.Intersects(slots[i].slotAnimation.hitBox)&& tile.timeSinceLastSlot>500)
                            {
                                slots[i].letter = tile.letter;
                                tile.inSlot = true;
                                slots[i].full = true;
                                tile.slotPostition = slots[i].position;
                                tile.slotNum = i;
                               
                               
                                
                            }
                        }
                    }
                }
            }
        }
        public void TileTileCollision()
        {
            
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        
                            if (tiles[i].inSlot == true&& tiles[i].slotNum!=-1)
                            {
                                tiles[i].tileBody.OnCollision += TileInSlotCollison;
                                 
                        }
                    }
                    
                
            
        }
        public bool TileInSlotCollison(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Tile tileA =null;
            Tile tileB=null;
            Vector2 contactPos = contact.Manifold.LocalPoint;
            Vector2 aVel = fixtureA.Body.GetLinearVelocityFromWorldPoint(contactPos);
            Vector2 bVel = fixtureB.Body.GetLinearVelocityFromWorldPoint(contactPos);
            Vector2 relVel = aVel - bVel;
            float force = relVel.Length() * fixtureA.Body.Mass * fixtureB.Body.Mass;
            if(force>20)
            {
                foreach(Tile tile in tiles)
                {
                   if(fixtureA.Equals(tile.tileBody.FixtureList[0]))
                   {
                       tileA = tile;
                   }
                   if (fixtureB.Equals(tile.tileBody.FixtureList[0]))
                   {
                       tileB = tile;
                   }
                }
                if(tileA !=null && tileB!=null)
                {
                if (tileA.inSlot == true&& tileB.inSlot == false&& tileB.timeSinceLastSlot>500)
                {
                    tileA.slotPostition = new Vector2(0, 0);
                    tileA.inSlot = false;
                    slots[tileA.slotNum].letter = "";
                    slots[tileA.slotNum].full = false;
                    tileA.slotNum = -1;

                }
                }
            }
            return true;
        }
        public void TileCleaner()
        {
           
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].tileAnimation.Position.Y > windowHeight)
                {
                    world.RemoveBody(tiles[i].tileBody);
                    tiles.RemoveAt(i);
                }
            }
        }
        public void TileInSlotClick()
        {

            if (mouseState.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released )
            {
                Vector2 position = ConvertUnits.ToSimUnits(mouseState.Position.ToVector2());
                 Fixture fixture = world.TestPoint(position);
                 if (fixture != null)
                 {
                     body = fixture.Body;
                     foreach (Tile tile in tiles)
                     {
                         if (tile.tileBody.Equals(body))
                         {
                             if (tile.inSlot == true)
                             {
                                 tile.slotPostition = new Vector2(0, 0);
                                 tile.inSlot = false;
                                 slots[tile.slotNum].letter = "";
                                 slots[tile.slotNum].full = false;
                                 tile.slotNum = -1;
                                 
                             }
                         }
                     }
                 }
            }

        }
        public void MouseDrag()
        {
           Vector2 position = ConvertUnits.ToSimUnits(mouseState.Position.ToVector2());

            if (mouseState.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {
                
                Fixture fixture = world.TestPoint(position);
                if (fixture != null)
                {
                    
                    body = fixture.Body;
                    if ( ConvertUnits.ToDisplayUnits(body.Position.Y) > 500)
                    {
                        mouseJoint = new FixedMouseJoint(body, position);
                        mouseJoint.MaxForce = 100 * body.Mass;
                        world.AddJoint(mouseJoint);
                        body.Awake = true;
                        foreach (Tile tile in tiles)
                        {
                            if (tile.tileBody.Equals(body))
                            {
                                tile.tileAnimation.color = Color.Green;
                            }
                        }
                    }
                }

            }
            if (body != null)
            {


                if (mouseState.LeftButton == ButtonState.Pressed && ConvertUnits.ToDisplayUnits(body.Position.Y) > 500)
                {
                    if (mouseJoint != null)
                    {
                        mouseJoint.WorldAnchorB = position;
                    }

                }

                if (mouseState.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed || ConvertUnits.ToDisplayUnits(body.Position.Y) < 500)
                {
                    if (mouseJoint != null)
                    {
                        foreach (Tile tile in tiles)
                        {
                            if (tile.tileBody.Equals(body))
                            {
                                tile.tileAnimation.color = Color.White;
                            }
                        }
                        world.RemoveJoint(mouseJoint);
                        mouseJoint = null;
                    }
                }
            }

        }
        Body body;
        MouseState oldMouse;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            oldMouse = mouseState;
            mouseState = Mouse.GetState();
            
            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            MouseDrag();
           foreach(Slot slot in slots)
           {
               slot.Update(gameTime);
           }
            foreach (Tile tile in tiles)
            {
                tile.Update(gameTime);
            }
            if(tiles.Count<30)
            {
                GenerateTiles(1);
            }
            TileInSlotClick();
            TileSlotCollision();
            TileTileCollision();
            TileCleaner();
            TestLetters();
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                SubmitWord();
            }
            base.Update(gameTime);
        }

        Color textColor;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            foreach (Slot slot in slots)
            {
                slot.Draw(spriteBatch);
            }
            spriteBatch.DrawString(wordFont, currentWord, new Vector2(200, 250),
               textColor, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(wordFont, score.ToString(), new Vector2(windowWidth-wordFont.MeasureString(score.ToString()).X, 10),
              textColor, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(lineTex,new Rectangle(0,500,windowWidth, 1), null,Color.Black,0,
            new Vector2(0, 0),SpriteEffects.None,0);

            foreach (Tile tile in tiles)
            {
                tile.Draw(spriteBatch);
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
