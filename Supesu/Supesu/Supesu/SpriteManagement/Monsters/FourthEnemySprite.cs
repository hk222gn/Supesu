using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Supesu.Weapons.Projectiles;
using Supesu.StateManagement.Levels;

namespace Supesu.SpriteManagement.Monsters
{
    class FourthEnemySprite : Sprite
    {
        public FourthEnemySprite(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty)
        {
            alive = true;
            scoreAmount = 30;
            hitBox.Width = 50;
            hitBox.Height = 50;
        }

        public FourthEnemySprite(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life, int millisecondsPerFrame)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty, millisecondsPerFrame)
        {
            alive = true;
            scoreAmount = 30;
            hitBox.Width = 50;
            hitBox.Height = 50;
        }

        public override Vector2 direction
        {
            get { return new Vector2(0, 0); }
        }

        public override void FireProjectile()
        {

            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2((this.position.X + frameSize.X / 2) - 2, this.position.Y + 20), 0.50f, new Rectangle(0, 0, 12, 12), game.Content));
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            shoot += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            SetHitbox();

            base.Update(gameTime, clientBounds);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            base.Draw(spriteBatch);
        }
    }
}
