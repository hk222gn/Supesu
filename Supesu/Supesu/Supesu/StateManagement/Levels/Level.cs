using Supesu.SpriteManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Supesu.StateManagement.Levels
{
    abstract class Level
    {
        public List<Sprite> enemyList = new List<Sprite>();
        public FirstBossSprite boss;
        public Sprite ship;
        public ContentManager content;
        public Texture2D background;
        public Game1 game;
        public CurrentLevelStage stage = CurrentLevelStage.enemyStage1;

        public Level(ContentManager content, Game1 game)
        {
            this.content = content;
            this.game = game;

            //If there is no ship created, make one.
            if (ship == null)
            {
                ship = new ShipSprite(game, game.Content.Load<Texture2D>(@"Images/ShipTrans"),
                new Vector2(game.Window.ClientBounds.Width / 2 - 25, 600),
                new Point(50, 50),
                5,
                new Point(1, 0),
                new Point(3, 1),
                new Vector2(9, 9),
                false
                , 16);//Standard life, for now. Use an enum, set values, make 10 * dificulty, 4,2,1??

                ship.hitBox.Width = 35;
                ship.hitBox.Height = 40;
            }

            InitializeLevelSprites();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            ship.Draw(spriteBatch);

            switch (stage)
            {
                case CurrentLevelStage.enemyStage1:
                    break;
                case CurrentLevelStage.enemyStage2:
                    break;
                case CurrentLevelStage.enemyStage3:
                    break;
                case CurrentLevelStage.bossStage:

                    //Draws the boss if he exists
                    if (boss != null)
                    {
                        boss.Draw(spriteBatch);
                    }

                    break;
                default:
                    break;
            }
            //Draws all player created bullets.
            foreach (var item in ship.Bullet)
            {
                item.Draw(spriteBatch);
            }

            //Draws all the enemies.
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Draw(spriteBatch);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            CheckBulletCollision();

            //TODO: If the player dies, kill the ship.
            if (!ship.alive)
            {
                stage = CurrentLevelStage.none;
                return;
            }

            //Updates the ships position, and which frame to draw.
            ship.Update(gameTime, game.Window.ClientBounds);

            for (int i = 0; i < enemyList.Count; i++)
            {
                if (!enemyList[i].alive)
                {
                    enemyList.Remove(enemyList[i]);
                }
                else
                {
                    enemyList[i].Update(gameTime, game.Window.ClientBounds);
                }
            }

            //Updates the bullets position, anda removes it if it hits the top of the screen.
            for (int i = 0; i < ship.Bullet.Count; i++)
            {
                if (!ship.Bullet[i].alive)
                {
                    ship.Bullet.Remove(ship.Bullet[i]);
                }
                else
                {
                    ship.Bullet[i].Update(gameTime.ElapsedGameTime.Milliseconds);
                }
            }

            MoveSprites();

            if (enemyList.Count > 0)
            {
                EnemyShoot();
            }
        }

        public abstract void InitializeLevelSprites();

        public void WinScreen()
        {
            background = content.Load<Texture2D>(@"Images/Win");
        }

        public void CheckBulletCollision()
        {
            for (int i = 0; i < ship.Bullet.Count; i++)
            {
                //Checks if the target hit is a boss or a normal enemy.
                if (boss != null)
                {
                   if (ship.Bullet[i].hitBox.Intersects(boss.hitBox) || ship.Bullet[i].hitBox.Intersects(boss.nonMoveableBossHitbox))
                    {
                        boss.Life -= ship.Bullet[i].damageAmount;

                        ship.Bullet[i].alive = false;

                        if (boss.Life <= 0)
                        {
                            boss.alive = false;
                            return;
                        }
                    }
                }
                else
                {
                    for (int q = 0; q < enemyList.Count; q++)
                    {
                        if (ship.Bullet[i].hitBox.Intersects(enemyList[q].hitBox))
                        {
                            //Removes life from the enemy
                            enemyList[q].Life -= ship.Bullet[i].damageAmount;
                            //If the bullet hit something, remove it.
                            ship.Bullet[i].alive = false;

                            if (enemyList[q].Life <= 0)
                            {
                                //Sets the enemy to dead status incase the HP is 0 or below, the enemy is removed in the main update function for the game state.
                                enemyList[q].alive = false;
                            }
                        }
                    }
                }
            }

            if (boss != null)
            {
                if (boss.laserLeft != null && boss.laserRight != null)
                {
                    if (!boss.laserStruckTarget && boss.laserLeft.hitBox.Intersects(ship.hitBox))
                    {
                        ship.Life -= boss.laserLeft.damageAmount;
                        boss.laserStruckTarget = true;

                        if (ship.Life > 0)
                        {
                            ship.struck.Play();
                        }
                    }
                    else if (!boss.laserStruckTarget && boss.laserRight.hitBox.Intersects(ship.hitBox))
                    {
                        ship.Life -= boss.laserRight.damageAmount;
                        boss.laserStruckTarget = true;

                        if (ship.Life > 0)
                        {
                            ship.struck.Play();
                        }
                    }
                }

                for (int i = 0; i < boss.Bullet.Count; i++)
                {
                    if (boss.Bullet[i].hitBox.Intersects(ship.hitBox))
                    {
                        ship.Life -= boss.Bullet[i].damageAmount;

                        if (ship.Life > 0)
                        {
                            ship.struck.Play();
                        }
                        boss.Bullet[i].alive = false;
                    }
                }
            }

            for (int i = 0; i < enemyList.Count; i++)
            {
                for (int q = 0; q < enemyList[i].Bullet.Count; q++)
                {
                    if (enemyList[i].Bullet[q].hitBox.Intersects(ship.hitBox))
                    {
                        ship.Life -= enemyList[i].Bullet[q].damageAmount;

                        if (ship.Life > 0)
                        {
                            ship.struck.Play();
                        }
                        enemyList[i].Bullet[q].alive = false;
                    }
                }
                
            }

            if (ship.Life <= 0)
            {
                ship.alive = false;

                ship.death.Play();
            }
        }

        public void MoveSprites()
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].move >= 0.3f)
                {
                    if (enemyList[i].moveDirection)
                    {
                        enemyList[i].position.X += 12;
                        enemyList[i].hitBox.X += 12;
                    }
                    else
                    {
                        enemyList[i].position.X -= 12;
                        enemyList[i].hitBox.X -= 12;
                    }
                    enemyList[i].move = 0;
                }
            }

            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].position.X <= 0)
                {
                    enemyList[i].position.X = 0;
                    if (!enemyList[i].moveDirection)
                    {
                        for (int q = 0; q < enemyList.Count; q++)
                        {
                            enemyList[q].moveDirection = true;
                        }
                    }
                }
                else if (enemyList[i].position.X >= 760)
                {
                    enemyList[i].position.X = 755;
                    if (enemyList[i].moveDirection)
                    {
                        for (int q = 0; q < enemyList.Count; q++)
                        {
                            enemyList[q].moveDirection = false;
                            enemyList[q].position.Y += 25;
                        }
                    }
                }
            }
        }

        public void EnemyShoot()
        {
            Random rand = new Random();
            int i = rand.Next(0, enemyList.Count);

            if (enemyList.Contains(enemyList[i]))
            {
                if (Sprite.shoot >= 1500)
                {
                    Sprite.shoot = 0;
                    enemyList[i].FireProjectile();
                }
            }
        }
    }
}
