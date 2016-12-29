using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace breakout
{
    public class Editor
    {
        private GameObject[] cursor;
        private int cursorindex;
        private GameContent gamecontent;
        private SpriteBatch spritebatch;
        private Viewport viewport;
        private MouseState currentmousestate;
        private MouseState previousmousestate;
        private Vector2 snapped;
        private Level level;

        private bool ongrid;
        private bool overblock; 

        private GameObject selectedblock;

        public Editor(GameContent gamecontent,SpriteBatch spritebatch,Viewport viewport)
        {
            this.gamecontent = gamecontent;
            this.spritebatch = spritebatch;
            this.viewport = viewport;

            cursor = new GameObject[7];
            cursor[0] = new GameObject(Color.Red,gamecontent.block_sm,0,"Block Red");
            cursor[1] = new GameObject(Color.Orange, gamecontent.block_sm, 1, "Block Orange");
            cursor[2] = new GameObject(Color.Yellow, gamecontent.block_sm, 2, "Block Yellow");
            cursor[3] = new GameObject(Color.Green, gamecontent.block_sm, 3, "Block Green");
            cursor[4] = new GameObject(Color.Blue, gamecontent.block_sm, 4, "Block Blue");
            cursor[5] = new GameObject(Color.Purple, gamecontent.block_sm, 5, "Block Purple");
            cursor[6] = new GameObject(Color.White, gamecontent.ball_sm, 6, "Ball White");
            cursorindex = 0;
            level = new Level(0, "New Level", viewport.Width,viewport.Height, gamecontent);            
        }
        public void SaveLevel()
        {
            string filepath = Directory.GetCurrentDirectory() + "\\Level" + level.number + ".lvl";
            if (File.Exists(filepath)) File.Delete(filepath);
            
            FileStream fs = File.Create(filepath);
            BinaryWriter writer = new BinaryWriter(fs);
            writer.Write("Test Level");
            foreach (GameObject block in level.blocks)
            {
                writer.Write(block.editorid);                
                writer.Write(block.position.X);
                writer.Write(block.position.Y);
            }                       
            fs.Close();          
        }
        public void LoadLevel()
        {
            string filepath = Directory.GetCurrentDirectory() + "\\Level" + level.number + ".lvl";
            FileStream fs = File.OpenRead(filepath);
            BinaryReader reader = new BinaryReader(fs);
            short editorid;
            float x;
            float y;

            if (File.Exists(filepath))
            {
                level.Clear();
                level.name = reader.ReadString();

                while(fs.Position < fs.Length)
                {
                    editorid = reader.ReadInt16();
                    x = reader.ReadSingle();
                    y = reader.ReadSingle();
                    GameObject current = new GameObject(cursor[editorid].color, cursor[editorid].texture,editorid,cursor[editorid].editordesc, new Vector2(x, y));
                    level.blocks.Add(current);
                }
            }            
            fs.Close();
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
            if (currentmousestate.Position.Y < viewport.Height - (gamecontent.block_sm.Height * 8))
            {
                ongrid = true;
                snapped.X = (int)(currentmousestate.Position.X / gamecontent.block_sm.Width) * gamecontent.block_sm.Width;
                snapped.Y = 1 + (int)(currentmousestate.Position.Y / gamecontent.block_sm.Height) * gamecontent.block_sm.Height;
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
                    cursor[cursorindex].position = snapped;
                    if (currentmousestate.LeftButton == ButtonState.Pressed && previousmousestate.LeftButton == ButtonState.Released)
                    {
                        level.blocks.Add(new GameObject(cursor[cursorindex].color, cursor[cursorindex].texture, cursor[cursorindex].editorid, cursor[cursorindex].editordesc,cursor[cursorindex].position));
                    }
                }
            }
            else
            {
                ongrid = false;
                if (currentmousestate.LeftButton == ButtonState.Pressed && previousmousestate.LeftButton == ButtonState.Released)
                {
                    SaveLevel();
                }
                if (currentmousestate.RightButton == ButtonState.Pressed && previousmousestate.RightButton == ButtonState.Released)
                {
                    LoadLevel();
                }
            }
            // finish update
            previousmousestate = currentmousestate;
        }

        public void Draw()
        {            
            DrawGrid();
            foreach (GameObject block in level.blocks)
            {
                block.Draw(spritebatch);
            }
            if (ongrid && !overblock)
            {
                GameContent.DrawText(cursor[cursorindex].editordesc, cursor[cursorindex].color, GameContent.textposition.BottomLeft, gamecontent.font_GoodDog, spritebatch, viewport);
                cursor[cursorindex].Draw(spritebatch);
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
            for (int x = 1; x < viewport.Width / gamecontent.block_sm.Width; x++)
            {
                for (int y = 1; y < (viewport.Height / gamecontent.block_sm.Height) - 7; y++)
                {
                    Line l1 = new Line(new Vector2(x * gamecontent.block_sm.Width, 0), new Vector2(x * gamecontent.block_sm.Width, viewport.Height - (gamecontent.block_sm.Height * 8)), 1, Color.Gray, gamecontent.pixel);
                    Line l2 = new Line(new Vector2(0, y * gamecontent.block_sm.Height), new Vector2(viewport.Width,y * gamecontent.block_sm.Height), 1, Color.Gray, gamecontent.pixel);
                    l1.Update();
                    l1.Draw(spritebatch);
                    l2.Update();
                    l2.Draw(spritebatch);
                }
            }
        }
    }
}
