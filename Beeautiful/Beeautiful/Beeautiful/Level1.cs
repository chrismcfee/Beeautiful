using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;

namespace Beeautiful
{
    class Level1 : Level 
    {
        //float Timer = 5000; // milliseconds
        #region Variables
        

        public static Level1 instance;

        GraphicsDeviceManager graphics;
        

        SpriteBatch spriteBatch;
        SpriteFont scoreFont;
        Player player;
        //bool DoesSpawnBoss = false;

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
        public Texture2D boss1Texture;

        public Texture2D stingRed;
        public Texture2D stingGreen;

        public Texture2D beatleBig;
        public Texture2D beatleSmall;

        //Explosions for sting-beatle collisions
        Texture2D explosionTexture;
        Texture2D explosionTextureGreen;

        Song backgroundMusic;

        #endregion

        List<Explosion> explosions;
        List<Beatle> beatles;
        List<Notification> notifications;
        List<BackgroundElement> backgroundObjects;
        List<Enemy> enemies;
        List<Sting> stings;

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

        public List<Beatle> Beatles
        {
            get { return beatles; }
        }

        public List<Notification> Notifications
        {
            get { return notifications; }
        }

        public List<Enemy> Enemies
        {
            get { return enemies; }
        }

        public List<Sting> Stings
        {
            get { return stings; }
        }

        
        #endregion

        public Level1(Game g)
        {
           
           //Level1.instance = this;
           //boss = new List<Boss1>();
            beatles = new List<Beatle>();
            explosions = new List<Explosion>();
            notifications = new List<Notification>();
            backgroundElements = new List<Texture2D>();
            backgroundObjects = new List<BackgroundElement>();
            enemies = new List<Enemy>();
            stings = new List<Sting>();
            LevelState = LEVELSTATE.StartMenu;
            //textures
            #region
            scoreFont = g.Content.Load<SpriteFont>("score");
            spriteBatch = new SpriteBatch(g.GraphicsDevice);

            //level1 background
            background = g.Content.Load<Texture2D>("background_level_1");
            backgroundElements.Add(g.Content.Load<Texture2D>("Sprites/speedLine"));
            backgroundElements.Add(g.Content.Load<Texture2D>("Sprites/bigcloud"));
            backgroundElements.Add(g.Content.Load<Texture2D>("Sprites/smallcloud"));
            blank = g.Content.Load<Texture2D>("Sprites/blank");

            enemyShip = g.Content.Load<Texture2D>("Sprites/attacking_red_bee_enemy");

            backgroundMusic = g.Content.Load<Song>("Audio/Map1");

            //player textures
            List<Texture2D> shipTextures = new List<Texture2D>();
            shipTextures.Add(g.Content.Load<Texture2D>("Sprites/JerryCenter/Jerry_Center"));
            shipTextures.Add(g.Content.Load<Texture2D>("Sprites/JerryLeft/Jerry_left"));
            shipTextures.Add(g.Content.Load<Texture2D>("Sprites/JerryRight/Jerry_right"));
            playerLivesGraphic = g.Content.Load<Texture2D>("Sprites/lifeicon");
            playerShield = g.Content.Load<Texture2D>("Sprites/shield");

            //Stings
            stingRed = g.Content.Load<Texture2D>("Sprites/enemy_bee_stinger");
            stingGreen = g.Content.Load<Texture2D>("Sprites/Jerry_bee_stinger");

            //Beatles
            beatleBig = g.Content.Load<Texture2D>("Sprites/beatleBig");
            beatleSmall = g.Content.Load<Texture2D>("Sprites/beatleSmall");

            //Explosions
            explosionTexture = g.Content.Load<Texture2D>("Sprites/stingRedShot");
            explosionTextureGreen = g.Content.Load<Texture2D>("Sprites/stingGreenShot");

            //Boses
            boss1Texture = g.Content.Load<Texture2D>("Sprites/boss_placeholder");

            player = new Player(shipTextures,Game1.instance.screenBounds);

            PrepareLevel();

            // MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(backgroundMusic);

            LevelState= LEVELSTATE.StartMenu;
            #endregion
        }
        private void PrepareLevel()
        {
            player.Reset();

            kills = 0;
            playerScore = 0;

            beatles.Clear();
            enemies.Clear();
            stings.Clear();
            notifications.Clear();
            explosions.Clear();

            //Initialize random beatles
            Random rand = new Random();
            int randomAmt = rand.Next(400, 400);
            for (int i = 0; i < randomAmt; i++)
            {
                bool bigBeatle = (rand.Next() % 2 == 0) ? true : false;
                float speed = !bigBeatle ? rand.Next(1, 4) : rand.Next(2, 8);
                beatles.Add(new Beatle(bigBeatle, speed, new Vector2(rand.Next(0, Game1.instance.screenBounds.Width), rand.Next(-10000, 0))));
            }

            int randomEnemies = rand.Next(120, 120);
            for (int i = 0; i < randomEnemies; i++)
            {
                enemies.Add(new Enemy(enemyShip, new Vector2(rand.Next(0, Game1.instance.screenBounds.Width), rand.Next(-10000, 0)), rand.Next(8, 16) * 1000, rand.Next(2, 20) / 3 * 100));
            }


        }

        public override void Update(GameTime t)
        {
            timeSinceStateChange += t.ElapsedGameTime.Milliseconds;
            //Game level keybindings
            KeyboardState keyboardState = Keyboard.GetState();

            if (LevelState != LEVELSTATE.Paused)
            {
                //Update background elements
                if (backgroundObjects.Count < 15)
                    backgroundObjects.Add(new BackgroundElement(backgroundElements,Game1.instance.screenBounds));

                //Update background objects
                for (int i = backgroundObjects.Count - 1; i >= 0; i--)
                {
                    backgroundObjects[i].Update(t);
                    if (backgroundObjects[i].BelowScreen)
                        backgroundObjects.RemoveAt(i);
                }
            }

            switch (LevelState)
            {
                case LEVELSTATE.Paused:
                    {
                        if (keyboardState.IsKeyDown(Keys.Escape))
                        {
                            if (CanChangeState)
                            {
                                MediaPlayer.Resume();
                                LevelState = LEVELSTATE.Running;
                                return;
                            }
                        }
                        if (keyboardState.IsKeyDown(Keys.Enter))
                        {
                            if (CanChangeState)
                            {
                                LevelState = LEVELSTATE.GameOver;
                                MediaPlayer.Resume();
                                return;
                            }
                        }
                        if (keyboardState.IsKeyDown(Keys.F))
                        {
                            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                            this.graphics.IsFullScreen = true;
                        }
                        break;
                    }
                case LEVELSTATE.GameOver:
                    {
                        timeSinceLastFlash += t.ElapsedGameTime.Milliseconds;
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
                                LevelState = LEVELSTATE.Running;
                            }
                            if (keyboardState.IsKeyDown(Keys.Escape))
                            {
                                LevelState = LEVELSTATE.GameOver;
                            }
                        }
                        break;
                    }
                case LEVELSTATE.StartMenu:
                    {
                        timeSinceLastFlash += t.ElapsedGameTime.Milliseconds;
                        if (timeSinceLastFlash > flashInterval)
                        {
                            flashing = !flashing;
                            timeSinceLastFlash = 0;
                        }
                        if (keyboardState.IsKeyDown(Keys.Enter))
                        {
                            LevelState = LEVELSTATE.Running;
                        }
                        if (keyboardState.IsKeyDown(Keys.Escape))
                            LevelState = LEVELSTATE.GameOver;
                        break;
                    }
                case LEVELSTATE.Running:
                    {
                        if (CanChangeState)
                        {
                            if (keyboardState.IsKeyDown(Keys.Escape))
                            {
                                MediaPlayer.Pause();
                                LevelState = LEVELSTATE.Paused;
                                return;
                            }

                        }
                        if (player.Lives < 0)
                        {
                            LevelState = LEVELSTATE.GameOver;
                            return;
                        }

                        //Update background elements
                        if (backgroundObjects.Count < 15)
                            backgroundObjects.Add(new BackgroundElement(backgroundElements, Game1.instance.screenBounds));

                        //Update background objects
                        for (int i = backgroundObjects.Count - 1; i >= 0; i--)
                        {
                            backgroundObjects[i].Update(t);
                            if (backgroundObjects[i].BelowScreen)
                                backgroundObjects.RemoveAt(i);
                        }

                        //Update player position, check keypresses
                        player.Update(t);

                        //Modify enemy and beatle health at higher sting levels
                        if (player.StingLevel != 0)
                        {
                            foreach (Enemy enemy in enemies)
                            {
                                if (enemy.Bounds.Y < 0)
                                    enemy.Health = enemy.baseHealth * 1.1f * player.StingLevel;
                            }
                            foreach (Beatle beatle in beatles)
                            {
                                if (beatle.Bounds.Y < 0)
                                    beatle.Health = beatle.baseHealth * 1.1f * player.StingLevel;
                            }
                        }

                        //Update notifications
                        for (int i = notifications.Count - 1; i > -1; i--)
                        {
                            notifications[i].Update(t);
                            if (!notifications[i].Visible)
                                notifications.RemoveAt(i);
                        }

                        //Update stings and all things stings can collide with
                        for (int i = stings.Count - 1; i > -1; i--)
                        {
                            stings[i].Update();
                            //Update and remove enemies as needed
                            for (int j = enemies.Count - 1; j > -1; j--)
                            {
                                //Enemy-Player collision
                                if (i == 0)
                                {
                                    enemies[j].Update(t);
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

                                //Sting-enemy collision
                                if (stings[i].Visible && enemies[j].Visible && !(stings[i] is EnemySting) && stings[i].Bounds.Intersects(enemies[j].Bounds))
                                {
                                    stings[i].Visible = false;
                                    enemies[j].Damage((int)stings[i].Damage);
                                    Texture2D expTexToUse = player.StingLevel == 0 ? explosionTexture : explosionTextureGreen;
                                    explosions.Add(new Explosion(expTexToUse, stings[i].Position));
                                }

                                if (!enemies[j].Visible)
                                {
                                    enemies.RemoveAt(j);
                                }
                            }

                            //Sting-Beatle collisions
                            for (int q = beatles.Count - 1; q > -1; q--)
                            {
                                if (i == 0)
                                {
                                    beatles[q].Update();
                                    if (beatles[q].Visible && beatles[q].Bounds.Intersects(player.Bounds) && !player.Invincible)
                                    {
                                        player.Lives = player.Lives - 1;
                                        beatles[q].Visible = false;
                                        player.Respawn();
                                    }

                                    if (beatles[q].Visible && beatles[q].Bounds.Intersects(player.Bounds) && player.Shielded)
                                    {
                                        beatles[q].Damage(50);
                                    }
                                }
                                if (stings[i].Visible && stings[i] is EnemySting && beatles[q].Visible && stings[i].Bounds.Intersects(beatles[q].Bounds))
                                    stings[i].Visible = false;
                                if (stings[i].Visible && beatles[q].Visible && !(stings[i] is EnemySting) && stings[i].Bounds.Intersects(beatles[q].Bounds))
                                {
                                    stings[i].Visible = false;
                                    beatles[q].Damage(stings[i].Damage);
                                    Texture2D expTexToUse = player.StingLevel == 0 ? explosionTexture : explosionTextureGreen;
                                    explosions.Add(new Explosion(expTexToUse, stings[i].Position));
                                }
                                if (!beatles[q].Visible)
                                {
                                    beatles.RemoveAt(q);
                                }
                            }

                            //Sting-Player collisions
                            if (stings[i].Visible && stings[i] is EnemySting && stings[i].Bounds.Intersects(player.Bounds))
                            {
                                if (player.Shielded)
                                    stings[i].Visible = false;
                                else
                                {
                                    player.Lives = player.Lives - 1;
                                    stings[i].Visible = false;
                                    player.Respawn();
                                }
                            }

                            if (!stings[i].Visible)
                                stings.RemoveAt(i);
                        }

                        if (stings.Count == 0)
                        {
                            foreach (Enemy enemy in enemies)
                                enemy.Update(t);
                            foreach (Beatle beatle in beatles)
                                beatle.Update();
                        }

                        for (int i = explosions.Count - 1; i > -1; i--)
                        {
                            explosions[i].Update(t);
                            if (!explosions[i].Visible)
                                explosions.RemoveAt(i);
                        }

                        base.Update(t);
                        break;
                    }
                default:
                    break;
            
            }
            
        }            

        

        public override void Draw(SpriteBatch sp)
        {
        
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, sp.GraphicsDevice.Viewport.Width, sp.GraphicsDevice.Viewport.Height), Color.White);

            foreach (BackgroundElement element in backgroundObjects)
                element.Draw(spriteBatch);

            switch (LevelState)
            {
                case LEVELSTATE.GameOver:
                    {
                        spriteBatch.DrawString(scoreFont, "Game Over", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Game Over").X / 2, (int)Game1.instance.screenBounds.Height / 4), Color.White);
                        spriteBatch.DrawString(scoreFont, "Score: " + playerScore * 100, new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Score: " + playerScore * 100).X / 2, (int)Game1.instance.screenBounds.Height / 4 + scoreFont.MeasureString("Score: " + playerScore * 100).Y), Color.White);
                        Color flashColor = flashing ? Color.White : Color.Yellow;
                        spriteBatch.DrawString(scoreFont, "Press Enter to Play Again", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to Play Again").X / 2, (int)Game1.instance.screenBounds.Height / 3 * 2), flashColor);
                        spriteBatch.DrawString(scoreFont, "Press Escape to Quit", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Press Escape to Quit").X / 2, (int)Game1.instance.screenBounds.Height / 4 * 3), Color.White);
                        break;
                    }
                case LEVELSTATE.StartMenu:
                    {
                        spriteBatch.DrawString(scoreFont, "Beeautiful", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Beeautiful").X / 2, (int)Game1.instance.screenBounds.Height / 4), Color.White);
                        spriteBatch.DrawString(scoreFont, "Developed by the Java Tigers", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Developed by the Java Tigers").X / 2, (int)Game1.instance.screenBounds.Height / 4 + scoreFont.MeasureString("Developed by the Java Tigers").Y), Color.White);
                        Color flashColor = flashing ? Color.White : Color.Yellow;
                        spriteBatch.DrawString(scoreFont, "Press Enter to Play", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to Play").X / 2, (int)Game1.instance.screenBounds.Height / 3 * 2), flashColor);
                        spriteBatch.DrawString(scoreFont, "Press Escape to Quit", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Press Escape to Quit").X / 2, (int)Game1.instance.screenBounds.Height / 4 * 3), Color.White);
                        spriteBatch.DrawString(scoreFont, "Press F for Full Screen (Coming Soon)", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Press F for Full Screen (Coming Soon)").X / 2, (int)Game1.instance.screenBounds.Height / 5 * 4), Color.White);

                        break;
                    }
                case LEVELSTATE.Paused:
                    {
                        spriteBatch.DrawString(scoreFont, "Paused", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Paused").X / 2, (int)Game1.instance.screenBounds.Height / 3), Color.White);
                        spriteBatch.DrawString(scoreFont, "Press Enter to End Game", new Vector2((int)Game1.instance.screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to End Game").X / 2, (int)Game1.instance.screenBounds.Height / 2), Color.White);
                        goto case LEVELSTATE.Running;
                    }
                case LEVELSTATE.Running:
                    {
                        player.Draw(spriteBatch);

                        foreach (Beatle beatle in beatles)
                            beatle.Draw(spriteBatch);

                        foreach (Enemy enemy in enemies)
                            enemy.Draw(spriteBatch);

                        foreach (Sting sting in stings)
                            sting.Draw(spriteBatch);

                        foreach (Explosion explosion in explosions)
                            explosion.Draw(spriteBatch);

                        for (int i = 0; i < player.Lives; i++)
                            spriteBatch.Draw(playerLivesGraphic, new Rectangle(40 * i + 10, 10, playerLivesGraphic.Width, playerLivesGraphic.Height), Color.White);

                        string scoreText = "" + playerScore * 100;
                        spriteBatch.DrawString(scoreFont, scoreText, new Vector2(Game1.instance.screenBounds.Width - scoreFont.MeasureString(scoreText).X - 30, 5), Color.White);

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
            base.Draw(sp);
        }
    }

 }
    

