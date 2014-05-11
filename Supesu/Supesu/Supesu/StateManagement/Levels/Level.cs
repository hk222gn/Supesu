using Supesu.SpriteManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Supesu.HighScore;
using Supesu.SoundHandler;
using System.Threading;

namespace Supesu.StateManagement.Levels
{
    abstract class Level
    {
        //Make abstract method here for stage management. IE, a class where the current stage of the level is decided. 
        //You override it in the level1+ classes and decide there what is going to happen in each stage.

        public List<Sprite> enemyList = new List<Sprite>();
        public FirstBossSprite boss;
        public Sprite ship;
        public ContentManager content;
        public Texture2D background;
        public Game1 game;
        public bool startScoreMultiplierTimer = false, saved = false; //TODO: When the player gets to level 2, change this to false again.
        public float timeUntillScoreMultiplierChanceOver = 0f, scoreMultiplierTimer = 0f;
        public int killsInScoreMultiplierChance = 0;
        public CurrentLevelStage stage = CurrentLevelStage.enemyStage1;

        bool stage1Initialized = false, stage2Initialized = false, stage3Initialized = false, bossStageInitialized = false;

        public Level(ContentManager content, Game1 game)
        {
            this.content = content;
            this.game = game;

            //If there is no ship created, make one.
            
            if (ship == null)
            {
                CreateShip();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            ship.Draw(spriteBatch);

            if (boss != null)
            {
                boss.Draw(spriteBatch);
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
            GameStageHandler();

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
                    //Adds score for the enemy kill.
                    InGameScreen.playerScore += (enemyList[i].scoreAmount * InGameScreen.scoreMultiplier) * (int)InGameScreen.difficulty;
                    //Plays deathsound for enemy and removes it.
                    Sounds.SoundBank.PlayCue("EnemyDeath");
                    enemyList.Remove(enemyList[i]);
                    //An enemy has been killed, start the timer for multiplier X2 and add a kill to the required kill amount.
                    startScoreMultiplierTimer = true;
                    killsInScoreMultiplierChance += 1;
                }
                else
                {
                    enemyList[i].Update(gameTime, game.Window.ClientBounds);
                }
            }

            //Handles the score mutliplier
            if (startScoreMultiplierTimer)
            {
                //If the player killed enough monsters.
                if (killsInScoreMultiplierChance >= 6 && InGameScreen.scoreMultiplier == 1 && timeUntillScoreMultiplierChanceOver < 5)
                {
                    InGameScreen.scoreMultiplier += 1;
                }

                //Multiplier active
                if (killsInScoreMultiplierChance >= 5 && scoreMultiplierTimer < 10)
                {
                    scoreMultiplierTimer += (float)game.TargetElapsedTime.TotalSeconds;

                    //Reset the multiplier
                    if (scoreMultiplierTimer >= 10)
                    {
                        startScoreMultiplierTimer = false;
                        timeUntillScoreMultiplierChanceOver = 0;
                        scoreMultiplierTimer = 0;
                        killsInScoreMultiplierChance = 0;
                        if (InGameScreen.scoreMultiplier > 1)
                        {
                            InGameScreen.scoreMultiplier -= 1;
                        }
                    }
                }//Played didn't kill fast enough
                else if (timeUntillScoreMultiplierChanceOver == 5)
                {
                    startScoreMultiplierTimer = false;
                    timeUntillScoreMultiplierChanceOver = 0;
                }
                else
                {//An enemy was killed, start the timer.
                    timeUntillScoreMultiplierChanceOver += (float)game.TargetElapsedTime.TotalSeconds;
                }
                
            }
           

            //Updates the bullets position, and removes it if it hits the top of the screen.
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

        private void CreateShip()
        {
            if (true)
            {
                
            }
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

        public virtual void GameStageHandler()
        {
            switch (stage)
            {
                case CurrentLevelStage.enemyStage1:

                    if (!stage1Initialized)
                    {
                        InitializeStage1();
                        stage1Initialized = true;
                    }

                    if (enemyList.Count == 0)
                    {
                        stage = CurrentLevelStage.enemyStage2;
                    }
                    break;
                case CurrentLevelStage.enemyStage2:

                    if (!stage2Initialized)
                    {
                        InitializeStage2();
                        stage2Initialized = true;
                    }

                    if (enemyList.Count == 0)
                    {
                        stage = CurrentLevelStage.enemyStage3;
                    }
                    break;
                case CurrentLevelStage.enemyStage3:

                    if (!stage3Initialized)
                    {
                        InitializeStage3();
                        stage3Initialized = true;
                    }

                    if (enemyList.Count == 0)
                    {
                        stage = CurrentLevelStage.bossStage;
                    }
                    break;
                case CurrentLevelStage.bossStage:

                    //Spawn the boss if there is none
                    if (boss == null && stage != CurrentLevelStage.playerWonStage && !bossStageInitialized)
                    {
                        InitializeStageBoss();
                        bossStageInitialized = true;
                    }

                    //The boss is dead, give score, remove him and show a win screen.
                    if (boss != null && !boss.alive)
                    {
                        //Gives score for the boss kill.
                        Sounds.SoundBank.PlayCue("BossDeath");
                        stage = CurrentLevelStage.playerWonStage;
                        InGameScreen.playerScore += boss.scoreAmount * InGameScreen.scoreMultiplier;
                        boss = null;
                        Thread.Sleep(1000);
                    }
                    break;
                case CurrentLevelStage.playerWonStage:
                    WinScreen();

                    if (!saved)
                    {
                        HighScores.SaveHighscoreToFile();
                        saved = true;
                    }
                    
                    break;
                default:
                    break;
            }
        }

        public abstract void InitializeStage1();

        public abstract void InitializeStage2();

        public abstract void InitializeStage3();

        public abstract void InitializeStageBoss();

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
                        else
                            Sounds.SoundBank.PlayCue("EnemyStruck");
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
                                //If the enemy has more than 0 HP, make a struck sound. We don't want to interfere with the death sound.
                            else
                                Sounds.SoundBank.PlayCue("EnemyStruck");
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
                            Sounds.SoundBank.PlayCue("ShipStruck");
                        }
                    }
                    else if (!boss.laserStruckTarget && boss.laserRight.hitBox.Intersects(ship.hitBox))
                    {
                        ship.Life -= boss.laserRight.damageAmount;
                        boss.laserStruckTarget = true;

                        if (ship.Life > 0)
                        {
                            Sounds.SoundBank.PlayCue("ShipStruck");
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
                            Sounds.SoundBank.PlayCue("ShipStruck");
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
                            Sounds.SoundBank.PlayCue("ShipStruck");
                        }
                        enemyList[i].Bullet[q].alive = false;
                    }
                }
                
            }

            if (ship.Life <= 0)
            {
                ship.alive = false;

                Sounds.SoundBank.PlayCue("ShipDeath");
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
                    }
                    else
                    {
                        enemyList[i].position.X -= 12;
                    }
                    enemyList[i].move = 0;

                    //enemyList[i].hitBox.X = (int)enemyList[i].position.X;
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
                            //enemyList[q].hitBox.Y = (int)enemyList[q].position.Y;
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
                if (Sprite.shoot >= 2500)
                {
                    Sprite.shoot = 0;
                    enemyList[i].FireProjectile();
                }
            }
        }
    }
}
