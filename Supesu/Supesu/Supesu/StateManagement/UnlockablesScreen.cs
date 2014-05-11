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
        standard
    }
    class UnlockablesScreen : Screen
    {
        Texture2D mUnlockablesScreenBackground;
        Game1 _game;
        private int totalScore = 0; // Used to see if the player can activate the unlockables.
        public static ShipType shipType = ShipType.standard;// Decides which ship will be used
        
        public UnlockablesScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mUnlockablesScreenBackground = content.Load<Texture2D>("Images/Unlockables");
            _game = game;
            //Initiate totalScore;
            GetTotalScore();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                Sounds.SoundBank.PlayCue("MenuBack");
                screenEvent.Invoke(this, new EventArgs());
                //TODO: implement UnlockablesScreen Update method.
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mUnlockablesScreenBackground, Vector2.Zero, Color.White);
            base.Draw(spriteBatch);
        }

        private void GetTotalScore()
        {
            HighScores.HighScoreData data = HighScores.LoadHighScores(HighScores.fileName);

            for (int i = 0; i < data.count; i++)
            {
                totalScore += data.score[i];
            }
        }
        //TODO: Implement unlockables screen
    }
}
