using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Supesu.SpriteManagement;
using Supesu.StateManagement.Levels;
using Supesu.SoundHandler;
using Supesu.HighScore;

namespace Supesu
{
    public enum CurrentLevel
    {
        none,
        level1,
        level2,
        level3,
        bonusLevel
    }

    public enum CurrentLevelStage
    {
        none,
        enemyStage1,
        enemyStage2,
        enemyStage3,
        bossStage,
        playerWonStage
    }

    public enum Difficulty
    {
        easy = 1, 
        normal = 2,
        hard = 3
    }
    //FIX IN ORDER
    //TODO: Change the way additional bullet damage works with dificulties.
    //TODO: Change the enemy bullet velocity, or change the amount of bullets/speed that the shoot. It's a bit too hectic right now.
    //TODO: General, there's most likely a memory leak due to the way i handle sprite loading.

    //Make static variables in game class, load all the texture required in those variables, saves memory and loading time?
    
    class InGameScreen : Screen
    {
        Texture2D mPauseBackground;
        Texture2D scoreOverlay;
        Texture2D playerHealthBar;
        Texture2D gameOver;
        Game1 _game;
        private bool isPaused = false;
        private bool pauseKeyDown = false;
        KeyboardState prevKeyboard;
        KeyboardState keyboard;
        public static CurrentLevel level = CurrentLevel.level1;
        Level currentLevel;
        SpriteFont playerLifeFont, scoreFont;
        ContentManager content;
        public static int playerScore = 0, scoreMultiplier = 1;
        public static Difficulty difficulty;
        private bool difficultySet = false;
        private Color easyColor = Color.White, normalColor = Color.Red, hardColor = Color.Red;
        private int maxShipLife; // Used to display the life correctly
        public static int maxBossLife; // This is set in Level.cs where the boss is spawned.
        
        public InGameScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mPauseBackground = content.Load<Texture2D>("Images/PausedYes");
            _game = game;
            this.content = content;
            playerLifeFont = content.Load<SpriteFont>(@"Fonts/General");
            scoreFont = content.Load<SpriteFont>(@"Fonts/SpriteFont1");
            scoreOverlay = content.Load<Texture2D>(@"Images/Score");
            playerHealthBar = content.Load<Texture2D>(@"Images/Healthbar");
            gameOver = content.Load<Texture2D>(@"Images/GameOver");
            difficulty = Difficulty.easy;
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            //Force the user to make a selection of difficulty.
            if (!difficultySet)
            {
                //Make the selection here.

                DifficultyMenu();

                //If the user doesn't actually want to play the game, let him go back to the title screen.
                if (CheckKeystroke(Keys.Escape))
                {
                    Sounds.SoundBank.PlayCue("MenuBack");
                    screenEvent.Invoke(this, new EventArgs());
                }
            }
            else
            {
                if (!isPaused)
                {
                    if (currentLevel != null && currentLevel.stage == CurrentLevelStage.none)
                    {
                        //TODO: Make a death screen appear or something.
                        if (currentLevel != null)
                        {
                            currentLevel = null;
                            Thread.Sleep(1500);
                        }
                    }
                    else if (currentLevel == null)
                    {
                        if (CheckKeystroke(Keys.R))
                        {
                            Sounds.SoundBank.PlayCue("MenuHit");
                            if (level == CurrentLevel.level1)
                            {
                                currentLevel = new Level1(content, _game);
                            }
                            else
                                currentLevel = new Level2(content, _game);
                            playerScore = 0;
                        }
                        else if (CheckKeystroke(Keys.Escape))
                        {
                            Sounds.SoundBank.PlayCue("MenuBack");
                            screenEvent.Invoke(this, new EventArgs());
                            playerScore = 0;
                            difficulty = Difficulty.easy;
                            return;
                        }
                    }
                    else
                    {
                        if (currentLevel.stage == CurrentLevelStage.playerWonStage)
                        {
                            if (CheckKeystroke(Keys.Enter))
                            {
                                switch (level)
                                {
                                    case CurrentLevel.level2:
                                        currentLevel = new Level2(content, _game);
                                        break;
                                    case CurrentLevel.level3:
                                        screenEvent.Invoke(this, new EventArgs());
                                        break;
                                    case CurrentLevel.bonusLevel:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        currentLevel.Update(gameTime);
                    }
                }
                CheckPauseKey(keyboard);
            }

            prevKeyboard = keyboard;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!difficultySet)
            {
                spriteBatch.DrawString(playerLifeFont, "Easy", new Vector2(50, _game.Window.ClientBounds.Height / 2 - 150), easyColor);
                spriteBatch.DrawString(playerLifeFont, "Normal", new Vector2(50, _game.Window.ClientBounds.Height / 2 - 50), normalColor);
                spriteBatch.DrawString(playerLifeFont, "Hard", new Vector2(50, _game.Window.ClientBounds.Height / 2 + 50), hardColor);

                spriteBatch.DrawString(scoreFont, "Esc for menu.", new Vector2(1, 695), Color.Red);
            }
            else
            {
                if (!isPaused)
                {
                    if (currentLevel == null)
                    {
                        //Draws the death screen.
                        spriteBatch.Draw(gameOver, Vector2.Zero, Color.Red);
                    }
                    else
                    {
                        currentLevel.Draw(spriteBatch);

                        DrawUI(spriteBatch);
                    }
                }
                else
                {
                    spriteBatch.Draw(mPauseBackground, Vector2.Zero, Color.White);
                    //DRAW PAUSE STUFF HERE OK
                }
            }

            base.Draw(spriteBatch);
        }

        private bool CheckKeystroke(Keys key)
        {
            return (keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key));
        }

        private void BeginPause()
        {
            isPaused = true;
            //TODO: Pause stuff here.
        }

        private void EndPause()
        {
            isPaused = false;
            //TODO: Resume stuff here.
        }

        private void CheckPauseKey(KeyboardState ks)
        {
            bool pauseKeyDownThisFrame = (ks.IsKeyDown(Keys.Escape));

            if (CheckKeystroke(Keys.Enter) && isPaused)
            {
                isPaused = false;
                screenEvent.Invoke(this, new EventArgs());
                return;
            }

            if (!pauseKeyDown && pauseKeyDownThisFrame)
            {
                if (!isPaused)
                    BeginPause();
                else
                    EndPause();
            }
            pauseKeyDown = pauseKeyDownThisFrame;
        }

        private void DifficultyMenu()
        {
            if (CheckKeystroke(Keys.Enter))
            {
                Sounds.SoundBank.PlayCue("MenuHit");
                switch (difficulty)
                {
                    case Difficulty.easy :
                        difficulty = Difficulty.easy;
                        break;
                    case Difficulty.normal :
                        difficulty = Difficulty.normal;
                        break;
                    case Difficulty.hard :
                        difficulty = Difficulty.hard;
                        break;
                }
                InGameScreen.playerScore = 0; // Make sure the score is set to 0 when the player starts a new game.
                difficultySet = true;

                //Sets the current level to 1 and initializes it as the player has chosen a dificulty, we can create the level now.
                currentLevel = new Level1(content, _game);

                maxShipLife = Level.ship.Life;
            }

            if (CheckKeystroke(Keys.Down) && (int)difficulty < 3)
            {
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
                SetAllRed();
                difficulty += 1;
            }
            else if (CheckKeystroke(Keys.Up) == true && (int)difficulty > 1)
            {
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
                SetAllRed();
                difficulty -= 1;
            }

            switch (difficulty)
            {
                case Difficulty.easy:
                    easyColor = Color.White;
                    break;
                case Difficulty.normal:
                    normalColor = Color.White;
                    break;
                case Difficulty.hard:
                    hardColor = Color.White;
                    break;
                default:
                    break;
            }
        }

        private void SetAllRed()
        {
            easyColor = Color.Red;
            normalColor = Color.Red;
            hardColor = Color.Red;
        }

        private void DrawUI(SpriteBatch spriteBatch)
        {
            //Draw UI.
            //Life bar and life
            spriteBatch.Draw(playerHealthBar, new Rectangle(600, 690, playerHealthBar.Width, 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.LightGray);

            spriteBatch.Draw(playerHealthBar, new Rectangle(600, 690, (int)(playerHealthBar.Width * ((double)Level.ship.Life / maxShipLife)), 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.Red);

            spriteBatch.Draw(playerHealthBar, new Rectangle(600, 690, playerHealthBar.Width, 30), new Rectangle(0, 0, playerHealthBar.Width, 30), Color.White);

            spriteBatch.DrawString(scoreFont, "" + Level.ship.Life, new Vector2(689, 690), Color.Yellow);

            spriteBatch.DrawString(scoreFont, "Hitpoints", new Vector2(655, 664), Color.Yellow);
            //Draws boss life bar incase there is one
            if (currentLevel.boss != null)
            {
                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, playerHealthBar.Width, 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.LightGray);

                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, (int)(playerHealthBar.Width * ((double)currentLevel.boss.Life / maxBossLife)), 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.Red);

                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, playerHealthBar.Width, 30), new Rectangle(0, 0, playerHealthBar.Width, 30), Color.White);

                spriteBatch.DrawString(scoreFont, "" + currentLevel.boss.Life, new Vector2(689, 0), Color.Yellow);

                spriteBatch.DrawString(scoreFont, "Boss", new Vector2(685, 25), Color.Yellow);
            }

            if (currentLevel.secondBoss != null)
            {
                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, playerHealthBar.Width, 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.LightGray);

                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, (int)(playerHealthBar.Width * ((double)currentLevel.secondBoss.Life / maxBossLife)), 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.Red);

                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, playerHealthBar.Width, 30), new Rectangle(0, 0, playerHealthBar.Width, 30), Color.White);

                spriteBatch.DrawString(scoreFont, "" + currentLevel.secondBoss.Life, new Vector2(689, 0), Color.Yellow);

                spriteBatch.DrawString(scoreFont, "Boss", new Vector2(685, 25), Color.Yellow);
            }
            //Score UI.
            spriteBatch.Draw(scoreOverlay, new Rectangle(0, 0, 200, 80), Color.White);

            //Player score.
            spriteBatch.DrawString(scoreFont, "Score: ", new Vector2(2, 2), Color.Red);
            spriteBatch.DrawString(scoreFont, string.Format("{0}", playerScore), new Vector2(66, 2), Color.White);

            //Score multiplyer.
            spriteBatch.DrawString(scoreFont, "Multi: ", new Vector2(2, 35), Color.Red);
            spriteBatch.DrawString(scoreFont, string.Format("{0}", scoreMultiplier), new Vector2(66, 35), Color.White);
            spriteBatch.DrawString(scoreFont, "x", new Vector2(80, 35), Color.Red);

            //Esc for paus text
            spriteBatch.DrawString(playerLifeFont, "Esc for menu", new Vector2(2, 685), Color.Red);

            //Dificulty.
            if (difficulty == Difficulty.easy)
            {
                spriteBatch.DrawString(scoreFont, "Difficulty: ", new Vector2(2, 665), Color.Red);
                spriteBatch.DrawString(scoreFont, "Easy", new Vector2(104, 665), Color.White);
            }
            else if (difficulty == Difficulty.normal)
            {
                spriteBatch.DrawString(scoreFont, "Difficulty: ", new Vector2(2, 665), Color.Red);
                spriteBatch.DrawString(scoreFont, "Normal", new Vector2(104, 665), Color.White);
            }
            else
            {
                spriteBatch.DrawString(scoreFont, "Difficulty: ", new Vector2(2, 665), Color.Red);
                spriteBatch.DrawString(scoreFont, "Hard", new Vector2(104, 665), Color.White);
            }
        }
    }
}