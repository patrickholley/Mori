using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using TexturePackerAtlas;

namespace Mori
{
    class Character : IGraphic {
        private Game1 Game;
        private ContentManager Content;
        private string directory;
        private string prefix;
        private Vector2 position = new Vector2(500, 500);
        private Dictionary<string, TextureRegion2D[]> animations = new Dictionary<string, TextureRegion2D[]>();
        private string[] animationStrs;
        private byte frame = 0;
        private double speed = 0.06D;
        private double timer = 0.06D;
        private TexturePackerAtlas.TexturePackerAtlas NGTPFile;
        private Controls controls = new Controls();
        private string action = "Idle__";
        private KeyboardState oldKeyboardState;
        enum Direction { Right, Left };
        private Direction direction = Direction.Right;
        private int characterWidth = 130;
        private int characterHeight = 225;
        private ActStatus actStatus = new ActStatus(false, false, false, false, 0, 0);
        private Dictionary<string, ActStatus> actStatuses = new Dictionary<string, ActStatus>() {
            { "Attack__", new ActStatus(IsActing: true, IsClimbing: false, IsJumping: false, IsRunning: false, XOffset: 0, YOffset: 23) },
            { "Climb_", new ActStatus(false, true, false, false, 0, 0) },
            { "Glide_", new ActStatus(false, false, false, false, 0, 0) },
            { "Idle__", new ActStatus(false, false, false, false, 0, 0) },
            { "Jump__", new ActStatus(false, false, true, false, 0, 0) },
            { "Jump_Attack__", new ActStatus(true, false, true, false, 0, 0) },
            { "Jump_Throw__", new ActStatus(true, false, true, false, 0, 0) },
            { "Run__", new ActStatus(false, false, false, true, 0, 0) },
            { "Slide__", new ActStatus(true, false, false, false, 0, 0) },
            { "Throw__", new ActStatus(true, false, false, false, 0, 2) },
        };

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

        public void UnloadContent() {}

        public bool IsNewKeyDown(Keys key) {
            return Keyboard.GetState().IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
        }

        private void UpdateAction(string action) {
            this.action = action;
            actStatus = actStatuses[action];
            frame = 0;
            timer = speed;
        }

        public void Update(GameTime gameTime) {
            KeyboardState keyboardState = Keyboard.GetState();

            if (!actStatus.IsActing) {
                if (keyboardState.IsKeyDown(controls.RightKey)) {
                    direction = Direction.Right;
                } else if (keyboardState.IsKeyDown(controls.LeftKey)) {
                    direction = Direction.Left;
                }
            }

            if (actStatus.IsClimbing) {
                if (keyboardState.IsKeyDown(controls.SlideKey))
                    UpdateAction("Idle__");
            } else if (keyboardState.IsKeyDown(controls.ClimbKey))
                UpdateAction("Climb_");
            else if (!actStatus.IsJumping) {
                if (IsNewKeyDown(controls.JumpKey))
                    UpdateAction("Jump__");
                else if (!actStatus.IsActing) {
                    if (IsNewKeyDown(controls.AttackKey))
                        UpdateAction("Attack__");
                    else if (IsNewKeyDown(controls.ThrowKey))
                        UpdateAction("Throw__");
                    else if (IsNewKeyDown(controls.SlideKey))
                        UpdateAction("Slide__");
                    else if (keyboardState.IsKeyDown(controls.RightKey)
                        || keyboardState.IsKeyDown(controls.LeftKey)) {
                        if (!actStatus.IsRunning)
                            UpdateAction("Run__");
                    } else if (!actStatus.IsClimbing && action != "Idle__")
                        UpdateAction("Idle__");
                }
            } else if (!actStatus.IsActing) {
                if (IsNewKeyDown(controls.AttackKey))
                    UpdateAction("Jump_Attack__");
                else if (IsNewKeyDown(controls.ThrowKey))
                    UpdateAction("Jump_Throw__");
                else if (IsNewKeyDown(controls.JumpKey))
                    UpdateAction("Glide_");
            }

            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (timer < 0) {
                if (!actStatus.IsClimbing || keyboardState.IsKeyDown(controls.ClimbKey)) {
                    frame++;
                }
                if (frame > 9) {
                    frame = 0;
                    if (actStatus.IsActing || actStatus.IsJumping)
                        UpdateAction("Idle__");
                }
                timer = speed;
            }

            oldKeyboardState = keyboardState;
        }

        public void AnimateSprite(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, string prefix) {
            TexturePackerRegion region = NGTPFile.getRegion($"{prefix}00{frame}");
            TexturePackerRectangle TPSource = region.SourceRectangle;
            TexturePackerRectangle TPFrame = region.Frame;

            float rotation = 0f;
            int sourceWidth = TPFrame.Width;
            int sourceHeight = TPFrame.Height;
            float destinationX = position.X + TPSource.X;
            float destinationY = position.Y - (region.SourceSize.Height - TPSource.Height - TPSource.Y - actStatus.YOffset);
            SpriteEffects faceDirection = SpriteEffects.None;

            if (direction == Direction.Left) {
                destinationX = position.X - TPSource.X - TPSource.Width + characterWidth;
                faceDirection = SpriteEffects.FlipHorizontally;
            }

            if (region.IsRotated) {
                if (direction == Direction.Left) faceDirection = SpriteEffects.FlipVertically;
                rotation = -(float)Math.PI / 2;
                sourceWidth = TPFrame.Height;
                sourceHeight = TPFrame.Width;
                destinationX += TPFrame.Width;
            }


            Rectangle sourceRectangle = new Rectangle(
                TPFrame.X,
                TPFrame.Y,
                sourceWidth,
                sourceHeight
            );

            Vector2 destinationPosition = new Vector2(
                destinationX,
                destinationY
            );

            Vector2 origin = new Vector2(
                sourceRectangle.Width * region.PivotPoint.X,
                sourceRectangle.Height * region.PivotPoint.Y
            );

            spriteBatch.Draw(
                NGTPFile.Texture,
                destinationPosition,
                sourceRectangle,
                Color.White,
                rotation,
                origin,
                1f,
                faceDirection,
                0f
            );
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
            AnimateSprite(spriteBatch, graphics, action);
            ShapeExtensions.DrawLine(spriteBatch, new Vector2(0, position.Y), new Vector2(1920, position.Y), Color.Black);
        }
    }
}
