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
        public bool wide;
        public bool full;
        public int number;
        public string name;

        public GameObject paddle;
        public GameContent gamecontent;

        public List<GameObject> blocks;
        public List<GameObject> blocksremove;

        public List<GameObject> balls;
        public List<GameObject> ballsremove;
        public Texture2D background;

        public GameObject[] cursor;

        public Level(int number,string name,int width,int height,bool wide,bool full,GameContent gamecontent)
        {
            this.number = number;
            this.name = name;
            this.width = width;
            this.height = height;
            this.wide = wide;
            this.full = full;
            this.gamecontent = gamecontent;           

            paddle = new GameObject(Color.AliceBlue,gamecontent.paddle_sm,new Vector2(width/2,height-(gamecontent.paddle_sm.Height * 2)));
            
            blocks = new List<GameObject>();
            blocksremove = new List<GameObject>();
            balls = new List<GameObject>();
            ballsremove = new List<GameObject>();

            cursor = new GameObject[7];
            cursor[0] = new GameObject(Color.Red, gamecontent.block_sm, 0, "Block Red",0);
            cursor[1] = new GameObject(Color.Orange, gamecontent.block_sm, 1, "Block Orange",1);
            cursor[2] = new GameObject(Color.Yellow, gamecontent.block_sm, 2, "Block Yellow",2);
            cursor[3] = new GameObject(Color.Green, gamecontent.block_sm, 3, "Block Green",3);
            cursor[4] = new GameObject(Color.Blue, gamecontent.block_sm, 4, "Block Blue",4);
            cursor[5] = new GameObject(Color.Purple, gamecontent.block_sm, 5, "Block Purple",5);
            cursor[6] = new GameObject(Color.Yellow, gamecontent.ball_sm, 6, "Ball Yellow",0);
            background = gamecontent.background_sm;
        }            

        public void Draw(SpriteBatch spritebatch)
        {
            foreach (GameObject block in blocks)
            {
                block.Draw(spritebatch);
            }
            foreach (GameObject ball in balls)
            {
                ball.Draw(spritebatch);
            }
        }
        public void ClearBlocks()
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

        public void ClearBalls()
        {
            foreach (GameObject ball in balls)
            {
                ballsremove.Add(ball);
            }
            foreach (GameObject ball in ballsremove)
            {
                balls.Remove(ball);
            }
        }

        public void Save()
        {
            System.Diagnostics.Debug.Print("Starting save..fullscreen is " + full);
            string filepath = Directory.GetCurrentDirectory() + "\\Level" + number + ".lvl";
            if (File.Exists(filepath)) File.Delete(filepath);

            FileStream fs = File.Create(filepath);
            BinaryWriter writer = new BinaryWriter(fs);
            writer.Write("Test Level");
            writer.Write(wide);
            writer.Write(full);
            System.Diagnostics.Debug.Print("Saved level fullscreen " + full);
            foreach (GameObject block in blocks)
            {
                writer.Write(block.editorid);
                writer.Write(block.position.X);
                writer.Write(block.position.Y);
            }
            foreach (GameObject ball in balls)
            {
                writer.Write(ball.editorid);
                writer.Write(ball.position.X);
                writer.Write(ball.position.Y);
            }
            fs.Close();
        }
        public void Load()
        {
            string filepath = Directory.GetCurrentDirectory() + "\\Level" + number + ".lvl";
            FileStream fs; 
            BinaryReader reader;
            short editorid;
            float x;
            float y;
            bool fulltemp;
            bool enlarge = false;
            bool shrink = false;

            if (File.Exists(filepath))
            {
                fs = File.OpenRead(filepath);
                reader = new BinaryReader(fs);
                ClearBlocks();
                ClearBalls();
                name = reader.ReadString();
                wide = reader.ReadBoolean();
                fulltemp = reader.ReadBoolean();
                System.Diagnostics.Debug.Print ("Loaded level " + name + " fullscreen is " + fulltemp);
                if (fulltemp==true && full==false)
                {
                    shrink = true;
                }
                if (fulltemp == false && full == true)
                {
                    enlarge = true;
                }

                while (fs.Position < fs.Length)
                {
                        
                        editorid = reader.ReadInt16();
                        x = reader.ReadSingle();
                        y = reader.ReadSingle();

                        if (enlarge)
                        {
                            x *= 2;
                            y *= 2;
                        }
                        if (shrink)
                        {
                            x /= 2;
                            y /= 2;
                        }
                        GameObject current = new GameObject(cursor[editorid].color, cursor[editorid].texture, editorid, cursor[editorid].editordesc, new Vector2(x, y),cursor[editorid].sound);
                        if (current.editorid == 6)
                        {
                            current.velocity.X = 2;
                            current.velocity.Y = 2;
                            balls.Add(current);
                        }
                        else
                        {
                            blocks.Add(current);
                        } 
                }
                fs.Close();
            }            
        }        

        public void ChangeResolution(bool newfull,int w,int h)
        {
            full = newfull;
            width = w;
            height = h;
            System.Diagnostics.Debug.Print ("Resolution Changed to fullscreen = " + full);
            for (int i = 0; i < 6; i++)
            {
                if (full)
                {
                    cursor[i].texture = gamecontent.block_lg;

                }
                else
                {
                    cursor[i].texture = gamecontent.block_sm;
                }
            }
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
