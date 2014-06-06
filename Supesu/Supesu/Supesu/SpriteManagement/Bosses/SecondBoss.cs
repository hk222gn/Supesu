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

    class SecondBoss : Sprite
    {
        public List<Rectangle> hitBoxList = new List<Rectangle>();
        private SecondBossAttackStage sbas = SecondBossAttackStage.threeSplitterBullets;

        private readonly int amountOfBarrageBullets = 11;
        private bool changeBarrageStartPosition = false;
        private bool changeWaveStartPosition = false;
        private float changeBossStage = 0f;// Used as a timer between the different stages.
        private bool pauseStage = false;

        public SecondBoss(Game1 game, Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, bool animate, int life)
            : base(game, textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, animate, life * (int)InGameScreen.difficulty)
        {
            alive = true;

            scoreAmount = 120;

            //Adds hitboxes for the eyes. Order is: 1st left eye, 2nd middle eye, third right eye.
            hitBoxList.Add(new Rectangle(20 +(int)position.X, 32 + (int)position.Y, 50, 50));
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

            //Adds hitboxes for the eyes. Order is: 1st left eye, 2nd middle eye, third right eye.
            hitBoxList.Add(new Rectangle(20 + (int)position.X, 32 + (int)position.Y, 50, 50));
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

            if (!pauseStage)
            {
                //Depending on the current stage, fires projectiles with different timings.
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
            }
            else
            {
                //Renders the boss invulnerable and disables shooting for the player
                if (changeBossStage == 0f)
                {
                    Sounds.SoundBank.PlayCue("LaserWarning");
                    canTakeDamage = false;
                    Level.Ship.canShoot = false;
                }
                changeBossStage += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                //Allows the boss to take damage again and the player to shoot. The boss stage has changed.
                if (changeBossStage > 1000f)
                {
                    pauseStage = !pauseStage;
                    changeBossStage = 0f;
                    canTakeDamage = true;
                    Level.Ship.canShoot = true;
                }
            }

            if (Life >= 325)
            {
                currentFrame.X = 0;
            }
            //Boss life is under 325, change stage.
            else if (Life >= 200)
            {
                if (sbas != SecondBossAttackStage.barrage)
                {
                    pauseStage = !pauseStage;
                    //Removes the hitbox as the eye closes
                    hitBoxList.RemoveAt(hitBoxList.Count - 1);
                }
                //This closes the eye
                currentFrame.X = 1;
                //Changes the stage
                sbas = SecondBossAttackStage.barrage;
                
            }
            //Boss life is under 200, changer stage.
            else if (Life > 0)
            {
                if (sbas != SecondBossAttackStage.wave)
                {
                    pauseStage = !pauseStage;
                    //Removes the hitbox as the second eye closes
                    hitBoxList.RemoveAt(hitBoxList.Count - 1);
                }
                //This closes the second eye
                currentFrame.X = 2;
                //Changes the stage
                sbas = SecondBossAttackStage.wave;
            }
            else
            {
                //All eyes closed, boss is dead.
                currentFrame.X = 3;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
      
        public override void FireProjectile()
        {
            

            //Fires different patterns of bullets depending on the current stage.
            switch (sbas)
            {
                case SecondBossAttackStage.threeSplitterBullets:

                    //Used to give some of the projectiles random X speed.
                    Random random = new Random();
                    double mantissa = (random.NextDouble() * 1) - 0.5;
                    double exponent = Math.Pow(2.0, random.Next(-2, 2));
                    float speed = (float)(mantissa * exponent);

                    Level.AddBullet(new SplitterBullet(new Vector2(speed, -1), new Vector2(45 + (int)position.X, 56 + (int)position.Y), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 6));

                    Level.AddBullet(new SplitterBullet(new Vector2(0, -1), new Vector2(97 + (int)position.X, 77 + (int)position.Y), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 6));

                    Level.AddBullet(new SplitterBullet(new Vector2(speed, -1), new Vector2(146 + (int)position.X, 52 + (int)position.Y), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 6));
                    break;
                case SecondBossAttackStage.barrage:
                    if (!changeBarrageStartPosition)
                    {
                        for (int i = 0; i < amountOfBarrageBullets; i++)
                        {
                            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2(50 + 65 * i, (int)position.Y + 200), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 8));
                        }
                        changeBarrageStartPosition = true;
                    }
                    else
                    {
                        for (int i = 0; i < amountOfBarrageBullets; i++)
                        {
                            Level.AddBullet(new NormalEnemyBullet(new Vector2(0, -1), new Vector2(75 + 65 * (i + 1), (int)position.Y + 200), 0.5f, new Rectangle(0, 0, 12, 12), game.Content, 8));
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
