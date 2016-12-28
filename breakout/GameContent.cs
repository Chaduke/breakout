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

        public GameContent(IServiceProvider provider)
        {
            manager = new ContentManager(provider, "Content");
            blocksound = new SoundEffect[6];           
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
            background_sm = manager.Load<Texture2D>("Graphics\\background_sm");
            background_lg = manager.Load<Texture2D>("Graphics\\background_lg");
            paddle_sm = manager.Load<Texture2D>("Graphics\\paddle_sm");
            paddle_lg = manager.Load<Texture2D>("Graphics\\paddle_lg");
            ball_sm = manager.Load<Texture2D>("Graphics\\ball_sm");
            ball_lg = manager.Load<Texture2D>("Graphics\\ball_lg");
            block_sm = manager.Load<Texture2D>("Graphics\\block_sm");
            block_lg = manager.Load<Texture2D>("Graphics\\block_lg");
            pixel = manager.Load<Texture2D>("Graphics\\pixel");
            pointer = manager.Load<Texture2D>("Graphics\\pointer");
        }

        public static void DrawText(string msg, Color color,textposition position, SpriteFont font, SpriteBatch spritebatch, Viewport viewport)
        {
            Vector2 textsize = font.MeasureString(msg);           

            switch (position)
            {
                case textposition.BottomLeft:
                    spritebatch.DrawString(font, msg, new Vector2(5, viewport.Height - textsize.Y), color);
                    break;
                case textposition.BottomMiddle:
                    spritebatch.DrawString(font, msg, new Vector2((viewport.Width / 2) - (textsize.X / 2), viewport.Height - textsize.Y), color);
                    break;
                case textposition.BottomRight:
                    spritebatch.DrawString(font, msg, new Vector2(viewport.Width - (textsize.X + 5), viewport.Height - (textsize.Y + 5)), color);
                    break;
                case textposition.Middle:
                    spritebatch.DrawString(font, msg, new Vector2((viewport.Width / 2) - (textsize.X / 2), (viewport.Height / 2) - (textsize.Y / 2)), color);
                    break;
                case textposition.TopLeft:
                    spritebatch.DrawString(font, msg, new Vector2(5, 5), color);
                    break;
                case textposition.TopMiddle:
                    spritebatch.DrawString(font, msg, new Vector2((viewport.Width / 2) - (textsize.X / 2), 5), color);
                    break;
                case textposition.TopRight:
                    spritebatch.DrawString(font, msg, new Vector2(viewport.Width - (textsize.X + 5), 5), color);
                    break;
            }

        }

    }
}
