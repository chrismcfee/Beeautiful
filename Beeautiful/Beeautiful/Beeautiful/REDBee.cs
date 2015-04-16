using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beeautiful
{
    public class REDBee
    {
        Texture2D texture;
        Vector2 position;
        Vector2 motion;
        bool visible = true;
        float health = 20;
        public float baseHealth = 20;
        
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
        }

        public REDBee(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            this.motion = Vector2.Zero;
            
        }

        public void Damage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                visible = false;
                int credit = 3;
                Game1.instance.kills += credit;
                Game1.instance.playerScore += credit;
                Game1.instance.Notifications.Add(new Notification("+" + credit * 100, 300, position));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
                spriteBatch.Draw(texture, position, Color.White);
        }

        internal void Update(GameTime gameTime)
        {
            

            motion.X = 0;
            motion.Y = 1;
               float movement = position.X - Game1.instance.User.Position.X;
            if (position.X > 0 && position.X < Game1.instance.User.Position.X)
            {
                //if (movement > 0)
                    motion.X += .2f;
                //else
                 //   motion.X += .5f;
                
                    //motion.X -= -1.0f;
            }
            if (position.X > Game1.instance.User.Position.X)
            {
                //if (movement > 0)
                //    motion.X -= .5f;
                //else
                    motion.X -= .2f;

                //motion.X -= -1.0f;
            }
                //else {
                //    motion.X += 1.0f;
           // }
            position += motion * gameTime.ElapsedGameTime.Milliseconds / 2;    
        }
            

        
    }
}