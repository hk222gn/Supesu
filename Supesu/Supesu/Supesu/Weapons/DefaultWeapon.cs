using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Supesu.SpriteManagement;

namespace Supesu.Weapons
{
    class DefaultWeapon
    {
        protected float timeToNextAttack = 0f;
        protected float delay = 1f;

        public DefaultWeapon(ShipSprite owner)
        {

        }

        public virtual void Update(float elapsedTime)
        {
            if (timeToNextAttack > 0f)
            {
                timeToNextAttack = MathHelper.Max(timeToNextAttack - elapsedTime, 0f);
            }
        }

        public virtual void Shoot(Vector2 direction)
        {
            if (timeToNextAttack > 0f)
            {
                return;
            }

            timeToNextAttack = delay;

            SpawnProjectile(direction);
        }

        public virtual void SpawnProjectile(Vector2 direction)
        {

        }
    }
}
