using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WorldExplorationGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _playerTexture;
        private Texture2D _grassTexture;
        private Point _playerPosition; // Using Point to represent tile coordinates
        private double _timeSinceLastMove = 0;
        private int[,] _map; // 2D array to represent the map

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false; // Hide the mouse cursor
        }

        protected override void Initialize()
        {
            // Set the game to full screen
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            _playerPosition = new Point(4, 5); // Initial position of the player in tile coordinates

            int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);
            int screenHeightInTiles = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Height / scaledTileSize);
            int screenWidthInTiles = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Width / scaledTileSize);

            // Initialize the map with some tiles (0 for empty, 1 for grass)
            _map = new int[screenHeightInTiles, screenWidthInTiles];
            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    _map[y, x] = 1; // Fill the map with grass tiles
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _playerTexture = Content.Load<Texture2D>("player");
            _grassTexture = Content.Load<Texture2D>("grass"); // Load the grass texture
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _timeSinceLastMove += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastMove >= Constants.MovementCooldown)
            {
                var keyboardState = Keyboard.GetState();

                int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);
                int screenHeightInTiles = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Height / scaledTileSize);
                int screenWidthInTiles = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Width / scaledTileSize);

                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                {
                    _playerPosition.Y = Math.Max(0, _playerPosition.Y - Constants.MovementSpeed);
                    _timeSinceLastMove = 0;
                }
                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                {
                    _playerPosition.Y = Math.Min(screenHeightInTiles - 1, _playerPosition.Y + Constants.MovementSpeed);
                    _timeSinceLastMove = 0;
                }
                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                {
                    _playerPosition.X = Math.Max(0, _playerPosition.X - Constants.MovementSpeed);
                    _timeSinceLastMove = 0;
                }
                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    _playerPosition.X = Math.Min(screenWidthInTiles - 1, _playerPosition.X + Constants.MovementSpeed);
                    _timeSinceLastMove = 0;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);

            int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);

            // Draw the map
            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    if (_map[y, x] == 1)
                    {
                        _spriteBatch.Draw(_grassTexture, new Rectangle(x * scaledTileSize, y * scaledTileSize, scaledTileSize, scaledTileSize), Color.White);
                    }
                }
            }

            // Draw the player
            _spriteBatch.Draw(_playerTexture, new Rectangle(_playerPosition.X * scaledTileSize, _playerPosition.Y * scaledTileSize, scaledTileSize, scaledTileSize), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}