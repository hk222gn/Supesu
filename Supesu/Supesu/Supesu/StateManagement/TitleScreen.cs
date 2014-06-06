using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Supesu.SoundHandler;
using System.Threading;

namespace Supesu
{
    public enum MenuChoices
    {
        StartGame = 4,
        Highscores = 3,
        Unlockables = 2,
        Options = 1,
        Exit = 0,
        Empty
    }
    class TitleScreen : Screen
    {
        Texture2D mTitleScreenBackground;
        Texture2D supesuTitle;
        Vector2 titlePosition = new Vector2(300, -50);
        Game1 game;
        SpriteFont spFont;
        KeyboardState prevKeyboard;
        KeyboardState keyboard;
        private MenuChoices menuChoices = MenuChoices.StartGame;
        private Color startGameChoice = Color.White;
        private Color highscoreChoice = Color.Red;
        private Color unlockablesChoice = Color.Red;
        private Color optionsChoice = Color.Red;
        private Color exitChoice = Color.Red;

        public TitleScreen(ContentManager content, EventHandler theScreenEvent, Game1 game1)
            : base(theScreenEvent)
        {
            mTitleScreenBackground = content.Load<Texture2D>("Images/Title");
            supesuTitle = content.Load<Texture2D>(@"Images/SupesuTitle");
            game = game1;
            spFont = content.Load<SpriteFont>("Fonts/General");
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            if (CheckKeystroke(Keys.Enter))
            {
                Sounds.SoundBank.PlayCue("MenuHit");
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
                        Thread.Sleep(400);
                        game.Exit();
                        break;
                    default:
                        break;
                }
            }

            //User pressed the up key.
            if (CheckKeystroke(Keys.Up) && (int)menuChoices < 4)
            {
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
                SetAllRed();
                menuChoices += 1;
            }
            //User pressed the down key.
            else if (CheckKeystroke(Keys.Down) == true && (int)menuChoices > 0)
            {
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
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
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTitleScreenBackground, Vector2.Zero, Color.White);

            //Draw title.
            DrawTitle(spriteBatch);

            //This is drawing the menu text. We will handle the menu with the keyboard so no hitboxes are needed.
            spriteBatch.DrawString(spFont, "Start Game", new Vector2(50, 220), startGameChoice);
            spriteBatch.DrawString(spFont, "Highscores", new Vector2(50, 320), highscoreChoice);
            spriteBatch.DrawString(spFont, "Unlockables", new Vector2(50, 420), unlockablesChoice);
            spriteBatch.DrawString(spFont, "Options", new Vector2(50, 520), optionsChoice);
            spriteBatch.DrawString(spFont, "Exit", new Vector2(50, 620), exitChoice);

            base.Draw(spriteBatch);
        }

        private void DrawTitle(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(supesuTitle, titlePosition, Color.White);

            if (titlePosition.Y < 100)
            {
                titlePosition.Y += 2;
            }
        }

        public void SetAllRed()
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