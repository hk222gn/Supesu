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

namespace Supesu.SpriteManagement.Monsters
{
    public enum SecondBossAttackStage
    {
        threeSplitterBullets, //Shoots one bullet from each eye, each bullet then splits into 3 bullets, the 2 on the edge with a x and y movement.
        barrage, //Shoots a large amount of bullets in waves with enough space to dodge relatively easy.
        wave //Shoots bullets in a wave formation from one side to the other, stopping just before it's impossible to dodge.
    }

    //TODO: Check hitboxes.
    class SecondBoss : Sprite
    {
        public List<Rectangle> hitBoxList = new List<Rectangle>();
        private SecondBossAttackStage sbas = SecondBossAttackStage.threeSplitterBullets;

        private readonly int amountOfBarrageBullets = 12;
        private bool changeBarrageStartPosition = false;
        private bool changeWaveStartPosition = false;

        public SecondBoss(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty)
        {
            alive = true;

            scoreAmount = 120;
            hitBoxList.Add(new Rectangle(20 +(int)position.X, 31 + (int)position.Y, 50, 50));
            hitBoxList.Add(new Rectangle(72 + (int)position.X, 52 + (int)position.Y, 50, 50));
            hitBoxList.Add(new Rectangle(121 + (int)position.X, 27 + (int)position.Y, 50, 50));
        }

        public SecondBoss(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life, int millisecondsPerFrame)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty, millisecondsPerFrame)
        {
            alive = true;

            scoreAmount = 120;
            hitBoxList.Add(new Rectangle(20 + (int)position.X, 31 + (int)position.Y, 50, 50));
            hitBoxList.Add(new Rectangle(72 + (int)position.X, 52 + (int)position.Y, 50, 50));
            hitBoxList.Add(new Rectangle(126 + (int)position.X, 17 + (int)position.Y, 50, 50));
        }

        public override Vector2 direction
        {
            get { return new Vector2(0, 0); }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            shoot += (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (sbas)
            {
                case SecondBossAttackStage.threeSplitterBullets:
                    if (shoot >= 0.8)
                    {
                        FireProjectile();
                        shoot = 0;
                    }
                    break;
                case SecondBossAttackStage.barrage:
                    if (shoot >= 0.4)
                    {
                        FireProjectile();
                        shoot = 0; 
                    }
                    break;
                case SecondBossAttackStage.wave:
                    if (shoot >= 1)
                    {
                        FireProjectile();
                        shoot = 0;
                    }
                    break;
                default:
                    break;
            }

            if (Life >= 325)
            {
                currentFrame.X = 0;
            }
            else if (Life >= 200)
            {
                currentFrame.X = 1;
                sbas = SecondBossAttackStage.barrage;
            }
            else if (Life > 0)
            {
                currentFrame.X = 2;
                sbas = SecondBossAttackStage.wave;
            }
            else
            {
                currentFrame.X = 3;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void FireProjectile()
        {
            Random random = new Random();
            double mantissa = (random.NextDouble() * 1) - 0.5;
            double exponent = Math.Pow(2.0, random.Next(-2, 2));
            float speed = (float)(mantissa * exponent);

            switch (sbas)
            {
                case SecondBossAttackStage.threeSplitterBullets:
                    Level.AddBullet(new SplitterBullet(new Vector2(speed, -1), new Vector2(45 + (int)position.X, 56 + (int)position.Y), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 6));

                    Level.AddBullet(new SplitterBullet(new Vector2(0, -1), new Vector2(97 + (int)position.X, 77 + (int)position.Y), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 6));

                    Level.AddBullet(new SplitterBullet(new Vector2(speed, -1), new Vector2(146 + (int)position.X, 52 + (int)position.Y), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 6));
                    break;
                case SecondBossAttackStage.barrage:
                    if (!changeBarrageStartPosition)
                    {
                        for (int i = 0; i < amountOfBarrageBullets; i++)
                        {
                            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2(50 + 60 * i, (int)position.Y + 200), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 8));
                        }
                        changeBarrageStartPosition = true;
                    }
                    else
                    {
                        for (int i = 0; i < amountOfBarrageBullets; i++)
                        {
                            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2(75 + 60 * (i + 1), (int)position.Y + 200), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 8));
                        }
                        changeBarrageStartPosition = false;
                    }
                    
                    break;
                case SecondBossAttackStage.wave:
                    if (!changeWaveStartPosition)
                    {
                        for (int i = 0; i < amountOfBarrageBullets * 2; i++)
                        {
                            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2(0 + 20 * (i + 1), (int)position.Y + 200), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 9));

                            changeWaveStartPosition = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < amountOfBarrageBullets * 2; i++)
                        {
                            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2(250 + 20 * (i + 1), (int)position.Y + 200), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 9));

                            changeWaveStartPosition = false;
                        }
                    }
                    
                    break;
                default:
                    break;
            }
        }
    }
}
