using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using TexturePackerAtlas;

namespace Mori
{
    class Character : IGraphic
    {
        private Game1 Game;
        private ContentManager Content;
        private string directory;
        private string prefix;
        private Vector2 position = new Vector2(100, 540);
        private Dictionary<string, TextureRegion2D[]> animations = new Dictionary<string, TextureRegion2D[]>();
        private string[] animationStrs;
        private byte frame = 0;
        private double speed = 0.06D;
        private double timer = 0.06D;
        private TexturePackerAtlas.TexturePackerAtlas NGTPFile;
        private Controls controls = new Controls();
        private string action = "Idle__";
        private KeyboardState oldKeyboardState;
        private bool canAttack = true;

        public Character(Game1 Game, string directory, string prefix, string[] animationStrs) {
            this.directory = directory;
            this.prefix = prefix;
            this.animationStrs = animationStrs;
            this.Game = Game;
            Content = new ContentManager(this.Game.Services, "Content");
        }

        public void LoadContent() {
            NGTPFile = Content.Load<TexturePackerAtlas.TexturePackerAtlas>("NinjaGirl/NGTPFile");
        }

        public void UnloadContent() {

        }

        public void Update(GameTime gameTime) {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(controls.AttackKey) && canAttack) {
                action = "Attack__";
                frame = 0;
                timer = speed;
                canAttack = false;
            }

            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (timer < 0) {
                frame++;
                if (frame > 9) {
                    action = "Idle__";
                    frame = 0;
                    canAttack = true;
                }
                timer = speed;
            }
        }

        public void ShowcaseSprite(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, string prefix) {
            TexturePackerRegion region = NGTPFile.getRegion($"{prefix}00{frame}");
            TexturePackerRectangle TPSource = region.SourceRectangle;
            TexturePackerRectangle TPFrame = region.Frame;
            Vector2 destinationRectangle;

            Rectangle sourceRectangle = new Rectangle(
                TPFrame.X,
                TPFrame.Y,
                TPFrame.Width,
                TPFrame.Height
            );

            Vector2 currentPosition = new Vector2(position.X, position.Y);

            float rotation;
            if (region.IsRotated) {
                rotation = (float)Math.PI * 1.5f;
                destinationRectangle = new Vector2(
                    currentPosition.X + TPSource.X,
                    currentPosition.Y + TPSource.Y + TPFrame.Height
                );
                sourceRectangle.Width = TPFrame.Height;
                sourceRectangle.Height = TPFrame.Width;
            } else {
                destinationRectangle = new Vector2(
                    currentPosition.X + TPSource.X,
                    currentPosition.Y + TPSource.Y
                );
                rotation = 0f;
            }

            spriteBatch.Draw(
                NGTPFile.Texture,
                destinationRectangle,
                sourceRectangle,
                Color.White,
                rotation,
                new Vector2(0, 0),
                1f,
                SpriteEffects.None,
                0f
            );
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
            ShowcaseSprite(spriteBatch, graphics, action);
        }
    }
}
