using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Supesu.Weapons;
using Supesu.Weapons.Projectiles;

namespace Supesu.SpriteManagement
{
    abstract class Sprite
    {
        protected Texture2D texture1;
        protected Point frameSize;
        protected Point currentFrame;
        protected Point sheetSize;
        protected int collisionOffset;
        protected int timeSinceLastFrame = 0;
        protected int millisecondsEachFrame;
        protected const int defaultMillisecondsEachFrame = 16;
        protected Vector2 speed;
        protected Vector2 position;
        protected Color color1 = Color.White;
        protected bool animate;
        public bool alive;
        public Rectangle hitBox;
        protected Game1 game;
        protected List<DefaultBullet> bullet = new List<DefaultBullet>();

        public List<DefaultBullet> Bullet
        {
            get { return bullet; }
        }

        public abstract Vector2 direction
        {
            get;
        }

        public Sprite(Game1 game,Texture2D texture, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, bool animate)
            : this(game, texture, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, animate,
            defaultMillisecondsEachFrame)
        {

        }

        public Sprite(Game1 game, Texture2D texture, Vector2 position, Point frameSize,
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
            this.game = game;
            SetHitbox();
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
        }

        //moves the hitbox with the sprite, also sets the hitbox range if there is none set.
        public void SetHitbox()
        {
            hitBox.X = (int)position.X;
            hitBox.Y = (int)position.Y;
            if (hitBox.Width == 0)
            {
                hitBox.Width = 50;
                hitBox.Height = 50;
            }
        }
    }
}
