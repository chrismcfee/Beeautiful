using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceShooter
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        enum gameState { Loading, StartMenu, Running, Paused, GameOver, Editor };

        #region Variables

        gameState state;

        public static Game1 instance;

        GraphicsDeviceManager graphics;
        Rectangle screenBounds;

        SpriteBatch spriteBatch;
        SpriteFont scoreFont;

        Player player;

        //To prevent holding down a button from changing state rapidly
        double stateChangeDelay = 100;
        double timeSinceStateChange = 0;

        bool flashing = false;
        double timeSinceLastFlash = 0;
        double flashInterval = 500;

        public int kills = 0;
        public int playerScore = 0;

        #region Textures

        public Texture2D playerShield;
        Texture2D playerLivesGraphic;

        Texture2D background;
        List<Texture2D> backgroundElements;

        Texture2D blank;

        public Texture2D enemyShip;
        
        public Texture2D laserRed;
        public Texture2D laserGreen;
        
        public Texture2D meteorBig;
        public Texture2D meteorSmall;

        //Explosions for laser-meteor collisions
        Texture2D explosionTexture;
        Texture2D explosionTextureGreen;

        Song backgroundMusic;

        #endregion

        List<Explosion> explosions;
        List<Meteor> meteors;
        List<Notification> notifications;
        List<BackgroundElement> backgroundObjects;
        List<Enemy> enemies;
        List<Laser> lasers;

        #endregion

        #region Fields

        public bool CanChangeState
        {
            get
            {
                if (timeSinceStateChange > stateChangeDelay)
                {
                    timeSinceStateChange = 0;
                    return true;
                }
                return false;
            }
        }

        public Player User
        {
            get { return player; }
        }

        public List<Explosion> Explosions
        {
            get { return Explosions; }
        }

        public List<Meteor> Meteors
        {
            get { return meteors; }
        }

        public List<Notification> Notifications
        {
            get { return notifications; }
        }

        public List<Enemy> Enemies
        {
            get { return enemies; }
        }

        public List<Laser> Lasers
        {
            get { return lasers; }
        }

        #endregion


        public Game1()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            screenBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            meteors = new List<Meteor>();
            explosions = new List<Explosion>();
            notifications = new List<Notification>();
            backgroundElements = new List<Texture2D>();
            backgroundObjects = new List<BackgroundElement>();
            enemies = new List<Enemy>();
            lasers = new List<Laser>();
            state = gameState.Loading;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Spritefont for scores & notifications
            scoreFont = Content.Load<SpriteFont>("score");
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Purple background
            background = Content.Load<Texture2D>("backgroundColor");
            backgroundElements.Add(Content.Load<Texture2D>("speedLine"));
            backgroundElements.Add(Content.Load<Texture2D>("starBig"));
            backgroundElements.Add(Content.Load<Texture2D>("starSmall"));
            blank = Content.Load<Texture2D>("blank");

            enemyShip = Content.Load<Texture2D>("enemyShip");

            backgroundMusic = Content.Load<Song>("loop-transit");

            //Ship textures
            List<Texture2D> shipTextures = new List<Texture2D>();
            shipTextures.Add(Content.Load<Texture2D>("player"));
            shipTextures.Add(Content.Load<Texture2D>("playerleft"));
            shipTextures.Add(Content.Load<Texture2D>("playerright"));
            playerLivesGraphic = Content.Load<Texture2D>("life");
            playerShield = Content.Load<Texture2D>("shield");

            //Lasers
            laserRed = Content.Load<Texture2D>("laserRed");
            laserGreen = Content.Load<Texture2D>("laserGreen");

            //Meteors
            meteorBig = Content.Load<Texture2D>("meteorBig");
            meteorSmall = Content.Load<Texture2D>("meteorSmall");

            //Explosions
            explosionTexture = Content.Load<Texture2D>("laserRedShot");
            explosionTextureGreen = Content.Load<Texture2D>("laserGreenShot");

            player = new Player(shipTextures, screenBounds);

            PrepareLevel();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);

            state = gameState.StartMenu;
        }

        protected override void UnloadContent()
        {
        }

        private void PrepareLevel()
        {
            player.Reset();

            kills = 0;
            playerScore = 0;

            meteors.Clear();
            enemies.Clear();
            lasers.Clear();
            notifications.Clear();
            explosions.Clear();

            //Initialize random meteors
            Random rand = new Random();
            int randomAmt = rand.Next(100, 300);
            for (int i = 0; i < randomAmt; i++)
            {
                bool bigMeteor = (rand.Next() % 2 == 0) ? true : false;
                float speed = !bigMeteor ? rand.Next(2, 8) : rand.Next(1, 4);
                meteors.Add(new Meteor(bigMeteor, speed, new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-10000, 0))));
            }

            int randomEnemies = rand.Next(30, 100);
            for (int i = 0; i < randomEnemies; i++)
            {
                enemies.Add(new Enemy(enemyShip, new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-10000, 0)), rand.Next(2, 4) * 1000, rand.Next(1, 10) / 3 * 100));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            timeSinceStateChange += gameTime.ElapsedGameTime.Milliseconds;
            //Game level keybindings
            KeyboardState keyboardState = Keyboard.GetState();

            if (state != gameState.Paused)
            {
                //Update background elements
                if (backgroundObjects.Count < 15)
                    backgroundObjects.Add(new BackgroundElement(backgroundElements, screenBounds));

                //Update background objects
                for (int i = backgroundObjects.Count - 1; i >= 0; i--)
                {
                    backgroundObjects[i].Update(gameTime);
                    if (backgroundObjects[i].BelowScreen)
                        backgroundObjects.RemoveAt(i);
                }
            }

            switch(state)
            {
                case gameState.Paused:
                    {
                        if (keyboardState.IsKeyDown(Keys.Escape))
                        {
                            if (CanChangeState)
                            {
                                MediaPlayer.Resume();
                                state = gameState.Running;
                                return;
                            }
                        }
                        if (keyboardState.IsKeyDown(Keys.Enter))
                        {
                            if (CanChangeState)
                            {
                                state = gameState.GameOver;
                                MediaPlayer.Resume();
                                return;
                            }
                        }
                        break;
                    }
                case gameState.GameOver:
                    {
                        timeSinceLastFlash += gameTime.ElapsedGameTime.Milliseconds;
                        if (timeSinceLastFlash > flashInterval)
                        {
                            flashing = !flashing;
                            timeSinceLastFlash = 0;
                        }

                        if (CanChangeState)
                        {
                            if (keyboardState.IsKeyDown(Keys.Enter))
                            {
                                PrepareLevel();
                                state = gameState.Running;
                            }
                            if (keyboardState.IsKeyDown(Keys.Escape))
                            {
                                Exit();
                            }
                        }
                        break;
                    }
                case gameState.StartMenu:
                    {
                        timeSinceLastFlash += gameTime.ElapsedGameTime.Milliseconds;
                        if (timeSinceLastFlash > flashInterval)
                        {
                            flashing = !flashing;
                            timeSinceLastFlash = 0;
                        }
                        if (keyboardState.IsKeyDown(Keys.Enter))
                        {
                            state = gameState.Running;
                        }
                        if (keyboardState.IsKeyDown(Keys.Escape))
                            Exit();
                        break;
                    }
                case gameState.Running: 
                {
                    if (CanChangeState)
                    {
                        if (keyboardState.IsKeyDown(Keys.Escape))
                        {
                            MediaPlayer.Pause();
                            state = gameState.Paused;
                            return;
                        }
                    }
                    if (player.Lives < 0)
                    {
                        state = gameState.GameOver;
                        return;
                    }

                    //Update background elements
                    if (backgroundObjects.Count < 15)
                        backgroundObjects.Add(new BackgroundElement(backgroundElements, screenBounds));

                    //Update background objects
                    for (int i = backgroundObjects.Count - 1; i >= 0; i--)
                    {
                        backgroundObjects[i].Update(gameTime);
                        if (backgroundObjects[i].BelowScreen)
                            backgroundObjects.RemoveAt(i);
                    }

                    //Update player position, check keypresses
                    player.Update(gameTime);

                    //Modify enemy and meteor health at higher laser levels
                    if (player.LaserLevel != 0)
                    {
                        foreach (Enemy enemy in enemies)
                        {
                            if (enemy.Bounds.Y < 0)
                                enemy.Health = enemy.baseHealth * 1.2f * player.LaserLevel;
                        }
                        foreach (Meteor meteor in meteors)
                        {
                            if (meteor.Bounds.Y < 0)
                                meteor.Health = meteor.baseHealth * 1.2f * player.LaserLevel;
                        }
                    }

                    //Update notifications
                    for (int i = notifications.Count - 1; i > -1; i--)
                    {
                        notifications[i].Update(gameTime);
                        if (!notifications[i].Visible)
                            notifications.RemoveAt(i);
                    }

                    //Update lasers and all things lasers can collide with
                    for (int i = lasers.Count - 1; i > -1; i--)
                    {
                        lasers[i].Update();
                        //Update and remove enemies as needed
                        for (int j = enemies.Count - 1; j > -1; j--)
                        {
                            //Enemy-Player collision
                            if (i == 0)
                            {
                                enemies[j].Update(gameTime);
                                if (enemies[j].Visible && enemies[j].Bounds.Intersects(player.Bounds))
                                {
                                    if (player.Shielded)
                                        enemies[j].Damage(50);
                                    else
                                    {
                                        player.Lives = player.Lives - 1;
                                        enemies[j].Visible = false;
                                        player.Respawn();
                                    }
                                }
                            }

                            //Laser-enemy collision
                            if (lasers[i].Visible && enemies[j].Visible && !(lasers[i] is EnemyLaser) && lasers[i].Bounds.Intersects(enemies[j].Bounds))
                            {
                                lasers[i].Visible = false;
                                enemies[j].Damage((int)lasers[i].Damage);
                                Texture2D expTexToUse = player.LaserLevel == 0 ? explosionTexture : explosionTextureGreen;
                                explosions.Add(new Explosion(expTexToUse, lasers[i].Position));
                            }

                            if (!enemies[j].Visible)
                                enemies.RemoveAt(j);
                        }

                        //Laser-Meteor collisions
                        for (int q = meteors.Count - 1; q > -1; q--)
                        {
                            if (i == 0)
                            {
                                meteors[q].Update();
                                if (meteors[q].Visible && meteors[q].Bounds.Intersects(player.Bounds) && !player.Invincible)
                                {
                                    player.Lives = player.Lives - 1;
                                    meteors[q].Visible = false;
                                    player.Respawn();
                                }

                                if (meteors[q].Visible && meteors[q].Bounds.Intersects(player.Bounds) && player.Shielded)
                                {
                                    meteors[q].Damage(50);
                                }
                            }
                            if (lasers[i].Visible && lasers[i] is EnemyLaser && meteors[q].Visible && lasers[i].Bounds.Intersects(meteors[q].Bounds))
                                lasers[i].Visible = false;
                            if (lasers[i].Visible && meteors[q].Visible && !(lasers[i] is EnemyLaser) && lasers[i].Bounds.Intersects(meteors[q].Bounds))
                            {
                                lasers[i].Visible = false;
                                meteors[q].Damage(lasers[i].Damage);
                                Texture2D expTexToUse = player.LaserLevel == 0 ? explosionTexture : explosionTextureGreen;
                                explosions.Add(new Explosion(expTexToUse, lasers[i].Position));
                            }
                            if (!meteors[q].Visible)
                                meteors.RemoveAt(q);
                        }

                        //Laser-Player collisions
                        if (lasers[i].Visible && lasers[i] is EnemyLaser && lasers[i].Bounds.Intersects(player.Bounds))
                        {
                            if (player.Shielded)
                                lasers[i].Visible = false;
                            else
                            {
                                player.Lives = player.Lives - 1;
                                lasers[i].Visible = false;
                                player.Respawn();
                            }
                        }

                        if (!lasers[i].Visible)
                            lasers.RemoveAt(i);
                    }

                    if (lasers.Count == 0)
                    {
                        foreach (Enemy enemy in enemies)
                            enemy.Update(gameTime);
                        foreach (Meteor meteor in meteors)
                            meteor.Update();
                    }

                    for (int i = explosions.Count - 1; i > -1; i--)
                    {
                        explosions[i].Update(gameTime);
                        if (!explosions[i].Visible)
                            explosions.RemoveAt(i);
                    }

                    base.Update(gameTime);
                    break;
                }
                default:
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            foreach (BackgroundElement element in backgroundObjects)
                element.Draw(spriteBatch);

            switch(state)
            {
                case gameState.GameOver:
                    {
                        spriteBatch.DrawString(scoreFont, "Game Over", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Game Over").X / 2, (int)screenBounds.Height / 4), Color.White);
                        spriteBatch.DrawString(scoreFont, "Score: " + playerScore * 100, new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Score: " + playerScore * 100).X / 2, (int)screenBounds.Height / 4 + scoreFont.MeasureString("Score: " + playerScore * 100).Y), Color.White);
                        Color flashColor = flashing ? Color.White : Color.Yellow;
                        spriteBatch.DrawString(scoreFont, "Press Enter to Play Again", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to Play Again").X / 2, (int)screenBounds.Height / 3 * 2), flashColor);
                        spriteBatch.DrawString(scoreFont, "Press Escape to Quit", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Escape to Quit").X / 2, (int)screenBounds.Height / 4 * 3), Color.White);
                        break;
                    }
                case gameState.StartMenu:
                    {
                        spriteBatch.DrawString(scoreFont, "Simple Space Shooter", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Simple Space Shooter").X / 2, (int)screenBounds.Height / 4), Color.White);
                        spriteBatch.DrawString(scoreFont, "By Ddl2829", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("By Ddl2829").X / 2, (int)screenBounds.Height / 4 + scoreFont.MeasureString("By Ddl2829").Y), Color.White);
                        Color flashColor = flashing ? Color.White : Color.Yellow;
                        spriteBatch.DrawString(scoreFont, "Press Enter to Play", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to Play").X / 2, (int)screenBounds.Height / 3 * 2), flashColor);
                        spriteBatch.DrawString(scoreFont, "Press Escape to Quit", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Escape to Quit").X / 2, (int)screenBounds.Height / 4 * 3), Color.White);
                        
                        break;
                    }
                case gameState.Paused:
                    {
                        spriteBatch.DrawString(scoreFont, "Paused", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Paused").X / 2, (int)screenBounds.Height / 3), Color.White);
                        spriteBatch.DrawString(scoreFont, "Press Enter to End Game", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to End Game").X / 2, (int)screenBounds.Height / 2), Color.White);
                        goto case gameState.Running;
                    }
                case gameState.Running:
                    {
                        player.Draw(spriteBatch);

                        foreach (Meteor meteor in meteors)
                            meteor.Draw(spriteBatch);

                        foreach (Enemy enemy in enemies)
                            enemy.Draw(spriteBatch);

                        foreach (Laser laser in lasers)
                            laser.Draw(spriteBatch);

                        foreach (Explosion explosion in explosions)
                            explosion.Draw(spriteBatch);

                        for (int i = 0; i < player.Lives; i++)
                            spriteBatch.Draw(playerLivesGraphic, new Rectangle(40 * i + 10, 10, playerLivesGraphic.Width, playerLivesGraphic.Height), Color.White);

                        string scoreText = "" + playerScore * 100;
                        spriteBatch.DrawString(scoreFont, scoreText, new Vector2(screenBounds.Width - scoreFont.MeasureString(scoreText).X - 30, 5), Color.White);

                        spriteBatch.Draw(blank, new Rectangle(8, 43, (int)player.MaxShieldPower / 30 + 4, 24), Color.Black);
                        spriteBatch.Draw(blank, new Rectangle(10, 45, (int)player.MaxShieldPower / 30, 20), Color.White);
                        spriteBatch.Draw(blank, new Rectangle(10, 45, (int)player.ShieldPower / 30, 20), Color.Blue);

                        if (player.ShieldCooldown)
                            spriteBatch.Draw(blank, new Rectangle(10, 45, (int)player.ShieldPower / 30, 20), Color.Red);

                        foreach (Notification notification in notifications)
                            notification.Draw(spriteBatch, scoreFont);

                        
                        break;
                    }
                
                }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
