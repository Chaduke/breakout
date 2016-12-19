using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;

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
        Ball[] balls;
        int ballcount;
        Random r;
        public const int maxballs = 10;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        //Mouse states used to track Mouse button press
        MouseState currentMouseState;
        MouseState previousMouseState;

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
            paddle = new Paddle();
            paddleMoveSpeed = 8.0f;
            ballcount = 5;
            r = new Random();
            balls = new Ball[maxballs];

            for (int i = 0; i < ballcount; i++)
            {
                balls[i] = new Ball();               
                balls[i].Speed.X = r.Next(1, 3);
                balls[i].Speed.Y = r.Next(1, 3);
                balls[i].Position.X = r.Next(0, GraphicsDevice.Viewport.Width);
                balls[i].Position.Y = r.Next(0, GraphicsDevice.Viewport.Height);                
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
            Vector2 paddlePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            paddle.Initialize(Content.Load<Texture2D>("Graphics\\paddle1"), paddlePosition);
            for (int i = 0; i < ballcount; i++)
            {
                balls[i].Initialize(Content.Load<Texture2D>("Graphics\\ball1"));
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
            for (int i = 0; i < ballcount; i++)
            {
                if (balls[i].Position.X + balls[i].Texture.Width > paddle.Position.X && balls[i].Position.X < paddle.Position.X + paddle.Texture.Width &&
                    balls[i].Position.Y > paddle.Position.Y - balls[i].Texture.Height)
                {
                    balls[i].Position.Y = paddle.Position.Y - balls[i].Texture.Height;
                    balls[i].Speed.Y = -balls[i].Speed.Y;
                }
            }
        }

        private void UpdateBalls(GameTime gameTime)
        {
            for (int i = 0; i < ballcount; i++)
            {
                balls[i].Position.X += balls[i].Speed.X;
                balls[i].Position.Y += balls[i].Speed.Y;
                if (balls[i].Position.X > GraphicsDevice.Viewport.Width - balls[i].Texture.Width)
                {
                    // bounce off right wall
                    balls[i].Speed.X = -balls[i].Speed.X;
                    balls[i].Position.X = GraphicsDevice.Viewport.Width - balls[i].Texture.Width;
                }
                if (balls[i].Position.X < 0)
                {
                    // bounce off left wall
                    balls[i].Speed.X = -balls[i].Speed.X;
                    balls[i].Position.X = 0;
                }
                if (balls[i].Position.Y > GraphicsDevice.Viewport.Height - balls[i].Texture.Height)
                {
                    // hit the floor
                    balls[i].Position.Y = 0;
                }
                if (balls[i].Position.Y < 0)
                {
                    // bounce off top wall
                    balls[i].Speed.Y = -balls[i].Speed.Y;
                    balls[i].Position.Y = 0;
                }
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

            // paddle.Position.X = currentMouseState.X;
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            paddle.Draw(spriteBatch);
            for (int i = 0; i < ballcount; i++)
            {
                balls[i].Draw(spriteBatch);
            }            
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
