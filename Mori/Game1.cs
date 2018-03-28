using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mori
{
    public enum GameState { MainMenu, InGame }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private MainMenu mainMenu;
        private Forest_1 forest_1;
        private GameState currentState;

        public Game1() {
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferHeight = 1080,
                PreferredBackBufferWidth = 1920,
            };
            mainMenu = new MainMenu(this);
            forest_1 = new Forest_1(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            currentState = GameState.MainMenu;
        }

        public GameState CurrentState
        {
            get => currentState;
            set {
                currentState = value;

                switch (currentState) {
                    case GameState.MainMenu:
                        mainMenu.LoadContent();
                        break;
                    case GameState.InGame:
                        forest_1.LoadContent();
                        break;
                    default:
                        break;
                }
            }
        }
        
        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainMenu.LoadContent();
        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            switch (currentState) {
                case GameState.MainMenu:
                    mainMenu.Update(gameTime);
                    break;
                case GameState.InGame:
                    forest_1.Update(gameTime);
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (currentState) {
                case GameState.MainMenu:
                    mainMenu.Draw(spriteBatch, graphics);
                    break;
                case GameState.InGame:
                    forest_1.Draw(spriteBatch, graphics);
                    break;
                default:
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
