using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Supesu.Weapons.Projectiles;

namespace Supesu.SpriteManagement
{
    class BonusMonster : Sprite
    {
        public BonusMonster(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life)
        {
            alive = true;
            Random rand = new Random();
            scoreAmount = rand.Next(0, 300) - 150;
            hitBox.Width = 50;
            hitBox.Height = 50;
        }

        public BonusMonster(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life, int millisecondsPerFrame)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty, millisecondsPerFrame)
        {
            alive = true;
            Random rand = new Random();
            scoreAmount = rand.Next(0, 300) - 150;
            hitBox.Width = 50;
            hitBox.Height = 50;
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
            SetHitbox();

            base.Update(gameTime, clientBounds);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
