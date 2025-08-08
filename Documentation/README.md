**Documentation**

---

`Software and Libraries`
- [**MonoGame**](https://monogame.net/): .NET Framework for Game Development in C#
- [**SignalR**](https://learn.microsoft.com/de-de/aspnet/core/tutorials/signalr?view=aspnetcore-8.0&WT.mc_id=dotnet-35129-website&tabs=visual-studio): Real-Time Communication for Client/Server Setup

---

`Structure`
````
BoardGames/
│
├── BoardGamesClient/
│   ├── Content/                            # Content Pipeline
│   ├── src/
│   │   ├── Framework/
│   │   │   └── GameEngine/         
│   │   │       ├── Board.cs                # Abstract Board Class
│   │   │       ├── GameState.cs            # Abstract Game State Class
│   │   │       ├── IGame.cs                # Interface for Games
│   │   │       ├── IGameManager.cs         # Interface for Game Managers
│   │   │       ├── Move.cs                 # Abstract Move Classes
│   │   │       └── Player.cs               # Base Class for Player Data
│   │   └── Games/
│   │       ├── Chess/                      # Chess Game Classes
│   │       │   ├── ChessBoard.cs
│   │       │   ├── ChessGame.cs
│   │       │   ├── ChessGameManager.cs
│   │       │   ├── ChessGameState.cs
│   │       │   └── ChessMove.cs
│   │       └── TicTacToe/                  # Tic Tac Toe Game Classes
│   │           ├── TicTacToeBoard.cs
│   │           ├── TicTacToeGame.cs
│   │           ├── TicTacToeGameManager.cs
│   │           ├── TicTacToeGameState.cs
│   │           └── TicTacToeMove.cs
│   ├── Chess.cs                            # Chess Singleplayer
│   ├── ChessOnline.cs                      # Chess Multiplayer
│   ├── TicTacToe.cs                        # Tic Tac Toe Singleplayer
│   ├── TicTacToeOnline.cs                  # Tic Tac Toe Multiplayer
│   ├── Game1.cs                            # Switch between Games
│   ├── Program.cs                          
│   └── BoardGamesFramework.csproj
│
├── ChessServer/                            # Chess Server (SignalR)
│   ├── Program.cs
│   └── ChessServer.csproj
│
├── TicTacToeServer/                        # Tic Tac Toe Server (SignalR)
│   ├── Program.cs
│   └── TicTacToeServer.csproj
│
└── BoardGamesMultiplayer.sln
````
---

- Main Page (Current Page)
- [Entry Point](./Pages/ENTRY.md)
- [Chess (Singleplayer)](./Pages/CHESS.md)
- [Tic Tac Toe (Singleplayer)](./Pages/TTT.md)
- [Chess (Multiplayer)](./Pages/CHESSMP.md)
- [Tic Tac Toe (Multiplayer)](./Pages/TTTMP.md)

---

- [Repository](../)

---