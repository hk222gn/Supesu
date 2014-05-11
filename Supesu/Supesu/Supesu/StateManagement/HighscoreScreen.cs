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
using System.IO;

namespace Supesu.StateManagement
{
    class HighscoreScreen : Screen
    {
        Texture2D mHighscoreScreenBackground;
        Texture2D rowBars;
        Game1 _game;
        HighScores.HighScoreData highScoreData;
        SpriteFont bigText, normalText;

        Color[] colorData = { Color.Linen };
        
        public HighscoreScreen(ContentManager content, EventHandler theScreenEvent, Game1 game)
            : base(theScreenEvent)
        {
            mHighscoreScreenBackground = content.Load<Texture2D>("Images/Highscore");
            _game = game;

            rowBars = new Texture2D(game.GraphicsDevice, 1, 1);
            rowBars.SetData<Color>(colorData);

            bigText = content.Load<SpriteFont>(@"Fonts/General");
            normalText = content.Load<SpriteFont>(@"Fonts/SpriteFont1");
            //Loads up all the highscores in the save file.
            if (File.Exists(HighScores.fileName))
            {
                highScoreData = HighScores.LoadHighScores(HighScores.fileName);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                Sounds.SoundBank.PlayCue("MenuBack");
                screenEvent.Invoke(this, new EventArgs());
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int totalScore = 0;

            spriteBatch.Draw(mHighscoreScreenBackground, Vector2.Zero, Color.White);

            spriteBatch.DrawString(bigText, "Esc for title screen", new Vector2(1, 685), Color.Red);

            if (highScoreData.count >= 0)
            {
                //TODO: Draw this with formulas instead.
                //Draw highscores here

                //Draw rows.
                //After Player name
                spriteBatch.Draw(rowBars, new Rectangle(367, 100, 5, 500), Color.Red);
                //After Level
                spriteBatch.Draw(rowBars, new Rectangle(515, 100, 5, 500), Color.Red);
                //Under Player name, Level and Score
                spriteBatch.Draw(rowBars, new Rectangle(150, 140, 500, 5), Color.Red);

                //Row info
                spriteBatch.DrawString(bigText, "Player name", new Vector2(150, 100), Color.Red);
                spriteBatch.DrawString(bigText, "Level", new Vector2(400, 100), Color.Red);
                spriteBatch.DrawString(bigText, "Score", new Vector2(550, 100), Color.Red);

                //Draws the score from the loaded highscore file.
                for (int i = 0; i < highScoreData.count; i++)
                {
                    spriteBatch.DrawString(normalText, highScoreData.playerName[i], new Vector2(200, 110 + 40 * (i + 1)), Color.Red);
                    spriteBatch.DrawString(normalText, "" + highScoreData.level[i], new Vector2(435, 110 + 40 * (i + 1)), Color.Red);
                    
                    if (highScoreData.score[i] == 420)
                    {
                        spriteBatch.DrawString(normalText, "" + highScoreData.score[i] + " BLAZE IT FAGGET", new Vector2(565, 110 + 40 * (i + 1)), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(normalText, "" + highScoreData.score[i], new Vector2(565, 110 + 40 * (i + 1)), Color.Red);
                    }
                    totalScore += highScoreData.score[i];
                }

                //Draws the total combined score.
                spriteBatch.DrawString(bigText, "Your total score is: " + totalScore, new Vector2(230, 620), Color.Red);
            }
            else
            {
                //THERE IS NO HIGHSCORES
                spriteBatch.DrawString(bigText, "Your total score is: " + totalScore, new Vector2(_game.Window.ClientBounds.Width / 2 - 230, _game.Window.ClientBounds.Height / 2), Color.Red);
            }
            base.Draw(spriteBatch);
        }
        //TODO: Implement highscore screen
    }
}
