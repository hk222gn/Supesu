using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Supesu.SpriteManagement;

namespace Supesu
{
    class InGameScreen : Screen
    {
        Texture2D mInGameScreenBackground;
        Texture2D mPauseBackground;
        Game1 _game;
        private bool isPaused = false;
        private bool pauseKeyDown = false;
        KeyboardState prevKeyboard;
        KeyboardState keyboard;
        Sprite ship;
        List<Sprite> spriteList = new List<Sprite>();
        List<Sprite> enemyList = new List<Sprite>();
        
        public InGameScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mInGameScreenBackground = content.Load<Texture2D>("Images/Ingame");
            mPauseBackground = content.Load<Texture2D>("Images/PausedYes");
            _game = game;

            InitializeGameSprites();
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            CheckPauseKey(keyboard);

            if (!isPaused)
            {
                //Updates the ships position, and which frame to draw.
                ship.Update(gameTime, _game.Window.ClientBounds);

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

                //Updates all the enemies and removes them incase they're dead.
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (!enemyList[i].alive)
                    {
                        enemyList.Remove(enemyList[i]);
                    }
                    else
                    {
                        enemyList[i].Update(gameTime, _game.Window.ClientBounds);
                    }
                }

                CheckBulletCollision();
                
            }

            prevKeyboard = keyboard;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isPaused)
            {
                spriteBatch.Draw(mInGameScreenBackground, Vector2.Zero, Color.White);

                ship.Draw(spriteBatch);

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
            else
            {
                spriteBatch.Draw(mPauseBackground, Vector2.Zero, Color.White);
                //DRAW PAUSE STUFF HERE OK
            }

            base.Draw(spriteBatch);
        }

        private bool CheckKeystroke(Keys key)
        {
            return (keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key));
        }

        private void BeginPause()
        {
            isPaused = true;
            //TODO: Pause stuff here.
        }

        private void EndPause()
        {
            isPaused = false;
            //TODO: Resume stuff here.
        }

        private void CheckPauseKey(KeyboardState ks)
        {
            bool pauseKeyDownThisFrame = (ks.IsKeyDown(Keys.Escape));

            if (CheckKeystroke(Keys.Enter) && isPaused)
            {
                isPaused = false;
                screenEvent.Invoke(this, new EventArgs());
                return;
            }

            if (!pauseKeyDown && pauseKeyDownThisFrame)
            {
                if (!isPaused)
                    BeginPause();
                else
                    EndPause();
            }
            pauseKeyDown = pauseKeyDownThisFrame;
        }

        private void InitializeGameSprites()
        {
            int amountOfEnemiesPerRow = 10;
            int makeSpace = 60;
            int enemyStartPosition = _game.Window.ClientBounds.Width / amountOfEnemiesPerRow;
            int lastEnemyPosition = enemyStartPosition;

            //If there is no ship created, make one.
            if (ship == null)
            {
                ship = new ShipSprite(_game, _game.Content.Load<Texture2D>(@"Images/ShipTrans"),
                new Vector2(_game.Window.ClientBounds.Width / 2 - 25, 600),
                new Point(50, 50),
                5,
                new Point(1, 0),
                new Point(3, 1),
                new Vector2(9, 9),
                false);
            }

            for (int i = 0; i < amountOfEnemiesPerRow; i++)
            {
                enemyList.Add(new StandardEnemySprite(_game, _game.Content.Load<Texture2D>(@"Images/StandardEnemySprite"),
                new Vector2(lastEnemyPosition + makeSpace, 100),
                new Point(50, 50),
                5,
                new Point(0, 0),
                new Point(3, 1),
                new Vector2(2, 2),
                true, 100));

                lastEnemyPosition += makeSpace;
            }
            lastEnemyPosition = enemyStartPosition;
            for (int i = 0; i < amountOfEnemiesPerRow; i++)
            {
                enemyList.Add(new SecondaryEnemySprite(_game, _game.Content.Load<Texture2D>(@"Images/SecondaryEnemyTransparent"), 
                    new Vector2(lastEnemyPosition + makeSpace, 100 + makeSpace),
                    new Point(50, 50),
                    5,
                    new Point(0, 0),
                    new Point(3, 1),
                    new Vector2(2, 2),
                    true, 100));

                lastEnemyPosition += makeSpace;
            }

            //TODO: Add some kind of script to make enemies/phase handling easier.
            
        }

        private void CheckBulletCollision()
        {
            for (int i = 0; i < ship.Bullet.Count; i++)
            {
                for (int q = 0; q < enemyList.Count; q++)
			    {
                    if (ship.Bullet[i].hitBox.Intersects(enemyList[q].hitBox))
                    {
                        enemyList[q].alive = false;
                        ship.Bullet[i].alive = false;
                    }
			    }
                
            }
            
        }

    }
}
