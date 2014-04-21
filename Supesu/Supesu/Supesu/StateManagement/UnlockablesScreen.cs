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
    class UnlockablesScreen : Screen
    {
        Texture2D mUnlockablesScreenBackground;
        Game1 _game;
        //TODO: Fully implement the Unlockables screen, as it is now it's simply just meant to be there, in order to finish the TitleMenu.
        
        public UnlockablesScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mUnlockablesScreenBackground = content.Load<Texture2D>("Images/Unlockables");
            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                screenEvent.Invoke(this, new EventArgs());
                //TODO: implement UnlockablesScreen Update method.
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mUnlockablesScreenBackground, Vector2.Zero, Color.White);
            base.Draw(spriteBatch);
        }
        //TODO: Implement unlockables screen
    }
}
