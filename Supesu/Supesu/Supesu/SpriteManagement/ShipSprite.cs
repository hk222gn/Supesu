using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Supesu.SpriteManagement
{
    class ShipSprite : Sprite
    {
        public ShipSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate)
        { 

        }
        public ShipSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, millisecondsPerFrame)
        { 

        }

        public override Vector2 direction
        {
            get 
            {
                Vector2 inputDirection = Vector2.Zero;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    inputDirection.X -= 1;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    inputDirection.X += 1;
                }
                return inputDirection * speed;
            }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move the sprite based on direction
            position += direction;
            // If sprite is off the screen, move it back within the game window
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 0;
            if (position.X > clientBounds.Width - frameSize.X)
                position.X = clientBounds.Width - frameSize.X;
            if (position.Y > clientBounds.Height - frameSize.Y)
                position.Y = clientBounds.Height - frameSize.Y;
            base.Update(gameTime, clientBounds);
        }
    }
}
