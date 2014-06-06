using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Supesu
{
    class Screen
    {
        protected EventHandler screenEvent;

        public Screen(EventHandler theScreenEvent)
        {
            screenEvent = theScreenEvent;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
