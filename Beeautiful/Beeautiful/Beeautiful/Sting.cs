using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beeautiful
{
    public class Sting
    {
        #region Variables

        public Vector2 motion;
        public Vector2 position;
        public float speed = 20.0f;
        public bool visible = true;
        public int stingLevel;
        public Texture2D texture;
        public Rectangle bounds;

        #endregion

        #region Fields

        public float Damage
        {
            get { return (stingLevel + 1) * 10; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
        }


        #endregion


        public Sting(Texture2D texture, Vector2 position, int stingLevel)
        {
            motion = Vector2.Zero;
            this.position = position;
            this.texture = texture;
            this.bounds = texture.Bounds;
            this.visible = true;
            this.stingLevel = stingLevel;
            motion.Y = -1;
        }
        public virtual void Update()
        {
            if (position.Y < 0)
                visible = false;
            position += motion * speed;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(visible)
                spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
