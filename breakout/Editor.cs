using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace breakout
{
    class Editor
    {

        private GameObject[] cursor;
        private int cursorindex;
        private GameContent gamecontent;
        private SpriteBatch spritebatch;
        private Viewport viewport;
        private MouseState currentmousestate;
        private MouseState previousmousestate;
        private Vector2 snapped;
        public Level level;
        public Editor(GameContent gamecontent,SpriteBatch spritebatch,Viewport viewport)
        {
            this.gamecontent = gamecontent;
            this.spritebatch = spritebatch;
            this.viewport = viewport;
            this.cursor = new GameObject[7];
            this.cursor[0] = new GameObject(Color.Red,gamecontent.block_sm,0,"Block Red");
            this.cursor[1] = new GameObject(Color.Orange, gamecontent.block_sm, 1, "Block Orange");
            this.cursor[2] = new GameObject(Color.Yellow, gamecontent.block_sm, 2, "Block Yellow");
            this.cursor[3] = new GameObject(Color.Green, gamecontent.block_sm, 3, "Block Green");
            this.cursor[4] = new GameObject(Color.Blue, gamecontent.block_sm, 4, "Block Blue");
            this.cursor[5] = new GameObject(Color.Purple, gamecontent.block_sm, 5, "Block Purple");
            this.cursor[6] = new GameObject(Color.White, gamecontent.ball_sm, 6, "Ball White");
            this.cursorindex = 0;
            this.level = new Level(0, "New Level", viewport.Width,viewport.Height, gamecontent);            
        }
        public void Update(MouseState mousestate)
        {
            int wheelchange;
            currentmousestate = mousestate;            
            
            wheelchange = currentmousestate.ScrollWheelValue - previousmousestate.ScrollWheelValue;
            if (wheelchange!=0)
            {
                if (wheelchange > 0) cursorindex++; else cursorindex--;
                if (cursorindex > 6) cursorindex = 0;
                if (cursorindex < 0) cursorindex = 6;                
            }            
            
            // snap cursor to grid
            snapped.X = (int)(currentmousestate.Position.X / gamecontent.block_sm.Width) * gamecontent.block_sm.Width;
            snapped.Y = 1 + (int)(currentmousestate.Position.Y / gamecontent.block_sm.Height) * gamecontent.block_sm.Height;
            cursor[cursorindex].Position = snapped;

            if (currentmousestate.LeftButton == ButtonState.Pressed && previousmousestate.LeftButton == ButtonState.Released)
            {
                level.blocks.Add(new GameObject(cursor[cursorindex].RGB, cursor[cursorindex].Texture, cursor[cursorindex].Position));
            }
            // finish update
            previousmousestate = currentmousestate;
        }

        public void Draw()
        {            
            DrawGrid();
            GameContent.DrawText(cursor[cursorindex].editordesc, cursor[cursorindex].RGB, GameContent.textposition.BottomLeft, gamecontent.font_GoodDog,spritebatch,viewport);
            foreach(GameObject block in level.blocks)
            {
                block.Draw(spritebatch);
            }
            cursor[cursorindex].Draw(spritebatch);

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
