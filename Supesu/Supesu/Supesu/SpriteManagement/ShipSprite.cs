﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Supesu.Weapons.Projectiles;
using Microsoft.Xna.Framework.Audio;

namespace Supesu.SpriteManagement
{
    class ShipSprite : Sprite
    {
        //TODO: Make sounds global, kinda, so all have deathsounds,hitsounds etc.

        KeyboardState keyboard, prevKeyboard;
        public ShipSprite(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life)
        {
            alive = true;
            death = game.Content.Load<SoundEffect>(@"Sounds/Death");
            struck = game.Content.Load<SoundEffect>(@"Sounds/Struck");
        }

        public ShipSprite(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life, int millisecondsPerFrame)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life, millisecondsPerFrame)
        {
            alive = true;
            death = game.Content.Load<SoundEffect>(@"Sounds/Death");
            struck = game.Content.Load<SoundEffect>(@"Sounds/Struck");
        }

        public override Vector2 direction
        {
            get 
            {
                Vector2 inputDirection = Vector2.Zero;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    inputDirection.X -= 0.6f;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    inputDirection.X += 0.6f;
                }
                return inputDirection * speed;
            }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            keyboard = Keyboard.GetState();
            // Move the sprite based on direction
            position += direction;
            // If sprite is off the screen, move it back within the game window
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 600;
            if (position.X > clientBounds.Width - frameSize.X)
                position.X = clientBounds.Width - frameSize.X;
            if (position.Y > clientBounds.Height - frameSize.Y)
                position.Y = clientBounds.Height - frameSize.Y;

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

            FireProjectile();

            prevKeyboard = keyboard;

            SetHitbox();

            base.Update(gameTime, clientBounds);
        }

        public override void FireProjectile()
        {
            //Press X to shoot, then decides which bullet to fire and creates an instance of that bullet.
            if (CheckKeystroke(Keys.X))
            {
                if (game.bulletType == BulletType.standard)
                {
                    Bullet.Add(new StandardBullet(new Vector2(0, 1), new Vector2((this.position.X + frameSize.X / 2) - 2, this.position.Y + 20), 1.2f, new Rectangle(0, 0, 3, 5), game.Content));
                }
                else if (game.bulletType == BulletType.special)
                {
                    Bullet.Add(new SpecialBullet(new Vector2(0, 1), new Vector2((this.position.X + frameSize.X / 2) - 2, this.position.Y + 20), 1.8f, new Rectangle(0, 0, 3, 5), game.Content));
                }
            }

            //Switches weapon, this is only for testing purposes.
            //if (CheckKeystroke(Keys.D1))
            //{
            //    game.bulletType = BulletType.standard;
            //}
            //else if (CheckKeystroke(Keys.D2))
            //{
            //    game.bulletType = BulletType.special;
            //}
        }

        private bool CheckKeystroke(Keys key)
        {
            return (keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key));
        }
    }
}