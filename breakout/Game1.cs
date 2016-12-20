using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace breakout
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Paddle paddle;       
        List<Ball> balls;
        List<Ball> ballremove;

        List<Block> blocks;
        List<Block> blockremove;

        int ballcount;
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

        // A movement speed for the paddle
        float paddleMoveSpeed;

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

            
            paddle = new Paddle();
            paddleMoveSpeed = 8.0f;
            ballcount = 5;
            r = new Random();
            balls = new List<Ball>();
            ballremove = new List<Ball>();

            blocks = new List<Block>();
            blockremove = new List<Block>();

            int i=0;
            while (i < ballcount)
            {
                Ball b = new Ball();                            
                b.Speed.X = r.Next(3, 7);
                b.Speed.Y = r.Next(3, 7);
                b.Position.X = r.Next(0, GraphicsDevice.Viewport.Width);
                b.Position.Y = 0;
                balls.Add(b);
                i++;         
            }
            i = 0;
            while (i < 7)
            {
                Block b = new Block();
                b.Position.X = (112 * i) + 10;
                b.Position.Y = 100;
                blocks.Add(b);
                i ++;
            }


            //Enable the FreeDrag gesture.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("Graphics\\background");
            Vector2 paddlePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            paddle.Initialize(Content.Load<Texture2D>("Graphics\\paddle_3d"), paddlePosition);
            foreach (Ball b in balls)
            {
                b.Initialize(Content.Load<Texture2D>("Graphics\\ball_3d"));
            }
            foreach (Block b in blocks)
            {
                b.Initialize(Content.Load<Texture2D>("Graphics\\block_3d"));
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
            
            foreach (Ball b in balls)
            {
                if (b.Position.X + b.Texture.Width > paddle.Position.X && b.Position.X < paddle.Position.X + paddle.Texture.Width &&
                    b.Position.Y > paddle.Position.Y - b.Texture.Height)
                {
                    b.Position.Y = paddle.Position.Y - b.Texture.Height;
                    b.Speed.Y = -b.Speed.Y;
                }
            }
        }

        private void UpdateBalls(GameTime gameTime)
        {
            try
            {
                foreach (Ball b in balls)
                {
                    b.Position.X += b.Speed.X;
                    b.Position.Y += b.Speed.Y;
                    if (b.Position.X > GraphicsDevice.Viewport.Width - b.Texture.Width)
                    {
                        // bounce off right wall
                        b.Speed.X = -b.Speed.X;
                        b.Position.X = GraphicsDevice.Viewport.Width - b.Texture.Width;
                    }
                    if (b.Position.X < 0)
                    {
                        // bounce off left wall
                        b.Speed.X = -b.Speed.X;
                        b.Position.X = 0;
                    }
                    if (b.Position.Y > GraphicsDevice.Viewport.Height - b.Texture.Height)
                    {
                        // hit the floor
                        ballremove.Add(b);
                        continue;
                    }
                    if (b.Position.Y < 0)
                    {
                        // bounce off top wall
                        b.Speed.Y = -b.Speed.Y;
                        b.Position.Y = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            foreach(Ball b in ballremove)
            {
                balls.Remove(b);
            }
        }                

        private void UpdatePaddle(GameTime gameTime)
        {
            // Get Thumbstick Controls
            paddle.Position.X += currentGamePadState.ThumbSticks.Left.X * paddleMoveSpeed;
            paddle.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * paddleMoveSpeed;

            // Use the Keyboard / Dpad
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

            paddle.Position.X = currentMouseState.X;
            // paddle.Position.Y = currentMouseState.Y;

            // Make sure that the paddle does not go out of bounds
            paddle.Position.X = MathHelper.Clamp(paddle.Position.X, 0, GraphicsDevice.Viewport.Width - paddle.Texture.Width);
            paddle.Position.Y = MathHelper.Clamp(paddle.Position.Y, GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height / 4), GraphicsDevice.Viewport.Height  - (paddle.Texture.Height * 2));

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
            foreach (Ball b in balls)
            {
                b.Draw(spriteBatch);
            }
            foreach (Block b in blocks)
            {
                b.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
