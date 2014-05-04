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
using Supesu.StateManagement.Levels;

namespace Supesu
{
    public enum CurrentLevel
    {
        none,
        level1,
        level2,
        level3,
        bonusLevel
    }

    public enum CurrentLevelStage
    {
        none,
        enemyStage1,
        enemyStage2,
        enemyStage3,
        bossStage
    }
    //FIX IN ORDER
    //TODO: Make changes to the boss laser, only one laser is shown for some reason.
    //TODO: Make the enemies shoot faster, but only one from each row.
    //TODO: General, there's most likely a memory leak due to the way i handle sprite loading.
    //TODO: Everything feels a bit clunky, think about that shit.
    
    
    class InGameScreen : Screen
    {
        Texture2D mPauseBackground;
        Game1 _game;
        private bool isPaused = false;
        private bool pauseKeyDown = false;
        KeyboardState prevKeyboard;
        KeyboardState keyboard;
        public CurrentLevel level = CurrentLevel.level1;
        Level currentLevel;
        ContentManager content;
        
        public InGameScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mPauseBackground = content.Load<Texture2D>("Images/PausedYes");
            _game = game;
            this.content = content;

            //Sets the current level to 1 and initializes it.
            currentLevel = new Level1(content, game);
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            if (!isPaused)
            {
                if (currentLevel != null && currentLevel.stage == CurrentLevelStage.none)
                {
                    //TODO: Make a death screen appear or something.
                    if (currentLevel != null)
	                {
                        currentLevel = null;
                        Thread.Sleep(500);
	                }
                }
                else if (currentLevel == null)
                {
                    if (CheckKeystroke(Keys.R))
                    {
                        currentLevel = new Level1(content, _game);
                    }
                    else if (CheckKeystroke(Keys.Escape))
                    {
                        screenEvent.Invoke(this, new EventArgs());
                        return;
                    }
                }
                else
                {
                    switch (level)
                    {
                        case CurrentLevel.level1:
                            break;
                        case CurrentLevel.level2:
                            break;
                        case CurrentLevel.level3:
                            break;
                        case CurrentLevel.bonusLevel:
                            break;
                        default:
                            break;
                    }
                    currentLevel.Update(gameTime);
                }
            }

            CheckPauseKey(keyboard);

            prevKeyboard = keyboard;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isPaused)
            {
                if (currentLevel == null)
                {
                    spriteBatch.Draw(mPauseBackground, Vector2.Zero, Color.Red);
                }
                else
                    currentLevel.Draw(spriteBatch);
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
    }
}
