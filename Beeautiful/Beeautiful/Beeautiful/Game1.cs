using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Beeautiful
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
        bool DoesSpawnBoss = false;

        //To prevent holding down a button from changing state rapidly
        double stateChangeDelay = 100;
        double timeSinceStateChange = 0;

        bool flashing = false;
        
        double timeSinceLastFlash = 0;
        double flashInterval = 500;

        public int kills = 0;
        public int playerScore = 0;

        public bool isBoss1Dead = false;

        #region Textures

        public Texture2D playerShield;
        Texture2D playerLivesGraphic;

        Texture2D background;
        List<Texture2D> backgroundElements;

        Texture2D blank;

        public Texture2D enemyShip;

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
        List<Boss1> boss1s;
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


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.IsFullScreen = true;
            instance = this;
            //graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
            screenBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            boss1s = new List<Boss1>();
            beatles = new List<Beatle>();
            explosions = new List<Explosion>();
            notifications = new List<Notification>();
            backgroundElements = new List<Texture2D>();
            backgroundObjects = new List<BackgroundElement>();
            enemies = new List<Enemy>();
            stings = new List<Sting>();
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

            //level1 background
            background = Content.Load<Texture2D>("background_level_1");
            backgroundElements.Add(Content.Load<Texture2D>("Sprites/speedLine"));
            backgroundElements.Add(Content.Load<Texture2D>("Sprites/bigcloud"));
            backgroundElements.Add(Content.Load<Texture2D>("Sprites/smallcloud"));
            blank = Content.Load<Texture2D>("Sprites/blank");

            enemyShip = Content.Load<Texture2D>("Sprites/attacking_red_bee_enemy");

            backgroundMusic = Content.Load<Song>("Audio/Map1");

            //player textures
            List<Texture2D> shipTextures = new List<Texture2D>();
            shipTextures.Add(Content.Load<Texture2D>("Sprites/JerryCenter/Jerry_Center"));
            shipTextures.Add(Content.Load<Texture2D>("Sprites/JerryLeft/Jerry_left"));
            shipTextures.Add(Content.Load<Texture2D>("Sprites/JerryRight/Jerry_right"));
            playerLivesGraphic = Content.Load<Texture2D>("Sprites/lifeicon");
            playerShield = Content.Load<Texture2D>("Sprites/neoshielf");

            //Stings
            stingRed = Content.Load<Texture2D>("Sprites/enemy_bee_stinger");
            stingGreen = Content.Load<Texture2D>("Sprites/Jerry_bee_stinger");

            //Beatles
            beatleBig = Content.Load<Texture2D>("Sprites/beatleBig");
            beatleSmall = Content.Load<Texture2D>("Sprites/beatleSmall");

            //Explosions
            explosionTexture = Content.Load<Texture2D>("Sprites/stingRedShot");
            explosionTextureGreen = Content.Load<Texture2D>("Sprites/stingGreenShot");

            player = new Player(shipTextures, screenBounds);

            PrepareLevel();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);

            state = gameState.StartMenu;
        }

        protected override void UnloadContent()
        {
        }

        public void InitiateBossSequence(int playerScore)
        {
            if ((playerScore > 100) && (DoesSpawnBoss = false)) //playerScore must be greater than 45000 in actuality
            {
                spriteBatch.DrawString(scoreFont, "BOSS INCOMING", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("BOSS INCOMING").X / 2, (int)screenBounds.Height / 4), Color.White);
                //spriteBatch.DrawString(scoreFont, "Score: " + playerScore * 100, new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Score: " + playerScore * 100).X / 2, (int)screenBounds.Height / 4 + scoreFont.MeasureString("Score: " + playerScore * 100).Y), Color.White);
                //Color flashColor = flashing ? Color.White : Color.Yellow;
                //spriteBatch.DrawString(scoreFont, "Press Enter to Play Again", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to Play Again").X / 2, (int)screenBounds.Height / 3 * 2), flashColor);
                //spriteBatch.DrawString(scoreFont, "Press Escape to Quit", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Escape to Quit").X / 2, (int)screenBounds.Height / 4 * 3), Color.White);
                //break;
                //spawn boss
            }

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
            int randomAmt = rand.Next(400,400);
            for (int i = 0; i < randomAmt; i++)
            {
                bool bigBeatle = (rand.Next() % 2 == 0) ? true : false;
                float speed = !bigBeatle ? rand.Next(2, 8) : rand.Next(4, 16);
                beatles.Add(new Beatle(bigBeatle, speed, new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-10000, 0))));
            }

            int randomEnemies = rand.Next(120, 120);
            for (int i = 0; i < randomEnemies; i++)
            {
                enemies.Add(new Enemy(enemyShip, new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-10000, 0)), rand.Next(8, 16) * 1000, rand.Next(2, 20) / 3 * 100));
            }

            int randomBoss1s = rand.Next(1, 1);
            /*for (int i = 0; i < randomEnemies; i++)
            {
                boss1s.Add(new Boss1(boss1Texture, new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-10000, 0)), rand.Next(8, 16) * 1000, rand.Next(2, 20) / 3 * 100));
            }*/

        }
        /*public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.IsFullScreen = true;
        }
        SoundEffect soundEngine;
        SoundEffectInstance soundEngineInstance;
        SoundEffect soundHyperspaceActivation;
        bool checkActivity(KeyboardState keyboardState, GamePadState gamePadState)
        {
            // Check to see if the input states are different from last frame
            GamePadState nonpacketGamePadState = new GamePadState(
            gamePadState.ThumbSticks, gamePadState.Triggers,
            gamePadState.Buttons, gamePadState.DPad);
            bool keybidle = keyboardState.GetPressedKeys().Length == 0;
            //bool gamepidle = blankGamePadState == nonpacketGamePadState;
            if (keybidle)
            {
                //no activity;
                return false;
            }
            return true;
        }
        bool checkExitKey(KeyboardState keyboardState, GamePadState gamePadState)
        {
            // Check to see whether ESC was pressed on the keyboard
            // or BACK was pressed on the controller.
            if (keyboardState.IsKeyDown(Keys.Escape) ||
            gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
                return true;
            }
            return false;
        }*/


        protected override void Update(GameTime gameTime)
        {

            if (playerScore > 65000)
            {
                
                InitiateBossSequence(playerScore);
                DoesSpawnBoss = true;
            }

            //if (keyboardState.IsKeyDown(Keys.F))
            //{
            //    this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //    this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //    this.graphics.IsFullScreen = true;
            //}
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

            switch (state)
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
                        if (keyboardState.IsKeyDown(Keys.F))
                        {
                            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                            this.graphics.IsFullScreen = true;
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
                            notifications[i].Update(gameTime);
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

                                //Sting-enemy collision
                                if (stings[i].Visible && enemies[j].Visible && !(stings[i] is EnemySting) && stings[i].Bounds.Intersects(enemies[j].Bounds))
                                {
                                    stings[i].Visible = false;
                                    enemies[j].Damage((int)stings[i].Damage);
                                    Texture2D expTexToUse = player.StingLevel == 0 ? explosionTexture : explosionTextureGreen;
                                    explosions.Add(new Explosion(expTexToUse, stings[i].Position));
                                }

                                if (!enemies[j].Visible)
                                    enemies.RemoveAt(j);
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
                                    beatles.RemoveAt(q);
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
                                enemy.Update(gameTime);
                            foreach (Beatle beatle in beatles)
                                beatle.Update();
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

            switch (state)
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
                        spriteBatch.DrawString(scoreFont, "Beeautiful", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Beeautiful").X / 2, (int)screenBounds.Height / 4), Color.White);
                        spriteBatch.DrawString(scoreFont, "Developed by the Java Tigers", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Developed by the Java Tigers").X / 2, (int)screenBounds.Height / 4 + scoreFont.MeasureString("Developed by the Java Tigers").Y), Color.White);
                        Color flashColor = flashing ? Color.White : Color.Yellow;
                        spriteBatch.DrawString(scoreFont, "Press Enter to Play", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to Play").X / 2, (int)screenBounds.Height / 3 * 2), flashColor);
                        spriteBatch.DrawString(scoreFont, "Press Escape to Quit", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Escape to Quit").X / 2, (int)screenBounds.Height / 4 * 3), Color.White);
                        spriteBatch.DrawString(scoreFont, "Press F for Full Screen (Coming Soon)", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press F for Full Screen (Coming Soon)").X / 2, (int)screenBounds.Height / 5 * 4), Color.White);
                        spriteBatch.DrawString(scoreFont, "Press LEFT CTRL FOR HIDDEN POWAH!", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press LEFT CTRL FOR HIDDEN POWAH").X / 2, (int)screenBounds.Height / 6 * 5), Color.White);

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
