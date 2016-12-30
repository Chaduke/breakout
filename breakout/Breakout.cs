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
     
     * level editor - level name input, ball placement (trapped balls)
     * assign score and other properties to blocks
     * powerup objects - extra balls, sticky paddle, shooting paddle
     * moving blocks, different block sizes, rotation, animation
     * gravity
     * sound and music themes, get sounds on blocks working with load / save, get pan working
     * sounds for different game events
     * screen effects for game events
     * write basic music for background on levels
     * optional backgrounds for levels
     * main menu - story mode, level editor, options
     * game icon
    
    */

    public class Breakout : Game
    {
        enum gamestate
        {
            MainMenu,Editor,WaitingStart,WaitingBall,BallLaunched,GameOver
        }        
       
        gamestate currentstate;
        GraphicsDeviceManager graphics;
        public bool wide;
        public bool full;
        SpriteBatch spritebatch; 
               
        public Level level;
        GameContent gamecontent;
        public Editor editor;

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

        Random r;
        int score;
        int ballsleft;
              
        public Breakout()
        {
            graphics = new GraphicsDeviceManager(this);            
        }
        
        protected override void Initialize()
        {
            int currentwidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            
            if (currentwidth == 1920)
            {
                wide = true;
            }
            else
            {
                wide = false;
            }
            // start in windowed mode
            full = false;
            if (wide)
            {
                graphics.PreferredBackBufferWidth = 960;
                graphics.PreferredBackBufferHeight = 540;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 640;
                graphics.PreferredBackBufferHeight = 512;
            }           
            graphics.ApplyChanges();

            spritebatch = new SpriteBatch(GraphicsDevice);
            gamecontent = new GameContent(Content.ServiceProvider,wide);

            r = new Random();
            
            fx_volume = 0.2f;
            fx_pitch = 0.0f;
            fx_pan = 0.0f;

            editor = new Editor(gamecontent,spritebatch,GraphicsDevice.Viewport,wide,full);
            currentstate = gamestate.Editor;

            score = 0;
            ballsleft = 5;
            level = new Level(0,"",GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height,wide,full,gamecontent);
            level.Load();  

            base.Initialize();
        }
        
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
                    editor.Update(currentMouseState);
                    if (editor.level.loadmain)
                    {
                        level = editor.level;
                        editor.level.loadmain = false;
                        currentstate = gamestate.WaitingBall;
                        previousMouseState = currentMouseState;
                        level.ClearBalls();
                    }
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
            if (currentKeyboardState.IsKeyDown(Keys.RightAlt) && currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                //check the current screen mode
                if (graphics.IsFullScreen)
                {

                    //switch to windowed
                    // 16:9 ratio 960:540
                    // 4:3 ratio 640:512
                    full = false;
                    if (wide)
                    {
                        graphics.PreferredBackBufferWidth = 960;
                        graphics.PreferredBackBufferHeight = 540;
                        level.width = 960;
                        level.height = 540;
                    }
                    else
                    {
                        graphics.PreferredBackBufferWidth = 640;
                        graphics.PreferredBackBufferHeight = 512;
                        level.width = 640;
                        level.height = 512;
                    }                      
                }
                else
                {
                    //switch to fullscreen
                    // 16:9 ratio 1920:1080
                    // 4:3 ratio 1280:1024
                    full = true;
                    if (wide)
                    {
                        graphics.PreferredBackBufferWidth = 1920;
                        graphics.PreferredBackBufferHeight = 1080;
                        level.width = 1920;
                        level.height = 1080;
                    }
                    else
                    {
                        graphics.PreferredBackBufferWidth =  1280;
                        graphics.PreferredBackBufferHeight = 1024;
                        level.width = 1280;
                        level.height = 1024;
                    }                   
                }
                level.ChangeResolution(full);
                editor.ChangeResolution(full);
                graphics.ApplyChanges();
                graphics.ToggleFullScreen();
                editor.viewport = GraphicsDevice.Viewport;                
            }
            // Switch to Main Menu
            if (currentKeyboardState.IsKeyDown(Keys.F1) && previousKeyboardState.IsKeyUp(Keys.F1))
            {
                currentstate = gamestate.MainMenu;
            }
            // Switch to Level Editor
            if (currentKeyboardState.IsKeyDown(Keys.F2) && previousKeyboardState.IsKeyUp(Keys.F2))
            {
                currentstate = gamestate.Editor;
            }
            // Switch to Game
            if (currentKeyboardState.IsKeyDown(Keys.F3) && previousKeyboardState.IsKeyUp(Keys.F3))
            {
                currentstate = gamestate.WaitingBall;
            }            
            // Exit Program
            if (currentKeyboardState.IsKeyDown(Keys.Escape)) Exit();

            // Lock mouse to game window
            // if (currentMouseState.X < 0) Mouse.SetPosition(0, currentMouseState.Y);
            // if (currentMouseState.X > GraphicsDevice.Viewport.Width) Mouse.SetPosition(GraphicsDevice.Viewport.Width, currentMouseState.Y);
            // if (currentMouseState.Y < 0) Mouse.SetPosition(currentMouseState.X, 0);
            // if (currentMouseState.Y > GraphicsDevice.Viewport.Height) Mouse.SetPosition(currentMouseState.X, GraphicsDevice.Viewport.Height);

            // add ball on click
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                if (currentstate == gamestate.WaitingBall)
                {
                    if (graphics.IsFullScreen)
                    {
                        level.balls.Add(new GameObject(Color.Yellow, gamecontent.ball_lg,
                            new Vector2(r.Next(0, GraphicsDevice.Viewport.Width),
                            GraphicsDevice.Viewport.Height - 100), new Vector2(r.Next(-7, 7), r.Next(-7, -5))));
                    }
                    else
                    {
                        level.balls.Add(new GameObject(Color.Yellow, gamecontent.ball_sm,
                            new Vector2(r.Next(0, GraphicsDevice.Viewport.Width),
                            GraphicsDevice.Viewport.Height - 100), new Vector2(r.Next(-7, 7), r.Next(-7, -5))));
                    }
                        
                    currentstate = gamestate.BallLaunched;
                }

                if (currentstate == gamestate.GameOver)
                {
                    ballsleft = 5;
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
                    level.Load();
                }

            }
            base.Update(gameTime);
        }
          

        private void UpdateBalls(GameTime gameTime)
        {
            foreach (GameObject ball in level.balls)
            {
                ball.position.X += ball.velocity.X;
                ball.position.Y += ball.velocity.Y;
                fx_pan = ((ball.position.X * 2) / GraphicsDevice.Viewport.Width) - 1;
                if (fx_pan < -1.0f) fx_pan = -1.0f;
                if (fx_pan > 1.0f) fx_pan = 1.0f;

                if (ball.position.X > GraphicsDevice.Viewport.Width - ball.texture.Width)
                {
                    // bounce off right wall                    
                    gamecontent.wallsound.Play(fx_volume,fx_pitch,fx_pan);
                    ball.velocity.X = -ball.velocity.X;
                    ball.position.X = GraphicsDevice.Viewport.Width - ball.texture.Width;
                }
                if (ball.position.X < 0)
                {
                    // bounce off left wall
                    gamecontent.wallsound.Play(fx_volume, fx_pitch, fx_pan);
                    ball.velocity.X = -ball.velocity.X;
                    ball.position.X = 0;
                }
                if (ball.position.Y > GraphicsDevice.Viewport.Height - ball.texture.Height)
                {
                    // hit the floor
                    level.ballsremove.Add(ball);
                    continue;
                }
                if (ball.position.Y < 0)
                {
                    // bounce off top wall
                    gamecontent.wallsound.Play(fx_volume, fx_pitch, fx_pan);
                    ball.velocity.Y = -ball.velocity.Y;
                    ball.position.Y = 0;
                }
                if ((ball.position.Y + ball.texture.Height > level.paddle.position.Y) && (ball.position.X + ball.texture.Width > level.paddle.position.X) && (ball.position.X < level.paddle.position.X + level.paddle.texture.Width))
                {                    
                    gamecontent.paddlesound.Play(fx_volume, fx_pitch, fx_pan);
                    ball.position.Y = level.paddle.position.Y - ball.texture.Height;
                    ball.velocity.Y = -ball.velocity.Y;

                    // find center of ball and paddle
                    int ballcenter = (int)(ball.position.X) + (ball.texture.Width / 2);
                    int paddlecenter = (int)(level.paddle.position.X) + (level.paddle.texture.Width / 2);
                    int paddleside = ballcenter - paddlecenter;
                    ball.velocity.X = paddleside / 2;                       

                    // increase horizontal ball speed
                    if (ball.velocity.X < 0)
                    {
                        ball.velocity.X -= 0.2f;
                    }
                    else
                    {
                        ball.velocity.X += 0.2f;
                    }
                }
                // ball collisions with blocks
                foreach (GameObject block in level.blocks)
                {
                    if (ball.BoundingBox.Intersects(block.BoundingBox))
                    {
                        // Debug.Print("Panning is {0}", fx_pan);
                        gamecontent.blocksound[block.sound].Play(fx_volume, fx_pitch, fx_pan);
                        if (ball.velocity.Y < 0)
                        {                                
                            ball.velocity.Y -= 0.01f;
                        }
                        else
                        {
                            ball.velocity.Y += 0.01f;
                        }
                        ball.velocity.Y = -ball.velocity.Y;
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
                    level.Load();
                }
            }  

            foreach(GameObject ball in level.ballsremove)
            {
                level.balls.Remove(ball);
            }
            if (level.balls.Count == 0)
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
            level.paddle.position.X = currentMouseState.X;
            // paddle.Position.Y = currentMouseState.Y;

            // Make sure that the paddle does not go out of bounds
            level.paddle.position.X = MathHelper.Clamp(level.paddle.position.X, 0, GraphicsDevice.Viewport.Width - level.paddle.texture.Width);
            // paddle.Position.Y = MathHelper.Clamp(paddle.Position.Y, GraphicsDevice.Viewport.Height - (GraphicsDevice.Viewport.Height / 4), GraphicsDevice.Viewport.Height  - (paddle.Texture.Height * 2));
            
        }
        /// <summary
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.Black);          
            
            spritebatch.Begin();
            spritebatch.Draw(level.background, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            switch (currentstate)
            {
                case gamestate.MainMenu:
                    GameContent.DrawText("Main Menu", Color.MonoGameOrange,GameContent.textposition.TopMiddle, gamecontent.font_GoodDog,spritebatch,GraphicsDevice.Viewport);
                    break;
                case gamestate.Editor:
                    editor.Draw();                    
                    break;
                case gamestate.WaitingBall:
                    GameContent.DrawText("Left Click to Launch Ball", Color.MonoGameOrange, GameContent.textposition.Middle, gamecontent.font_GoodDog,spritebatch, GraphicsDevice.Viewport);                   
                    level.paddle.Draw(spritebatch);
                    DrawBlocks();
                    DrawGameInfo();
                    break;
                case gamestate.BallLaunched:                    
                    level.paddle.Draw(spritebatch);
                    DrawBlocks();
                    // draw balls
                    foreach (GameObject ball in level.balls)
                    {
                        ball.Draw(spritebatch);
                    }
                    DrawGameInfo();
                    break;
                case gamestate.GameOver:
                    DrawBlocks();
                    GameContent.DrawText("Game Over", Color.Red, GameContent.textposition.Middle, gamecontent.font_GoodDog, spritebatch, GraphicsDevice.Viewport);                    
                    DrawGameInfo();
                    break;
            }
            spritebatch.End();
            base.Draw(gameTime);
        }
        
        private void DrawBlocks()
        {
            // draw blocks
            foreach (GameObject block in level.blocks)
            {
                block.Draw(spritebatch);
            }
        }
        private void DrawGameInfo()
        {
            // draw game info
            GameContent.DrawText("Score : " + score, Color.Gray, GameContent.textposition.TopLeft, gamecontent.font_GoodDog, spritebatch, GraphicsDevice.Viewport);
            GameContent.DrawText("Balls Left : " + ballsleft, Color.Gray, GameContent.textposition.TopRight,gamecontent.font_GoodDog, spritebatch, GraphicsDevice.Viewport);
            GameContent.DrawText("Level " + level.number + " - " + level.name, Color.Green, GameContent.textposition.TopMiddle, gamecontent.font_arial, spritebatch, GraphicsDevice.Viewport); 
        }

        
    }
}
