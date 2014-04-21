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
    class ControlDetectorScreen : Screen
    {
        Texture2D mControlDetectorScreenBackground;
        SpriteFont spFont;
        Game _game;
        KeyboardState keyboard;
        KeyboardState prevKeyboard;

        public ControlDetectorScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mControlDetectorScreenBackground = content.Load<Texture2D>("Images/Control");

            spFont = content.Load<SpriteFont>("Fonts/General");
            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            for (int aPlayer = 0; aPlayer < 4; aPlayer++)
            {
                //If the enter key is pressed, store that in prevKeyboard.
                if (keyboard.IsKeyDown(Keys.Enter) == true)
                {
                    prevKeyboard = keyboard;
                }
                //If the key was released, it will run this IF instead, meaning it was released.
                else if (prevKeyboard.IsKeyDown(Keys.Enter))
                {
                    playerOne = (PlayerIndex)aPlayer;
                    screenEvent.Invoke(this, new EventArgs());
                    return;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mControlDetectorScreenBackground, Vector2.Zero, Color.White);

            spriteBatch.DrawString(spFont, "Please press enter", new Vector2(_game.Window.ClientBounds.Width / 2 - 130, _game.Window.ClientBounds.Height - 100), Color.Red);
            base.Draw(spriteBatch);
        }
    }
}
