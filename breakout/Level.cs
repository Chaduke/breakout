using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace breakout
{
    class Level
    {
        public int number;
        public string name;
        public List<GameObject> blocks;
        public List<GameObject> blocksremove;

        public List<GameObject> balls;
        public List<GameObject> ballsremove;

        public int ballsleft;

        public void Reload(Texture2D blocktexture,Texture2D balltexture,bool full)
        {
            foreach(GameObject block in blocks)
            {
                block.Texture = blocktexture;
                if (full)
                {
                    block.Position.X *= 2;
                    block.Position.Y *= 2;
                }
                else
                {
                    block.Position.X /= 2;
                    block.Position.Y /= 2;
                }
            }
            foreach (GameObject ball in balls)
            {
                ball.Texture = balltexture;
                if (full)
                {
                    ball.Position.X *= 2;
                    ball.Position.Y *= 2;
                }
                else
                {
                    ball.Position.X /= 2;
                    ball.Position.Y /= 2;
                }
            }
        }

        public void Create(Texture2D blockTexture)
        {
            // setup first row set
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 40; col++)
                {
                    int x = (blockTexture.Width * col);
                    int y = (blockTexture.Height * 5) + (row * blockTexture.Height);
                    Color c = new Color();
                    switch (row)
                    {
                        case 0:
                            c = Color.Red;
                            break;
                        case 1:
                            c = Color.Orange;
                            break;
                        case 2:
                            c = Color.Yellow;
                            break;                       
                    }
                    GameObject block = new GameObject(c, blockTexture, new Vector2(x, y));
                    block.scorevalue = 100 - (row * 10);
                    block.sound = row;
                    blocks.Add(block);
                }
            }
            // steup second row set
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 40; col++)
                {
                    int x = (blockTexture.Width * col);
                    int y = (blockTexture.Height * 12) + (row * blockTexture.Height);
                    Color c = new Color();
                    switch (row)
                    {
                        case 0:
                            c = Color.Green;
                            break;
                        case 1:
                            c = Color.Blue;
                            break;
                        case 2:
                            c = Color.Purple;
                            break;                       
                    }
                    GameObject block = new GameObject(c, blockTexture, new Vector2(x, y));
                    block.scorevalue = 50 - (row * 10);
                    block.sound = row + 3;
                    blocks.Add(block);
                }

            }
        }

    }
}  
