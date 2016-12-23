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
    
     * level editor 
     * embedded balls     
     * ball angle off paddle tweaks
     * sound and music
     *    
    */    
       
    public class Game1 : Game
    {
        enum gamestate
        {
            MainMenu,Editor,WaitingBall,BallLaunched,GameOver
        }
        gamestate currentstate;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont arial;
        GameObject paddle;        
        int score;

        List<GameObject> balls;
        List<GameObject> ballremove;

        int ballsleft;
        Level level;          
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
        Texture2D balltexture;

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
            currentstate = gamestate.WaitingBall;
            score = 0;
            r = new Random();
            balls = new List<GameObject>();
            ballremove = new List<GameObject>();
            ballsleft = 3;
            level = new Level();
            level.number = 1;
            level.name = "Getting Started";
            level.blocks = new List<GameObject>();
            level.blocksremove = new List<GameObject>();  

            //Enable the FreeDrag gesture.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;
            base.Initialize();
        }
       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            arial=Content.Load<SpriteFont>("Arial");
            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("Graphics\\background_sm");
            Texture2D paddleTexture = Content.Load<Texture2D>("Graphics\\paddle_sm");
            Vector2 paddlePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            paddle = new GameObject(Color.White,paddleTexture,paddlePosition);
            balltexture = Content.Load<Texture2D>("Graphics\\ball_sm");           
            Texture2D blockTexture = Content.Load<Texture2D>("Graphics\\block_sm");
            level.Create(blockTexture);
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

            switch(currentstate)
            {
                case gamestate.MainMenu:
                    break;
                case gamestate.Editor:
                    break;
                case gamestate.WaitingBall:
                    UpdatePaddle(gameTime);
                    break;
                case gamestate.BallLaunched:
                    UpdatePaddle(gameTime);
                    UpdateBalls(gameTime);
                    break;
                case gamestate.GameOver:
                    break;                
            }
            if (currentKeyboardState.IsKeyDown(Keys.F2))
            {
                currentstate = gamestate.Editor;
            }
            if (currentKeyboardState.IsKeyDown(Keys.F5))
            {
                currentstate = gamestate.WaitingBall;
            }
            if (currentKeyboardState.IsKeyDown(Keys.F1))
            {
                currentstate = gamestate.MainMenu;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Escape)) Exit();
            base.Update(gameTime);
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
                    if (ball.BoundingBox.Intersects(paddle.BoundingBox))
                    {
                        ball.Position.Y = paddle.Position.Y - ball.Texture.Height;
                        ball.Velocity.Y = -ball.Velocity.Y;

                        // find center of ball and paddle
                        int ballcenter = (int)(ball.Position.X) + (ball.Texture.Width / 2);
                        int paddlecenter = (int)(paddle.Position.X) + (paddle.Texture.Width / 2);
                        int paddleside = ballcenter - paddlecenter;

                        if (paddleside > 0)
                        {
                            // ball is on the right side of the paddle
                            if (ball.Velocity.X < 0)
                            {
                                // ball is traveling to the left, so reverse it
                                ball.Velocity.X = -ball.Velocity.X;
                            }
                        }
                        else
                        {
                            // ball is on the left side of the paddle
                            if (ball.Velocity.X > 0)
                            {
                                // ball is travelling to the right, so reverse it
                                ball.Velocity.X = -ball.Velocity.X;
                            }
                        }
                        // increase horizontal ball speed
                        if (ball.Velocity.X < 0)
                        {
                            ball.Velocity.X -= 0.1f;
                        }
                        else
                        {
                            ball.Velocity.X += 0.1f;
                        }
                    }
                    // ball collisions with blocks
                    foreach (GameObject block in level.blocks)
                    {
                        if (ball.BoundingBox.Intersects(block.BoundingBox))
                        {
                            if (ball.Velocity.Y < 0)
                            {                                
                                ball.Velocity.Y -= 0.1f;
                            }
                            else
                            {
                                ball.Velocity.Y += 0.1f;
                            }
                            ball.Velocity.Y = -ball.Velocity.Y;
                            level.blocksremove.Add(block);
                            score += block.scorevalue;
                            break;
                        }
                    }
                    foreach (GameObject block in level.blocksremove)
                    {
                        level.blocks.Remove(block);
                    }
                    if (level.blocks.Count == 0)
                    {
                        currentstate = gamestate.WaitingBall;
                        Texture2D blockTexture = Content.Load<Texture2D>("Graphics\\block_sm");
                        level.Create(blockTexture);
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
            if (balls.Count == 0)
            {
                if (currentstate == gamestate.BallLaunched)
                {
                    ballsleft--;
                    currentstate = gamestate.WaitingBall;
                    if (ballsleft==0)
                    {
                        currentstate = gamestate.GameOver;
                    }
                }               
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
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (currentstate == gamestate.WaitingBall)
                {
                    balls.Add(new GameObject(Color.Yellow, balltexture, new Vector2(r.Next(0, GraphicsDevice.Viewport.Width), GraphicsDevice.Viewport.Height - 100), new Vector2(r.Next(5, 7), r.Next(-7, -5))));
                    currentstate = gamestate.BallLaunched;
                }
                
            }
        }
        /// <summary
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.Black);
            string msg;
            Vector2 textSize;
            
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            switch (currentstate)
            {
                case gamestate.MainMenu:
                    msg = "Main Menu";
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), 0), Color.Yellow);
                    break;
                case gamestate.Editor:
                    msg = "Level Editor";
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), 0), Color.Yellow);
                    break;
                case gamestate.WaitingBall:
                    msg = "Left Click to Launch Ball";
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), (GraphicsDevice.Viewport.Height / 2) - (textSize.Y / 2)), Color.AliceBlue);
                    // draw paddle
                    paddle.Draw(spriteBatch);
                    //draw blocks
                    foreach (GameObject block in level.blocks)
                    {
                        block.Draw(spriteBatch);
                    }
                    // draw game info
                    spriteBatch.DrawString(arial, "Balls Left : " + ballsleft, new Vector2(0, 0), Color.Yellow);
                    msg = "Score : " + score;
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2(GraphicsDevice.Viewport.Width - textSize.X, 0), Color.Yellow);
                    msg = "Level " + level.number + " - " + level.name;
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), 0), Color.Yellow);
                    break;
                case gamestate.BallLaunched:
                    // draw paddle
                    paddle.Draw(spriteBatch);
                    // draw blocks
                    foreach (GameObject block in level.blocks)
                    {
                        block.Draw(spriteBatch);
                    }
                    // draw balls
                    foreach (GameObject ball in balls)
                    {
                        ball.Draw(spriteBatch);
                    }
                    // draw game info
                    spriteBatch.DrawString(arial, "Balls Left : " + ballsleft, new Vector2(0, 0), Color.Yellow);
                    msg = "Score : " + score;
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2(GraphicsDevice.Viewport.Width - textSize.X, 0), Color.Yellow);
                    msg = "Level " + level.number + " - " + level.name;
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), 0), Color.Yellow);
                    break;
                case gamestate.GameOver:
                    // draw blocks
                    foreach (GameObject block in level.blocks)
                    {
                        block.Draw(spriteBatch);
                    }
                    msg = "Game Over";
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), (GraphicsDevice.Viewport.Height / 2) - (textSize.Y / 2)), Color.AliceBlue);
                    // draw game info
                    spriteBatch.DrawString(arial, "Balls Left : " + ballsleft, new Vector2(0, 0), Color.Yellow);
                    msg = "Score : " + score;
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2(GraphicsDevice.Viewport.Width - textSize.X, 0), Color.Yellow);
                    msg = "Level " + level.number + " - " + level.name;
                    textSize = arial.MeasureString(msg);
                    spriteBatch.DrawString(arial, msg, new Vector2((GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), 0), Color.Yellow);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }        
    }
}
