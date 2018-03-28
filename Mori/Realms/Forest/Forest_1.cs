using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mori
{
    class Forest_1 : IGraphic
    {
        private Game1 Game;
        private ContentManager Content;
        private Character NinjaGirl;

        public Forest_1(Game1 Game) {
            this.Game = Game;
            Content = new ContentManager(this.Game.Services, "Content");
            NinjaGirl = new Character(Game, "NinjaGirl", "NG", new string[] { "Attack" });
        }

        public void LoadContent() {
            NinjaGirl.LoadContent();
        }

        public void UnloadContent() {
            NinjaGirl.UnloadContent();
        }

        public void Update(GameTime gameTime) {
            NinjaGirl.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
            NinjaGirl.Draw(spriteBatch, graphics);
        }
    }
}
