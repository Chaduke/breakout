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
        public Viewport viewport;
        private MouseState currentmousestate;
        private MouseState previousmousestate;
        private Vector2 snapped;
        private Level level;

        private bool ongrid;
        private bool overblock;
        private bool wide;
        private byte button_hover;

        private GameObject selectedblock;

        public Editor(GameContent gamecontent,SpriteBatch spritebatch,Viewport viewport,bool wide)
        {
            this.gamecontent = gamecontent;
            this.spritebatch = spritebatch;
            this.viewport = viewport;
            this.wide = wide;            
            cursorindex = 0;
            button_hover = 0;
            level = new Level(0, "", viewport.Width,viewport.Height,wide,gamecontent);
            level.Load();                      
        }
        public void ReloadCursor(bool full)
        {
            for(int i=0;i<6;i++)
            {
                if (full)
                {
                    level.cursor[i].texture = gamecontent.block_lg;
                    foreach(GameObject block in level.blocks)
                    {
                        block.texture = gamecontent.block_lg;
                        block.position.X *= 2;
                        block.position.Y *= 2;
                    }
                }
                else
                {
                    level.cursor[i].texture = gamecontent.block_sm;
                    foreach (GameObject block in level.blocks)
                    {
                        block.texture = gamecontent.block_sm;
                        block.position.X /= 2;
                        block.position.Y /= 2;
                    }
                }
            }
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
            if (currentmousestate.Position.Y < viewport.Height - (level.cursor[0].texture.Height * 8))
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
                        level.blocks.Add(new GameObject(level.cursor[cursorindex].color, level.cursor[cursorindex].texture, level.cursor[cursorindex].editorid, level.cursor[cursorindex].editordesc, level.cursor[cursorindex].position));
                    }
                }
            }
            else
            {
                // process bottom of screen
                ongrid = false;
                button_hover = 0;
                // find load button
                if(currentmousestate.Position.X > viewport.Width - (gamecontent.button_load.Width + 5) * 4 && currentmousestate.Position.X < viewport.Width - (gamecontent.button_load.Width + 5) * 3
                    && currentmousestate.Position.Y > (viewport.Height - gamecontent.button_load.Height + 5) && currentmousestate.Position.Y < viewport.Height - 5)
                {
                    button_hover = 1;    
                }
                // find save button
                if (currentmousestate.Position.X > viewport.Width - (gamecontent.button_load.Width + 5) * 3 && currentmousestate.Position.X < viewport.Width - (gamecontent.button_load.Width + 5) * 2
                    && currentmousestate.Position.Y > (viewport.Height - gamecontent.button_load.Height + 5) && currentmousestate.Position.Y < viewport.Height - 5)
                {
                    button_hover = 2;
                }
                // find test button
                if (currentmousestate.Position.X > viewport.Width - (gamecontent.button_load.Width + 5) * 2 && currentmousestate.Position.X < viewport.Width - (gamecontent.button_load.Width + 5) 
                    && currentmousestate.Position.Y > (viewport.Height - gamecontent.button_load.Height + 5) && currentmousestate.Position.Y < viewport.Height - 5)
                {
                    button_hover = 3;
                }
                // find clear button
                if (currentmousestate.Position.X > viewport.Width - (gamecontent.button_load.Width + 5) && currentmousestate.Position.X < viewport.Width - 5
                    && currentmousestate.Position.Y > (viewport.Height - gamecontent.button_load.Height + 5) && currentmousestate.Position.Y < viewport.Height - 5)
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
                            level.Save();
                            break;
                        case 3:
                            break;
                            level.Test();
                        case 4:
                            level.Clear();
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
            spritebatch.Draw(gamecontent.button_load, new Vector2(viewport.Width - ((gamecontent.button_load.Width + 5) * 4), viewport.Height - gamecontent.button_load.Height - 5), Color.White);
            spritebatch.Draw(gamecontent.button_save, new Vector2(viewport.Width - ((gamecontent.button_load.Width + 5) * 3), viewport.Height - gamecontent.button_load.Height - 5), Color.White);
            spritebatch.Draw(gamecontent.button_test, new Vector2(viewport.Width - ((gamecontent.button_load.Width + 5) * 2), viewport.Height - gamecontent.button_load.Height - 5), Color.White);
            spritebatch.Draw(gamecontent.button_clear, new Vector2(viewport.Width - ((gamecontent.button_load.Width + 5)), viewport.Height - gamecontent.button_load.Height - 5), Color.White);
            if (button_hover > 0)
            {
                spritebatch.Draw(gamecontent.button_outline, new Vector2(viewport.Width - ((gamecontent.button_load.Width + 5) * (5 - button_hover)), viewport.Height - gamecontent.button_load.Height - 5), Color.White);
            }

            foreach (GameObject block in level.blocks)
            {
                block.Draw(spritebatch);
            }
            if (ongrid && !overblock)
            {
                GameContent.DrawText(level.cursor[cursorindex].editordesc, level.cursor[cursorindex].color, GameContent.textposition.BottomLeft, gamecontent.font_GoodDog, spritebatch, viewport);
                level.cursor[cursorindex].Draw(spritebatch);
            }
            else
            {
                if (overblock)
                {
                    // selectedblock = level.blocks.Find(x => x.position.Equals(snapped));
                    GameContent.DrawText(selectedblock.editordesc, selectedblock.color, GameContent.textposition.BottomLeft, gamecontent.font_GoodDog, spritebatch, viewport);                   
                }
                else
                {
                    GameContent.DrawText("Current Level : " + level.number, Color.White, GameContent.textposition.BottomLeft, gamecontent.font_GoodDog, spritebatch, viewport);                   
                }
                spritebatch.Draw(gamecontent.pointer, new Vector2(currentmousestate.X, currentmousestate.Y), Color.White);
            }
        }
        private void DrawGrid()
        {
            for (int x = 1; x < viewport.Width / level.cursor[0].texture.Width; x++)
            {
                for (int y = 1; y < (viewport.Height / level.cursor[0].texture.Height) - 7; y++)
                {
                    Line l1 = new Line(new Vector2(x * level.cursor[0].texture.Width, 0), new Vector2(x * level.cursor[0].texture.Width, viewport.Height - (level.cursor[0].texture.Height * 8)), 1, Color.Gray, gamecontent.pixel);
                    Line l2 = new Line(new Vector2(0, y * level.cursor[0].texture.Height), new Vector2(viewport.Width,y * level.cursor[0].texture.Height), 1, Color.Gray, gamecontent.pixel);
                    l1.Update();
                    l1.Draw(spritebatch);
                    l2.Update();
                    l2.Draw(spritebatch);
                }
            }
        }
    }
}
