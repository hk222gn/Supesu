using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Supesu.Weapons.Projectiles;
using Supesu.SoundHandler;
using Supesu.StateManagement.Levels;

namespace Supesu.SpriteManagement.Monsters
{
    class SecondBoss : Sprite
    {
        public List<Rectangle> hitBoxList = new List<Rectangle>();

        public SecondBoss(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty)
        {
            alive = true;

            scoreAmount = 120;
            hitBoxList.Add(new Rectangle(20 +(int)position.X, 30 + (int)position.Y, 50, 50));
            hitBoxList.Add(new Rectangle(72 + (int)position.X, 52 + (int)position.Y, 50, 50));
            hitBoxList.Add(new Rectangle(121 + (int)position.X, 22 + (int)position.Y, 50, 50));
        }

        public SecondBoss(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life, int millisecondsPerFrame)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty, millisecondsPerFrame)
        {
            alive = true;

            scoreAmount = 120;

            hitBoxList.Add(new Rectangle(25 + (int)position.X, 30 + (int)position.X, 50, 50));
            hitBoxList.Add(new Rectangle(77 + (int)position.X, 48 + (int)position.X, 50, 50));
            hitBoxList.Add(new Rectangle(126 + (int)position.X, 17 + (int)position.X, 50, 50));
        }

        public override Vector2 direction
        {
            get { return new Vector2(0, 0); }
        }

        public override void FireProjectile()
        {

        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            shoot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (shoot >= 1)
            {
                shoot = 0;
                FireProjectile();
            }

            if (Life >= 350)
            {
                currentFrame.X = 1;
            }
            else if (Life >= 180)
            {
                currentFrame.X = 2;
            }
            else if (Life > 0)
            {
                currentFrame.X = 3;
            }
            else
            {
                currentFrame.X = 4;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
