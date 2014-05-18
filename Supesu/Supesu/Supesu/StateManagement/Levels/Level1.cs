using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Supesu.SpriteManagement;

namespace Supesu.StateManagement.Levels
{
    class Level1 : Level
    {
        //Used to make the sprites.
        int amountOfEnemiesPerRow = 1;
        int makeSpace = 60;
        int enemyStartPosition;
        int lastEnemyPosition;

        public Level1(ContentManager content, Game1 game)
            : base(content, game)
        {
            background = this.content.Load<Texture2D>(@"Images/Ingame");
            lastEnemyPosition = enemyStartPosition;
            enemyStartPosition = 80;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (boss != null)
            {
                boss.Update(gameTime, game.Window.ClientBounds);
            }

            base.Update(gameTime);
        }

        public override void InitializeStage1()
        {
            CreateFirstEnemy();
        }

        public override void InitializeStage2()
        {
            CreateSecondEnemy();
        }

        public override void InitializeStage3()
        {
            CreateThirdEnemy();
        }

        public override void InitializeStageBoss()
        {
            //SPAWN BOSS, THINK IS GOOD
            boss = new FirstBossSprite(game, game.Content.Load<Texture2D>(@"Images/FirstBossSprite"),
                new Vector2(game.Window.ClientBounds.Width / 2 - 100, -30),
                new Point(200, 200),
                5,
                new Point(0, 0),
                new Point(3, 1),
                new Vector2(2, 2),
                true,
                150,//If this is changed, change the value in InGameScreen => Draw boss life.
                150);
        }

        private void CreateFirstEnemy()
        {
            for (int i = 0; i < amountOfEnemiesPerRow; i++)
            {
                enemyList.Add(new StandardEnemySprite(game, game.Content.Load<Texture2D>(@"Images/StandardEnemySprite"),
                new Vector2(lastEnemyPosition + makeSpace, 100 + makeSpace * 2),
                new Point(50, 50),
                5,
                new Point(0, 0),
                new Point(3, 1),
                new Vector2(2, 2),
                true,
                6, //Enemy life.
                100));

                lastEnemyPosition += makeSpace;
            }

            lastEnemyPosition = enemyStartPosition;
        }

        private void CreateSecondEnemy()
        {
            for (int i = 0; i < amountOfEnemiesPerRow; i++)
            {
                enemyList.Add(new StandardEnemySprite(game, game.Content.Load<Texture2D>(@"Images/StandardEnemySprite"),
                new Vector2(lastEnemyPosition + makeSpace, 100 + makeSpace * 2),
                new Point(50, 50),
                5,
                new Point(0, 0),
                new Point(3, 1),
                new Vector2(2, 2),
                true,
                6, //Enemy life.
                100));

                lastEnemyPosition += makeSpace;
            }

            lastEnemyPosition = enemyStartPosition;

            for (int i = 0; i < amountOfEnemiesPerRow; i++)
            {
                enemyList.Add(new SecondaryEnemySprite(game, game.Content.Load<Texture2D>(@"Images/SecondaryEnemyTransparent"),
                    new Vector2(lastEnemyPosition + makeSpace, 100 + makeSpace),
                    new Point(50, 50),
                    5,
                    new Point(0, 0),
                    new Point(3, 1),
                    new Vector2(2, 2),
                    true,
                    9, // Enemy life.
                    100));

                lastEnemyPosition += makeSpace;
            }

            lastEnemyPosition = enemyStartPosition;
        }

        private void CreateThirdEnemy()
        {
            for (int i = 0; i < amountOfEnemiesPerRow; i++)
            {
                enemyList.Add(new StandardEnemySprite(game, game.Content.Load<Texture2D>(@"Images/StandardEnemySprite"),
                new Vector2(lastEnemyPosition + makeSpace, 100 + makeSpace * 2),
                new Point(50, 50),
                5,
                new Point(0, 0),
                new Point(3, 1),
                new Vector2(2, 2),
                true,
                6, //Enemy life.
                100));

                lastEnemyPosition += makeSpace;
            }

            lastEnemyPosition = enemyStartPosition;

            for (int i = 0; i < amountOfEnemiesPerRow; i++)
            {
                enemyList.Add(new SecondaryEnemySprite(game, game.Content.Load<Texture2D>(@"Images/SecondaryEnemyTransparent"),
                    new Vector2(lastEnemyPosition + makeSpace, 100 + makeSpace),
                    new Point(50, 50),
                    5,
                    new Point(0, 0),
                    new Point(3, 1),
                    new Vector2(2, 2),
                    true,
                    9, // Enemy life.
                    100));

                lastEnemyPosition += makeSpace;
            }

            lastEnemyPosition = enemyStartPosition;

            for (int i = 0; i < amountOfEnemiesPerRow; i++)
            {
                enemyList.Add(new ThirdEnemySprite(game, game.Content.Load<Texture2D>(@"Images/ThirdEnemy"),
                    new Vector2(lastEnemyPosition + makeSpace, 100),
                    new Point(50, 50),
                    5,
                    new Point(0, 0),
                    new Point(3, 1),
                    new Vector2(2, 2),
                    true,
                    12, // Enemy life.
                    100));

                lastEnemyPosition += makeSpace;
            }

            lastEnemyPosition = enemyStartPosition;
        }

    }
}
