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

        public void Create(Texture2D blockTexture)
        {
            // setup first row set
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 16; col++)
                {
                    int x = (blockTexture.Width * col);
                    int y = (blockTexture.Height * 3) + (row * blockTexture.Height);
                    Color c = new Color();
                    switch (row)
                    {
                        case 0:
                            c = Color.Red;
                            break;
                        case 1:
                            c = Color.Turquoise;
                            break;
                        case 2:
                            c = Color.Wheat;
                            break;
                        case 3:
                            c = Color.Orange;
                            break;
                        default:
                            c = Color.White;
                            break;
                    }
                    GameObject block = new GameObject(c, blockTexture, new Vector2(x, y));
                    block.scorevalue = 100 - (row * 10);
                    blocks.Add(block);
                }
            }
            // steup second row set
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 16; col++)
                {
                    int x = (blockTexture.Width * col);
                    int y = (blockTexture.Height * 10) + (row * blockTexture.Height);
                    Color c = new Color();
                    switch (row)
                    {
                        case 0:
                            c = Color.Blue;
                            break;
                        case 1:
                            c = Color.DeepPink;
                            break;
                        case 2:
                            c = Color.Green;
                            break;
                        default:
                            c = Color.White;
                            break;
                    }
                    GameObject block = new GameObject(c, blockTexture, new Vector2(x, y));
                    block.scorevalue = 50 - (row * 10);
                    blocks.Add(block);
                }

            }
        }

    }
}  
