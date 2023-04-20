using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

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
        Sprite testEnemy;
        Texture2D eTexture;
        int frameCount = 0;
        #endregion
        
        Texture2D cursorTexture, projTexture, testBox;
        Rectangle cursorRect;
        MouseState mouseState, prevMouseState;
        KeyboardState keyState;
        List<Projectile> projList = new List<Projectile>();
        List<Sprite> enemyList = new List<Sprite>();
        SpriteFont font, font2, titleFont;

        enum GameState
        {
            Menu,
            Game,
            Lose,
            Win
        }

        GameState currentState = GameState.Menu;

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
            player = new Sprite(200, 200, 63, 83, 31, 51, 3);
            player.makeHitbox(-22, -20, 43, 50);
            
            // Enemy 1 
            testEnemy = new Sprite(500, 400, 130, 151, 130, 170, 20);
            testEnemy.makeHitbox(-50, -50, 110, 111);
            enemyList.Add(testEnemy);

            // Enemy 2
            testEnemy = new Sprite(1000, 820, 130, 151, 130, 170, 20);
            testEnemy.makeHitbox(-50, -50, 110, 111);
            enemyList.Add(testEnemy);

            // Enemy 3
            testEnemy = new Sprite(260, 100, 130, 151, 130, 170, 20);
            testEnemy.makeHitbox(-50, -50, 110, 111);
            enemyList.Add(testEnemy);

            cursorRect = new Rectangle(0, 0, 25, 25);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pTexture = Content.Load<Texture2D>("player");
            cursorTexture = Content.Load<Texture2D>("cursor");
            projTexture = Content.Load<Texture2D>("projectile");
            eTexture = Content.Load<Texture2D>("enemy");
            testBox = Content.Load<Texture2D>("box");

            font = Content.Load<SpriteFont>("font");
            font2 = Content.Load<SpriteFont>("font2");
            titleFont = Content.Load<SpriteFont>("titleFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevMouseState = mouseState;

            keyState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            #region Cursor Stuff
            cursorRect.X = (int)mousePosition.X - 12;
            cursorRect.Y = (int)mousePosition.Y - 12;
            #endregion

            switch (currentState)
            {
                case GameState.Menu:
                    if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Enter))
                    {
                        currentState = GameState.Game;
                    }

                    break;
                case GameState.Game:
                    frameCount += 1;

                    #region Player Movement & Rotation
                    Vector2 distance = mousePosition - new Vector2(player.Rect.X, player.Rect.Y);
                    player.Rotation = (float)Math.Atan2(distance.Y, distance.X) + (float)Math.PI / 2;

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

                    #region Enemy Stuff
                    for (int i = enemyList.Count - 1; i >= 0; i--)
                    {
                        Vector2 eDistance = new Vector2(enemyList[i].Rect.X, enemyList[i].Rect.Y) - new Vector2(player.Rect.X, player.Rect.Y);
                        enemyList[i].Rotation = (float)Math.Atan2(eDistance.Y, eDistance.X) + (3 * (float)Math.PI / 2);
                    }

                    for (int i = enemyList.Count - 1; i >= 0; i--)
                    {
                        if (enemyList[i].Health == 0)
                        {
                            enemyList.RemoveAt(i);
                        }
                    }
                    #endregion

                    #region Projectile Stuff
                    // Player Projectiles
                    if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                    {
                        projList.Add(new Projectile(player.Rect, mouseState, gameTime));
                    }

                    // Enemy Projectiles
                    for (int i = enemyList.Count - 1; i >= 0; i--)
                    {
                        if (frameCount % 120 == 0)
                        {
                            projList.Add(new Projectile(enemyList[i].Rect, player.Rect, gameTime));
                        }
                    }

                    // Update Projectiles
                    for (int i = projList.Count - 1; i >= 0; i--)
                    {
                        projList[i].Move(gameTime);
                        bool removed = false;
                        while (true)
                        {
                            if (gameTime.TotalGameTime.TotalMilliseconds - projList[i].StartTime > 1300)
                            {
                                projList.RemoveAt(i);
                                break;
                            }
                            for (int j = enemyList.Count - 1; j >= 0; j--)
                            {
                                if (projList[i].Hitbox.Intersects(enemyList[j].Hitbox) && !projList[i].IsFromEnemy)
                                {
                                    enemyList[j].Health--;
                                    projList.RemoveAt(i);
                                    removed = true;
                                    break;
                                }
                            }
                            if (removed)
                            {
                                break;
                            }
                            if (projList[i].Hitbox.Intersects(player.Hitbox) && projList[i].IsFromEnemy)
                            {
                                player.Health--;
                                projList.RemoveAt(i);
                                break;
                            }
                            break;
                        }
                    }
                    #endregion

                    if (player.Health == 0)
                    {
                        currentState = GameState.Lose;
                    }
                    if (enemyList[0].Health == 0 && enemyList[1].Health == 0 && enemyList[2].Health == 0)
                    {
                        currentState = GameState.Win;
                    }

                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            switch(currentState)
            {
                case GameState.Menu:
                    GraphicsDevice.Clear(Color.Cornsilk);

                    _spriteBatch.Begin();

                    _spriteBatch.DrawString(titleFont, "DEEPIO", new Vector2((GraphicsDevice.Viewport.Width / 2) - (titleFont.MeasureString("DEEPIO").X /2), 100), Color.Black);
                    _spriteBatch.DrawString(font2, "Press the space bar or enter to start!", new Vector2((GraphicsDevice.Viewport.Width / 2) - (font2.MeasureString("Press the space bar or enter to start!").X / 2), 600), Color.Black);
                    _spriteBatch.Draw(cursorTexture, cursorRect, Color.White);

                    _spriteBatch.End();

                    break;
                case GameState.Game:
                    GraphicsDevice.Clear(Color.DarkGray);

                    _spriteBatch.Begin();

                    for (int i = projList.Count - 1; i >= 0; i--)
                    {
                        _spriteBatch.Draw(projTexture, projList[i].Position, Color.White);
                    }

                    for (int i = enemyList.Count - 1; i >= 0; i--)
                    {
                        enemyList[i].Draw(_spriteBatch, eTexture, Color.White, SpriteEffects.None);
                    }

                    player.Draw(_spriteBatch, pTexture, Color.White, SpriteEffects.None);
                    _spriteBatch.Draw(cursorTexture, cursorRect, Color.White);

                    _spriteBatch.Draw(testBox, player.Hitbox, Color.Red);

                    _spriteBatch.DrawString(font, "Health: " + player.Health.ToString(), new Vector2(10, 10), Color.White);

                    _spriteBatch.End();

                    break;
                case GameState.Lose:
                    GraphicsDevice.Clear(Color.Black);

                    _spriteBatch.Begin();

                    //_spriteBatch.Draw(titleFont, "YOU LOSE", new Vector2((GraphicsDevice.Viewport.Width / 2)))

                    _spriteBatch.End();

                    break;
                case GameState.Win:

                    break;
            }

            base.Draw(gameTime);
        }
    }

    public class Sprite
    {
        public Rectangle Rect { get; set; }
        public Rectangle Hitbox { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public int Health { get; set; }

        public Sprite(int rectX, int rectY, int rectW, int rectH, int vectX, int vectY, int newHealth) 
        {
            this.Rect = new Rectangle(rectX, rectY, rectW, rectH);
            this.Origin = new Vector2(vectX, vectY);
            this.Health = newHealth;
        }

        public void makeHitbox(int xChange, int yChange, int width, int height)
        {
            this.Hitbox = new Rectangle(this.Rect.X + xChange, this.Rect.Y + yChange, width, height);
        }

        public void Draw(SpriteBatch sb, Texture2D text, Color c, SpriteEffects se)
        {
            sb.Draw(text, this.Rect, null, c, this.Rotation, this.Origin, se, 0.0f);
        }

        public void MoveX(int moveLength)
        {
            int newInt = this.Rect.X + moveLength;
            int newHitInt = this.Hitbox.X + moveLength;
            this.Rect = new Rectangle(newInt, this.Rect.Y, this.Rect.Width, this.Rect.Height);
            this.Hitbox = new Rectangle(newHitInt, this.Hitbox.Y, this.Hitbox.Width, this.Hitbox.Height);
        }

        public void MoveY(int moveLength)
        {
            int newInt = this.Rect.Y + moveLength;
            int newHitInt = this.Hitbox.Y + moveLength;
            this.Rect = new Rectangle(this.Rect.X, newInt, this.Rect.Width, this.Rect.Height);
            this.Hitbox = new Rectangle(this.Hitbox.X, newHitInt, this.Hitbox.Width, this.Hitbox.Height);
        }
    }

    public class Projectile
    {
        public Rectangle Hitbox { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public Vector2 Origin { get; set; }
        public float Speed { get; set; }
        public float StartTime { get; set; }
        public bool IsFromEnemy { get; set; }

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

            this.Hitbox = new Rectangle(this.Position.ToPoint(), new Point(13, 13));

            this.StartTime = (int)gt.TotalGameTime.TotalMilliseconds;
            this.Speed = 0.58f;
            this.Origin = new Vector2(originRect.X - 10, originRect.Y - 5);
            this.IsFromEnemy = false;
        }

        public Projectile(Rectangle originRect, Rectangle targetRect, GameTime gt)
        {
            /*
             * nv has to exist, due to the fact that .Normalize() does not
             * edit the Vector2 it is called on, so instead direction 
             * must be copied, then normalized, and finally reset.
             */
            this.Direction = new Vector2(targetRect.X, targetRect.Y) - new Vector2(originRect.X, originRect.Y);
            Vector2 nv = this.Direction;
            nv.Normalize();
            this.Direction = nv;

            this.StartTime = (int)gt.TotalGameTime.TotalMilliseconds;
            this.Speed = 0.58f;
            this.Origin = new Vector2(originRect.X - 10, originRect.Y - 5);
            this.IsFromEnemy = true;
        }

        public void Move(GameTime gt)
        {
            this.Position = this.Origin + (this.Speed * ((int)gt.TotalGameTime.TotalMilliseconds - this.StartTime) * this.Direction);
            this.Hitbox = new Rectangle(this.Position.ToPoint(), new Point(13, 13));
        }
    }
}
