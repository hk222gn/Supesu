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
    class SpecialBullet : DefaultBullet
    {
        public SpecialBullet(Vector2 direction, Vector2 position, float speed, int damageAmount, ContentManager content)
            : base(direction, position, speed, damageAmount)
        {
            LoadTexture(content);
        }

        public override void LoadTexture(ContentManager content)
        {
            bulletTexture = content.Load<Texture2D>("Images/SpecialBullet");
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
