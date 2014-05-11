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
        private MenuChoices menuChoice;
        public BulletType bulletType;
        private Sounds sounds;

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
            bulletType = BulletType.standard;
            sounds = new Sounds();

            if (!File.Exists(HighScores.fileName))
            {
                //Make a new one with a few dummy scores in.
                HighScores.HighScoreData data = new HighScores.HighScoreData(3);

                data.playerName[0] = "Cool guy";
                data.level[0] = 1;
                data.score[0] = 300;

                data.playerName[1] = "Awesome guy";
                data.level[1] = 1;
                data.score[1] = 200;

                data.playerName[2] = "Not so cool guy";
                data.level[2] = 1;
                data.score[2] = 10;

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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mControlScreen = new ControlDetectorScreen(this.Content, new EventHandler(ControlDetectorScreenEvent), this);
            mTitleScreen = new TitleScreen(this.Content, new EventHandler(TitleScreenEvent), this);
            mInGameScreen = new InGameScreen(this.Content, new EventHandler(InGameEvent), this);
            mOptionsScreen = new OptionsScreen(this.Content, new EventHandler(OptionsScreenEvent), this);
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
            // Shut the game down instantly
            if (Keyboard.GetState().IsKeyDown(Keys.Delete) == true)
                this.Exit();

            //Updates the current screen.
            mCurrentScreen.Update(gameTime);

            //Updates the global audio engine.
            sounds.AudioEngine.Update();

            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            mCurrentScreen.Draw(spriteBatch);

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
                mCurrentScreen = mHighscoreScreen;

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
    }
}
