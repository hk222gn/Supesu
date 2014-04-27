using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Supesu.Weapons.Projectiles
{
    public enum BulletType
    {
        standard,
        special
    }
    public class DefaultBullet
    {
        protected static Texture2D bulletTexture;
        protected float speed;
        protected Vector2 velocity;
        protected Vector2 position;
        public int damageAmount;
        public Rectangle hitBox;
        public bool alive = true;
        public GameWindow gameWindow;

        public DefaultBullet(Vector2 direction, Vector2 position, float speed, int damageAmount)
        {
            this.velocity = direction;
            this.position = position;
            this.speed = speed;
            this.damageAmount = damageAmount;
            //Sets a hitbox for the bullet
            hitBox.Width = 3;
            hitBox.Height = 5;
            SetHitbox();
        }

        public virtual void Update(float elapsedTime)
        {
            if (this.position.Y <= 0)
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
