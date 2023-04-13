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
        Sprite player;
        #endregion
        
        #region Enemy Variables
        #endregion
        
        Texture2D cursorTexture, projTexture;
        SpriteFont font;
        Rectangle cursorRect;
        MouseState mouseState, prevMouseState;
        KeyboardState keyState;
        List<Projectile> projList = new List<Projectile>();

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
            player = new Sprite(200, 200, 63, 83, 31, 51);
            //enemy = new Sprite(0, 0, 150, 150, 0, 0);
            //eRect = new Rectangle(0, 0, 150, 150);

            cursorRect = new Rectangle(0, 0, 25, 25);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pTexture = Content.Load<Texture2D>("player");
            cursorTexture = Content.Load<Texture2D>("cursor");
            projTexture = Content.Load<Texture2D>("projectile");

            font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevMouseState = mouseState;

            keyState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            #region Player Movement & Rotation
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            Vector2 distance = mousePosition - new Vector2(player.Rect.X, player.Rect.Y);
            player.Rotation = (float)Math.Atan2(distance.Y, distance.X) + (float)Math.PI/2;

            if (keyState.IsKeyDown(Keys.W))
            {
                player.MoveY(-2);
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                player.MoveY(2);
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                player.MoveX(-2);
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                player.MoveX(2);
            }
            #endregion

            #region Cursor Stuff
            cursorRect.X = (int)mousePosition.X - 12;
            cursorRect.Y = (int)mousePosition.Y - 12;
            #endregion

            #region Projectile Stuff
            // Player Projectiles
            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                projList.Add(new Projectile(player.Rect, mouseState, gameTime));
            }

            // Update Projectiles
            for (int i = projList.Count - 1; i >= 0; i--)
            {
                projList[i].Move(gameTime);
                if (gameTime.TotalGameTime.TotalMilliseconds - projList[i].StartTime > 1300)
                {
                    projList.RemoveAt(i);
                }
            }
            #endregion


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            for (int i = projList.Count - 1; i >= 0; i--)
            {
                _spriteBatch.Draw(projTexture, projList[i].Position, Color.White);
            }

            player.Draw(_spriteBatch, pTexture, Color.White, SpriteEffects.None);
            _spriteBatch.Draw(cursorTexture, cursorRect, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class Sprite
    {
        public Rectangle Rect { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }

        public Sprite(int rectX, int rectY, int rectW, int rectH, int vectX, int vectY) 
        {
            this.Rect = new Rectangle(rectX, rectY, rectW, rectH);
            this.Origin = new Vector2(vectX, vectY);
        }

        public void Draw(SpriteBatch sb, Texture2D text, Color c, SpriteEffects se)
        {
            sb.Draw(text, this.Rect, null, c, this.Rotation, this.Origin, se, 0.0f);
        }

        public void MoveX(int moveLength)
        {
            int newInt = this.Rect.X + moveLength;
            this.Rect = new Rectangle(newInt, this.Rect.Y, this.Rect.Width, this.Rect.Height);
        }

        public void MoveY(int moveLength)
        {
            int newInt = this.Rect.Y + moveLength;
            this.Rect = new Rectangle(this.Rect.X, newInt, this.Rect.Width, this.Rect.Height);
        }
    }

    public class Projectile
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public Vector2 Origin { get; set; }
        public float Speed { get; set; }
        public float StartTime { get; set; }

        public Projectile(Rectangle originRect, MouseState ms, GameTime gt)
        {
            /*
             * nv has to exist, due to the fact that .Normalize() does not
             * edit the Vector2 it is called on, so instead direction 
             * must be copied, then normalized, and finally reset.
             */
            this.Direction = new Vector2(ms.X, ms.Y) - new Vector2(originRect.X, originRect.Y);
            Vector2 nv = this.Direction;
            nv.Normalize();
            this.Direction = nv;

            this.StartTime = (int)gt.TotalGameTime.TotalMilliseconds;
            this.Speed = 0.58f;
            this.Origin = new Vector2(originRect.X - 10, originRect.Y - 5);
        }

        public void Move(GameTime gt)
        {
            this.Position = this.Origin + (this.Speed * ((int)gt.TotalGameTime.TotalMilliseconds - this.StartTime) * this.Direction);
        }
    }
}
