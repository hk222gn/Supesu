using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Supesu.StateManagement
{
    class HighscoreScreen : Screen
    {
        Texture2D mHighscoreScreenBackground;
        Game1 _game;
        //TODO: Fully implement the Highscore screen, as it is now it's simply just meant to be there, in order to finish the TitleMenu.
        
        public HighscoreScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mHighscoreScreenBackground = content.Load<Texture2D>("Images/Highscore");
            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                screenEvent.Invoke(this, new EventArgs());
                //TODO: implement HighscoreScreen Update method.
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mHighscoreScreenBackground, Vector2.Zero, Color.White);
            base.Draw(spriteBatch);
        }
        //TODO: Implement highscore screen
    }
}
