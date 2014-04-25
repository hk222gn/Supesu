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
    abstract class Sprite
    {
        Texture2D texture1;
        protected Point frameSize;
        Point currentFrame;
        Point sheetSize;
        int collisionOffset;
        int timeSinceLastFrame = 0;
        int millisecondsEachFrame;
        const int defaultMillisecondsEachFrame = 16;
        protected Vector2 speed;
        protected Vector2 position;
        protected Color color1 = Color.White;
        private bool animate;
        

        public abstract Vector2 direction
        {
            get;
        }

        public Sprite(Texture2D texture, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, bool animate)
            : this(texture, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, animate,
            defaultMillisecondsEachFrame)
        {

        }

        public Sprite(Texture2D texture, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            bool animate, int millisecondsEachFrame)
        {
            this.texture1 = texture;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            this.millisecondsEachFrame = millisecondsEachFrame;
            this.animate = animate;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture1, position, new Rectangle(currentFrame.X * frameSize.X,
                currentFrame.Y * frameSize.Y,
                frameSize.X, frameSize.Y),
                color1, 0, Vector2.Zero,
                1f, SpriteEffects.None, 0);
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            if (animate)
            {
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > millisecondsEachFrame)
                {
                    timeSinceLastFrame = 0;
                    ++currentFrame.X;
                    if (currentFrame.X >= sheetSize.X)
                    {
                        currentFrame.X = 0;
                        ++currentFrame.Y;
                        if (currentFrame.Y >= sheetSize.Y)
                            currentFrame.Y = 0;
                    }
                }
                return;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                currentFrame.X = 2;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                currentFrame.X = 0;
            }
            else
                currentFrame.X = 1;
        }
    }
}
