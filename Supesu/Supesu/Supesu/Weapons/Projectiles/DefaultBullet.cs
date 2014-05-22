using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Supesu.StateManagement;

namespace Supesu.Weapons.Projectiles
{
    public class DefaultBullet
    {
        protected Texture2D bulletTexture;
        protected float speed;
        public Vector2 velocity;
        public Vector2 position;
        public readonly int damageAmount;
        public Rectangle hitBox;
        public bool alive = true;
        public bool split = false;

        //This is for initializing enemy bullets.
        public DefaultBullet(Vector2 direction, Vector2 position, float speed, Rectangle hitBox, int damageAmount)
        {
            this.velocity = direction;
            this.position = position;
            this.speed = speed;
            this.damageAmount = damageAmount * (int)InGameScreen.difficulty;
        }

        //This is for initializing the players bullets
        public DefaultBullet(Vector2 direction, Vector2 position, float speed, Rectangle hitBox)
        {
            this.velocity = direction;
            this.position = position;
            this.speed = speed;

            if (UnlockablesScreen.bulletType == BulletType.standard)
                this.damageAmount = 3;

            else if (UnlockablesScreen.bulletType == BulletType.second)
                this.damageAmount = 6;

            else if (UnlockablesScreen.bulletType == BulletType.third)
                this.damageAmount = 9;

            else
                this.damageAmount = 12;
        }

        public virtual void Update(float elapsedTime)
        {
            if (this.position.Y <= 0 || this.position.Y >= 720)
            {
                alive = false;
                return;
            }

            position -= velocity * elapsedTime * speed;
            SetHitbox();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void LoadTexture(ContentManager content)
        {

        }

        //Moves the position of the hitbox when the bullet moves.
        public void SetHitbox()
        {
            hitBox.X = (int)position.X;
            hitBox.Y = (int)position.Y;
        }
    }
}
