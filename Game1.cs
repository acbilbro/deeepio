using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace deeepio
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region Player Variables
        Texture2D pTexture;
        Rectangle pRect;
        float pRotation = 0.0f;
        Vector2 pOrigin;
        #endregion
        
        #region Enemy Variables
        #endregion
        
        Texture2D cursorTexture, projTexture;
        Rectangle cursorRect;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Deeepio";
            IsMouseVisible = false;
            _graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 960;
            _graphics.ApplyChanges();

            // Game stuff
            pOrigin = new Vector2(257, 297);
            pRect = new Rectangle(200, 200, 63, 83);

            cursorRect = new Rectangle(0, 0, 25, 25);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pTexture = Content.Load<Texture2D>("player");
            cursorTexture = Content.Load<Texture2D>("cursor");
            projTexture = Content.Load<Texture2D>("projectile");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            Vector2 distance = mousePosition - new Vector2(pRect.X, pRect.Y);
            pRotation = (float)Math.Atan2(distance.Y, distance.X) + (float)Math.PI/2;

            if (keyState.IsKeyDown(Keys.W)) {
                pRect.Y -= 2;
            }
            if (keyState.IsKeyDown(Keys.S)) {
                pRect.Y += 2;
            }
            if (keyState.IsKeyDown(Keys.A)) {
                pRect.X -= 2;
            }
            if (keyState.IsKeyDown(Keys.D)) {
                pRect.X += 2;
            }

            cursorRect.X = (int)mousePosition.X - 12;
            cursorRect.Y = (int)mousePosition.Y - 12;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(pTexture, pRect, null, Color.White, pRotation, new Vector2(31, 51), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(cursorTexture, cursorRect, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class Projectile
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public Vector2 Origin { get; set; }
        public float Speed { get; set; }
        public int StartTime { get; set; }

        public Projectile(Rectangle originRect, MouseState ms, GameTime gt)
        {
            this.Direction = new Vector2(ms.X, ms.Y) - new Vector2(originRect.Width / 2, originRect.Top);
            this.Direction.Normalize();
            this.StartTime = gt.TotalGameTime.TotalMilliseconds;
        }

        public void Move(GameTime gt)
        {
            this.Position = this.Origin + this.Speed * (gt.TotalGameTime.TotalMilliseconds - this.StartTime) * this.Direction;
        }
    }
}
