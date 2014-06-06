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
        KeyboardState prevKeyboard;
        KeyboardState keyboard;
        private int scoresPerPage = 12;
        private int page = 0;

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
            keyboard = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                Sounds.SoundBank.PlayCue("MenuBack");
                screenEvent.Invoke(this, new EventArgs());
            }

            if (highScoreData.count >= 0)
            {
                if (CheckKeystroke(Keys.Left) && page > 0)
                {
                    //Change page
                    page -= 1;
                    Sounds.SoundBank.PlayCue("MenuChoiceChange");
                }

                if (CheckKeystroke(Keys.Right) && page < highScoreData.count / scoresPerPage)
                {
                    //Change page
                    page += 1;
                    Sounds.SoundBank.PlayCue("MenuChoiceChange");
                }
            }

            prevKeyboard = keyboard;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int totalScore = 0;

            spriteBatch.Draw(mHighscoreScreenBackground, Vector2.Zero, Color.White);

            spriteBatch.DrawString(bigText, "Esc for menu", new Vector2(1, 685), Color.Red);

            if (highScoreData.count >= 0)
            {
                //Draw highscores here
                
                //Draw rows.
                //After Player name
                spriteBatch.Draw(rowBars, new Rectangle(367, 100, 5, 500), Color.Red);
                //After Level
                spriteBatch.Draw(rowBars, new Rectangle(515, 100, 5, 500), Color.Red);
                //Under Player name, Level and Score
                spriteBatch.Draw(rowBars, new Rectangle(150, 140, 500, 5), Color.Red);

                //Row header
                spriteBatch.DrawString(bigText, "Player name", new Vector2(150, 100), Color.Red);
                spriteBatch.DrawString(bigText, "Level", new Vector2(400, 100), Color.Red);
                spriteBatch.DrawString(bigText, "Score", new Vector2(550, 100), Color.Red);

                //Draws the score from the loaded highscore file.
                if (scoresPerPage * (page + 1) > highScoreData.count)
                {
                    for (int i = scoresPerPage * page; i < highScoreData.count; i++)
                    {
                        spriteBatch.DrawString(normalText, highScoreData.playerName[i], new Vector2(200, 110 + 40 * (i % scoresPerPage + 1)), Color.Red);
                        spriteBatch.DrawString(normalText, "" + highScoreData.level[i], new Vector2(435, 110 + 40 * (i % scoresPerPage + 1)), Color.Red);

                        spriteBatch.DrawString(normalText, "" + highScoreData.score[i], new Vector2(565, 110 + 40 * (i % scoresPerPage + 1)), Color.Red);
                    }
                }
                else
                {
                    for (int i = scoresPerPage * page; i < scoresPerPage * (page + 1) - 1; i++)
                    {
                        spriteBatch.DrawString(normalText, highScoreData.playerName[i], new Vector2(200, 110 + 40 * (i % scoresPerPage + 1)), Color.Red);
                        spriteBatch.DrawString(normalText, "" + highScoreData.level[i], new Vector2(435, 110 + 40 * (i % scoresPerPage + 1)), Color.Red);

                        spriteBatch.DrawString(normalText, "" + highScoreData.score[i], new Vector2(565, 110 + 40 * (i % scoresPerPage + 1)), Color.Red);
                    }
                }
                
                //Draw page index
                spriteBatch.DrawString(normalText, "Page:", new Vector2(150, 600), Color.Red);
                for (int i = 0; i < highScoreData.count / scoresPerPage + 1; i++)
                {
                    if (i == page)
                        spriteBatch.DrawString(normalText, "" + (i + 1), new Vector2(210 + 20 * i, 600), Color.White);
                    else
                        spriteBatch.DrawString(normalText, "" + (i + 1), new Vector2(210 + 20 * i, 600), Color.Red);
                }

                // Calculates and draws the total combined score.
                for (int i = 0; i < highScoreData.count; i++)
                {
                    totalScore += highScoreData.score[i];
                }
                spriteBatch.DrawString(bigText, "Your total score is: " + totalScore, new Vector2(230, 650), Color.Red);
            }
            else
            {
                //THERE IS NO HIGHSCORES
                spriteBatch.DrawString(bigText, "There are no highscores!", new Vector2(_game.Window.ClientBounds.Width / 2, _game.Window.ClientBounds.Height / 2), Color.Red);
            }
            base.Draw(spriteBatch);
        }

        private bool CheckKeystroke(Keys key)
        {
            return (keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key));
        }
    }
}
