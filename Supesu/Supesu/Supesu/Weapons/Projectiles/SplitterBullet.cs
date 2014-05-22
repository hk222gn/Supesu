using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Supesu.StateManagement.Levels;

namespace Supesu.Weapons.Projectiles
{
    class SplitterBullet : DefaultBullet
    {
        private bool hasSplitted = false;
        private ContentManager content;
        public SplitterBullet(Vector2 direction, Vector2 position, float speed, Rectangle hitBox, ContentManager content, int damageAmount)
            : base(direction, position, speed, hitBox, damageAmount)
        {
            LoadTexture(content);
            this.content = content;
            //Sets a hitbox for the bullet
            SetHitbox();
        }

        public override void LoadTexture(ContentManager content)
        {
            bulletTexture = content.Load<Texture2D>("Images/NormalEnemyBullet");
        }

        public override void Update(float elapsedTime)
        {
            if (position.Y >= 200 && !hasSplitted)
            {
                Level.AddBullet(new NormalEnemyBullet(new Vector2(+0.5f, -1), new Vector2(this.position.X + 5, this.position.Y), 0.5f, new Rectangle(0, 0, 12, 12), content, 3));
                Level.AddBullet(new NormalEnemyBullet(new Vector2(-0.5f, -1), new Vector2(this.position.X - 5, this.position.Y), 0.5f, new Rectangle(0, 0, 12, 12), content, 3));

                hasSplitted = true;
            }

            base.Update(elapsedTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletTexture, position, Color.White);
            base.Draw(spriteBatch);
        }
    }
}
