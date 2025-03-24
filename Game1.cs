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
        private Texture2D _treeTexture; // Add tree texture
        private Point _playerPosition; // Using Point to represent tile coordinates
        private double _timeSinceLastMove = 0;
        private int[,] _map; // 2D array to represent the map
        private int _screenHeightInTiles;
        private int _screenWidthInTiles;
        private Random _random = new Random(); // Random number generator

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

            // Initialize the map with 400x400 tiles (0 for empty, 1 for grass, 2 for tree)
            _map = new int[Constants.MapSize, Constants.MapSize];
            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    _map[y, x] = 1; // Fill the map with grass tiles
                }
            }

            // Calculate the number of trees based on the density parameter
            int numberOfTrees = (int)(Constants.MapSize * Constants.MapSize * Constants.TreeDensity);
            for (int i = 0; i < numberOfTrees; i++)
            {
                int x = _random.Next(0, Constants.MapSize);
                int y = _random.Next(0, Constants.MapSize);
                _map[y, x] = 2; // Place a tree at the random position
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _playerTexture = Content.Load<Texture2D>("player");
            _grassTexture = Content.Load<Texture2D>("grass"); // Load the grass texture
            _treeTexture = Content.Load<Texture2D>("tree"); // Load the tree texture
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _timeSinceLastMove += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastMove >= Constants.MovementCooldown)
            {
                var keyboardState = Keyboard.GetState();
                Point newPlayerPosition = _playerPosition;

                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                {
                    newPlayerPosition.Y = Math.Max(0, _playerPosition.Y - Constants.MovementSpeed);
                }
                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                {
                    newPlayerPosition.Y = Math.Min(Constants.MapSize - 1, _playerPosition.Y + Constants.MovementSpeed);
                }
                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                {
                    newPlayerPosition.X = Math.Max(0, _playerPosition.X - Constants.MovementSpeed);
                }
                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    newPlayerPosition.X = Math.Min(Constants.MapSize - 1, _playerPosition.X + Constants.MovementSpeed);
                }

                // Check for collision before updating the player position
                if (!CheckCollision(newPlayerPosition, _map))
                {
                    _playerPosition = newPlayerPosition;
                }

                _timeSinceLastMove = 0;
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
                    else if (_map[y, x] == 2)
                    {
                        _spriteBatch.Draw(_treeTexture, new Rectangle(x * scaledTileSize + offsetX, y * scaledTileSize + offsetY - scaledTileSize, scaledTileSize, scaledTileSize * 2), Color.White);
                    }
                }
            }

            // Draw the player at the center of the screen
            _spriteBatch.Draw(_playerTexture, new Rectangle((_screenWidthInTiles / 2) * scaledTileSize, (_screenHeightInTiles / 2) * scaledTileSize - scaledTileSize, scaledTileSize, scaledTileSize * 2), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool CheckCollision(Point playerPosition, int[,] map)
        {
            // Check collision only on the bottom 16x16 pixels of the sprite
            int bottomX = playerPosition.X;
            int bottomY = playerPosition.Y + 1; // Bottom 16x16 pixels start at y + 1

            if (bottomX < 0 || bottomX >= map.GetLength(1) || bottomY < 0 || bottomY >= map.GetLength(0))
            {
                return false;
            }

            // Check for collision with trees (bottom 16x16 pixels)
            if (map[bottomY, bottomX] == 2)
            {
                return true;
            }

            return map[bottomY, bottomX] != 0;
        }
    }
}