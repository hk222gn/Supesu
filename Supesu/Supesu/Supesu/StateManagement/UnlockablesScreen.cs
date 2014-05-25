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

namespace Supesu.StateManagement
{
    public enum ShipType
    {
        standard,
        second,
        third,
        fourth
    }

    public enum BulletType
    {
        standard,
        second,
        third,
        fourth
    }

    public enum UnlockableRow
    {
        ship,
        bullet
    }
    class UnlockablesScreen : Screen
    {
        Texture2D mUnlockablesScreenBackground;
        Game1 _game;
        SpriteFont bigText, mediumText;
        private int totalScore = 0; // Used to see if the player can activate the unlockables.
        public static ShipType shipType = ShipType.standard;// Decides which ship will be used
        public static BulletType bulletType = BulletType.standard;// Decides which bullet will be used
        private UnlockableRow unlockableRow = UnlockableRow.ship;
        private KeyboardState keyboard;
        private KeyboardState prevKeyboard;
        private Texture2D standardShip, secondShip, thirdShip, fourthShip;
        private Texture2D standardBullet, secondBullet, thirdBullet, fourthBullet;
        private Texture2D unlockableFrame;
        private Texture2D notChosen;
        private Texture2D notUnlocked;

        public static ShipType ShipType
        {
            get { return shipType; }
        }

        public static BulletType BulletType
        {
            get { return bulletType; }
        }
        public UnlockablesScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mUnlockablesScreenBackground = content.Load<Texture2D>("Images/Unlockables");
            _game = game;
            bigText = content.Load<SpriteFont>(@"Fonts/General");
            mediumText = content.Load<SpriteFont>(@"Fonts/SpriteFont1");
            LoadAllTextures(content);
            //Initiate totalScore;
            GetTotalScore();
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                Sounds.SoundBank.PlayCue("MenuBack");

                CheckIfChosenUnlockableIsUnlocked();

                screenEvent.Invoke(this, new EventArgs());
            }

            if (CheckKeystroke(Keys.Left))
            {
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
                if (unlockableRow == UnlockableRow.ship && shipType > 0)
                {
                    shipType -= 1;
                }
                else if (unlockableRow == UnlockableRow.bullet && bulletType > 0)
                {
                    bulletType -= 1;
                }
            }

            if (CheckKeystroke(Keys.Right))
            {
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
                if (unlockableRow == UnlockableRow.ship && (int)shipType < 3)
                {
                    shipType += 1;
                }

                else if (unlockableRow == UnlockableRow.bullet && (int)bulletType < 3)
                {
                    bulletType += 1;
                }
                
            }

            if (CheckKeystroke(Keys.Up) && unlockableRow > 0)
            {
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
                unlockableRow -= 1;
            }

            if (CheckKeystroke(Keys.Down) && (int)unlockableRow < 1)
            {
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
                unlockableRow += 1;
            }

            prevKeyboard = keyboard;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mUnlockablesScreenBackground, Vector2.Zero, Color.White);

            spriteBatch.DrawString(bigText, "Your total score: " + totalScore, new Vector2(2, -1), Color.Red);

            spriteBatch.DrawString(bigText, "Esc for menu", new Vector2(1, 685), Color.Red);

            // First unlockable header.
            spriteBatch.DrawString(bigText, "Ships", new Vector2(100, 100), Color.Red);
            DrawShipUnlockables(spriteBatch);
            // Second unlockable header.
            spriteBatch.DrawString(bigText, "Weapons", new Vector2(100, 220), Color.Red);
            DrawBulletUnlockables(spriteBatch);

            base.Draw(spriteBatch);
        }

        private void DrawShipUnlockables(SpriteBatch spriteBatch)
        {
            //First ship type
            spriteBatch.Draw(unlockableFrame, new Rectangle(110, 150, 52, 52), Color.White);
            spriteBatch.Draw(standardShip, new Vector2(111, 152), new Rectangle(50, 0, 50, 50),//Decides which part of the sprite to draw.
            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (shipType != ShipType.standard)
            {
                spriteBatch.Draw(notChosen, new Rectangle(110, 150, 52, 52), Color.White);
            }

            //Second ship type
            spriteBatch.Draw(unlockableFrame, new Rectangle(172, 150, 52, 52), Color.White);
            if (totalScore < 2500)
            {
                spriteBatch.Draw(notUnlocked, new Rectangle(173, 152, 50, 50), Color.White);
            }
            else
            {
                spriteBatch.Draw(secondShip, new Vector2(173, 152), new Rectangle(50, 0, 50, 50),//Decides which part of the sprite to draw.
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if (shipType != ShipType.second)
            {
                spriteBatch.Draw(notChosen, new Rectangle(172, 150, 52, 52), Color.White);
            }

            //Third ship type
            spriteBatch.Draw(unlockableFrame, new Rectangle(233, 150, 52, 52), Color.White);
            if (totalScore < 7500)
            {
                spriteBatch.Draw(notUnlocked, new Rectangle(234, 152, 50, 50), Color.White);
            }
            else
            {
                spriteBatch.Draw(thirdShip, new Vector2(234, 152), new Rectangle(50, 0, 50, 50),//Decides which part of the sprite to draw.
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if (shipType != ShipType.third)
            {
                spriteBatch.Draw(notChosen, new Rectangle(233, 150, 52, 52), Color.White);
            }

            //Fourth ship type
            spriteBatch.Draw(unlockableFrame, new Rectangle(294, 150, 52, 52), Color.White);
            if (totalScore < 15000)
            {
                spriteBatch.Draw(notUnlocked, new Rectangle(295, 152, 50, 50), Color.White);
            }
            else
            {
                spriteBatch.Draw(fourthShip, new Vector2(295, 152), new Rectangle(50, 0, 50, 50),//Decides which part of the sprite to draw.
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if (shipType != ShipType.fourth)
            {
                spriteBatch.Draw(notChosen, new Rectangle(294, 150, 52, 52), Color.White);
            }
        }

        private void DrawBulletUnlockables(SpriteBatch spriteBatch)
        {
            //First weapon type
            spriteBatch.Draw(unlockableFrame, new Rectangle(110, 270, 52, 52), Color.White);
            spriteBatch.Draw(standardBullet, new Vector2(134, 292), new Rectangle(0, 0, 5, 8),
            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (bulletType != BulletType.standard)
            {
                spriteBatch.Draw(notChosen, new Rectangle(110, 270, 52, 52), Color.White);
            }

            //Second weapon type
            spriteBatch.Draw(unlockableFrame, new Rectangle(172, 270, 52, 52), Color.White);
            if (totalScore < 2500)
            {
                spriteBatch.Draw(notUnlocked, new Rectangle(173, 272, 50, 50), Color.White);
            }
            else
            {
                spriteBatch.Draw(secondBullet, new Vector2(196, 292), new Rectangle(0, 0, 5, 8),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if (bulletType != BulletType.second)
            {
                spriteBatch.Draw(notChosen, new Rectangle(172, 270, 52, 52), Color.White);
            }

            //Third weapon type
            spriteBatch.Draw(unlockableFrame, new Rectangle(233, 270, 52, 52), Color.White);
            if (totalScore < 7500)
            {
                spriteBatch.Draw(notUnlocked, new Rectangle(234, 272, 50, 50), Color.White);
            }
            else
            {
                spriteBatch.Draw(thirdBullet, new Vector2(251, 288), new Rectangle(2, 2, 16, 16),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if (bulletType != BulletType.third)
            {
                spriteBatch.Draw(notChosen, new Rectangle(233, 270, 52, 52), Color.White);
            }

            //Fourth weapon type
            spriteBatch.Draw(unlockableFrame, new Rectangle(294, 270, 52, 52), Color.White);
            if (totalScore < 15000)
            {
                spriteBatch.Draw(notUnlocked, new Rectangle(295, 272, 50, 50), Color.White);
            }
            else
            {
                spriteBatch.Draw(fourthBullet, new Vector2(313, 282), new Rectangle(0, 0, 15, 27),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if (bulletType != BulletType.fourth)
            {
                spriteBatch.Draw(notChosen, new Rectangle(294, 270, 52, 52), Color.White);
            }
        }

        private void CheckIfChosenUnlockableIsUnlocked()
        {
            if (shipType == ShipType.second && totalScore < 2500)
            {
                shipType = ShipType.standard;
            }
            else if (shipType == ShipType.third && totalScore < 7500)
            {
                shipType = ShipType.standard;
            }
            else if (shipType == ShipType.fourth && totalScore < 15000)
            {
                shipType = ShipType.standard;
            }

            if (bulletType == BulletType.second && totalScore < 2500)
            {
                bulletType = BulletType.standard;
            }
            else if (bulletType == BulletType.third && totalScore < 7500)
            {
                bulletType = BulletType.standard;
            }
            else if (bulletType == BulletType.fourth && totalScore < 15000)
            {
                bulletType = BulletType.standard;
            }
        }

        private void LoadAllTextures(ContentManager content)
        {
            //Unlockable frames, etc
            unlockableFrame = content.Load<Texture2D>(@"Images/UnlockableFrame");
            notChosen = content.Load<Texture2D>(@"Images/NotChosen");
            notUnlocked = content.Load<Texture2D>(@"Images/NotUnlocked");

            //Ship
            standardShip = content.Load<Texture2D>(@"Images/ShipTrans");
            secondShip = content.Load<Texture2D>(@"Images/Ship2");
            thirdShip = content.Load<Texture2D>(@"Images/ThirdShip");
            fourthShip = content.Load<Texture2D>(@"Images/FourthShip");
            
            //Bullets
            standardBullet = content.Load<Texture2D>(@"Images/StandardBullet");
            secondBullet = content.Load<Texture2D>(@"Images/SpecialBullet");
            thirdBullet = content.Load<Texture2D>(@"Images/ThirdPlayerBullet");
            fourthBullet = content.Load<Texture2D>(@"Images/FourthPlayerBullet");
        }

        private void GetTotalScore()
        {
            HighScores.HighScoreData data = HighScores.LoadHighScores(HighScores.fileName);

            for (int i = 0; i < data.count; i++)
            {
                totalScore += data.score[i];
            }
        }

        private bool CheckKeystroke(Keys key)
        {
            return (keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key));
        }
    }
}
