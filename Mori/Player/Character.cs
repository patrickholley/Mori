using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private byte runSpeed = 25;
        private byte jumpHeight = 25;
        private byte frameNum = 0;
        private double frameSpeed = 0.06D;
        private double frameTimer = 0.06D;
        private TexturePackerAtlas.TexturePackerAtlas NGTPFile;
        private Controls controls = new Controls();
        private string action = "Idle";
        private KeyboardState oldKeyboardState;
        enum Direction { Right, Left };
        private Direction direction = Direction.Right;
        private int characterWidth = 130;
        private int characterHeight = 225;
        private ActStatus actStatus = new ActStatus(false, false, 0, 0);
        private Dictionary<string, ActStatus> actStatuses = new Dictionary<string, ActStatus>() {
            { "Attack", new ActStatus(true, false, 0, 23) },
            { "Climb", new ActStatus(false, false, 15, 0) },
            { "Dead", new ActStatus(true, false, 0, 0) },
            { "Glide", new ActStatus(true, false, 0, -10) },
            { "Idle", new ActStatus(false, false, 0, 0) },
            { "Jump", new ActStatus(false, true, 0, 0) },
            { "JumpAttack", new ActStatus(true, true, 0, 20) },
            { "JumpThrow", new ActStatus(true, true, 15, -20) },
            { "Run", new ActStatus(false, false, 0, 12) },
            { "Slide", new ActStatus(true, false, 0, 12) },
            { "Throw", new ActStatus(true, false, 22, 2) },
        };

        public Character(Game1 Game, string directory, string prefix) {
            this.directory = directory;
            this.prefix = prefix;
            this.Game = Game;
            Content = new ContentManager(this.Game.Services, "Content");
        }

        public void LoadContent() {
            NGTPFile = Content.Load<TexturePackerAtlas.TexturePackerAtlas>($"{directory}/{prefix}TPFile");
        }

        public void UnloadContent() {}

        public bool IsNewKeyDown(Keys key) {
            return Keyboard.GetState().IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
        }

        private void UpdateAction(string action) {
            this.action = action;
            actStatus = actStatuses[action];
            frameNum = 0;
            frameTimer = frameSpeed;
        }

        private void UpdateActionAfterNotJumpingNorActing(KeyboardState keyboardState) {
            if (IsNewKeyDown(controls.AttackKey))
                UpdateAction("Attack");
            else if (IsNewKeyDown(controls.ThrowKey))
                UpdateAction("Throw");
            else if (IsNewKeyDown(controls.SlideKey))
                UpdateAction("Slide");
            else if (keyboardState.IsKeyDown(controls.RightKey)
                || keyboardState.IsKeyDown(controls.LeftKey)) {
                if (action != "Run")
                    UpdateAction("Run");
            } else if (action != "Climb" && action != "Idle")
                UpdateAction("Idle");
        }

        private void UpdateActionAfterNotActing(KeyboardState keyboardState) {
            if (IsNewKeyDown(controls.AttackKey))
                UpdateAction("JumpAttack");
            else if (IsNewKeyDown(controls.ThrowKey))
                UpdateAction("JumpThrow");
            else if (IsNewKeyDown(controls.JumpKey))
                UpdateAction("Glide");
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

            if (action == "Climb") {
                if (keyboardState.IsKeyDown(controls.SlideKey))
                    UpdateAction("Idle");
            } else if (keyboardState.IsKeyDown(controls.ClimbKey))
                UpdateAction("Climb");
            else if (!actStatus.IsJumping) {
                if (IsNewKeyDown(controls.JumpKey))
                    UpdateAction("Jump");
                else if (!actStatus.IsActing) {
                    UpdateActionAfterNotJumpingNorActing(keyboardState);
                }
            } else if (!actStatus.IsActing || actStatus.IsJumping) {
                UpdateActionAfterNotActing(keyboardState);
            }

            frameTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (frameTimer < 0) {
                if (action != "Climb" || keyboardState.IsKeyDown(controls.ClimbKey)) {
                    frameNum++;
                }
                if (frameNum > 9) {
                    frameNum = 0;
                    if (actStatus.IsActing || actStatus.IsJumping)
                        UpdateAction("Idle");
                }
                if (action == "JumpAttack") {
                    Console.WriteLine('!');
                };
                if (!actStatus.IsActing || actStatus.IsJumping) {
                    if (keyboardState.IsKeyDown(controls.RightKey)) position.X += runSpeed;
                    else if (keyboardState.IsKeyDown(controls.LeftKey)) position.X -= runSpeed;
                }
                if (actStatus.IsJumping && frameNum < 5) {
                    position.Y -= jumpHeight;
                } else if (position.Y < 500) position.Y += jumpHeight;

                frameTimer = frameSpeed;
            }

            oldKeyboardState = keyboardState;
        }

        public void AnimateSprite(SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
            TexturePackerRegion region = NGTPFile.getRegion($"{action}_{frameNum}");
            TexturePackerRectangle TPSource = region.SourceRectangle;
            TexturePackerRectangle TPFrame = region.Frame;

            float rotation = 0f;
            int sourceWidth = TPFrame.Width;
            int sourceHeight = TPFrame.Height;
            float destinationX = position.X + TPSource.X - actStatus.XOffset;
            float destinationY = position.Y - (region.SourceSize.Height - TPSource.Height - TPSource.Y - actStatus.YOffset);
            SpriteEffects faceDirection = SpriteEffects.None;

            if (direction == Direction.Left) {
                destinationX = position.X - TPSource.X - TPSource.Width + characterWidth + actStatus.XOffset;
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
            AnimateSprite(spriteBatch, graphics);
            ShapeExtensions.DrawLine(spriteBatch, new Vector2(0, 500), new Vector2(1920, 500), Color.Black);
        }
    }
}
