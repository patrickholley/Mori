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
        private bool isActing = false;
        private bool canTurn = true;
        private bool isJumping = false;
        private bool isRunning = false;
        private bool isClimbing = false;
        enum Direction { Right, Left };
        private Direction direction = Direction.Right;
        private int characterWidth = 130;
        private int characterHeight = 225;
        private Dictionary<string, int> animationYOffsets = new Dictionary<string, int>() {
            { "Idle__", 0 },
            { "Climb_", 0 },
            { "Jump__", 0 },
            { "Attack__", 23 },
            { "Throw__", 2 },
            { "Slide__", 0 },
            { "Jump_Attack__", 0 },
            { "Jump_Throw__", 0 },
            { "Glide_", 0 }
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

        public void UnloadContent() {

        }

        public bool IsNewKeyDown(Keys key) {
            return Keyboard.GetState().IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
        }

        public void Update(GameTime gameTime) {
            KeyboardState keyboardState = Keyboard.GetState();

            if (canTurn) {
                if (keyboardState.IsKeyDown(controls.RightKey)) {
                    direction = Direction.Right;
                } else if (keyboardState.IsKeyDown(controls.LeftKey)) {
                    direction = Direction.Left;
                }
            }

            if (isClimbing) {
                if (keyboardState.IsKeyDown(controls.SlideKey)) {
                    isClimbing = false;
                    action = "Idle__";
                    frame = 0;
                    timer = speed;
                }
            } else if (keyboardState.IsKeyDown(controls.ClimbKey)) {
                isClimbing = true;
                isJumping = false;
                action = "Climb_";
                frame = 0;
                timer = speed;
            } else if (!isJumping) {
                if (IsNewKeyDown(controls.JumpKey)) {
                    action = "Jump__";
                    frame = 0;
                    timer = speed;
                    isJumping = true;
                } else if (!isActing) {
                    if (IsNewKeyDown(controls.AttackKey)) {
                        action = "Attack__";
                        frame = 0;
                        timer = speed;
                        isActing = true;
                        canTurn = false;
                    } else if (IsNewKeyDown(controls.ThrowKey)) {
                        action = "Throw__";
                        frame = 0;
                        timer = speed;
                        isActing = true;
                        canTurn = false;
                    } else if (IsNewKeyDown(controls.SlideKey)) {
                        action = "Slide__";
                        frame = 0;
                        timer = speed;
                        isActing = true;
                        canTurn = false;
                    } else if (keyboardState.IsKeyDown(controls.RightKey)
                        || keyboardState.IsKeyDown(controls.LeftKey)) {
                        if (!isRunning) {
                            action = "Run__";
                            frame = 0;
                            isRunning = true;
                            timer = speed;
                        }
                    } else if (!isClimbing && action != "Idle__") {
                        action = "Idle__";
                        frame = 0;
                        timer = speed;
                        isRunning = false;
                    }
                }
            } else {
                if (!isActing) {
                    if (IsNewKeyDown(controls.AttackKey)) {
                        action = "Jump_Attack__";
                        frame = 0;
                        timer = speed;
                        isActing = true;
                        canTurn = false;
                    } else if (IsNewKeyDown(controls.ThrowKey)) {
                        action = "Jump_Throw__";
                        frame = 0;
                        timer = speed;
                        isActing = true;
                        canTurn = false;
                    } else if (IsNewKeyDown(controls.JumpKey)) {
                        action = "Glide_";
                        frame = 0;
                        timer = speed;
                        isActing = true;
                    }
                }
            }

            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (timer < 0) {
                if (!isClimbing || keyboardState.IsKeyDown(controls.ClimbKey)) {
                    frame++;
                }
                if (frame > 9) {
                    frame = 0;
                    if (isActing || isJumping) {
                        action = "Idle__";
                        isActing = false;
                        isJumping = false;
                        canTurn = true;
                    }
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
            float destinationY = position.Y - (region.SourceSize.Height - TPSource.Height - TPSource.Y - animationYOffsets[action]);
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
