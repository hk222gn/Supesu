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
    class InGameScreen : Screen
    {
        Texture2D mInGameScreenBackground;
        Texture2D mPauseBackground;
        Game1 _game;
        private bool isPaused = false;
        private bool pauseKeyDown = false;
        KeyboardState prevKeyboard;
        KeyboardState keyboard;
        
        public InGameScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mInGameScreenBackground = content.Load<Texture2D>("Images/Ingame");
            mPauseBackground = content.Load<Texture2D>("Images/PausedYes");
            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            CheckPauseKey(keyboard);

            if (!isPaused)
            {
                // DO STUFF
            }

            prevKeyboard = keyboard;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isPaused)
            {
                spriteBatch.Draw(mInGameScreenBackground, Vector2.Zero, Color.White);
            }
            else
            {
                spriteBatch.Draw(mPauseBackground, Vector2.Zero, Color.White);
                //DRAW PAUSE STUFF HERE OK
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
    }
}
