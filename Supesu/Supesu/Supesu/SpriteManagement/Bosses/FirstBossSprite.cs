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

namespace Supesu.SpriteManagement
{
    class FirstBossSprite : Sprite
    {
        float activateLaser = 0; // Timer for the laser
        bool laserActive = false, warningActivated = false;
        public bool laserStruckTarget;
        public DefaultBullet laserLeft, laserRight;

        Texture2D warningTexture;
        Color[] colorData = { Color.Linen };

        public FirstBossSprite(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty)
        {
            alive = true;

            scoreAmount = 60;

            warningTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            warningTexture.SetData<Color>(colorData);

            laserStruckTarget = false;

            //Creates the left hitbox.
            hitBox.X = 37 + (int)position.X;
            hitBox.Y = 156 + (int)position.Y;
            hitBox.Width = 30;
            hitBox.Height = 47;

            //Creates the right hitbox.
            nonMoveableBossHitbox.X = 121 + (int)position.X;
            nonMoveableBossHitbox.Y = 147 + (int)position.Y;
            nonMoveableBossHitbox.Width = 30;
            nonMoveableBossHitbox.Height = 47;
        }

        public FirstBossSprite(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life, int millisecondsPerFrame)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty, millisecondsPerFrame)
        {
            alive = true;

            scoreAmount = 60;

            warningTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            warningTexture.SetData<Color>(colorData);

            laserStruckTarget = false;

            //Creates the left hitbox.
            hitBox.X = 37 + (int)position.X;
            hitBox.Y = 156 + (int)position.Y;
            hitBox.Width = 30;
            hitBox.Height = 47;

            //Creates the right hitbox.
            nonMoveableBossHitbox.X = 121 + (int)position.X;
            nonMoveableBossHitbox.Y = 147 + (int)position.Y;
            nonMoveableBossHitbox.Width = 30;
            nonMoveableBossHitbox.Height = 47;
        }

        public override Vector2 direction
        {
            get { return new Vector2(0, 0); }
        }

        public override void FireProjectile()
        {
            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2((this.position.X + frameSize.X / 2) - 7, this.position.Y + 170), 0.65f, new Rectangle(0, 0, 12, 12), game.Content, 4));

            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2((this.position.X + frameSize.X / 2) - 37, this.position.Y + 160), 0.65f, new Rectangle(0, 0, 12, 12), game.Content, 4));

            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2((this.position.X + frameSize.X / 2) + 23, this.position.Y + 160), 0.65f, new Rectangle(0, 0, 12, 12), game.Content, 4));
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            shoot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (shoot >= 1)
            {
                shoot = 0;
                FireProjectile();
            }

            activateLaser += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Handles the boss laser stages.
            if (activateLaser >= 2 && !warningActivated && !laserActive)
            {
                //Warn the user about incoming laser
                Sounds.SoundBank.PlayCue("LaserWarning");
                warningActivated = true;
            }
            if (activateLaser >= 4)
            {
                laserLeft = new FirstBossLaser(Vector2.Zero, new Vector2((this.position.X + frameSize.X / 2) - 70, this.position.Y + 190), 0f, new Rectangle((int)(this.position.X + frameSize.X / 2 - 70), (int)this.position.Y + 190, 40, 720), game.Content, 9);
                laserRight = new FirstBossLaser(Vector2.Zero, new Vector2((this.position.X + frameSize.X / 2) + 30, this.position.Y + 190), 0f, new Rectangle((int)(this.position.X + frameSize.X / 2 + 30), (int)this.position.Y + 190, 40, 720), game.Content, 9);

                laserActive = true;
                
                activateLaser = 0;

                warningActivated = false;
            }
            else if (activateLaser >= 1.5 && laserActive)
            {
                laserLeft = null;
                laserRight = null;

                activateLaser = 0;
                laserActive = false;
                laserStruckTarget = false;
            }

            base.Update(gameTime, clientBounds);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            if (laserLeft != null && laserRight != null)
            {
                laserLeft.Draw(spriteBatch);
                laserRight.Draw(spriteBatch);
            }

            if (warningActivated)
            {
                spriteBatch.Draw(warningTexture, new Rectangle(330, 160, 40, 30), Color.FromNonPremultiplied(255, 174, 201, 280));
                spriteBatch.Draw(warningTexture, new Rectangle(430, 160, 40, 30), Color.FromNonPremultiplied(255, 174, 201, 280));
            }

            base.Draw(spriteBatch);
        }
    }
}
