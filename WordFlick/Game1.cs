using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics;
namespace WordFlick
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        MouseState mouseState;
        FixedMouseJoint mouseJoint;
        World world;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D lineTex;
        List<Tile> tiles;
        SpriteFont tileFont;
        Texture2D tileSprite;
        Random random;
        static public int windowHeight = 800;
        static public int windowWidth = 600;
        float width = ConvertUnits.ToSimUnits(windowWidth);

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
             this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;   
            graphics.ApplyChanges();
            tiles = new List<Tile>();
            random = new Random();
            base.Initialize();
            GenerateTiles(30);
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

            spriteBatch = new SpriteBatch(GraphicsDevice);
            tileSprite = Content.Load<Texture2D>("Tile");
            tileFont = Content.Load<SpriteFont>("Arial");
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
               
                int xPos = random.Next(tileSprite.Width / 2, windowWidth - tileSprite.Width / 2);
                int yPos = random.Next(tileSprite.Height*3);
                Body tileBody = BodyFactory.CreateRectangle(world,  ConvertUnits.ToSimUnits(tileSprite.Width-1),
                    ConvertUnits.ToSimUnits(tileSprite.Height-1), 5.0f);
                tileBody.BodyType = BodyType.Dynamic;
                tileBody.Position = ConvertUnits.ToSimUnits(xPos, yPos);
                Tile tile = new Tile(tileSprite, GetLetter().ToString(), tileFont, tileBody);
                tiles.Add(tile);
            }
        }
       

        public void MouseDrag()
        {
          

        }
        Body body;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState oldMouse = mouseState;
            mouseState = Mouse.GetState();
           
            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            Vector2 position = ConvertUnits.ToSimUnits(mouseState.Position.ToVector2());

            if (mouseState.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && mouseState.Position.Y > 500)
            {
                
                Fixture fixture = world.TestPoint(position);
                if (fixture != null)
                {
                    body = fixture.Body;
                    mouseJoint = new FixedMouseJoint(body, position);
                    mouseJoint.MaxForce = 100 * body.Mass;
                    world.AddJoint(mouseJoint);
                    body.Awake = true;
                    foreach(Tile tile in tiles)
                    {
                        if(tile.tileBody.Equals(body))
                        {
                            tile.tileAnimation.color = Color.Green;
                        }
                    }
                }

            }
            if (mouseState.LeftButton == ButtonState.Pressed && mouseState.Position.Y > 500)
            {
                if (mouseJoint != null)
                {
                    mouseJoint.WorldAnchorB = position;
                }
                
            }
           
            if (mouseState.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed||mouseState.Position.Y <500)
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
          
            foreach (Tile tile in tiles)
            {
                tile.Update(gameTime);
            }
           
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            foreach (Tile tile in tiles)
            {
                tile.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
