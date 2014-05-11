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
    //TODO: The enemies move weird sometimes, it also seems that their hitboxes are not following the Y position, nor the X position. CONFIRMED
    //TODO: Change the enemy bullet velocity, or change the amount of bullets/speed that the shoot. It's a bit too hectic right now.
    //TODO: General, there's most likely a memory leak due to the way i handle sprite loading.
    //TODO: Everything feels a bit clunky, think about that shit.

    //Make static variables in game class, load all the texture required in those variables, saves memory and loading time?
    
    
    class InGameScreen : Screen
    {
        Texture2D mPauseBackground;
        Texture2D scoreOverlay;
        Texture2D playerHealthBar;
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
            difficulty = Difficulty.easy;

            //Sets the current level to 1 and initializes it.
            currentLevel = new Level1(content, game);
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
                            currentLevel = new Level1(content, _game);
                            playerScore = 0;
                            difficulty = Difficulty.easy;
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
                        //TODO: Make an external method that handles the levels. For now there's only one level, so we don't need to have this at all. Just the update.

                        //switch (level)
                        //{
                        //    case CurrentLevel.level1:
                        //        break;
                        //    case CurrentLevel.level2:
                        //        break;
                        //    case CurrentLevel.level3:
                        //        break;
                        //    case CurrentLevel.bonusLevel:
                        //        break;
                        //    default:
                        //        break;
                        //}
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

                spriteBatch.DrawString(scoreFont, "Esc for title screen.", new Vector2(1, 695), Color.Red);
            }
            else
            {
                if (!isPaused)
                {
                    if (currentLevel == null)
                    {
                        //Draws the death screen.
                        spriteBatch.Draw(mPauseBackground, Vector2.Zero, Color.Red);
                    }
                    else
                    {
                        currentLevel.Draw(spriteBatch);
                    
                        //Draw UI.
                        //Life bar and life
                        spriteBatch.Draw(playerHealthBar, new Rectangle(600, 690, playerHealthBar.Width, 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.LightGray);

                        spriteBatch.Draw(playerHealthBar, new Rectangle(600, 690, (int)(playerHealthBar.Width * ((double)currentLevel.ship.Life / (16 * (int)difficulty))), 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.Red);

                        spriteBatch.Draw(playerHealthBar, new Rectangle(600, 690, playerHealthBar.Width, 30), new Rectangle(0, 0, playerHealthBar.Width, 30), Color.White);

                        spriteBatch.DrawString(scoreFont, "" + currentLevel.ship.Life, new Vector2(689, 690), Color.Yellow);

                        spriteBatch.DrawString(scoreFont, "Hitpoints", new Vector2(655, 664), Color.Yellow);
                        //Draws boss life bar incase there is one
                        if (currentLevel.boss != null)
                        {
                            spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, playerHealthBar.Width, 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.LightGray);

                            spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, (int)(playerHealthBar.Width * ((double)currentLevel.boss.Life / (150 * (int)difficulty))), 30), new Rectangle(0, 30, playerHealthBar.Width, 30), Color.Red);

                            spriteBatch.Draw(playerHealthBar, new Rectangle(600, 0, playerHealthBar.Width, 30), new Rectangle(0, 0, playerHealthBar.Width, 30), Color.White);

                            spriteBatch.DrawString(scoreFont, "" + currentLevel.boss.Life, new Vector2(689, 0), Color.Yellow);

                            spriteBatch.DrawString(scoreFont, "Boss", new Vector2(685, 25), Color.Yellow);
                        }
                        //Score UI.
                        spriteBatch.Draw(scoreOverlay, new Rectangle(0, 0, 200, 80), Color.White);

                        //Player score.
                        spriteBatch.DrawString(scoreFont, "Score: " + playerScore, new Vector2(2, 1), Color.Red);

                        //Score multiplyer.
                        spriteBatch.DrawString(scoreFont, string.Format("Multi: {0}x", scoreMultiplier), new Vector2(2, 35), Color.Red);

                        //Esc for paus text
                        spriteBatch.DrawString(playerLifeFont, "Esc for menu", new Vector2(2, 685), Color.Red);

                        //Dificulty.
                        if (difficulty == Difficulty.easy)
	                    {
                            spriteBatch.DrawString(scoreFont, "Difficulty: Easy", new Vector2(2, 665), Color.Red);
	                    }
                        else if (difficulty == Difficulty.normal)
                        {
                            spriteBatch.DrawString(scoreFont, "Difficulty: Normal", new Vector2(2, 665), Color.Red);
                        }
                        else
                            spriteBatch.DrawString(scoreFont, "Difficulty: Hard", new Vector2(2, 665), Color.Red);
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
                difficultySet = true;
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
    }
}
