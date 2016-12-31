using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

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
        public int vwidth;
        public int vheight;

        public const bool complex_collisions = false;

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
        int ballcount;
        GameObject newball;      
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
                vwidth = 960;
                vheight = 540;
                graphics.PreferredBackBufferWidth = vwidth;
                graphics.PreferredBackBufferHeight = vheight;
            }
            else
            {
                vwidth = 640;
                vheight = 512;
                graphics.PreferredBackBufferWidth = vwidth;
                graphics.PreferredBackBufferHeight = vheight;
            }           
            graphics.ApplyChanges();

            spritebatch = new SpriteBatch(GraphicsDevice);
            gamecontent = new GameContent(Content.ServiceProvider,wide);

            r = new Random();
            
            fx_volume = 0.0f;
            fx_pitch = -1.0f;
            fx_pan = 0.0f;

            editor = new Editor(gamecontent,spritebatch,vwidth,vheight,wide,full);
            currentstate = gamestate.WaitingBall;

            score = 0;
            ballsleft = 5;
            level = new Level(0,"Game Level",vwidth,vheight,wide,full,gamecontent);
            level.Load();

            MediaPlayer.Play(gamecontent.music);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = fx_volume;
           
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
                    if (editor.testlevel)
                    {                       
                        System.Diagnostics.Debug.Print("Testing started, editor fullscreen = " + editor.full + ", level fullscreen = " + level.full + ",editor.level fullscreen = " + editor.level.full);
                        editor.testlevel = false;
                        currentstate = gamestate.WaitingBall;
                        previousMouseState = currentMouseState;
                        level.number = editor.level.number;
                        level.Load();          
                    }
                    break;
                case gamestate.WaitingBall:
                    UpdatePaddle(gameTime);
                    UpdateBalls(gameTime);
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
            // things to change
            // full variable in main, level, editor, editor.level            
            // textures for background,blocks,paddle and balls in level and editor.level
            // positions for blocks, paddle and balls in level and editor.level
            // textures for cursor in level and editor.level

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
                        ChangeResolution(960, 540);
                    }
                    else
                    {
                        ChangeResolution(640, 512);
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
                        ChangeResolution(1920, 1080);
                    }
                    else
                    {
                        ChangeResolution(1280, 1024);
                    }                   
                }
                level.ChangeResolution(full,vwidth,vheight);               
                editor.ChangeResolution(full,vwidth,vheight);
                graphics.ApplyChanges();
                graphics.ToggleFullScreen();
                             
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
            // if (currentMouseState.X > vwidth) Mouse.SetPosition(vwidth, currentMouseState.Y);
            // if (currentMouseState.Y < 0) Mouse.SetPosition(currentMouseState.X, 0);
            // if (currentMouseState.Y > vheight) Mouse.SetPosition(currentMouseState.X, vheight);

            // add ball on click
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                if (currentstate == gamestate.WaitingBall)
                {
                    if (graphics.IsFullScreen)
                    {
                        newball = new GameObject(Color.Yellow, gamecontent.ball_lg,
                            new Vector2(r.Next(0, vwidth), vheight - gamecontent.ball_lg.Height * 5), new Vector2(r.Next(-7, 7), r.Next(-15, -10)));
                    }
                    else
                    {
                       newball = new GameObject(Color.Yellow, gamecontent.ball_sm,
                            new Vector2(r.Next(0, vwidth), vheight - gamecontent.ball_sm.Height * 5), new Vector2(r.Next(-7, 7), r.Next(-5, -3)));
                        
                    }
                    newball.collide = true;
                    level.balls.Add(newball);
                    currentstate = gamestate.BallLaunched;
                }

                if (currentstate == gamestate.GameOver)
                {
                    ballsleft = 5;
                    score = 0;  
                    currentstate = gamestate.WaitingBall;                   
                    level.Load();
                }

            }
            base.Update(gameTime);
        }
        private void ChangeResolution(int w,int h)
        {
            vwidth = w;
            vheight = h;
            graphics.PreferredBackBufferWidth = vwidth;
            graphics.PreferredBackBufferHeight = vheight;
        }  

        private void UpdateBalls(GameTime gameTime)
        {
            float ball_left, ball_right, ball_top, ball_bottom;
            float block_left, block_right, block_top, block_bottom;
            float o_left, o_right, o_top, o_bottom;

            foreach (GameObject ball in level.balls)
            {
                ball.position.X += ball.velocity.X;
                ball.position.Y += ball.velocity.Y;
                fx_pan = ((ball.position.X * 2) / vwidth) - 1;
                if (fx_pan < -1.0f) fx_pan = -1.0f;
                if (fx_pan > 1.0f) fx_pan = 1.0f;

                if (ball.position.X > vwidth - ball.texture.Width)
                {
                    // bounce off right wall                    
                    gamecontent.wallsound.Play(fx_volume,fx_pitch,fx_pan);
                    ball.velocity.X = -ball.velocity.X;
                    ball.position.X = vwidth - ball.texture.Width;
                    ball.collision_point = ball.position;
                    ball.collide = true;
                }
                if (ball.position.X < 0)
                {
                    // bounce off left wall
                    gamecontent.wallsound.Play(fx_volume, fx_pitch, fx_pan);
                    ball.velocity.X = -ball.velocity.X;
                    ball.position.X = 0;
                    ball.collision_point = ball.position;
                    ball.collide = true;
                }
                if (ball.position.Y < 0)
                {
                    // bounce off top wall
                    gamecontent.wallsound.Play(fx_volume, fx_pitch, fx_pan);
                    ball.velocity.Y = -ball.velocity.Y;
                    ball.position.Y = 0;
                    ball.collision_point = ball.position;
                    ball.collide = true;
                }
                if (ball.position.Y > vheight - ball.texture.Height)
                {
                    // lost ball
                    level.ballsremove.Add(ball);
                    continue;
                }
                ////////////////////////////////
                // ball collision with paddle //
                ////////////////////////////////
                if ((ball.position.Y + ball.texture.Height > level.paddle.position.Y) && 
                    (ball.position.X + ball.texture.Width > level.paddle.position.X) && 
                    (ball.position.X < level.paddle.position.X + level.paddle.texture.Width))
                {                    
                    gamecontent.paddlesound.Play(fx_volume, fx_pitch, fx_pan);
                    ball.position.Y = level.paddle.position.Y - ball.texture.Height;
                    ball.velocity.Y = -ball.velocity.Y;
                    ball.collision_point = ball.position;

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
                    ball.collide = true;
                }
                /////////////////////////////////
                // ball collisions with blocks //
                /////////////////////////////////
                foreach (GameObject block in level.blocks)
                {
                    if (ball.BoundingBox.Intersects(block.BoundingBox))
                    {
                        // find where ball collided with block
                        ball_left = ball.position.X;
                        ball_right = ball.position.X + ball.texture.Width;
                        ball_top = ball.position.Y;
                        ball_bottom = ball.position.Y + ball.texture.Height;

                        block_left = block.position.X;
                        block_right = block.position.X + block.texture.Width;
                        block_top = block.position.Y;
                        block_bottom = block.position.Y + block.texture.Height;

                        o_left = ball_right - block_left;
                        o_right = block_right - ball_left;
                        o_top = ball_bottom - block_top;
                        o_bottom = block_bottom - ball_top;                        

                        if (!ball.collide)
                        {
                            // do basic collisions
                            switch (FindSmallest(o_top, o_bottom, o_left, o_right))
                            {
                                case "top":
                                    ball.velocity.Y *= -1;
                                    ball.position.Y = block_top - ball.texture.Height - 1;
                                    break;
                                case "bottom":
                                    ball.velocity.Y *= -1;
                                    ball.position.Y = block_bottom + 1;
                                    break;
                                case "left":
                                    ball.velocity.X *= -1;
                                    ball.position.X = block_left - ball.texture.Width - 1;
                                    break;
                                case "right":
                                    ball.velocity.X *= -1;
                                    ball.position.X = block_right + 1;
                                    break;
                            }
                        }
                        else
                        {
                            // ball must travel a certain distance before colliding again with a block
                            float cd = (Vector2.Distance(ball.collision_point, ball.position));
                            if (cd > 50.0f)
                            {
                                // Debug.Print("Distance from last collision {0}", cd);
                                // track collision point
                                ball.collision_point = ball.position;

                                

                                if (o_left < ball.texture.Width && ball.velocity.X > 0 /*&& (o_left > o_top || o_left > o_bottom))*/ ||
                                    (o_right < ball.texture.Width && ball.velocity.X < 0 /*&& (o_right > o_top || o_right > o_bottom)*/))
                                {
                                    ball.velocity.X *= -1;
                                }

                                if (o_top < ball.texture.Height && ball.velocity.Y > 0 /*&& (o_top > o_left || o_top > o_right))*/ ||
                                    (o_bottom < ball.texture.Height && ball.velocity.Y < 0 /*&& (o_bottom > o_left || o_bottom > o_right)*/))
                                {
                                    ball.velocity.Y *= -1;
                                }
                                // increase Y velocity of ball
                                if (ball.velocity.Y < 0)
                                {
                                    ball.velocity.Y -= 0.01f;
                                }
                                else
                                {
                                    ball.velocity.Y += 0.01f;
                                }
                                // play block sound
                                // Debug.Print("Panning is {0}", fx_pan);
                                gamecontent.blocksound[block.sound].Play(fx_volume, fx_pitch, fx_pan);
                                // remove block
                                level.blocksremove.Add(block);
                                // update score
                                score += block.scorevalue;                                
                            } // finish collision distance loop
                        } // finish ball.collide loop
                    } // finish collision check loop

                } // finish foreach block loop       

                // remove destroyed blocks 
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
            // remove lost balls
            foreach(GameObject ball in level.ballsremove)
            {
                level.balls.Remove(ball);
            }
            // count collidable balls
            ballcount = 0;
            foreach(GameObject ball in level.balls)
            {
                if (ball.collide)
                {
                    ballcount++;
                    break;                    
                }                    
            }
            // no more collidables
            if (ballcount == 0)
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
        private string FindSmallest(float top,float bottom,float left,float right)
        {
            if (top < bottom && top < left && top < right) return "top"; 
            if (bottom < top && bottom < left && bottom < right) return "bottom"; 
            if (left < top && left < bottom && left < right) return "left";
            if (right < top && right < bottom && right < left) return "right";
            // avoid code path error
            return "";
        }

        private void UpdatePaddle(GameTime gameTime)
        {            
            level.paddle.position.X = currentMouseState.X;
            // Make sure that the paddle does not go out of bounds
            level.paddle.position.X = MathHelper.Clamp(level.paddle.position.X, 0, vwidth - level.paddle.texture.Width);
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
                    GameContent.DrawText("Main Menu", Color.MonoGameOrange,GameContent.textposition.TopMiddle, gamecontent.font_GoodDog,spritebatch,vwidth,vheight);
                    break;
                case gamestate.Editor:
                    editor.Draw();                    
                    break;
                case gamestate.WaitingBall:                   
                    level.paddle.Draw(spritebatch);
                    level.Draw(spritebatch);
                    DrawGameInfo();
                    GameContent.DrawText("Left Click to Launch Ball", Color.MonoGameOrange, GameContent.textposition.BottomRight, gamecontent.font_GoodDog, spritebatch, vwidth, vheight);
                    break;
                case gamestate.BallLaunched:                    
                    level.paddle.Draw(spritebatch);
                    level.Draw(spritebatch);
                    DrawGameInfo();
                    break;
                case gamestate.GameOver:
                    level.Draw(spritebatch);
                    GameContent.DrawText("Game Over", Color.Red, GameContent.textposition.Middle, gamecontent.font_GoodDog, spritebatch, vwidth,vheight);                    
                    DrawGameInfo();
                    break;
            }
            spritebatch.End();
            base.Draw(gameTime);
        }                
        
        private void DrawGameInfo()
        {
            // draw game info
            GameContent.DrawText("Score : " + score, Color.Gray, GameContent.textposition.TopLeft, gamecontent.font_GoodDog, spritebatch, vwidth, vheight);
            GameContent.DrawText("Balls Left : " + ballsleft, Color.Gray, GameContent.textposition.TopRight,gamecontent.font_GoodDog, spritebatch, vwidth, vheight);
            GameContent.DrawText("Level " + level.number + " - " + level.name, Color.Green, GameContent.textposition.TopMiddle, gamecontent.font_arial, spritebatch, vwidth, vheight); 
        }

        
    }
}
