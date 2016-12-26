using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace breakout
{
    class Editor
    {
        private int cursor;
        private GameContent gamecontent;
        private SpriteBatch spritebatch;
        private int width;
        private int height;
        private int currentwheelvalue;
        private int previouswheelvalue;

        public Editor(GameContent gamecontent,SpriteBatch spritebatch,int width,int height)
        {
            this.gamecontent = gamecontent;
            this.spritebatch = spritebatch;
            this.width = width;
            this.height = height;
            this.cursor = 1;
            this.previouswheelvalue = 0;
        }
        public void Update(MouseState mousestate)
        {
            int wheelchange;
            currentwheelvalue = mousestate.ScrollWheelValue;
            wheelchange = currentwheelvalue - previouswheelvalue;
            if (wheelchange!=0)
            {
                if (wheelchange > 0) cursor++; else cursor--;
                if (cursor > 6) cursor = 1;
                if (cursor < 1) cursor = 6;
                previouswheelvalue = currentwheelvalue;
            }
        }

        public void Draw(MouseState mousestate)
        {
            Color cursorcolor;
            DrawGrid();
            int lockedx,lockedy;

            switch (cursor)
            {
                case 1:
                    cursorcolor = Color.Red;
                    break;
                case 2:
                    cursorcolor = Color.Orange;
                    break;
                case 3:
                    cursorcolor = Color.Yellow;
                    break;
                case 4:
                    cursorcolor = Color.Green;
                    break;
                case 5:
                    cursorcolor = Color.Blue;
                    break;
                case 6:
                    cursorcolor = Color.Purple;
                    break;
                default:
                    cursorcolor=Color.White;
                    break;
            }
            // lock cursor to grid
            
            lockedx = (int)(mousestate.Position.X / gamecontent.block_sm.Width) * gamecontent.block_sm.Width;
            lockedy = 1 + (int)(mousestate.Position.Y / gamecontent.block_sm.Height) * gamecontent.block_sm.Height;

            spritebatch.Draw(gamecontent.block_sm, new Vector2(lockedx,lockedy),cursorcolor);

        }
        private void DrawGrid()
        {
            for (int x = 1; x < width / gamecontent.block_sm.Width; x++)
            {
                for (int y = 1; y < (height / gamecontent.block_sm.Height) - 7; y++)
                {
                    Line l1 = new Line(new Vector2(x * gamecontent.block_sm.Width, 0), new Vector2(x * gamecontent.block_sm.Width, height - (gamecontent.block_sm.Height * 8)), 1, Color.Gray, gamecontent.pixel);
                    Line l2 = new Line(new Vector2(0, y * gamecontent.block_sm.Height), new Vector2(width,y * gamecontent.block_sm.Height), 1, Color.Gray, gamecontent.pixel);
                    l1.Update();
                    l1.Draw(spritebatch);
                    l2.Update();
                    l2.Draw(spritebatch);
                }
            }
        }
    }
}
