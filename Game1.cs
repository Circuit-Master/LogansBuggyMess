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
        private int _screenHeightInTiles;
        private int _screenWidthInTiles;

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

            int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);
            _screenHeightInTiles = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Height / scaledTileSize);
            _screenWidthInTiles = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Width / scaledTileSize);

            // Set the initial player position to the center of the screen
            _playerPosition = new Point(Constants.MapSize / 2, Constants.MapSize / 2);

            // Initialize the map with 400x400 tiles (0 for empty, 1 for grass)
            _map = new int[Constants.MapSize, Constants.MapSize];
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

                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                {
                    _playerPosition.Y = Math.Max(0, _playerPosition.Y - Constants.MovementSpeed);
                    _timeSinceLastMove = 0;
                }
                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                {
                    _playerPosition.Y = Math.Min(Constants.MapSize - 1, _playerPosition.Y + Constants.MovementSpeed);
                    _timeSinceLastMove = 0;
                }
                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                {
                    _playerPosition.X = Math.Max(0, _playerPosition.X - Constants.MovementSpeed);
                    _timeSinceLastMove = 0;
                }
                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    _playerPosition.X = Math.Min(Constants.MapSize - 1, _playerPosition.X + Constants.MovementSpeed);
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

            // Calculate the offset to center the player on the screen
            int offsetX = (_screenWidthInTiles / 2) * scaledTileSize - _playerPosition.X * scaledTileSize;
            int offsetY = (_screenHeightInTiles / 2) * scaledTileSize - _playerPosition.Y * scaledTileSize;

            // Draw the map
            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    if (_map[y, x] == 1)
                    {
                        _spriteBatch.Draw(_grassTexture, new Rectangle(x * scaledTileSize + offsetX, y * scaledTileSize + offsetY, scaledTileSize, scaledTileSize), Color.White);
                    }
                }
            }

            // Draw the player at the center of the screen
            _spriteBatch.Draw(_playerTexture, new Rectangle((_screenWidthInTiles / 2) * scaledTileSize, (_screenHeightInTiles / 2) * scaledTileSize, scaledTileSize, scaledTileSize), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}