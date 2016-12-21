using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace breakout
{
    class GameObject
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color RGB;
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            }
        }

        public GameObject(Color rgb,Texture2D texture, Vector2 position)
        {
            this.RGB = rgb;
            this.Texture = texture;
            this.Position = position;
        }

        public GameObject(Color rgb,Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this.RGB = rgb;
            this.Texture = texture;
            this.Position = position;
            this.Velocity = velocity;
        }
       
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, RGB);
        }
    }
}
