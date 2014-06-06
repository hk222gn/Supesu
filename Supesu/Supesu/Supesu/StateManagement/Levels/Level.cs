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
using Supesu.Weapons.Projectiles;
using System.Threading;
using Supesu.SpriteManagement.Monsters;

namespace Supesu.StateManagement.Levels
{
    abstract class Level
    {

        public List<Sprite> enemyList = new List<Sprite>();
        public FirstBossSprite boss;
        public SecondBoss secondBoss;
        public static ShipSprite ship;
        public ContentManager content;
        public Texture2D background;
        public Game1 game;
        public bool startScoreMultiplierTimer = false, saved = false;
        public float timeUntillScoreMultiplierChanceOver = 0f, scoreMultiplierTimer = 0f;
        public int killsInScoreMultiplierChance = 0;
        public CurrentLevelStage stage = CurrentLevelStage.enemyStage1;
        private BonusMonster bonusMonster;
        private Texture2D bonusMonsterTexture;
        private static List<DefaultBullet> bullets = new List<DefaultBullet>();
        public static List<DefaultBullet> shipBullets = new List<DefaultBullet>();

        bool stage1Initialized = false, stage2Initialized = false, stage3Initialized = false, bossStageInitialized = false;

        public static ShipSprite Ship
        {
            get { return ship; }
        }

        public Level(ContentManager content, Game1 game)
        {
            this.content = content;
            this.game = game;
            bonusMonsterTexture = content.Load<Texture2D>(@"Images/BonusMonster");

            //Allways make a new ship.
            CreateShip();

            bullets.RemoveRange(0, bullets.Count);
            shipBullets.RemoveRange(0, shipBullets.Count);

            //Sets the score to 0 whenever a new level is started.
            InGameScreen.playerScore = 0;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            ship.Draw(spriteBatch);

            if (boss != null)
            {
                boss.Draw(spriteBatch);
            }   
            else if (secondBoss != null)
            {
                secondBoss.Draw(spriteBatch);
            }

            //Draws the bonus monster incase he exists.
            if (bonusMonster != null)
            {
                bonusMonster.Draw(spriteBatch);
            }

            //Draws all player created bullets.
            foreach (var item in shipBullets)
            {
                item.Draw(spriteBatch);
            }

            //Draws all the enemies.
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Draw(spriteBatch);
            }

            //Draws all the bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            //Checks and changes the current wave in the level.
            GameStageHandler();

            //Checks if the player gets hit, or hits anything with his bullets.
            CheckBulletCollision();

            //If there is no bonus monster, take care of it.
            HandleBonusMonster();

            //Updates the bullets position, anda removes it if it hits the top of the screen.
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].alive)
                {
                    bullets.Remove(bullets[i]);
                }
                else
                {
                    bullets[i].Update(gameTime.ElapsedGameTime.Milliseconds);
                }
            }

            //If there is a bonus monster, run his update method instead.
            if (bonusMonster != null)
            {
                bonusMonster.Update(gameTime, game.Window.ClientBounds);
            }
            
            if (!ship.alive)
            {
                stage = CurrentLevelStage.none;
                return;
            }

            //Checks if the enemies went too far down.
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].position.Y > 580)
                {
                    ship.alive = false;
                }
            }
             
            //Updates the ships position, and which frame to draw.
            ship.Update(gameTime, game.Window.ClientBounds);

            CheckForDeadEnemies(gameTime);

            HandleScoreMultiplier();

            //Updates the bullets position, and removes it if it hits the top of the screen.
            for (int i = 0; i < Level.shipBullets.Count; i++)
            {
                if (!Level.shipBullets[i].alive)
                {
                    Level.shipBullets.Remove(Level.shipBullets[i]);
                }
                else
                {
                    Level.shipBullets[i].Update(gameTime.ElapsedGameTime.Milliseconds);
                }
            }

            Sprite.move += (float)game.TargetElapsedTime.TotalSeconds;

            MoveSprites();

            if (enemyList.Count > 0)
            {
                EnemyShoot();
            }
        }

        //Creates a ship depending on the chosen ShipType.
        private void CreateShip()
        {
            if (UnlockablesScreen.ShipType == ShipType.standard)
            {
                ship = new ShipSprite(game, game.Content.Load<Texture2D>(@"Images/ShipTrans"),
                    new Vector2(game.Window.ClientBounds.Width / 2 - 25, 600),
                    new Point(50, 50),
                    5,
                    new Point(1, 0),
                    new Point(3, 1),
                    new Vector2(9, 9),
                    false
                    , 20);

                ship.hitBox.Width = 35;
                ship.hitBox.Height = 40;
            }
            else if (UnlockablesScreen.ShipType == ShipType.second)
            {
                ship = new ShipSprite(game, game.Content.Load<Texture2D>(@"Images/Ship2"),
                    new Vector2(game.Window.ClientBounds.Width / 2 - 25, 600),
                    new Point(50, 50),
                    5,
                    new Point(1, 0),
                    new Point(3, 1),
                    new Vector2(9, 9),
                    false
                    , 35);

                ship.hitBox.Width = 35;
                ship.hitBox.Height = 40;
            }
            else if (UnlockablesScreen.ShipType == ShipType.third)
            {
                ship = new ShipSprite(game, game.Content.Load<Texture2D>(@"Images/ThirdShip"),
                    new Vector2(game.Window.ClientBounds.Width / 2 - 25, 600),
                    new Point(50, 50),
                    5,
                    new Point(1, 0),
                    new Point(3, 1),
                    new Vector2(9, 9),
                    false
                    , 45);

                ship.spriteOffSet = 4;
                ship.hitBox.Width = 42;
                ship.hitBox.Height = 40;
            }
            else if (UnlockablesScreen.ShipType == ShipType.fourth)
            {
                ship = new ShipSprite(game, game.Content.Load<Texture2D>(@"Images/FourthShip"),
                    new Vector2(game.Window.ClientBounds.Width / 2 - 25, 600),
                    new Point(50, 50),
                    5,
                    new Point(1, 0),
                    new Point(3, 1),
                    new Vector2(9, 9),
                    false
                    , 70);

                ship.hitBox.Width = 42;
                ship.hitBox.Height = 40;
            }
        }

        public virtual void GameStageHandler()
        {
            //Handles the different stages in a level.
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
                        if (InGameScreen.level == CurrentLevel.level1)
                        {
                            InGameScreen.maxBossLife = boss.Life; //Needed to draw the life bar correctly
                        }
                        else
                            InGameScreen.maxBossLife = secondBoss.Life; //Needed to draw the life bar correctly
                        
                    }

                    //The boss is dead, give score, remove him and show a win screen.
                    if (boss != null && !boss.alive)
                    {
                        //Gives score for the boss kill.
                        Sounds.SoundBank.PlayCue("BossDeath");
                        stage = CurrentLevelStage.playerWonStage;
                        InGameScreen.playerScore += boss.scoreAmount * InGameScreen.scoreMultiplier * (int)InGameScreen.difficulty;
                        boss = null;
                        bullets.Clear();
                        Thread.Sleep(1000);
                    }
                    else if (secondBoss != null && !secondBoss.alive)
                    {
                        //Gives score for the boss kill.
                        Sounds.SoundBank.PlayCue("BossDeath");
                        stage = CurrentLevelStage.playerWonStage;
                        InGameScreen.playerScore += secondBoss.scoreAmount * InGameScreen.scoreMultiplier * (int)InGameScreen.difficulty;
                        secondBoss = null;
                        bullets.Clear();
                        Thread.Sleep(1000);
                    }
                    break;
                case CurrentLevelStage.playerWonStage:
                    if (!saved)
                    {
                        HighScores.SaveHighscoreToFile();
                        saved = true;
                        if (InGameScreen.level == CurrentLevel.level1)
                        {
                            LevelWon();
                        }
                        else
                            WinScreen();
                        
                        InGameScreen.level += 1;
                    }
                    break;
                default:
                    break;
            }
        }

        #region abstract initializers

        public abstract void InitializeStage1();

        public abstract void InitializeStage2();

        public abstract void InitializeStage3();

        public abstract void InitializeStageBoss();

        #endregion

        private void HandleBonusMonster()
        {
            if (bonusMonster == null && boss == null && stage != CurrentLevelStage.playerWonStage)
            {
                Random rand = new Random();
                if (rand.Next(1, 1000) == 998)
                {
                    bonusMonster = new BonusMonster(game, bonusMonsterTexture, new Vector2(850, 50), new Point(50, 50), 5, new Point(0, 1), new Point(3, 1), new Vector2(5, 5), true, 5, 50);
                }
            }
            else if (bonusMonster != null)
            {
                if (bonusMonster.position.X <= -50)
                {
                    bonusMonster = null;
                }
            }
        }

        public void LevelWon()
        {
            background = content.Load<Texture2D>(@"Images/LevelComplete");
        }

        private void WinScreen()
        {
            background = content.Load<Texture2D>(@"Images/Win");
        }

        public void CheckBulletCollision()
        {
            for (int i = 0; i < Level.shipBullets.Count; i++)
            {
                //Checks if the target hit is a boss or a normal enemy.
                if (boss != null && boss.canTakeDamage)
                {
                   //Checks if the boss is hit by a ship bullet and sets him to dead incase he was killed by the shot.
                    if (Level.shipBullets[i].hitBox.Intersects(boss.hitBox) || Level.shipBullets[i].hitBox.Intersects(boss.nonMoveableBossHitbox))
                    {
                        boss.Life -= Level.shipBullets[i].damageAmount;

                        Level.shipBullets[i].alive = false;

                        if (boss.Life <= 0)
                        {
                            boss.alive = false;
                            return;
                        }
                        else
                            Sounds.SoundBank.PlayCue("EnemyStruck");
                    }
                }
                else if (secondBoss != null && secondBoss.canTakeDamage)
                {
                    for (int q = 0; q < secondBoss.hitBoxList.Count; q++)
                    {
                        if (Level.shipBullets[i].hitBox.Intersects(secondBoss.hitBoxList[q]))
                        {
                            secondBoss.Life -= Level.shipBullets[i].damageAmount;

                            Level.shipBullets[i].alive = false;

                            if (secondBoss.Life <= 0)
                            {
                                secondBoss.alive = false;
                            }
                            else
                                Sounds.SoundBank.PlayCue("EnemyStruck");
                        }
                    }
                }
                else
                {
                    for (int q = 0; q < enemyList.Count; q++)
                    {
                        if (Level.shipBullets[i].hitBox.Intersects(enemyList[q].hitBox))
                        {
                            //Removes life from the enemy
                            enemyList[q].Life -= Level.shipBullets[i].damageAmount;
                            //If the bullet hit something, remove it.
                            Level.shipBullets[i].alive = false;

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
                if (bonusMonster != null && Level.shipBullets[i].hitBox.Intersects(bonusMonster.hitBox))
                {
                    bonusMonster.Life -= Level.shipBullets[i].damageAmount;

                    Level.shipBullets[i].alive = false;

                    if (bonusMonster.Life <= 0)
                    {
                        bonusMonster.alive = false;
                    }
                    else
                        Sounds.SoundBank.PlayCue("EnemyStruck");
                }
            }

            //Handles the boss lasers and bullets
            if (boss != null)
            {
                //Lasers
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
            }

            //Checks if the enemy bullets struck the ship
            for (int q = 0; q < bullets.Count; q++)
            {
                if (bullets[q].hitBox.Intersects(ship.hitBox))
                {
                    ship.Life -= bullets[q].damageAmount;

                    if (ship.Life > 0)
                    {
                        Sounds.SoundBank.PlayCue("ShipStruck");
                    }
                    bullets[q].alive = false;
                }
                    
            }

            // If the ships life is 0, set flag to dead and play a death sound.
            if (ship.Life <= 0)
            {
                ship.alive = false;

                Sounds.SoundBank.PlayCue("ShipDeath");
            }
        }

        public void MoveSprites()
        {
            if (bonusMonster != null)
            {
                bonusMonster.position.X -= 2;
            }

            if (Sprite.move >= 0.4f)
            {
                for (int i = 0; i < enemyList.Count; i++)
                {
                    //If they reached the edge, move them down
                    if (Sprite.goDown)
                    {
                        for (int q = 0; q < enemyList.Count; q++)
                        {
                            enemyList[q].position.Y += 25;
                        }
                        Sprite.goDown = false;
                        break;
                    }
                    //Move sprites to the right
                    else if (Sprite.moveDirection)
                    {
                        enemyList[i].position.X += 12;
                    }
                    else
                    {
                        enemyList[i].position.X -= 12;
                    }
                }
                Sprite.move = 0;
            }

            //After moving them, check if any of them has struck the edges of the screen.
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].position.X <= 0)
                {
                    if (!Sprite.moveDirection)
                    {
                        Sprite.moveDirection = true;
                        break;
                    }
                }
                else if (enemyList[i].position.X >= 750)
                {
                    if (Sprite.moveDirection)
                    {
                        Sprite.moveDirection = false;
                        Sprite.goDown = true;
                        break;
                    }
                }
            }
        }

        public void EnemyShoot()
        {
            Random rand = new Random();
            int i = rand.Next(0, enemyList.Count);// Decides which enemy that will shoot

            // Fire delay, depending on how many enemies there are, we want to get some consistency.
            int shootWhen = rand.Next(2300, 2600);
            int shootWhen2 = rand.Next(1600, 1800);
            int shootWhen3 = rand.Next(900, 1100);

            if (enemyList.Contains(enemyList[i]))
            {
                if (enemyList.Count > 20)
                {
                    if (Sprite.shoot >= shootWhen)
                    {
                        Sprite.shoot = 0;
                        enemyList[i].FireProjectile();
                    }
                }
                else if (enemyList.Count > 10)
                {
                    if (Sprite.shoot >= shootWhen2)
                    {
                        Sprite.shoot = 0;
                        enemyList[i].FireProjectile();
                    }
                }
                else
                {
                    if (Sprite.shoot >= shootWhen3)
                    {
                        Sprite.shoot = 0;
                        enemyList[i].FireProjectile();
                    }
                }
                
            }
        }

        private void CheckForDeadEnemies(GameTime gameTime)
        {
            //Checks if any of the enemies are dead, incase one is, add score, play a death sound, remove the enemy and start the score multiplier.
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
                    if (!startScoreMultiplierTimer)
                    {
                        startScoreMultiplierTimer = true;
                    }
                    killsInScoreMultiplierChance += 1;
                }
                else
                {
                    enemyList[i].Update(gameTime, game.Window.ClientBounds);
                }
            }

            //Checks if the bonus monster exists and if his flag is set to dead, if this is ture, reward the player and remove him.
            if (bonusMonster != null && !bonusMonster.alive)
            {
                InGameScreen.playerScore += (bonusMonster.scoreAmount * InGameScreen.scoreMultiplier) * (int)InGameScreen.difficulty;
                Sounds.SoundBank.PlayCue("EnemyDeath");

                bonusMonster = null;
            }
        }

        private void HandleScoreMultiplier()
        {
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
        }

        public static void AddBullet(DefaultBullet bullet)
        {
            bullets.Add(bullet);
        }
    }
}
