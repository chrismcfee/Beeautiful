using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beeautiful
{
    public class Explosion
    {
        #region Variables

        double lived = 0;
        double lifeSpan = 200;
        bool visible;

        Texture2D texture;
        Vector2 position;

        #endregion

        public bool Visible
        {
            get { return visible; }
        }

        public Explosion(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            visible = true;
        }

        public void Update(GameTime gameTime)
        {
            lived += gameTime.ElapsedGameTime.Milliseconds;
            if (lived > lifeSpan)
                visible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
                spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
