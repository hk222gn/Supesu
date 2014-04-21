using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Supesu
{
    public enum MenuChoices
    {
        Exit = 0,
        Options = 1,
        Unlockables = 2,
        Highscores = 3,
        StartGame = 4,
        Empty
    }
    class TitleScreen : Screen
    {
        Texture2D mTitleScreenBackground;
        Game1 game;
        SpriteFont spFont;
        KeyboardState prevKeyboard;
        KeyboardState keyboard;
        private MenuChoices menuChoices = MenuChoices.StartGame;
        private static Color startGameChoice = Color.White;
        private static Color highscoreChoice = Color.Red;
        private static Color unlockablesChoice = Color.Red;
        private static Color optionsChoice = Color.Red;
        private static Color exitChoice = Color.Red;

        public TitleScreen(ContentManager content, EventHandler theScreenEvent, Game1 game1)
            : base(theScreenEvent)
        {
            mTitleScreenBackground = content.Load<Texture2D>("Images/Title");
            game = game1;
            spFont = content.Load<SpriteFont>("Fonts/General");
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            if (CheckKeystroke(Keys.Escape))
            {
                screenEvent.Invoke(this, new EventArgs());
            }

            if (CheckKeystroke(Keys.Enter))
            {
                switch (menuChoices)
                {
                    case MenuChoices.StartGame:
                        ChangeScreen(menuChoices);
                        break;
                    case MenuChoices.Highscores:
                        ChangeScreen(menuChoices);
                        break;
                    case MenuChoices.Unlockables:
                        ChangeScreen(menuChoices);
                        break;
                    case MenuChoices.Options:
                        ChangeScreen(menuChoices);
                        break;
                    case MenuChoices.Exit:
                        game.Exit();
                        break;
                    default:
                        break;
                }
            }

            if (CheckKeystroke(Keys.Up) && (int)menuChoices < 4)
            {
                SetAllRed();
                menuChoices += 1;
            }
            else if (CheckKeystroke(Keys.Down) == true && (int)menuChoices > 0)
            {
                SetAllRed();
                menuChoices -= 1;
            }

            switch (menuChoices)
            {
                case MenuChoices.StartGame:
                    startGameChoice = Color.White;
                    break;
                case MenuChoices.Highscores:
                    highscoreChoice = Color.White;
                    break;
                case MenuChoices.Unlockables:
                    unlockablesChoice = Color.White;
                    break;
                case MenuChoices.Options:
                    optionsChoice = Color.White;
                    break;
                case MenuChoices.Exit:
                    exitChoice = Color.White;
                    break;
                default:
                    break;
            }

            prevKeyboard = keyboard;
            //TODO: Is there a better way to make a menu maybe?
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTitleScreenBackground, Vector2.Zero, Color.White);

            //This is drawing the menu text. We will handle the menu with the keyboard so no hitboxes are needed.
            spriteBatch.DrawString(spFont, "Start Game", new Vector2(50, 220), startGameChoice);
            spriteBatch.DrawString(spFont, "Highscores", new Vector2(50, 320), highscoreChoice);
            spriteBatch.DrawString(spFont, "Unlockables", new Vector2(50, 420), unlockablesChoice);
            spriteBatch.DrawString(spFont, "Options", new Vector2(50, 520), optionsChoice);
            spriteBatch.DrawString(spFont, "Exit", new Vector2(50, 620), exitChoice);
            base.Draw(spriteBatch);
        }
        private void SetAllRed()
        {
            startGameChoice = Color.Red;
            highscoreChoice = Color.Red;
            unlockablesChoice = Color.Red;
            optionsChoice = Color.Red;
            exitChoice = Color.Red;
        }

        private void ChangeScreen(MenuChoices menu)
        {
            game.SetTitleChoice(menu);
            screenEvent.Invoke(this, new EventArgs());
            Thread.Sleep(150);
        }

        private bool CheckKeystroke(Keys key)
        {
            return (keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key));
        }
    }
}

//if (currentMenuChoice == 4)
//{
//    game.SetGameStart(true);
//    screenEvent.Invoke(this, new EventArgs());
//    Thread.Sleep(150);
//}
//else if (currentMenuChoice == 3)
//{
//    // Implement the highscore state, 
//    // ?? Change SetGameStarts parameter to an int, send currentMenuChoice, depending on value, invoke into different states.
//}
//else if (currentMenuChoice == 2)
//{
//    // Implement unlockables state
//}
//else if (currentMenuChoice == 1)
//{
//    // Implement options state
//}
//else
//{
//    game.Exit();
//}


//if (currentMenuChoice == 4 && startGameChoice != Color.White)
//{
//    startGameChoice = Color.White;
//    highscoreChoice = Color.Red;
//}
//else if (currentMenuChoice == 3 && highscoreChoice != Color.White)
//{
//    highscoreChoice = Color.White;
//    startGameChoice = Color.Red;
//    unlockablesChoice = Color.Red;
//}
//else if (currentMenuChoice == 2 && unlockablesChoice != Color.White)
//{
//    unlockablesChoice = Color.White;
//    highscoreChoice = Color.Red;
//    optionsChoice = Color.Red;
//}
//else if (currentMenuChoice == 1 && optionsChoice != Color.White)
//{
//    optionsChoice = Color.White;
//    unlockablesChoice = Color.Red;
//    exitChoice = Color.Red;
//}
//else if (currentMenuChoice == 0 && exitChoice != Color.White)
//{
//    exitChoice = Color.White;
//    optionsChoice = Color.Red;
//}