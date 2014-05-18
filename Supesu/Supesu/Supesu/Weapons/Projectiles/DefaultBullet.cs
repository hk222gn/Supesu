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
    public class DefaultBullet
    {
        protected Texture2D bulletTexture;
        protected float speed;
        public Vector2 velocity;
        public Vector2 position;
        public int damageAmount;
        public Rectangle hitBox;
        public bool alive = true;
        public bool split = false;

        public DefaultBullet(Vector2 direction, Vector2 position, float speed, Rectangle hitBox)
        {
            this.velocity = direction;
            this.position = position;
            this.speed = speed;
            
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
