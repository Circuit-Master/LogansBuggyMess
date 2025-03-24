# Logan's Buggy Mess

Welcome to **BuggyMess**, a simple tile-based game where you control a player character on a grassy map. This game is built using MonoGame and demonstrates basic game development concepts such as tile maps, player movement, and screen scaling.

## Features

Full-screen mode with dynamic scaling, perfect for those big, beautiful screens
Tile-based map rendering that’s as smooth as a well-groomed tail
Basic player movement with keyboard controls to help you navigate the world

## Getting Started

### Prerequisites

- [MonoGame](http://www.monogame.net/downloads/)
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (pick your favorite den)

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/Circuit-Master/LogansBuggyMess
    ```
2. Open the project in your preferred IDE.

3. Restore NuGet packages:
    ```sh
    dotnet restore
    ```

4. Build and run the project:
    ```sh
    dotnet run
    ```

## Controls

- **W / Up Arrow**: Move up
- **S / Down Arrow**: Move down
- **A / Left Arrow**: Move left
- **D / Right Arrow**: Move right
- **Escape**: Exit the game

## Code Overview

### [Game1.cs](http://_vscodecontentref_/0)

This is the main game class that handles initialization, content loading, updating game logic, and drawing the game.

- **Initialize**: Sets the game to full-screen mode and initializes the tile map, making everything look paw-some.
- **LoadContent**: Loads textures for the player and grass tiles, keeping it all looking fresh.
- **Update**: Handles player input and updates the player's position, so you can keep moving forward.
- **Draw**: Renders the tile map and player character, bringing your adventure to life!

### [Constants.cs](http://_vscodecontentref_/1)

Contains game constants such as tile size, movement speed, and scale factor.

```csharp
namespace WorldExplorationGame
{
    public static class Constants
    {
        public const int TileSize = 16;
        public const int ScreenHeightInTiles = 10;
        public const double MovementCooldown = 0.2; // Cooldown time in seconds
        public const int MovementSpeed = 1; // Movement speed in tiles
        public const float ScaleFactor = 4.0f; // Scale factor for textures
    }
}
