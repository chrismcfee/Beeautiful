using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beeautiful
{
    public class EnemySting : Sting
    {
        public EnemySting(Texture2D texture, Vector2 position) : base(texture, position, 0)
        {
            this.texture = texture;
            this.position = position;
            this.motion = Vector2.Zero;
        }

        public override void Update()
        {
            if (position.Y < 0)
                visible = false;
            motion.Y = 1;
            position += motion * speed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
                spriteBatch.Draw(texture, new Vector2(position.X + Game1.instance.enemyShip.Width / 2 - Game1.instance.stingGreen.Width / 2, position.Y + 30), Color.White);
        }
    }
}
