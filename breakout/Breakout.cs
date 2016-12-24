using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace breakout
{
    /* TODO LIST
     
     * embedded balls     
     * sound and music
     * level editor    
    
    */

    public class Breakout : Game
    {
        enum gamestate
        {
            MainMenu,Editor,WaitingStart,WaitingBall,BallLaunched,GameOver
        }
        enum textposition
        {
            TopLeft,TopMiddle,TopRight,Middle,BottomLeft,BottomMiddle,BottomRight
        }        

        gamestate currentstate;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font_arial;
        SpriteFont font_GoodDog;
        GameObject paddle;        
        int score;
               
        Level level;          
        Random r;

        // sounds
        SoundEffect paddlesound;
        SoundEffect[] blocksound;
        SoundEffect wallsound;
        Single fx_volume, fx_pitch, fx_pan;

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
        Texture2D blocktexture;
        Texture2D paddletexture;
        Texture2D pixel;

        public Breakout()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
           
            graphics.PreferredBackBufferWidth = 960;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 540;   // set this value to the desired height of your window            
            graphics.ApplyChanges();
            currentstate = gamestate.WaitingBall;
            score = 0;
            r = new Random();            
            level = new Level();
            level.number = 1;
            level.name = "Getting Started";
            level.blocks = new List<GameObject>();
            level.blocksremove = new List<GameObject>();
            level.balls = new List<GameObject>();
            level.ballsremove = new List<GameObject>();
            level.ballsleft = 3;

            blocksound = new SoundEffect[6];
            fx_volume = 0.0f;
            fx_pitch = 0.0f;
            fx_pan = 0.0f;
            
            base.Initialize();
        }
       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // load fonts
            font_arial=Content.Load<SpriteFont>("Fonts\\Arial");
            font_GoodDog = Content.Load<SpriteFont>("Fonts\\GoodDog");
            // load sounds
            paddlesound = Content.Load<SoundEffect>("Audio\\c6");
            
            blocksound[0] = Content.Load<SoundEffect>("Audio\\d7");
            blocksound[1] = Content.Load<SoundEffect>("Audio\\e7");
            blocksound[2] = Content.Load<SoundEffect>("Audio\\f7");
            blocksound[3] = Content.Load<SoundEffect>("Audio\\g7");
            blocksound[4] = Content.Load<SoundEffect>("Audio\\a6");
            blocksound[5] = Content.Load<SoundEffect>("Audio\\b6");

            wallsound = Content.Load<SoundEffect>("Audio\\c7");
            // load graphics
            LoadGraphics("sm");
            level.Create(blocktexture);
        }
        private void LoadGraphics(string ScreenSize)
        {
            background = Content.Load<Texture2D>("Graphics\\background_" + ScreenSize);
            paddletexture = Content.Load<Texture2D>("Graphics\\paddle_" + ScreenSize);
            Vector2 paddlePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            paddle = new GameObject(Color.White, paddletexture, paddlePosition);
            balltexture = Content.Load<Texture2D>("Graphics\\ball_" + ScreenSize);
            blocktexture = Content.Load<Texture2D>("Graphics\\block_" + ScreenSize);           
            pixel = Content.Load<Texture2D>("Graphics\\pixel");
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

            // Read the current state of keyboard,gamepad,mouse
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();

            // updates based on game state
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

            // Get keyboard input

            // fullscreen toggle
            if (currentKeyboardState.IsKeyDown(Keys.RightAlt) && currentKeyboardState.IsKeyDown(Keys.Enter))
            {
                //check the current screen mode
                if (graphics.IsFullScreen)
                {
                    //switch to windowed
                    graphics.PreferredBackBufferWidth = 960;
                    graphics.PreferredBackBufferHeight = 540;
                    graphics.ApplyChanges();
                    graphics.ToggleFullScreen();
                    LoadGraphics("sm");
                    level.Reload(blocktexture,balltexture, false);
                }
                else
                {
                    //switch to fullscreen
                    graphics.PreferredBackBufferWidth = 1920;
                    graphics.PreferredBackBufferHeight = 1080;
                    graphics.ApplyChanges();                   
                    graphics.ToggleFullScreen();
                    LoadGraphics("lg");
                    level.Reload(blocktexture,balltexture, true);
                }
            }
            // Switch to Main Menu
            if (currentKeyboardState.IsKeyDown(Keys.F1))
            {
                currentstate = gamestate.MainMenu;
            }
            // Switch to Level Editor
            if (currentKeyboardState.IsKeyDown(Keys.F2))
            {
                currentstate = gamestate.Editor;
            }
            // Switch to Game
            if (currentKeyboardState.IsKeyDown(Keys.F3))
            {
                currentstate = gamestate.WaitingBall;
            }            
            // Exit Program
            if (currentKeyboardState.IsKeyDown(Keys.Escape)) Exit();

            // Lock mouse to game window
            if (currentMouseState.X < 0) Mouse.SetPosition(0, currentMouseState.Y);
            if (currentMouseState.X > GraphicsDevice.Viewport.Width) Mouse.SetPosition(GraphicsDevice.Viewport.Width, currentMouseState.Y);
            if (currentMouseState.Y < 0) Mouse.SetPosition(currentMouseState.X, 0);
            if (currentMouseState.Y > GraphicsDevice.Viewport.Height) Mouse.SetPosition(currentMouseState.X, GraphicsDevice.Viewport.Height);

            // add ball on click
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (currentstate == gamestate.WaitingBall)
                {
                    level.balls.Add(new GameObject(Color.Yellow, balltexture, new Vector2(r.Next(0, GraphicsDevice.Viewport.Width), GraphicsDevice.Viewport.Height - 100), new Vector2(r.Next(-7, 7), r.Next(-7, -5))));
                    currentstate = gamestate.BallLaunched;
                }

                if (currentstate == gamestate.GameOver)
                {
                    level.ballsleft = 3;
                    score = 0;  
                    currentstate = gamestate.WaitingBall;
                    foreach(GameObject ball in level.balls)
                    {
                        level.ballsremove.Add(ball);
                    }
                    foreach (GameObject ball in level.ballsremove)
                    {
                        level.balls.Remove(ball);
                    }
                    foreach (GameObject block in level.blocks)
                    {
                        level.blocksremove.Add(block);
                    }
                    foreach (GameObject block in level.blocksremove)
                    {
                        level.blocks.Remove(block);
                    }
                    level.Create(blocktexture);
                }

            }
            base.Update(gameTime);
        }
          

        private void UpdateBalls(GameTime gameTime)
        {
            try
            {
                foreach (GameObject ball in level.balls)
                {
                    ball.Position.X += ball.Velocity.X;
                    ball.Position.Y += ball.Velocity.Y;
                    if (ball.Position.X > GraphicsDevice.Viewport.Width - ball.Texture.Width)
                    {
                        // bounce off right wall
                        wallsound.Play(fx_volume,fx_pitch,fx_pan);
                        ball.Velocity.X = -ball.Velocity.X;
                        ball.Position.X = GraphicsDevice.Viewport.Width - ball.Texture.Width;
                    }
                    if (ball.Position.X < 0)
                    {
                        // bounce off left wall
                        wallsound.Play(fx_volume, fx_pitch, fx_pan);
                        ball.Velocity.X = -ball.Velocity.X;
                        ball.Position.X = 0;
                    }
                    if (ball.Position.Y > GraphicsDevice.Viewport.Height - ball.Texture.Height)
                    {
                        // hit the floor
                        level.ballsremove.Add(ball);
                        continue;
                    }
                    if (ball.Position.Y < 0)
                    {
                        // bounce off top wall
                        wallsound.Play(fx_volume, fx_pitch, fx_pan);
                        ball.Velocity.Y = -ball.Velocity.Y;
                        ball.Position.Y = 0;
                    }
                    if ((ball.Position.Y + ball.Texture.Height > paddle.Position.Y) && (ball.Position.X + ball.Texture.Width > paddle.Position.X) && (ball.Position.X < paddle.Position.X + paddle.Texture.Width))
                    {
                        paddlesound.Play(fx_volume, fx_pitch, fx_pan);
                        ball.Position.Y = paddle.Position.Y - ball.Texture.Height;
                        ball.Velocity.Y = -ball.Velocity.Y;

                        // find center of ball and paddle
                        int ballcenter = (int)(ball.Position.X) + (ball.Texture.Width / 2);
                        int paddlecenter = (int)(paddle.Position.X) + (paddle.Texture.Width / 2);
                        int paddleside = ballcenter - paddlecenter;
                        ball.Velocity.X = paddleside / 2;                       

                        // increase horizontal ball speed
                        if (ball.Velocity.X < 0)
                        {
                            ball.Velocity.X -= 0.2f;
                        }
                        else
                        {
                            ball.Velocity.X += 0.2f;
                        }
                    }
                    // ball collisions with blocks
                    foreach (GameObject block in level.blocks)
                    {
                        if (ball.BoundingBox.Intersects(block.BoundingBox))
                        {
                            blocksound[block.sound].Play(fx_volume, fx_pitch, fx_pan);
                            if (ball.Velocity.Y < 0)
                            {                                
                                ball.Velocity.Y -= 0.01f;
                            }
                            else
                            {
                                ball.Velocity.Y += 0.01f;
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
                        // advance level
                        level.Create(blocktexture);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            foreach(GameObject ball in level.ballsremove)
            {
                level.balls.Remove(ball);
            }
            if (level.balls.Count == 0)
            {
                if (currentstate == gamestate.BallLaunched)
                {
                    level.ballsleft--;
                    currentstate = gamestate.WaitingBall;
                    if (level.ballsleft ==0)
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
            
        }
        /// <summary
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.Black);          
            
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            switch (currentstate)
            {
                case gamestate.MainMenu:
                    DrawText("Main Menu", textposition.TopMiddle, font_GoodDog, Color.DarkOliveGreen);
                    break;
                case gamestate.Editor:
                    DrawGrid(spriteBatch);
                    DrawText("Level Editor", textposition.TopMiddle,font_GoodDog, Color.DarkOliveGreen);                    
                    break;
                case gamestate.WaitingBall:
                    DrawText("Left Click to Launch Ball", textposition.Middle, font_GoodDog, Color.DarkOliveGreen);                   
                    paddle.Draw(spriteBatch);
                    DrawBlocks();
                    DrawGameInfo();
                    break;
                case gamestate.BallLaunched:                    
                    paddle.Draw(spriteBatch);
                    DrawBlocks();
                    // draw balls
                    foreach (GameObject ball in level.balls)
                    {
                        ball.Draw(spriteBatch);
                    }
                    DrawGameInfo();
                    break;
                case gamestate.GameOver:
                    DrawBlocks();                    
                    DrawText("Game Over",textposition.Middle, font_GoodDog, Color.Red);                    
                    DrawGameInfo();
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void DrawGrid(SpriteBatch spriteBatch)            
        {
            for(int x = 1; x < GraphicsDevice.Viewport.Width / blocktexture.Width; x++)
            {
                for(int y = 1; y< GraphicsDevice.Viewport.Height / blocktexture.Height; y++)
                {
                    Line l1 = new Line(new Vector2(x * blocktexture.Width, 0),new Vector2(x * blocktexture.Width, GraphicsDevice.Viewport.Height),1,Color.Gray, pixel);
                    Line l2 = new Line(new Vector2(0, y * blocktexture.Height), new Vector2(GraphicsDevice.Viewport.Width, y * blocktexture.Height), 1, Color.Gray, pixel);
                    l1.Update();
                    l1.Draw(spriteBatch);
                    l2.Update();
                    l2.Draw(spriteBatch);
                }
            }
        }
        private void DrawBlocks()
        {
            // draw blocks
            foreach (GameObject block in level.blocks)
            {
                block.Draw(spriteBatch);
            }
        }
        private void DrawGameInfo()
        {

            // draw game info
            DrawText("Balls Left : " + level.ballsleft, textposition.TopLeft,font_GoodDog,Color.Gray);
            DrawText("Level " + level.number + " - " + level.name, textposition.TopMiddle, font_arial, Color.Green);
            DrawText("Score : " + score,textposition.TopRight, font_GoodDog, Color.Gray);          
            // DrawText("Mouse Coords : " + currentMouseState.X + "," + currentMouseState.Y, textposition.BottomMiddle, font_GoodDog, Color.Gray);          
        }

        private void DrawText(string msg,textposition tp,SpriteFont font,Color color)
        {
            Vector2 textsize = font.MeasureString(msg);
            int w = GraphicsDevice.Viewport.Width;
            int h = GraphicsDevice.Viewport.Height;

            switch (tp)
            {
                case textposition.BottomLeft:
                    spriteBatch.DrawString(font, msg, new Vector2(5, h - textsize.Y), color);
                    break;
                case textposition.BottomMiddle:
                    spriteBatch.DrawString(font, msg, new Vector2((w / 2) - (textsize.X / 2), h - textsize.Y), color);
                    break;
                case textposition.BottomRight:
                    spriteBatch.DrawString(font, msg, new Vector2(w - (textsize.X + 5), h - (textsize.Y + 5)), color);
                    break;
                case textposition.Middle:
                    spriteBatch.DrawString(font, msg, new Vector2((w / 2) - (textsize.X / 2), (h / 2) - (textsize.Y / 2)), color);
                    break;
                case textposition.TopLeft:
                    spriteBatch.DrawString(font, msg, new Vector2(5, 5), color);
                    break;
                case textposition.TopMiddle:
                    spriteBatch.DrawString(font, msg, new Vector2((w / 2) - (textsize.X / 2), 5), color);
                    break;
                case textposition.TopRight:
                    spriteBatch.DrawString(font, msg, new Vector2(w - (textsize.X+5), 5), color);
                    break;
            }

        }
    }
}
