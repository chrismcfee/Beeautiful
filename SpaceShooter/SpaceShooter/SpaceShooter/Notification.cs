using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    public class Notification
    {
        #region Variables

        static int colorSelect = 0;

        string text;
        Rectangle screenBounds;
        double lifeSpan;
        double lived = 0;
        bool visible;
        bool centered = true;
        Vector2 position;
        Color color = Color.White;

        #endregion

        public String Text
        {
            get { return text; }
        }

        public bool Visible
        {
            get { return visible; }
        }

        public Notification(String text, double life, Rectangle screenBounds)
        {
            this.text = text;
            this.screenBounds = screenBounds;
            this.lifeSpan = life;
            visible = true;
        }
        public Notification(String text, double life, Vector2 position)
        {
            this.text = text;
            this.lifeSpan = life;
            this.position = position;
            this.centered = false;
            visible = true;
            colorSelect += 1;
            if (colorSelect > 4)
                colorSelect = 0;
            switch (colorSelect)
            {
                case 0:
                    color = Color.White;
                    break;
                case 1:
                    color = Color.Blue;
                    break;
                case 2:
                    color = Color.Green;
                    break;
                case 3:
                    color = Color.Red;
                    break;
                case 4:
                    color = Color.Purple;
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            this.lived += gameTime.ElapsedGameTime.Milliseconds;
            if (lived > lifeSpan)
                visible = false;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            if (visible)
            {
                if (centered)
                {
                    spriteBatch.DrawString(spriteFont, text, new Vector2(screenBounds.Width / 2 - spriteFont.MeasureString(text).X / 2, screenBounds.Height / 3 - spriteFont.MeasureString(text).Y / 2), color);
                }
                else
                {
                    spriteBatch.DrawString(spriteFont, text, position, color);
                }
            }
        }
    }
}
