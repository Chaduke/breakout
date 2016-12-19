using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace breakout
{
    class Ball
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Speed;

        public void Initialize(Texture2D texture)
        {
            Texture = texture;            
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
