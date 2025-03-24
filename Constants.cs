using System.Drawing;

namespace WorldExplorationGame
{
    public static class Constants
    {
        public const int TileSize = 16;
        public const int ScreenHeightInTiles = 10;
        public const double MovementCooldown = 0.075; // Cooldown time in seconds
        public const int MovementSpeed = 1; // Movement speed in tiles
        public const float ScaleFactor = 5.0f; // Scale factor for textures
        public const int PreferredBackBufferWidth = 1920; // Default width
        public const int PreferredBackBufferHeight = 1080; // Default height
        public static readonly Point InitialPlayerPosition = new Point(4, 5); // Initial position of the player
        public const int MapSize = 400; // Size of the map (400x400 tiles)
    }
}