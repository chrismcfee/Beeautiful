using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooter
{
    public class Player
    {
        #region Variables

        Vector2 position;
        Vector2 motion;

        float shipSpeed = 5.0f;

        int lives;
        int maxLives = 5;

        bool isInvincible;
        bool isVisible;
        bool shielded;
        bool shieldCooldown;
        double timeSinceRespawn;
        double timeInvincibleAfterRespawn = 3000;
        double shieldPower = 3000;
        double maxShieldPower = 3000;
        double shieldRegenRate = 0.3f;
        double shieldDepleteRate = 1.0f;

        List<Texture2D> textures;
        int currentTexture = 0;

        Rectangle screenBounds;

        int laserLevel = 0;

        double lastFireTime = 0;

        KeyboardState keyboardState;

        #endregion

        #region Fields

        public bool ShieldCooldown
        {
            get { return shieldCooldown; }
        }

        public double MaxShieldPower
        {
            get { return maxShieldPower; }
        }

        public double ShieldPower 
        {
            get { return shieldPower; }
        }

        public bool Shielded
        {
            get { return shielded; }
        }

        public int LaserLevel
        {
            get { return laserLevel; }
        }

        public bool Invincible
        {
            get { return isInvincible; }
        }

        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, textures[currentTexture].Width, textures[currentTexture].Height); }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        #endregion

        public Player(List<Texture2D> textures, Rectangle screenBounds)
        {
            this.textures = textures;
            this.screenBounds = screenBounds;
            this.lives = maxLives;
            isVisible = true;
            shielded = false;
            shieldCooldown = false;
        }

        public void setInStartPosition()
        {
            currentTexture = 0;
            position.X = ((screenBounds.Width / 2) - (textures[currentTexture].Width / 2));
            position.Y = ((screenBounds.Height / 3) * 2 + (textures[currentTexture].Height / 2));
        }

        public void Update(GameTime gameTime)
        {
            if (!shielded && shieldPower < maxShieldPower)
                shieldPower += shieldRegenRate * gameTime.ElapsedGameTime.Milliseconds;
            if (shieldPower <= 0)
            {
                shielded = false;
                shieldCooldown = true;
                shieldPower = 0;
            }
            if (shieldPower >= maxShieldPower)
            {
                shieldPower = maxShieldPower;
                shieldCooldown = false;
            }
            if(shielded && !(timeSinceRespawn < timeInvincibleAfterRespawn))
                shieldPower -= shieldDepleteRate * gameTime.ElapsedGameTime.Milliseconds;
            

            if (Game1.instance.kills > 20 && laserLevel == 0)
            {
                laserLevel = 1;
                Game1.instance.Notifications.Add(new Notification("Lasers Improved", 2000, screenBounds));
            }
            if (Game1.instance.kills > 50 && laserLevel == 1)
            {
                laserLevel = 2;
                Game1.instance.Notifications.Add(new Notification("Lasers Improved", 2000, screenBounds));
            }
            if (Game1.instance.kills > 100 && laserLevel == 2)
            {
                laserLevel = 3;
                Game1.instance.Notifications.Add(new Notification("Lasers Improved", 2000, screenBounds));
            }

            timeSinceRespawn += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceRespawn > timeInvincibleAfterRespawn && isInvincible)
            {
                isInvincible = false;
            }
            else if (timeSinceRespawn < timeInvincibleAfterRespawn && lives < maxLives)
            {
                if (timeSinceRespawn % 10 == 0)
                {
                    isVisible = false;
                }
                else
                {
                    isVisible = true;
                }
            }

            lastFireTime += gameTime.ElapsedGameTime.Milliseconds;
            motion = Vector2.Zero;
            keyboardState = Keyboard.GetState();

            if (!shieldCooldown)
            {
                if (keyboardState.IsKeyDown(Keys.LeftControl))
                    Shield(true);
                else
                    Shield(false);
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !shielded)
                Shoot(gameTime);

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                currentTexture = 1;
                motion.X = -1;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                currentTexture = 2;
                motion.X = 1;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
                    currentTexture = 0;
                motion.Y = -1;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
                    currentTexture = 0;
                motion.Y = 1;
            }
            if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
                currentTexture = 0;

            position += motion * shipSpeed;
            LockToScreen();
        }

        private void Shield(bool active)
        {
            if (active)
            {
                if (shieldPower >= 0)
                {
                    shielded = true;
                    isInvincible = true;
                    return;
                }
            }
            shielded = false;
            isInvincible = false;
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

        private void Shoot(GameTime gameTime)
        {
            if (lastFireTime > 50)
            {
                Texture2D laser = Game1.instance.laserRed;
                if (laserLevel >= 1)
                {
                    laser = Game1.instance.laserGreen;
                }
                if (laserLevel < 2)
                {
                    Game1.instance.Lasers.Add(new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 2 - laser.Width / 2, position.Y - 30), laserLevel));
                }
                if (laserLevel == 2)
                {
                    Game1.instance.Lasers.Add(new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 - laser.Width / 2, position.Y - 30), laserLevel));
                    Game1.instance.Lasers.Add(new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 * 2 - laser.Width / 2, position.Y - 30), laserLevel));
                }
                if (laserLevel == 3)
                {
                    Game1.instance.Lasers.Add(new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 - laser.Width / 2, position.Y - 30), laserLevel));
                    Game1.instance.Lasers.Add(new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 * 2 - laser.Width / 2, position.Y - 30), laserLevel));
                    Laser right = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 - laser.Width / 2, position.Y - 30), laserLevel);
                    Laser left = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 * 2 - laser.Width / 2, position.Y - 30), laserLevel);
                    right.motion.X = 1;
                    right.motion.Y = -1;
                    left.motion.X = -1;
                    left.motion.Y = -1;
                    Game1.instance.Lasers.Add(right);
                    Game1.instance.Lasers.Add(left);
                }
                lastFireTime = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                if (shielded)
                {
                    Texture2D shield = Game1.instance.playerShield;
                    spriteBatch.Draw(shield, new Rectangle((int)position.X - 25, (int)position.Y - 30, shield.Width, shield.Height), Color.White);
                }
                spriteBatch.Draw(textures[currentTexture], new Rectangle((int)position.X, (int)position.Y, textures[currentTexture].Width, textures[currentTexture].Height), Color.White);
            }
        }

        public void Reset()
        {
            this.lives = maxLives;
            setInStartPosition();
            laserLevel = 0;
            shieldPower = maxShieldPower;
            shieldCooldown = false;
        }

        public void Respawn()
        {
            timeSinceRespawn = 0;
            setInStartPosition();
            isInvincible = true;
            Game1.instance.kills = 0;
            laserLevel = 0;
            shieldCooldown = false;
            shieldPower = maxShieldPower;
            foreach (Enemy enemy in Game1.instance.Enemies)
            {
                if (enemy.Bounds.Y < 0)
                    enemy.Health = enemy.baseHealth;
            }
            foreach (Meteor meteor in Game1.instance.Meteors)
            {
                if (meteor.Bounds.Y < 0)
                    meteor.Health = meteor.baseHealth;
            }
        }
    }
}
