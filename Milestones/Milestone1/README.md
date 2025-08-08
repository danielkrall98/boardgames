# Short description of the milestone #

## Work since the last Milestone ##
- Project set up
- Framework implemented (Game Engine and Network)

## Project Structure so far ##
````
BoardGames/
└── src/
    ├── BoardGamesFramework/
    │   ├── GameEngine/                     
    │   │   ├── GameState.cs            # Game state logic
    │   │   ├── Board.cs                # Abstract class for boards
    │   │   ├── Player.cs               # Player-related data
    │   │   ├── Move.cs                 # Represents moves
    │   │   ├── IGame.cs                # Interface for Games
    │   │   ├── IGameManager.cs         # Interface for Game Manager
    │   │   └── Renderer.cs             # Handles rendering
    │   └── Network/
    │       └── GameHub.cs              # SignalR
    ├── Games/                          # Implementations of our Games
    │   ├── TicTacToe/                  # TicTacToe Implementation
    │   └── Chess/                      # Chess Implementation
    ├── WebClient/                      # Web Client for Web-App  
    └── Server/                         # Server to manage Game Sessions and Players               
````

## Abstract Classes, Interfaces etc. ##
GameEngine/
```c#
// Manages the current game state
public abstract class GameState
    {
        public bool IsGameOver { get; protected set; }    // check whether game has ended
        public abstract void Reset();                     // reset game to initial state
        public abstract bool CheckWinCondition();         // check if a player has won
        public abstract void MakeMove(Move move);         // Updates game state with player's move
    }
```

```c#
// Represents size and layout of the board
public abstract class Board
    {
        public int Rows { get; protected set; }            // dimensions of the board
        public int Columns { get; protected set; }
        
        public abstract void Initialize();                 // inizialize board (starting pieces etc)
        public abstract void Display();                    // Render the board
    }
```

```c#
// Represents the game's player
public class Player
    {
        public string Id { get; }                          // basic player information
        public string Name { get; }

        public Player(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
```

```c#
// Represents a single move made by a player
public abstract class Move
    {
        public Player Player { get; }                       // Player who made the move
        
        protected Move(Player player)                       // Extended in subclasses to feature
        {                                                   // coordinates in TicTacToe or
            Player = player;                                // piece information in Chess
        }
    }
```

```c#
// Core functionality for any game implemented with the framework
public interface IGame
    {
        GameState State { get; }                            // tracks game state
        void Initialize();                                  // sets up game (board, players)
        bool IsMoveValid(Move move);                        // validate if move is legal
        void PlayMove(Move move);                           // process move and update game state
    }
```

<br></br>
Network/
```c#
// Real-time communication (SignalR)
public class GameHub : Hub
    {
        // adds a player to a game group
        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("PlayerJoined", Context.ConnectionId);
        }

        // sends a move to the server (which validates and broadcasts it to other players)
        public async Task MakeMove(string gameId, Move move)
        {
            // Process move on server, validate, and update game state
            await Clients.Group(gameId).SendAsync("ReceiveMove", move);
        }
    }
```

## Plans for Milestone 2 ##
- Validate Framework (Classes/Structure/etc)
- Use Framework's Classes and Interfaces to implement TicTacToe
- Create Pieces with MonoGame's 2DTexture Class
