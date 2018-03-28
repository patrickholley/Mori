using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;

namespace Mori
{
    class MainMenu : IGraphic {
        private BitmapFont titleFont;
        private BitmapFont optionFont;
        private ContentManager Content;
        private Game1 Game;
        private KeyboardState oldKeyboardState;
        private Song moriSong;
        private SoundEffect throwSF;
        private SoundEffect impactSF;
        private Texture2D forestBG;
        private Texture2D barButton;
        private Texture2D kunaiImg;

        private string[] options = new string[] { "Start Game", "Exit" };
        private int selectedOption = 0;
        private double speed;
        private double timer;

        public MainMenu(Game1 Game) {
            this.Game = Game;
            Content = new ContentManager(this.Game.Services, "Content");
        }

        private void RunSelected() {
            impactSF.Play();

            switch (options[selectedOption]) {
                case "Start Game":
                    Game.CurrentState = GameState.InGame;
                    UnloadContent();
                    break;
                case "Exit":
                    Game.Exit();
                    break;
                default:
                    break;
            }
        }

        private void DrawOption(SpriteBatch spriteBatch, string option, float yPosition, int windowWidth, Vector2 baseOptionSize) {
            Rectangle buttonPlace = new Rectangle((int)(((windowWidth - baseOptionSize.X - 80) / 2) + 40),
                (int)(yPosition - 7),
                (int)baseOptionSize.X,
                (int)baseOptionSize.Y);

            Vector2 stringSize = (Vector2)optionFont.MeasureString(option);
            
            Vector2 position = new Vector2((windowWidth - stringSize.X) / 2, yPosition);

            spriteBatch.Draw(barButton,
                buttonPlace,
                Color.White);

            spriteBatch.DrawString(optionFont,
                option,
                position,
                Color.White);

            if (selectedOption == Array.IndexOf(options, option)) {
                Rectangle kunaiPlace = buttonPlace;
                kunaiPlace.Height /= 2;
                kunaiPlace.Width = kunaiPlace.Height * 5;
                kunaiPlace.X -= (kunaiPlace.Width + 10);
                kunaiPlace.Y += (kunaiPlace.Height / 2);
                spriteBatch.Draw(kunaiImg,
                    kunaiPlace,
                    Color.White);
            }
        }

        public void LoadContent() {
            titleFont = Content.Load<BitmapFont>("Fonts/SenseiHead/SenseiHead");
            optionFont = Content.Load<BitmapFont>("Fonts/SunnBody/SunnBody");
            moriSong = Content.Load<Song>("Music/Shenyang");
            throwSF = Content.Load<SoundEffect>("Sounds/KunaiThrow");
            impactSF = Content.Load<SoundEffect>("Sounds/KunaiImpact");
            forestBG = Content.Load<Texture2D>("Backgrounds/Forest");
            barButton = Content.Load<Texture2D>("Buttons/Bar");
            kunaiImg = Content.Load<Texture2D>("NinjaGirl/Kunai");

            MediaPlayer.Play(moriSong);
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.IsRepeating = true;
            speed = 0.25D;
            timer = speed;
        }

        public void UnloadContent() {
            Content.Unload();
            MediaPlayer.Stop();
        }

        public void Update(GameTime gameTime) {
            KeyboardState keyboardState = Keyboard.GetState();
            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.Enter)) {
                RunSelected();
            } else if (keyboardState.IsKeyDown(Keys.Down)) {
                if (oldKeyboardState.IsKeyUp(Keys.Down) || timer <= 0) {
                    selectedOption++;
                    timer = speed;
                    throwSF.Play(1, 1, 0);
                }
            } else if (keyboardState.IsKeyDown(Keys.Up)) {
                if (oldKeyboardState.IsKeyUp(Keys.Up) || timer <= 0) {
                    selectedOption--;
                    timer = speed;
                    throwSF.Play(1, 1, 0);
                }
            }

            if (selectedOption > options.Length - 1)
                selectedOption = 0;
            else if (selectedOption < 0)
                selectedOption = options.Length - 1;

            oldKeyboardState = keyboardState;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
            spriteBatch.Draw(forestBG, new Rectangle(0, 0, 1920, 1080), Color.White);

            int windowWidth = graphics.PreferredBackBufferWidth;
            int windowHeight = graphics.PreferredBackBufferHeight;
            Vector2 titleFontSize = titleFont.MeasureString("Mori");
            float titleYPosition = windowHeight / 10;

            spriteBatch.DrawString(titleFont,
                "Mori",
                new Vector2((windowWidth - titleFontSize.X) / 2, titleYPosition),
                new Color(255, 71, 71));

            titleYPosition += titleFontSize.Y;
            Vector2 subTitleFontSize = titleFont.MeasureString("Forest Ninja") / 2;

            spriteBatch.DrawString(titleFont,
                "Forest Ninja",
                new Vector2((windowWidth - subTitleFontSize.X) / 2, titleYPosition),
                Color.DarkSeaGreen,
                0f,
                new Vector2(0, 0),
                0.5f,
                new SpriteEffects(),
                0f);

            Vector2 baseOptionSize = new Vector2(80, 20) + (Vector2)optionFont.MeasureString("Start Game");
            float optionYPosition = (float) (windowHeight * 0.9) - (2 * baseOptionSize.Y);

            for (int i = options.Length - 1; i >= 0; i--) {
                DrawOption(spriteBatch, options[i], optionYPosition, windowWidth, baseOptionSize);
                optionYPosition -= baseOptionSize.Y + 40;
            }
        }
    }
}
