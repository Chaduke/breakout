using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace breakout
{
    /* TODO LIST
     * 
     * check on resizing blocks
     * sprite effects / colors
     * ball count
     * score
     * levels
     * embedded balls
     * increased ball speed
     * ball angle off paddle     
    */
    
       
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameObject paddle; 

        List<GameObject> balls;
        List<GameObject> ballremove;

        List<GameObject> blocks;
        List<GameObject> blockremove;
         
        Random r;        

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        //Mouse states used to track Mouse button press
        MouseState currentMouseState;
        MouseState previousMouseState;

        Texture2D background;        

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
            graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 600;   // set this value to the desired height of your window
            graphics.ApplyChanges();              
           
            r = new Random();
            balls = new List<GameObject>();
            ballremove = new List<GameObject>();

            blocks = new List<GameObject>();
            blockremove = new List<GameObject>();  

            //Enable the FreeDrag gesture.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;
            base.Initialize();
        }
       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("Graphics\\background_sm");
            Texture2D paddleTexture = Content.Load<Texture2D>("Graphics\\paddle_sm");
            Vector2 paddlePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            paddle = new GameObject(Color.White,paddleTexture,paddlePosition);
            GameObject ball = new GameObject(Color.Yellow,Content.Load<Texture2D>("Graphics\\ball_sm"), new Vector2(r.Next(0, GraphicsDevice.Viewport.Width), GraphicsDevice.Viewport.Height - 100), new Vector2(r.Next(5, 7), r.Next(-7,-5)));
            balls.Add(ball);
            Texture2D blockTexture = Content.Load<Texture2D>("Graphics\\block_sm");
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 16; col++)
                {
                    int x = (blockTexture.Width * col);
                    int y = 100 + (row * blockTexture.Height);
                    Color c = new Color();
                    switch (row)
                    {
                        case 0:
                            c = Color.Red;
                            break;
                        case 1:
                            c = Color.Turquoise;
                            break;
                        case 2:
                            c = Color.Wheat;
                            break;
                        case 3:
                            c = Color.YellowGreen;
                            break;
                        case 4:
                            c = Color.Salmon;
                            break;
                        case 5:
                            c = Color.PapayaWhip;
                            break;
                        case 6:
                            c = Color.MediumPurple;
                            break;
                        case 7:
                            c = Color.DarkOrchid;
                            break;
                        default:
                            c = Color.White;
                            break;
                    }             

                    GameObject block = new GameObject(c, blockTexture, new Vector2(x, y));
                    blocks.Add(block);
                }
            }  
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();

            //Update the player
            UpdatePaddle(gameTime);
            // Update the balls
            UpdateBalls(gameTime);

            CollisionCheck();
            if (currentKeyboardState.IsKeyDown(Keys.Escape)) Exit();
            base.Update(gameTime);
        }

        private void CollisionCheck()
        {
            
            foreach (GameObject ball in balls)
            {
                if (ball.BoundingBox.Intersects(paddle.BoundingBox))               
                {
                    ball.Position.Y = paddle.Position.Y - ball.Texture.Height;
                    ball.Velocity.Y = -ball.Velocity.Y;
                }
                foreach(GameObject block in blocks)
                {
                    if (ball.BoundingBox.Intersects(block.BoundingBox))
                    {
                        ball.Velocity.Y = -ball.Velocity.Y;
                        blockremove.Add(block);
                        break;
                    }
                }               
            }
            foreach (GameObject block in blockremove)
            {
                blocks.Remove(block);
            }
        }

        private void UpdateBalls(GameTime gameTime)
        {
            try
            {
                foreach (GameObject ball in balls)
                {
                    ball.Position.X += ball.Velocity.X;
                    ball.Position.Y += ball.Velocity.Y;
                    if (ball.Position.X > GraphicsDevice.Viewport.Width - ball.Texture.Width)
                    {
                        // bounce off right wall
                        ball.Velocity.X = -ball.Velocity.X;
                        ball.Position.X = GraphicsDevice.Viewport.Width - ball.Texture.Width;
                    }
                    if (ball.Position.X < 0)
                    {
                        // bounce off left wall
                        ball.Velocity.X = -ball.Velocity.X;
                        ball.Position.X = 0;
                    }
                    if (ball.Position.Y > GraphicsDevice.Viewport.Height - ball.Texture.Height)
                    {
                        // hit the floor
                        ballremove.Add(ball);
                        continue;
                    }
                    if (ball.Position.Y < 0)
                    {
                        // bounce off top wall
                        ball.Velocity.Y = -ball.Velocity.Y;
                        ball.Position.Y = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            foreach(GameObject ball in ballremove)
            {
                balls.Remove(ball);
            }
        }                

        private void UpdatePaddle(GameTime gameTime)
        {
            // Get Thumbstick Controls
            // paddle.Position.X += currentGamePadState.ThumbSticks.Left.X * paddleMoveSpeed;
            // paddle.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * paddleMoveSpeed;

            // Use the Keyboard / Dpad
            /*
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                paddle.Position.X -= paddleMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                paddle.Position.X += paddleMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                paddle.Position.Y -= paddleMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                paddle.Position.Y += paddleMoveSpeed;
            }
                        
            */
            paddle.Position.X = currentMouseState.X;
            // paddle.Position.Y = currentMouseState.Y;

            // Make sure that the paddle does not go out of bounds
            paddle.Position.X = MathHelper.Clamp(paddle.Position.X, 0, GraphicsDevice.Viewport.Width - paddle.Texture.Width);
            // paddle.Position.Y = MathHelper.Clamp(paddle.Position.Y, GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height / 4), GraphicsDevice.Viewport.Height  - (paddle.Texture.Height * 2));

        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0,0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            paddle.Draw(spriteBatch);
            foreach (GameObject ball in balls)
            {
                ball.Draw(spriteBatch);
            }
            foreach (GameObject block in blocks)
            {
                block.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
