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
     
     * level editor    
     * embedded balls     
     * sound and music     
    
    */

    public class Breakout : Game
    {
        enum gamestate
        {
            MainMenu,Editor,WaitingStart,WaitingBall,BallLaunched,GameOver
        }        
       
        gamestate currentstate;
        GraphicsDeviceManager graphics;        
        SpriteBatch spritebatch; 
               
        Level level;
        GameContent gamecontent;
        Editor editor;

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
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 540;
            graphics.ApplyChanges();
            spritebatch = new SpriteBatch(GraphicsDevice);
            gamecontent = new GameContent(Content.ServiceProvider);

            r = new Random();
            
            fx_volume = 0.2f;
            fx_pitch = 0.0f;
            fx_pan = 0.0f;

            editor = new Editor(gamecontent,spritebatch,GraphicsDevice.Viewport);
            currentstate = gamestate.Editor;

            score = 0;
            ballsleft = 5;
            level = new Level(1, "New Beginnings",GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height,gamecontent);
            level.Create();  

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
                    level.Reload(false);
                }
                else
                {
                    //switch to fullscreen
                    graphics.PreferredBackBufferWidth = 1920;
                    graphics.PreferredBackBufferHeight = 1080;
                    graphics.ApplyChanges();                   
                    graphics.ToggleFullScreen();                    
                    level.Reload(true);
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
            // if (currentMouseState.X < 0) Mouse.SetPosition(0, currentMouseState.Y);
            // if (currentMouseState.X > GraphicsDevice.Viewport.Width) Mouse.SetPosition(GraphicsDevice.Viewport.Width, currentMouseState.Y);
            // if (currentMouseState.Y < 0) Mouse.SetPosition(currentMouseState.X, 0);
            // if (currentMouseState.Y > GraphicsDevice.Viewport.Height) Mouse.SetPosition(currentMouseState.X, GraphicsDevice.Viewport.Height);

            // add ball on click
            if (currentMouseState.LeftButton == ButtonState.Pressed)
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
                    level.Create();
                }

            }
            base.Update(gameTime);
        }
          

        private void UpdateBalls(GameTime gameTime)
        {
            foreach (GameObject ball in level.balls)
            {
                ball.Position.X += ball.Velocity.X;
                ball.Position.Y += ball.Velocity.Y;
                Single fx_pan = ((ball.Position.X * 2) / GraphicsDevice.Viewport.Width) - 1;
                if (fx_pan < -1.0f) fx_pan = -1.0f;
                if (fx_pan > 1.0f) fx_pan = 1.0f;

                if (ball.Position.X > GraphicsDevice.Viewport.Width - ball.Texture.Width)
                {
                    // bounce off right wall                    
                    gamecontent.wallsound.Play(fx_volume,fx_pitch,fx_pan);
                    ball.Velocity.X = -ball.Velocity.X;
                    ball.Position.X = GraphicsDevice.Viewport.Width - ball.Texture.Width;
                }
                if (ball.Position.X < 0)
                {
                    // bounce off left wall
                    gamecontent.wallsound.Play(fx_volume, fx_pitch, fx_pan);
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
                    gamecontent.wallsound.Play(fx_volume, fx_pitch, fx_pan);
                    ball.Velocity.Y = -ball.Velocity.Y;
                    ball.Position.Y = 0;
                }
                if ((ball.Position.Y + ball.Texture.Height > level.paddle.Position.Y) && (ball.Position.X + ball.Texture.Width > level.paddle.Position.X) && (ball.Position.X < level.paddle.Position.X + level.paddle.Texture.Width))
                {                    
                    gamecontent.paddlesound.Play(fx_volume, fx_pitch, fx_pan);
                    ball.Position.Y = level.paddle.Position.Y - ball.Texture.Height;
                    ball.Velocity.Y = -ball.Velocity.Y;

                    // find center of ball and paddle
                    int ballcenter = (int)(ball.Position.X) + (ball.Texture.Width / 2);
                    int paddlecenter = (int)(level.paddle.Position.X) + (level.paddle.Texture.Width / 2);
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
                        Debug.Print("Panning is {0}", fx_pan);
                        gamecontent.blocksound[block.sound].Play(fx_volume, fx_pitch, fx_pan);
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
                    level.Create();
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
                    if (ballsleft ==0)
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
            level.paddle.Position.X = currentMouseState.X;
            // paddle.Position.Y = currentMouseState.Y;

            // Make sure that the paddle does not go out of bounds
            level.paddle.Position.X = MathHelper.Clamp(level.paddle.Position.X, 0, GraphicsDevice.Viewport.Width - level.paddle.Texture.Width);
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
            spritebatch.Draw(gamecontent.background_sm, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

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
