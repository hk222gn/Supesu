using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Supesu.StateManagement;
using Supesu.SpriteManagement;
using Supesu.Weapons.Projectiles;
using Supesu.SoundHandler;
using Supesu.HighScore;
using System.IO;
using Supesu.StateManagement.Levels;

namespace Supesu
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TitleScreen mTitleScreen;
        Screen mCurrentScreen;
        ControlDetectorScreen mControlScreen;
        InGameScreen mInGameScreen;
        OptionsScreen mOptionsScreen;
        HighscoreScreen mHighscoreScreen;
        UnlockablesScreen mUnlockablesScreen;
        KeyboardState keyboard, prevKeyboard;
        private MenuChoices menuChoice;
        private Sounds sounds;
        SpriteFont smallText;
        int totalFrames = 0;
        float elapsedTime = 0.0f;
        int fps = 0;
        public static bool drawFps = false;
        private bool debugActivated = false;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);

            sounds = new Sounds();
            smallText = Content.Load<SpriteFont>(@"Fonts/SpriteFont1");

            if (!File.Exists(HighScores.fileName))
            {
                //Make a new one with a few dummy scores in.
                HighScores.HighScoreData data = new HighScores.HighScoreData(1);

                data.playerName[0] = "Useless guy";
                data.level[0] = 1;
                data.score[0] = 0;

                HighScores.SaveHighScores(data, HighScores.fileName);
            }

            Sounds.SoundBank.PlayCue("MainMusic");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            mControlScreen = new ControlDetectorScreen(this.Content, new EventHandler(ControlDetectorScreenEvent), this);
            mTitleScreen = new TitleScreen(this.Content, new EventHandler(TitleScreenEvent), this);
            mInGameScreen = new InGameScreen(this.Content, new EventHandler(InGameEvent), this);
            mOptionsScreen = new OptionsScreen(this.Content, new EventHandler(OptionsScreenEvent), this, graphics);
            mHighscoreScreen = new HighscoreScreen(this.Content, new EventHandler(HighscoreScreenEvent), this);
            mUnlockablesScreen = new UnlockablesScreen(this.Content, new EventHandler(UnlockablesScreenEvent), this);

            mCurrentScreen = mTitleScreen;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            // Shut the game down instantly
            if (Keyboard.GetState().IsKeyDown(Keys.Delete) == true)
                this.Exit();

            //Handles FPS timer and sets the FPS to the total frames drawn each 1 second.
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime >= 1000.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsedTime = 0;
            }

            //Updates the current screen.
            mCurrentScreen.Update(gameTime);

            //Debug command handler
            if (Keyboard.GetState().IsKeyDown(Keys.F12) && !debugActivated)
            {
                RunDebugCommands();
                debugActivated = !debugActivated;
            }
            else if (debugActivated)
                RunDebugCommands();            

            //Updates the global audio engine.
            Sounds.AudioEngine.Update();

            prevKeyboard = keyboard;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //Add a frame everytime it's drawn
            totalFrames++;

            spriteBatch.Begin();

            mCurrentScreen.Draw(spriteBatch);

            if (drawFps)
            {
                spriteBatch.DrawString(smallText, string.Format("FPS: {0}", fps),
                    new Vector2(this.Window.ClientBounds.Width / 2 - 30, this.Window.ClientBounds.Height - 25), Color.Yellow);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ControlDetectorScreenEvent(object obj, EventArgs e)
        {
            mCurrentScreen = mTitleScreen;
        }

        private void TitleScreenEvent(object obj, EventArgs e)
        {
            if (menuChoice == MenuChoices.StartGame)
                mCurrentScreen = mInGameScreen;

            else if (menuChoice == MenuChoices.Highscores)
            {
                mCurrentScreen = new HighscoreScreen(this.Content, new EventHandler(HighscoreScreenEvent), this); ;
            }

            else if (menuChoice == MenuChoices.Unlockables)
                mCurrentScreen = mUnlockablesScreen;

            else if (menuChoice == MenuChoices.Options)
                mCurrentScreen = mOptionsScreen;

            menuChoice = MenuChoices.Empty;
        }

        private void InGameEvent(object obj, EventArgs e)
        {
            mCurrentScreen = mTitleScreen;
            mInGameScreen = new InGameScreen(this.Content, new EventHandler(InGameEvent), this);
        }

        private void OptionsScreenEvent(object obj, EventArgs e)
        {
            mCurrentScreen = mTitleScreen;
        }

        private void HighscoreScreenEvent(object obj, EventArgs e)
        {
            mCurrentScreen = mTitleScreen;

        }

        private void UnlockablesScreenEvent(object obj, EventArgs e)
        {
            mCurrentScreen = mTitleScreen;
        }

        public void SetTitleChoice(MenuChoices choice)
        {
            menuChoice = choice;
        }

        private bool CheckKeystroke(Keys key)
        {
            return (keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key));
        }

        private void RunDebugCommands()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && CheckKeystroke(Keys.D1))
            {
                if (Level.Ship != null)
                {
                    Level.Ship.Life = 9999;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && CheckKeystroke(Keys.D2))
            {
                mInGameScreen.ClearEnemyList();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && CheckKeystroke(Keys.D3))
            {
                InGameScreen.level += 1;
                if (InGameScreen.level == CurrentLevel.level2)
                {
                    mInGameScreen.ChangeLevel();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && CheckKeystroke(Keys.D4))
            {
                InGameScreen.playerScore = 99999;
                HighScore.HighScores.SaveHighscoreToFile();
                InGameScreen.playerScore = 0;
            }
        }
    }
}
