using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Supesu.SoundHandler;

namespace Supesu.StateManagement
{
    public enum OptionsChoice
    {
        music,
        effect,
        vsync,
        fps
    }

    class OptionsScreen : Screen
    {
        GraphicsDeviceManager graphics;
        Texture2D mOptionsScreenBackground;
        Texture2D rowBar;
        Texture2D volumeSlider;
        Texture2D checkBox;
        Game1 _game;
        SpriteFont bigText;
        KeyboardState prevKeyboard;
        KeyboardState keyboard;
        AudioCategory backgroundMusic, effects;
        OptionsChoice optionsChoice = OptionsChoice.music;
        private Color musicColor = Color.White, effectsColor = Color.Red, vsyncColor = Color.Red, drawFpsColor = Color.Red;

        float musicVolume = 1.0f, effectsVolume = 1.0f;

        Color[] colorData = { Color.Linen };
        
        public OptionsScreen(ContentManager content, EventHandler theScreenEvent, Game1 game, GraphicsDeviceManager graphics)
            : base(theScreenEvent)
        {
            mOptionsScreenBackground = content.Load<Texture2D>("Images/Options");
            _game = game;
            bigText = content.Load<SpriteFont>("Fonts/General");
            backgroundMusic = Sounds.AudioEngine.GetCategory("Music");
            effects = Sounds.AudioEngine.GetCategory("Effects");
            volumeSlider = content.Load<Texture2D>(@"Images/VolumeBar");
            checkBox = content.Load<Texture2D>(@"Images/CheckBox");

            this.graphics = graphics;

            rowBar = new Texture2D(game.GraphicsDevice, 1, 1);
            rowBar.SetData<Color>(colorData);
        }

        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
            {
                Sounds.SoundBank.PlayCue("MenuBack");
                screenEvent.Invoke(this, new EventArgs());
            }

            //Depending on active option, allows the player to change that specific option
            if (optionsChoice == OptionsChoice.music)
            {
                if (CheckKeystroke(Keys.Left))
                {
                    musicVolume = MathHelper.Clamp(musicVolume - 0.1f, 0.0f, 1.0f);
                    Sounds.SoundBank.PlayCue("MenuChoiceChange");
                }
                else if (CheckKeystroke(Keys.Right))
                {
                    musicVolume = MathHelper.Clamp(musicVolume + 0.1f, 0.0f, 1.0f);
                    Sounds.SoundBank.PlayCue("MenuChoiceChange");
                }
            } 
            else if (optionsChoice == OptionsChoice.effect)
            {
                if (CheckKeystroke(Keys.Left))
                {
                    effectsVolume = MathHelper.Clamp(effectsVolume - 0.1f, 0.0f, 1.0f);
                    Sounds.SoundBank.PlayCue("MenuChoiceChange");
                }
                else if (CheckKeystroke(Keys.Right))
                {
                    effectsVolume = MathHelper.Clamp(effectsVolume + 0.1f, 0.0f, 1.0f);
                    Sounds.SoundBank.PlayCue("MenuChoiceChange");
                }
            }
            else if (optionsChoice == OptionsChoice.vsync)
            {
                if (CheckKeystroke(Keys.Enter))
                {
                    graphics.SynchronizeWithVerticalRetrace = !graphics.SynchronizeWithVerticalRetrace;
                    Sounds.SoundBank.PlayCue("MenuChoiceChange");
                }
            }
            else if (optionsChoice == OptionsChoice.fps)
            {
                if (CheckKeystroke(Keys.Enter))
                {
                    Game1.drawFps = !Game1.drawFps;
                    Sounds.SoundBank.PlayCue("MenuChoiceChange");
                }
            }
            //else if (optionsChoice == OptionsChoice.fullscreen)
            //{

            //    if (CheckKeystroke(Keys.Enter))
            //    {
            //        graphics.IsFullScreen = !graphics.IsFullScreen;
            //         Sounds.SoundBank.PlayCue("MenuChoiceChange");

            //        graphics.ApplyChanges();
            //    }
            //}

            //Change selected option
            if (CheckKeystroke(Keys.Up) && optionsChoice > 0)
            {
                optionsChoice -= 1;
                ResetMenuChoiceColors();
                MenuChoiceColors();
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
            }
            else if (CheckKeystroke(Keys.Down) && (int)optionsChoice < 3)
            {
                optionsChoice += 1;
                ResetMenuChoiceColors();
                MenuChoiceColors();
                Sounds.SoundBank.PlayCue("MenuChoiceChange");
            }

            backgroundMusic.SetVolume(musicVolume);
            effects.SetVolume(effectsVolume);

            prevKeyboard = keyboard;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mOptionsScreenBackground, Vector2.Zero, Color.White);

            spriteBatch.DrawString(bigText, "Esc for menu", new Vector2(1, 685), Color.Red);

            //Draw the menu choices
            //Music volume
            spriteBatch.DrawString(bigText, "Music volume", new Vector2(120, 100), musicColor);

            spriteBatch.Draw(volumeSlider, new Rectangle(400, 116, volumeSlider.Width, 13), new Rectangle(0, 13, volumeSlider.Width, 13), Color.LightGray);
            spriteBatch.Draw(volumeSlider, new Rectangle(400, 116, (int)(volumeSlider.Width * ((double)musicVolume / 1.0f)), 13), new Rectangle(0, 13, volumeSlider.Width, 13), Color.Red);
            spriteBatch.Draw(volumeSlider, new Rectangle(400, 116, volumeSlider.Width, 15), new Rectangle(0, 0, volumeSlider.Width, 15), Color.White);

            //Effects volume
            spriteBatch.DrawString(bigText, "Effects volume", new Vector2(120, 160), effectsColor);

            spriteBatch.Draw(volumeSlider, new Rectangle(400, 176, volumeSlider.Width, 13), new Rectangle(0, 13, volumeSlider.Width, 13), Color.LightGray);
            spriteBatch.Draw(volumeSlider, new Rectangle(400, 176, (int)(volumeSlider.Width * ((double)effectsVolume / 1.0f)), 13), new Rectangle(0, 13, volumeSlider.Width, 13), Color.Red);
            spriteBatch.Draw(volumeSlider, new Rectangle(400, 176, volumeSlider.Width, 15), new Rectangle(0, 0, volumeSlider.Width, 15), Color.White);

            //Vsync on/off
            spriteBatch.DrawString(bigText, "VSync", new Vector2(120, 220), vsyncColor);

            spriteBatch.Draw(checkBox, new Rectangle(400, 236, checkBox.Width, 13), new Rectangle(0, 13, checkBox.Width, 13), Color.LightGray);
            if (graphics.SynchronizeWithVerticalRetrace)
            {
                spriteBatch.Draw(checkBox, new Rectangle(400, 236, checkBox.Width, 13), new Rectangle(0, 13, checkBox.Width, 13), Color.Red);
            }
            spriteBatch.Draw(checkBox, new Rectangle(400, 236, checkBox.Width, 15), new Rectangle(0, 0, checkBox.Width, 15), Color.White);
            
            //DrawFPS on/off
            spriteBatch.DrawString(bigText, "Draw FPS", new Vector2(120, 280), drawFpsColor);

            spriteBatch.Draw(checkBox, new Rectangle(400, 296, checkBox.Width, 13), new Rectangle(0, 13, checkBox.Width, 13), Color.LightGray);
            if (Game1.drawFps)
            {
                spriteBatch.Draw(checkBox, new Rectangle(400, 296, checkBox.Width, 13), new Rectangle(0, 13, checkBox.Width, 13), Color.Red);
            }
            spriteBatch.Draw(checkBox, new Rectangle(400, 296, checkBox.Width, 15), new Rectangle(0, 0, checkBox.Width, 15), Color.White);

            //Draw the bar that splits the options from instructions
            spriteBatch.Draw(rowBar, new Rectangle(100, 400, 550, 5), Color.Red);

            //Draw information about how the game is played.
            spriteBatch.DrawString(bigText, "Use the left and right arrow keys ", new Vector2(101, 430), Color.Red);
            spriteBatch.DrawString(bigText, "on your keyboard to move your ship.", new Vector2(101, 460), Color.Red);
            spriteBatch.DrawString(bigText, "The X key is used to shoot.", new Vector2(101, 500), Color.Red);
            spriteBatch.DrawString(bigText, "If you press the LEFT SHIFT key,", new Vector2(101, 540), Color.Red);
            spriteBatch.DrawString(bigText, "you will move at half speed.", new Vector2(101, 570), Color.Red);
             
            base.Draw(spriteBatch);
        }

        private bool CheckKeystroke(Keys key)
        {
            return (keyboard.IsKeyDown(key) && prevKeyboard.IsKeyUp(key));
        }

        private void ResetMenuChoiceColors()
        {
            musicColor = Color.Red;
            effectsColor = Color.Red;
            vsyncColor = Color.Red;
            drawFpsColor = Color.Red;
        }

        private void MenuChoiceColors()
        {
            switch (optionsChoice)
            {
                case OptionsChoice.music:
                    musicColor = Color.White;
                    break;
                case OptionsChoice.effect:
                    effectsColor = Color.White;
                    break;
                case OptionsChoice.vsync:
                    vsyncColor = Color.White;
                    break;
                case OptionsChoice.fps:
                    drawFpsColor = Color.White;
                    break;
                default:
                    break;
            }
        }
    }
}
