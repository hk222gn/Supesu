using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Supesu.SpriteManagement
{
    class StandardEnemySprite : Sprite
    {
        public StandardEnemySprite(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate)
        {
            alive = true;
        }

        public StandardEnemySprite(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int millisecondsPerFrame)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, millisecondsPerFrame)
        {
            alive = true;
        }

        public override Vector2 direction
        {
            get { return new Vector2(0, 0); }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {

            base.Update(gameTime, clientBounds);
        }
    }
}
