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
    class InGameScreenLevel1 : Screen
    {
        Texture2D mInGameScreenLevel1Background;
        Game1 _game;
        
        public InGameScreenLevel1(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mInGameScreenLevel1Background = content.Load<Texture2D>("Images/Ingame");
            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                screenEvent.Invoke(this, new EventArgs());
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mInGameScreenLevel1Background, Vector2.Zero, Color.White);
            base.Draw(spriteBatch);
        }
    }
}
