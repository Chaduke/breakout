using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace breakout
{
    class Level
    {
        public int width;
        public int height;
        public int number;
        public string name;

        public GameObject paddle;
        public GameContent gamecontent;

        public List<GameObject> blocks;
        public List<GameObject> blocksremove;

        public List<GameObject> balls;
        public List<GameObject> ballsremove;       

        public Level(int number,string name,int width,int height,GameContent gamecontent)
        {
            this.number = number;
            this.name = name;
            this.width = width;
            this.height = height;
            this.gamecontent = gamecontent;

            paddle = new GameObject(Color.AliceBlue,gamecontent.paddle_sm,new Vector2(width/2,height-(gamecontent.paddle_sm.Height * 2)));
            
            blocks = new List<GameObject>();
            blocksremove = new List<GameObject>();
            balls = new List<GameObject>();
            ballsremove = new List<GameObject>();            
        }
        public void Create()
        {
            Texture2D blockTexture = gamecontent.block_sm;
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

            // setup second row set
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

        public void Reload(bool full)
        {
            foreach(GameObject block in blocks)
            {
                
                if (full)
                {
                    block.Texture = gamecontent.block_lg;
                    block.Position.X *= 2;
                    block.Position.Y *= 2;
                }
                else
                {
                    block.Texture = gamecontent.block_sm;
                    block.Position.X /= 2;
                    block.Position.Y /= 2;
                }
            }
            foreach (GameObject ball in balls)
            {                
                if (full)
                {
                    ball.Texture = gamecontent.ball_lg;
                    ball.Position.X *= 2;
                    ball.Position.Y *= 2;
                }
                else
                {
                    ball.Texture = gamecontent.ball_sm;
                    ball.Position.X /= 2;
                    ball.Position.Y /= 2;
                }
            }
        }       

    }
}  
