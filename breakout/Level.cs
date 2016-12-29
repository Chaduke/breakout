using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace breakout
{
    public class Level
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
        public Texture2D background;

        private GameObject[] cursor;

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

            cursor = new GameObject[7];
            cursor[0] = new GameObject(Color.Red, gamecontent.block_sm, 0, "Block Red");
            cursor[1] = new GameObject(Color.Orange, gamecontent.block_sm, 1, "Block Orange");
            cursor[2] = new GameObject(Color.Yellow, gamecontent.block_sm, 2, "Block Yellow");
            cursor[3] = new GameObject(Color.Green, gamecontent.block_sm, 3, "Block Green");
            cursor[4] = new GameObject(Color.Blue, gamecontent.block_sm, 4, "Block Blue");
            cursor[5] = new GameObject(Color.Purple, gamecontent.block_sm, 5, "Block Purple");
            cursor[6] = new GameObject(Color.White, gamecontent.ball_sm, 6, "Ball White");
            background = gamecontent.background_sm;
        }
        public void Clear()
        {
            foreach(GameObject block in blocks)
            {
                blocksremove.Add(block);
            }
            foreach(GameObject block in blocksremove)
            {
                blocks.Remove(block);
            }
        }
        public void Load()
        {
            string filepath = Directory.GetCurrentDirectory() + "\\Level" + number + ".lvl";
            FileStream fs = File.OpenRead(filepath);
            BinaryReader reader = new BinaryReader(fs);
            short editorid;
            float x;
            float y;

            if (File.Exists(filepath))
            {
                Clear();
                name = reader.ReadString();
                while (fs.Position < fs.Length)
                {
                    editorid = reader.ReadInt16();
                    x = reader.ReadSingle();
                    y = reader.ReadSingle();
                    GameObject current = new GameObject(cursor[editorid].color, cursor[editorid].texture, editorid, cursor[editorid].editordesc, new Vector2(x, y));
                    blocks.Add(current);
                }
            }
            fs.Close();
        }        

        public void ChangeResolution(bool full)
        {
            if (full)
            {
                background = gamecontent.background_lg;             
                paddle.texture = gamecontent.paddle_lg;
                paddle.position = new Vector2(width / 2, height - (gamecontent.paddle_lg.Height * 2));
            }
            else
            {
                background = gamecontent.background_sm;
                paddle.texture = gamecontent.paddle_sm;
                paddle.position = new Vector2(width / 2, height - (gamecontent.paddle_sm.Height * 2));
            }
            foreach(GameObject block in blocks)
            {                
                if (full)
                {
                    block.texture = gamecontent.block_lg;
                    block.position.X *= 2;
                    block.position.Y *= 2;
                }
                else
                {
                    block.texture = gamecontent.block_sm;
                    block.position.X /= 2;
                    block.position.Y /= 2;
                }
            }
            foreach (GameObject ball in balls)
            {                
                if (full)
                {
                    ball.texture = gamecontent.ball_lg;
                    ball.position.X *= 2;
                    ball.position.Y *= 2;
                }
                else
                {
                    ball.texture = gamecontent.ball_sm;
                    ball.position.X /= 2;
                    ball.position.Y /= 2;
                }
            }
        }

        public void CreateBasic()
        {
            Texture2D blockTexture = gamecontent.block_sm;
            // setup first row set
            for (short row = 0; row < 3; row++)
            {
                for (short col = 0; col < 40; col++)
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
                    block.scorevalue = (short)(100 - (row * 10));
                    block.sound = row;
                    blocks.Add(block);
                }
            }

            // setup second row set
            for (short row = 0; row < 3; row++)
            {
                for (short col = 0; col < 40; col++)
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
                    block.scorevalue = (short)(50 - (row * 10));
                    block.sound = (short)(row + 3);
                    blocks.Add(block);
                }
            }
        }
    }
}  
