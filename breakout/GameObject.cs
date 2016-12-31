using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace breakout
{
    public class GameObject
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 collision_point;
        public Color color;
        public short scorevalue;
        public short sound;
        public short editorid;
        public string editordesc;
        public bool collide; 

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            }
        }        
        public GameObject(Color color, Texture2D texture,short editorid,string editordesc,short sound)
        {
            this.color = color;
            this.texture = texture;
            this.editorid = editorid;
            this.editordesc = editordesc;
            this.sound = sound;
            this.collide = false;
        }

        public GameObject(Color color, Texture2D texture, short editorid, string editordesc,Vector2 position,short sound)
        {
            this.color = color;
            this.texture = texture;
            this.editorid = editorid;
            this.editordesc = editordesc;
            this.position = position;
            this.collide = false;
            this.sound = sound;
        }
        public GameObject(Color color,Texture2D texture, Vector2 position)
        {
            this.color = color;
            this.texture = texture;
            this.position = position;
            this.collide = false;
        }
        public GameObject(Color color,Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this.color = color;
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.collide = false;
        }
       
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, color);
        }       
    }
}
