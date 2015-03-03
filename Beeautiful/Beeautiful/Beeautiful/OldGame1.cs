/*using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

The game should include:

    [ ]Intro screen briefly describing the game and explaining the controls (also include who created this game; group name or individual names and give your game a name).
    [ ]Two screens at the end of the game one for losing and the other for wining (if your game has infinite level then you do not need this), you want to include your names at the end or in game if you did not do it in number 1.
    [ ]Moving background or animated background or add a moving layer above your background.
    [ ]Using sprite font (score or lives or etc.) but you need to use it somewhere.
    [ ]Animation
    [ ]Sound Effects
    [ ]Songs
    [ ]Bosses (make sure you have more than one, if your game idea does not need boss then speak to me and I could allow your game not to have bosses)
    [ ]At least three levels (some of your games might not need this, speak to me).
    [ ]A wining (can be survival) and a losing states.
    [ ]Pause the game
    [ ]Written in object oriented way
    [ ]Game states
    [ ]A button to exit the game
    [ ]In game option to run in full screen (can be done by pressing a keyboard button to toggle full screen).

//you can use the built in function in xna to do every single animation
//or you can use timespan

//use speed for all vectors
//base speed = TBD

// ^ IMPORTANT:

//_______________

//TO DO:

//IN ISSUES ON GITHUB



namespace Beeautiful2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static private int TimeOutLimit = 1200000;
        private double timeoutCount = 0;

        HorizontallyScrollingBackground mScrollingBackground;

        GameObject player;

        List<GameObject> gameObjects;

        public Game1()
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
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //bPlayer = new Player();
            //eMyEnemy = new Enemy();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            soundEngine = Content.Load<SoundEffect>("Audio\\Music\\04_-_The_Black_Eyed_Peas_-_Imma_Be-TSM");
            soundEngineInstance = soundEngine.CreateInstance();
            soundHyperspaceActivation = Content.Load<SoundEffect>("Audio\\Music\\04_-_The_Black_Eyed_Peas_-_Imma_Be-TSM");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mScrollingBackground = new HorizontallyScrollingBackground(this.GraphicsDevice.Viewport);
            mScrollingBackground.AddBackground("Background01");
            mScrollingBackground.AddBackground("Background02");
            mScrollingBackground.AddBackground("Background03");
            mScrollingBackground.AddBackground("Background04");
            mScrollingBackground.AddBackground("Background05");
            mScrollingBackground.LoadContent(this.Content);
            // TODO: use this.Content to load your game content here
            player = new Player(Content.Load<Texture2D>("Sprites\\Player\\bee"), new Vector2(150, 200), _scale: 0.5f, _speed: 5.0f, _rotationSpeed: 0.05f);
            gameObjects = new List<GameObject>();
            gameObjects.Add(player);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (soundEngineInstance.State == SoundState.Stopped)
            {
                soundEngineInstance.Volume = 0.75f;
                soundEngineInstance.IsLooped = true;
                soundEngineInstance.Play();
            }
            else
                soundEngineInstance.Resume();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // Check to see if the user has exited
            if (checkExitKey(keyboardState, gamePadState))
            {
                base.Update(gameTime);
                return;
            }
            // Check to see if there has been any activity
            if (checkActivity(keyboardState, gamePadState) == false)
            {
                timeoutCount += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
                timeoutCount = 0;
            // Timeout if idle long enough
            if (timeoutCount > TimeOutLimit)
            {
                Exit();
                base.Update(gameTime);
                return;
            }

            foreach (GameObject myGameObject in gameObjects)
            {
                myGameObject.Update(gameTime);
            }
            mScrollingBackground.Update(gameTime, 160, HorizontallyScrollingBackground.HorizontalScrollDirection.Left);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //player.Draw(spriteBatch,gameTime);
            mScrollingBackground.Draw(spriteBatch);
            foreach (GameObject myGameObject in gameObjects)
            {
                myGameObject.Draw(spriteBatch, gameTime);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
*/