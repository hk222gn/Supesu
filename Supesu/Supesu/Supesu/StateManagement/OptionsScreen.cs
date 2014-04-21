using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Supesu.StateManagement
{
    class OptionsScreen : Screen
    {
        Texture2D mOptionsScreenBackground;
        Game1 _game;
        //TODO: Fully implement the options menu, as it is now it's simply just meant to be there, in order to finish the TitleMenu.
        
        public OptionsScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mOptionsScreenBackground = content.Load<Texture2D>("Images/Options");
            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                screenEvent.Invoke(this, new EventArgs());
                //TODO: implement OptionsScreen Update method.
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mOptionsScreenBackground, Vector2.Zero, Color.White);
            base.Draw(spriteBatch);
        }
    }
}
