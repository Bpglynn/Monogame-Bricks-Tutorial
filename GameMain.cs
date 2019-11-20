#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BloomPostprocess;
#endregion

namespace BricksGameTutorial
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameContent gameContent;
        BloomComponent bloom;

        public static ParticleManager<ParticleState> ParticleManager { get; private set; }

        public static int screenWidth = 0;
        public static int screenHeight = 0;

        private MouseState oldMouseState;
        private KeyboardState oldKeyboardState;

        private Paddle paddle;
        private Wall wall;
        private GameBorder gameBorder;
        private Ball ball;
        private bool readyToServeBall = true;
        private int ballsRemaining = 3;

        private bool useBloom = true;
        #endregion

        #region Initialize/Content load
        public GameMain()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            // Set game to 502x700 or screen max if smaller
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; // Get current screen width
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; // Get current screen height
            if (screenWidth >= 502)
            {
                screenWidth = 502;
            }
            if (screenHeight >= 700)
            {
                screenHeight = 700;
            }
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            bloom = new BloomComponent(this);
            bloom.Settings = new BloomSettings(null, 0.025f, 0.20f, 3f, 1f, 1f, 0.5f, screenWidth, screenHeight);
            //bloom.Settings = new BloomSettings(null, 0.5f, 0.75f, 0.5f, 0.5f, 0.5f, 4f, screenWidth, screenHeight);

            Components.Add(bloom);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameContent = new GameContent(Content);

            // Create game objects
            int paddleX = (screenWidth - gameContent.imgPaddle.Width) / 2; // Center the paddle on the screen to start
            int paddleY = screenHeight - 100;  // Set paddle to be 100 pixels from the bottom of the screen
            paddle = new Paddle(paddleX, paddleY, screenWidth, spriteBatch, gameContent);  // Create the game paddle
            wall = new Wall(1, 50, spriteBatch, gameContent); // Create walls of bricks
            gameBorder = new GameBorder(screenWidth, screenHeight, spriteBatch, gameContent); // Game play field borders
            ball = new Ball(screenWidth, screenHeight, spriteBatch, gameContent); // Game ball
        }
        #endregion

        #region Unload
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region Game Update/Draw
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (IsActive == false)
            {
                return;  // Our window is not active, skip update
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState newKeyboardState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();

            // Process mouse movement                               
            if (oldMouseState.X != newMouseState.X)
            {
                if (newMouseState.X >= 0 || newMouseState.X < screenWidth)
                {
                    paddle.MoveTo(newMouseState.X);
                }
            }

            // Process mouse left-click
            if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed && oldMouseState.X == newMouseState.X && oldMouseState.Y == newMouseState.Y && readyToServeBall)
            {
                ServeBall();
            }

            // Process keyboard events                           
            if (newKeyboardState.IsKeyDown(Keys.Left))
            {
                paddle.MoveLeft();
            }
            if (newKeyboardState.IsKeyDown(Keys.Right))
            {
                paddle.MoveRight();
            }

            // Process keyboard space bar toggle
            if (oldKeyboardState.IsKeyUp(Keys.Space) && newKeyboardState.IsKeyDown(Keys.Space) && readyToServeBall)
            {
                ServeBall();
            }

            oldMouseState = newMouseState;     
            oldKeyboardState = newKeyboardState;

            // Update particles
            ParticleManager.Update();

            // Toggle bloom
            if (newKeyboardState.IsKeyDown(Keys.B))
            {
                useBloom = !useBloom;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            Viewport viewport = device.Viewport;
            bloom.BeginDraw();

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(); // Everything below will be drawn to buffer
            paddle.Draw();
            wall.Draw();
            gameBorder.Draw();
            if (ball.Visible)
            {
                bool inPlay = ball.Move(wall, paddle);
                if (inPlay)
                {
                    ball.Draw();
                    ParticleManager.Draw(spriteBatch);
                }
                else
                {
                    ballsRemaining--;
                    readyToServeBall = true;
                }
            }
            spriteBatch.End(); // Everything will be drawn to screen from buffer
            base.Draw(gameTime);
        }

        private void ServeBall() // Start new round
        {
            if (ballsRemaining < 1)
            {
                ballsRemaining = 3;
                ball.Score = 0;
                wall = new Wall(1, 50, spriteBatch, gameContent);
            }
            readyToServeBall = false;
            float ballX = paddle.X + (paddle.Width) / 2;
            float ballY = paddle.Y - ball.Height;
            ball.Launch(ballX, ballY, -3, -3);
        }
        #endregion
    }
}
