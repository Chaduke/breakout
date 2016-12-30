using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;

namespace breakout
{
    public class GameContent
    {
        public ContentManager manager;
        bool widescreen;

        // fonts
        public SpriteFont font_arial;
        public SpriteFont font_GoodDog;

        // sounds
        public SoundEffect paddlesound;
        public SoundEffect[] blocksound;
        public SoundEffect wallsound;

        // textures
        public Texture2D background_sm;
        public Texture2D background_lg;
        public Texture2D ball_sm;
        public Texture2D ball_lg;
        public Texture2D block_sm;
        public Texture2D block_lg;
        public Texture2D paddle_sm;
        public Texture2D paddle_lg;
        public Texture2D pixel;
        public Texture2D pointer;

        public Texture2D button_load;
        public Texture2D button_save;
        public Texture2D button_test;
        public Texture2D button_clear;
        public Texture2D button_outline;

        public GameContent(IServiceProvider provider,bool widescreen)
        {
            manager = new ContentManager(provider, "Content");
            blocksound = new SoundEffect[6];
            this.widescreen = widescreen;
            LoadContent();
        }
        public enum textposition
        {
            TopLeft, TopMiddle, TopRight, Middle, BottomLeft, BottomMiddle, BottomRight
        }

        private void LoadContent()
        {
            // load fonts              
            font_arial = manager.Load<SpriteFont>("Fonts\\Arial");
            font_GoodDog = manager.Load<SpriteFont>("Fonts\\GoodDog");

            // load sounds
            paddlesound = manager.Load<SoundEffect>("Audio\\c6");
            wallsound = manager.Load<SoundEffect>("Audio\\c7");
            blocksound[0] = manager.Load<SoundEffect>("Audio\\d7");
            blocksound[1] = manager.Load<SoundEffect>("Audio\\e7");
            blocksound[2] = manager.Load<SoundEffect>("Audio\\f7");
            blocksound[3] = manager.Load<SoundEffect>("Audio\\g7");
            blocksound[4] = manager.Load<SoundEffect>("Audio\\a6");
            blocksound[5] = manager.Load<SoundEffect>("Audio\\b6");            

            // Load Textures
            if (widescreen)
            {
                background_sm = manager.Load<Texture2D>("Graphics\\background_sm");
                background_lg = manager.Load<Texture2D>("Graphics\\background_lg");
                paddle_sm = manager.Load<Texture2D>("Graphics\\paddle_sm");
                paddle_lg = manager.Load<Texture2D>("Graphics\\paddle_lg");                
                block_sm = manager.Load<Texture2D>("Graphics\\block_sm");
                block_lg = manager.Load<Texture2D>("Graphics\\block_lg");
            }
            else
            {
                background_sm = manager.Load<Texture2D>("Graphics\\background_sm_43");
                background_lg = manager.Load<Texture2D>("Graphics\\background_lg_43");
                paddle_sm = manager.Load<Texture2D>("Graphics\\paddle_sm_43");
                paddle_lg = manager.Load<Texture2D>("Graphics\\paddle_lg_43");                
                block_sm = manager.Load<Texture2D>("Graphics\\block_sm_43");
                block_lg = manager.Load<Texture2D>("Graphics\\block_lg_43");
            }            
            ball_sm = manager.Load<Texture2D>("Graphics\\ball_sm");
            ball_lg = manager.Load<Texture2D>("Graphics\\ball_lg");            
            pixel = manager.Load<Texture2D>("Graphics\\pixel");
            pointer = manager.Load<Texture2D>("Graphics\\pointer");
            button_load = manager.Load<Texture2D>("Graphics\\load");
            button_save = manager.Load<Texture2D>("Graphics\\save");
            button_test = manager.Load<Texture2D>("Graphics\\test");
            button_clear = manager.Load<Texture2D>("Graphics\\clear");
            button_outline = manager.Load<Texture2D>("Graphics\\outline");
        }

        public static void DrawText(string msg, Color color,textposition position, SpriteFont font, SpriteBatch spritebatch, int width,int height)
        {
            Vector2 textsize = font.MeasureString(msg);           

            switch (position)
            {
                case textposition.BottomLeft:
                    spritebatch.DrawString(font, msg, new Vector2(5, height - textsize.Y), color);
                    break;
                case textposition.BottomMiddle:
                    spritebatch.DrawString(font, msg, new Vector2((width / 2) - (textsize.X / 2), height - textsize.Y), color);
                    break;
                case textposition.BottomRight:
                    spritebatch.DrawString(font, msg, new Vector2(width - (textsize.X + 5), height - (textsize.Y + 5)), color);
                    break;
                case textposition.Middle:
                    spritebatch.DrawString(font, msg, new Vector2((width / 2) - (textsize.X / 2), (height / 2) - (textsize.Y / 2)), color);
                    break;
                case textposition.TopLeft:
                    spritebatch.DrawString(font, msg, new Vector2(5, 5), color);
                    break;
                case textposition.TopMiddle:
                    spritebatch.DrawString(font, msg, new Vector2((width / 2) - (textsize.X / 2), 5), color);
                    break;
                case textposition.TopRight:
                    spritebatch.DrawString(font, msg, new Vector2(width - (textsize.X + 5), 5), color);
                    break;
            }

        }

    }
}
