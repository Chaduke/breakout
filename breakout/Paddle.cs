using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace breakout
{
    class Paddle
    {        
        public Texture2D Texture;       
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position)        
        {
            Texture = texture;
            Position = position;
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
