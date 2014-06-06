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
        private bool difficultySet = false, nameSet = false;
        private Color easyColor = Color.White, normalColor = Color.Red, hardColor = Color.Red;
        private int maxShipLife; // Used to display the life correctly
        public static int maxBossLife; // This is set in Level.cs where the boss is spawned.

        string[] alphabet = new string[] { "a", "b", "c", "d", "e", "f", "g", "h",
           "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", 
           "x", "y", "z" };

        string[] playerName = new string[] { "a", "a", "a" };

        int letterPosition = 0, letterCounter = 0;
        int[] letterCounterMemory = new int[] { 0, 0, 0 };
        
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
            level = CurrentLevel.level1;
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
            else if (difficultySet && !nameSet)
            {
                ChooseName();

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
                    //Checks if the stage is none = player has died.
                    if (currentLevel != null && currentLevel.stage == CurrentLevelStage.none)
                    {
                        if (currentLevel != null)
                        {
                            currentLevel = null;
                            Thread.Sleep(1500);
                        }
                    }
                    //If the level is null, this means that the player has died. Present a death screen with retry and exit options.
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
                        // The player won the level or game, wait for the enter key then continue on.
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
            //Draw the player name change
            else if (difficultySet && !nameSet)
            {
                spriteBatch.DrawString(playerLifeFont, "Choose a name", new Vector2(275, 200), Color.Red);
                spriteBatch.DrawString(playerLifeFont, "Press enter when ready", new Vector2(225, 400), Color.Red); 

                //Draw the 3 letters.
                for (int i = 0; i < playerName.Length; i++)
			    {
                    var space = 32;

                    //Mark the one that's active with white
                    if (i == letterPosition)
                        spriteBatch.DrawString(playerLifeFont, playerName[i], new Vector2(360 + space * i, 250), Color.White);
                    else
                        spriteBatch.DrawString(playerLifeFont, playerName[i], new Vector2(360 + space * i, 250), Color.Red);
			    }  
            }
            else
            {
                if (!isPaused)
                {
                    //The player died, draw the death screen.
                    if (currentLevel == null)
                    {
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
                    //The game is paused, draw pause stuff here.
                    spriteBatch.Draw(mPauseBackground, Vector2.Zero, Color.White);
                }
            }

            base.Draw(spriteBatch);
        }

        private void ChooseName()
        {
            //Change letter
            if (CheckKeystroke(Keys.Down))
            {
                if (letterCounter > 0)
                {
                    letterCounter--;
                }

                playerName[letterPosition] = alphabet[letterCounter];
            }
            else if (CheckKeystroke(Keys.Up))
            {
                if (letterCounter < alphabet.Length - 1)
                {
                    letterCounter++;
                }

                playerName[letterPosition] = alphabet[letterCounter];
            }

            //Change letter position and remember which number the last letter had.
            if (CheckKeystroke(Keys.Right) && letterPosition < playerName.Length - 1)
            {
                if (letterPosition == 0)
                {
                    letterCounterMemory[letterPosition] = letterCounter;
                    letterCounter = letterCounterMemory[letterPosition + 1];
                }
                else if (letterPosition == 1)
                {
                    letterCounterMemory[letterPosition] = letterCounter;
                    letterCounter = letterCounterMemory[letterPosition + 1];
                }
                else
                {
                    letterCounterMemory[letterPosition] = letterCounter;
                }
                letterPosition += 1;
            }
            else if (CheckKeystroke(Keys.Left) && letterPosition > 0)
            {
                if (letterPosition == 0)
                {
                    letterCounterMemory[letterPosition] = letterCounter;
                }
                else if (letterPosition == 1)
                {
                    letterCounterMemory[letterPosition] = letterCounter;
                    letterCounter = letterCounterMemory[letterPosition - 1];
                }
                else
                {
                    letterCounterMemory[letterPosition] = letterCounter;
                    letterCounter = letterCounterMemory[letterPosition - 1];
                }
                letterPosition -= 1;
            }

            //Save the name.
            if (CheckKeystroke(Keys.Enter))
            {
                HighScores.playerName = String.Join(String.Empty, playerName);
                nameSet = true;
                Thread.Sleep(50);
            }
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

            //The player pressed the enter key while in the pause screen, exit the game state.
            if (CheckKeystroke(Keys.Enter) && isPaused)
            {
                isPaused = false;
                screenEvent.Invoke(this, new EventArgs());
                return;
            }

            //Pause key was pressed.
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
                //InGameScreen.playerScore = 0; // Make sure the score is set to 0 when the player starts a new game.
                difficultySet = true;

                //Sets the current level to 1 and initializes it as the player has chosen a dificulty, we can create the level now.
                currentLevel = new Level1(content, _game);

                //Sets the max ship life in order to draw life correctly for all ships.
                maxShipLife = Level.ship.Life;
            }

            //Change difficulty
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

            //Changes the color of the difficulty currently marked.
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
            //Draws boss life bar and life.
            if (currentLevel.boss != null)
            {
                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, playerHealthBar.Width, 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.LightGray);

                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, (int)(playerHealthBar.Width * ((double)currentLevel.boss.Life / maxBossLife)), 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.Red);

                spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, playerHealthBar.Width, 30), new Rectangle(0, 0, playerHealthBar.Width, 30), Color.White);

                spriteBatch.DrawString(scoreFont, "" + currentLevel.boss.Life, new Vector2(689, 0), Color.Yellow);

                spriteBatch.DrawString(scoreFont, "Boss", new Vector2(685, 25), Color.Yellow);
            }
            
            //Draw second boss life bar and life.
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

            //Draws the difficulty ingame.
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

        //Clears the enemy list. Used in cheats.
        public int ClearEnemyList()
        {
            if (currentLevel == null || currentLevel.enemyList.Count == 0)
            {
                return 0;
            }
            var killed = currentLevel.enemyList.Count;
            currentLevel.enemyList.Clear();
            return killed;
        }

        //Changes the level. Used in cheats.
        public string ChangeLevel()
        {
            if (currentLevel != null)
            {
                switch (level)
                {
                    case CurrentLevel.level2:
                        currentLevel = new Level2(content, _game);
                        return "The level was changed to level 2";
                    case CurrentLevel.level3:
                        break;
                    case CurrentLevel.bonusLevel:
                        break;
                    default:
                        break;
                }
            }
            var wrongLevel = (int)level;
            level -= 1;
            return string.Format("There is no level initiated or level {0} does not exist.", wrongLevel); // Currently not used as i have no way to display messages.
        }
    }
}