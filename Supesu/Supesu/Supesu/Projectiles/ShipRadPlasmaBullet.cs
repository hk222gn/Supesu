﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Supesu.Weapons.Projectiles
{
    class ShipRadPlasmaBullet : DefaultBullet
    {
        public ShipRadPlasmaBullet(Vector2 direction, Vector2 position, float speed, Rectangle hitBox, ContentManager content)
            : base(direction, position, speed, hitBox)
        {
            LoadTexture(content);
            //Sets a hitbox for the bullet
            SetHitbox();
        }

        public override void LoadTexture(ContentManager content)
        {
            bulletTexture = content.Load<Texture2D>("Images/FourthPlayerBullet");
        }

        public override void Update(float elapsedTime)
        {
            base.Update(elapsedTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletTexture, position, Color.White);
            base.Draw(spriteBatch);
        }
    }
}