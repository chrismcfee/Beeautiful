using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Beeautiful
{
    public class Boss
    {
        #region Variables

        Vector2 position;
        Vector2 motion;

        float shipSpeed = 5.0f;

        int lives;
        int maxLives = 5;

        bool isInvincible;
        bool isVisible;

        List<Texture2D> textures;
        int currentTexture = 0;

        Rectangle screenBounds;



        #endregion

        #region Fields






        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, textures[currentTexture].Width, textures[currentTexture].Height); }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        #endregion

        public Boss(List<Texture2D> textures, Rectangle screenBounds)
        {
            this.textures = textures;
            this.screenBounds = screenBounds;
            this.lives = maxLives;
            isVisible = true;

        }

        public void setInStartPosition()
        {
            currentTexture = 0;
            position.X = ((screenBounds.Width / 2) - (textures[currentTexture].Width / 2));
            position.Y = ((screenBounds.Height) * 2 + (textures[currentTexture].Height / 2));
        }

        public void Update(GameTime gameTime)
        {
            position += motion * shipSpeed;
            LockToScreen();
        }



        private void LockToScreen()
        {
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 0;
            if (position.X > screenBounds.Width - textures[currentTexture].Width)
                position.X = screenBounds.Width - textures[currentTexture].Width;
            if (position.Y > screenBounds.Height - textures[currentTexture].Height)
                position.Y = screenBounds.Height - textures[currentTexture].Height;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {


                spriteBatch.Draw(textures[currentTexture], new Rectangle((int)position.X, (int)position.Y, textures[currentTexture].Width, textures[currentTexture].Height), Color.White);
            }
        }


    }
}
