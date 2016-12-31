using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace breakout
{
    public class Editor
    {       
        private int cursorindex;
        private GameContent gamecontent;
        private SpriteBatch spritebatch;       
        private bool wide;
        public bool full;
        private int width;
        private int height;
        private MouseState currentmousestate;
        private MouseState previousmousestate;
        private Vector2 snapped;
        public Level level;

        private bool ongrid;
        private bool overblock;
        
        private byte button_hover;
        private GameObject selectedblock;

        public bool testlevel;

        public Editor(GameContent gamecontent,SpriteBatch spritebatch,int width,int height,bool wide,bool full)
        {
            this.gamecontent = gamecontent;
            this.spritebatch = spritebatch;
            this.width = width;
            this.height = height;
            this.wide = wide;
            this.full = full;
            cursorindex = 0;
            button_hover = 0;
            testlevel = false;
            level = new Level(0, "Editor Level", width,height,wide,full,gamecontent);
            level.Load();                      
        }
        public void ChangeResolution(bool newfull,int w,int h)
        {
            full = newfull;
            width = w;
            height = h;
            level.ChangeResolution(full,w,h);          
        }
                
        public void Update(MouseState mousestate)
        {
            int wheelchange;
            currentmousestate = mousestate;            
            
            wheelchange = currentmousestate.ScrollWheelValue - previousmousestate.ScrollWheelValue;
            if (wheelchange!=0)
            {
                if (wheelchange > 0) cursorindex++; else cursorindex--;
                cursorindex = MathHelper.Clamp(cursorindex, 0, 6);
                //  if (cursorindex > 6) cursorindex = 0;
                // if (cursorindex < 0) cursorindex = 6;                
            }

            // if on grid, snap cursor to grid, otherwise draw a mouse pointer
            if (currentmousestate.Position.Y < height - (level.cursor[0].texture.Height * 8))
            {
                ongrid = true;
                snapped.X = (int)(currentmousestate.Position.X / level.cursor[0].texture.Width) * level.cursor[0].texture.Width;
                snapped.Y = 1 + (int)(currentmousestate.Position.Y / level.cursor[0].texture.Height) * level.cursor[0].texture.Height;
                // check if block exists
                foreach(GameObject block in level.blocks)
                {
                    if (block.position == snapped)
                    {
                        overblock = true;
                        selectedblock = block;                        
                        break; 
                    }
                    else
                    {
                        overblock = false;                        
                    }                    
                }
                if (overblock)
                {
                    if (currentmousestate.RightButton == ButtonState.Pressed && previousmousestate.RightButton == ButtonState.Released)
                    {                       
                        level.blocks.Remove(selectedblock);                                      
                        overblock = false;
                    }
                }
                else
                {
                    level.cursor[cursorindex].position = snapped;
                    if (currentmousestate.LeftButton == ButtonState.Pressed && previousmousestate.LeftButton == ButtonState.Released)
                    {
                        if (cursorindex == 6)
                        {
                            level.balls.Add(new GameObject(level.cursor[cursorindex].color, level.cursor[cursorindex].texture, level.cursor[cursorindex].editorid, level.cursor[cursorindex].editordesc, level.cursor[cursorindex].position, level.cursor[cursorindex].sound));
                        }
                        else
                        {
                            level.blocks.Add(new GameObject(level.cursor[cursorindex].color, level.cursor[cursorindex].texture, level.cursor[cursorindex].editorid, level.cursor[cursorindex].editordesc, level.cursor[cursorindex].position, level.cursor[cursorindex].sound));
                        }
                        
                    }
                }
            }
            else
            {
                // process bottom of screen
                ongrid = false;
                button_hover = 0;
                // find load button
                if(currentmousestate.Position.X > width - (gamecontent.button_load.Width + 5) * 4 && currentmousestate.Position.X < width - (gamecontent.button_load.Width + 5) * 3
                    && currentmousestate.Position.Y > (height - gamecontent.button_load.Height + 5) && currentmousestate.Position.Y < height - 5)
                {
                    button_hover = 1;    
                }
                // find save button
                if (currentmousestate.Position.X > width - (gamecontent.button_load.Width + 5) * 3 && currentmousestate.Position.X < width - (gamecontent.button_load.Width + 5) * 2
                    && currentmousestate.Position.Y > (height - gamecontent.button_load.Height + 5) && currentmousestate.Position.Y < height - 5)
                {
                    button_hover = 2;
                }
                // find test button
                if (currentmousestate.Position.X > width - (gamecontent.button_load.Width + 5) * 2 && currentmousestate.Position.X < width - (gamecontent.button_load.Width + 5) 
                    && currentmousestate.Position.Y > (height - gamecontent.button_load.Height + 5) && currentmousestate.Position.Y < height - 5)
                {
                    button_hover = 3;
                }
                // find clear button
                if (currentmousestate.Position.X > width - (gamecontent.button_load.Width + 5) && currentmousestate.Position.X < width - 5
                    && currentmousestate.Position.Y > (height - gamecontent.button_load.Height + 5) && currentmousestate.Position.Y < height - 5)
                {
                    button_hover = 4;
                }

                if (currentmousestate.LeftButton == ButtonState.Pressed && previousmousestate.LeftButton == ButtonState.Released)
                {
                    switch (button_hover)
                    {
                        case 1:
                            level.Load();
                            break;
                        case 2:
                            System.Diagnostics.Debug.Print("Save button clicked, editor fullscreen = " + full + ", level fullscreen = " + level.full);
                            level.Save();
                            break;
                        case 3:
                            level.Save();
                            testlevel = true;
                            System.Diagnostics.Debug.Print("Test button clicked, editor fullscreen = " + full + ", level fullscreen = " + level.full);
                            break;
                        case 4:
                            level.ClearBlocks();
                            break;
                    }                 
                }               
            }
            // finish update
            previousmousestate = currentmousestate;
        }

        public void Draw()
        {            
            DrawGrid();
            // draw Buttons
            spritebatch.Draw(gamecontent.button_load, new Vector2(width - ((gamecontent.button_load.Width + 5) * 4), height - gamecontent.button_load.Height - 5), Color.White);
            spritebatch.Draw(gamecontent.button_save, new Vector2(width - ((gamecontent.button_load.Width + 5) * 3), height - gamecontent.button_load.Height - 5), Color.White);
            spritebatch.Draw(gamecontent.button_test, new Vector2(width - ((gamecontent.button_load.Width + 5) * 2), height - gamecontent.button_load.Height - 5), Color.White);
            spritebatch.Draw(gamecontent.button_clear, new Vector2(width - ((gamecontent.button_load.Width + 5)), height - gamecontent.button_load.Height - 5), Color.White);
            if (button_hover > 0)
            {
                spritebatch.Draw(gamecontent.button_outline, new Vector2(width - ((gamecontent.button_load.Width + 5) * (5 - button_hover)), height - gamecontent.button_load.Height - 5), Color.White);
            }

            level.Draw(spritebatch);           

            if (ongrid && !overblock)
            {
                GameContent.DrawText(level.cursor[cursorindex].editordesc, level.cursor[cursorindex].color, GameContent.textposition.BottomLeft, gamecontent.font_GoodDog, spritebatch, width,height);
                level.cursor[cursorindex].Draw(spritebatch);
            }
            else
            {
                if (overblock)
                {
                    // selectedblock = level.blocks.Find(x => x.position.Equals(snapped));
                    GameContent.DrawText(selectedblock.editordesc, selectedblock.color, GameContent.textposition.BottomLeft, gamecontent.font_GoodDog, spritebatch, width, height);                   
                }
                else
                {
                    GameContent.DrawText("Current Level : " + level.number, Color.White, GameContent.textposition.BottomLeft, gamecontent.font_GoodDog, spritebatch, width, height);                   
                }
                spritebatch.Draw(gamecontent.pointer, new Vector2(currentmousestate.X, currentmousestate.Y), Color.White);
            }
        }
        private void DrawGrid()
        {
            for (int x = 1; x < width / level.cursor[0].texture.Width; x++)
            {
                for (int y = 1; y < (height / level.cursor[0].texture.Height) - 7; y++)
                {
                    Line l1 = new Line(new Vector2(x * level.cursor[0].texture.Width, 0), new Vector2(x * level.cursor[0].texture.Width, height - (level.cursor[0].texture.Height * 8)), 1, Color.Gray, gamecontent.pixel);
                    Line l2 = new Line(new Vector2(0, y * level.cursor[0].texture.Height), new Vector2(width,y * level.cursor[0].texture.Height), 1, Color.Gray, gamecontent.pixel);
                    l1.Update();
                    l1.Draw(spritebatch);
                    l2.Update();
                    l2.Draw(spritebatch);
                }
            }
        }
    }
}
