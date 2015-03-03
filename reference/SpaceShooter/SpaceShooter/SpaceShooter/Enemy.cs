using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    public class Enemy
    {
        Texture2D texture;
        Vector2 position;
        Vector2 motion;
        bool visible = true;
        float health = 80;
        public float baseHealth = 80;
        double shotInterval;
        double timeSinceLastShot;

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

        public Enemy(Texture2D texture, Vector2 position, double fireInterval, double fireDelay)
        {
            this.texture = texture;
            this.position = position;
            this.motion = Vector2.Zero;
            this.timeSinceLastShot = fireDelay;
            this.shotInterval = fireInterval;
            if (fireDelay > fireInterval)
                this.shotInterval = fireDelay - fireInterval;
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
                Game1.instance.Notifications.Add(new Notification("+"+credit*100, 300, position));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(visible)
                spriteBatch.Draw(texture, position, Color.White);
        }

        internal void Update(GameTime gameTime)
        {
            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastShot >= shotInterval && position.Y < Game1.instance.User.Position.Y)
                Shoot();

            motion.X = 0;
            motion.Y = 1;
            float movement = position.X - Game1.instance.User.Position.X;
            if (position.Y > 0 && position.Y < Game1.instance.User.Position.Y)
            {
                if (movement > 0)
                    motion.X = -.5f;
                else
                    motion.X = .5f;
            }
            position += motion * gameTime.ElapsedGameTime.Milliseconds / 10;
        }

        public void Shoot()
        {
            timeSinceLastShot = 0;
            Game1.instance.Lasers.Add(new EnemyLaser(Game1.instance.laserGreen, position));
        }
    }
}
