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
        private Texture2D _treeTexture;
        private Point _playerPosition;
        private double _timeSinceLastMove = 0;
        private int[,] _map;
        private int _screenHeightInTiles;
        private int _screenWidthInTiles;
        private Random _random = new Random();

        private Rectangle _treeTopSourceRect;
        private Rectangle _treeBottomSourceRect;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            SetFullScreenMode();
            CalculateScreenDimensions();
            InitializeMap();
            PlaceTreesOnMap();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _playerTexture = Content.Load<Texture2D>("player");
            _grassTexture = Content.Load<Texture2D>("grass");
            _treeTexture = Content.Load<Texture2D>("tree");

            // Define source rectangles for the top and bottom halves of the tree texture
            _treeTopSourceRect = new Rectangle(0, 0, 16, 16);
            _treeBottomSourceRect = new Rectangle(0, 16, 16, 16);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _timeSinceLastMove += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastMove >= Constants.MovementCooldown)
            {
                HandlePlayerMovement();
                _timeSinceLastMove = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            // Draw grass layer
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            DrawGrass();
            _spriteBatch.End();

            // Draw bottom half of trees layer
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            DrawTreeBottoms();
            _spriteBatch.End();

            // Draw player layer
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            DrawPlayer();
            _spriteBatch.End();

            // Draw top half of trees layer
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            DrawTreeTops();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Initialization Methods

        private void SetFullScreenMode()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        private void CalculateScreenDimensions()
        {
            int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);
            _screenHeightInTiles = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Height / scaledTileSize);
            _screenWidthInTiles = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Width / scaledTileSize);
        }

        private void InitializeMap()
        {
            _playerPosition = new Point(Constants.MapSize / 2, Constants.MapSize / 2);
            _map = new int[Constants.MapSize, Constants.MapSize];

            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    _map[y, x] = 1; // Fill the map with grass tiles
                }
            }
        }

        private void PlaceTreesOnMap()
        {
            int numberOfTrees = (int)(Constants.MapSize * Constants.MapSize * Constants.TreeDensity);
            for (int i = 0; i < numberOfTrees; i++)
            {
                int x = _random.Next(0, Constants.MapSize);
                int y = _random.Next(0, Constants.MapSize);
                _map[y, x] = 2; // Place a tree at the random position
            }
        }

        #endregion

        #region Update Methods

        private void HandlePlayerMovement()
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

            if (!CheckCollision(newPlayerPosition))
            {
                _playerPosition = newPlayerPosition;
            }
        }

        private bool CheckCollision(Point playerPosition)
        {
            if (playerPosition.X < 0 || playerPosition.X >= _map.GetLength(1) || playerPosition.Y < 0 || playerPosition.Y >= _map.GetLength(0))
            {
                return true;
            }

            return _map[playerPosition.Y, playerPosition.X] != 1;
        }

        #endregion

        #region Draw Methods

        private void DrawGrass()
        {
            int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);
            int offsetX = (_screenWidthInTiles / 2) * scaledTileSize - _playerPosition.X * scaledTileSize;
            int offsetY = (_screenHeightInTiles / 2) * scaledTileSize - _playerPosition.Y * scaledTileSize;

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
        }

        private void DrawTreeBottoms()
        {
            int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);
            int offsetX = (_screenWidthInTiles / 2) * scaledTileSize - _playerPosition.X * scaledTileSize;
            int offsetY = (_screenHeightInTiles / 2) * scaledTileSize - _playerPosition.Y * scaledTileSize;

            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    if (_map[y, x] == 2)
                    {
                        // Draw the bottom half of the tree
                        _spriteBatch.Draw(_treeTexture, new Rectangle(x * scaledTileSize + offsetX, y * scaledTileSize + offsetY, scaledTileSize, scaledTileSize), _treeBottomSourceRect, Color.White);
                    }
                }
            }
        }

        private void DrawPlayer()
        {
            int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);
            _spriteBatch.Draw(_playerTexture, new Rectangle((_screenWidthInTiles / 2) * scaledTileSize, (_screenHeightInTiles / 2) * scaledTileSize - scaledTileSize, scaledTileSize, scaledTileSize * 2), Color.White);
        }

        private void DrawTreeTops()
        {
            int scaledTileSize = (int)(Constants.TileSize * Constants.ScaleFactor);
            int offsetX = (_screenWidthInTiles / 2) * scaledTileSize - _playerPosition.X * scaledTileSize;
            int offsetY = (_screenHeightInTiles / 2) * scaledTileSize - _playerPosition.Y * scaledTileSize;

            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    if (_map[y, x] == 2)
                    {
                        // Draw the top half of the tree
                        _spriteBatch.Draw(_treeTexture, new Rectangle(x * scaledTileSize + offsetX, y * scaledTileSize + offsetY - scaledTileSize, scaledTileSize, scaledTileSize), _treeTopSourceRect, Color.White);
                    }
                }
            }
        }

        #endregion
    }
}