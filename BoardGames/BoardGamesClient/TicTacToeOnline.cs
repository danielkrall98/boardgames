using System;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BoardGamesClient.src.Framework.GameEngine;
using BoardGamesClient.src.Games.TicTacToe;

namespace BoardGamesClient
{
    public class TicTacToeOnline : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _cellTexture;
        private SpriteFont _font;

        private TicTacToeGameManager _gameManager;
        private string _currentGameId;
        private Player _localPlayer, _joiningPlayer;
        private string _gameStateText;
        private Vector2 _gameStateTextPosition;

        private int _cellSize = 200;
        private int _boardOffsetX = 200;
        private int _boardOffsetY = 200;

        private HubConnection _connection;
        private bool _isMyTurn;

        public TicTacToeOnline()
        {
            _graphics = new GraphicsDeviceManager(this);

            // Window Size
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1000;
            //_graphics.IsFullScreen = true; // Make game fullscreen
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override async void Initialize()
        {
            try
            {
                base.Initialize();
                CenterBoard();

                _gameManager = new TicTacToeGameManager();
                _currentGameId = _gameManager.CreateGame<TicTacToeGame>();

                _gameStateText = "Connecting to the Server...";
                CenterText();

                // Setup SignalR Connection
                _connection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5000/ticTacToeHub")
                    .Build();

                _connection.On<string, string>("AssignSymbol", (symbol, connectionId) =>
                {
                    _localPlayer = new Player(symbol, $"Player {symbol}") { Id = connectionId };
                    _joiningPlayer = new Player(symbol == "X" ? "O" : "X", $"Player {(symbol == "X" ? "O" : "X")}");
                });

                _connection.On<string>("GameCreated", gameId =>
                {
                    _gameStateText = "Waiting for an Opponent...";
                    CenterText();
                });

                _connection.On<string>("GameJoined", gameId =>
                {
                    _gameStateText = "Opponent joined! Waiting for their Move.";
                    _isMyTurn = false;
                    CenterText();
                });

                _connection.On<string>("OpponentJoined", opponentId =>
                {
                    _gameStateText = "Opponent joined! Your Turn.";
                    _isMyTurn = true;
                    CenterText();
                });

                _connection.On<int, int, string>("ReceiveMove", (row, column, playerId) =>
                {
                    bool isOpponentMove = playerId != _localPlayer.Id;

                    // Apply Move
                    var move = new TicTacToeMove(isOpponentMove ? _joiningPlayer : _localPlayer, row, column);
                    _gameManager.MakeMove(_currentGameId, move);

                    // Update Game Text
                    if (_gameManager.IsGameOver(_currentGameId))
                    {
                        var winner = _gameManager.GetWinner(_currentGameId);
                        _gameStateText = winner != null ? $"{winner.Name} wins!" : "It's a Draw!";
                    }
                    else
                    {
                        _isMyTurn = !isOpponentMove;
                        _gameStateText = _isMyTurn ? "Your Turn!" : "Waiting for Opponent's Move...";
                    }

                    CenterText();
                });

                // Start SignalR Connection
                await _connection.StartAsync();
                await _connection.InvokeAsync("JoinGame"); // NOTE: war falsch - keine GameID

                Console.WriteLine("Connected to the server successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing game: {ex.Message}");
                Exit();
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // White Square Texture for Board Cells
            _cellTexture = new Texture2D(GraphicsDevice, 1, 1);
            _cellTexture.SetData(new[] { Color.White });

            // Load Font
            _font = Content.Load<SpriteFont>("Arial");
        }

        protected override async void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Allow Game Reset by pressing 'R'
            if (_gameManager.IsGameOver(_currentGameId))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    _gameManager.ResetGame(_currentGameId);
                    _gameStateText = "Game reset. Waiting for Opponent...";
                    CenterText();
                }

                return;
            }

            // Local PLayer's Turn
            if (_isMyTurn && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var mouseState = Mouse.GetState();
                int row = (mouseState.Y - _boardOffsetY) / _cellSize;
                int column = (mouseState.X - _boardOffsetX) / _cellSize;

                // Check if Move is valid
                if (row >= 0 && row < 3 && column >= 0 && column < 3)
                {
                    var move = new TicTacToeMove(_localPlayer, row, column);
                    if (_gameManager.MakeMove(_currentGameId, move))
                    {
                        // Send Move to Server
                        try
                        {
                            await _connection.InvokeAsync("SendMove", _currentGameId, row, column, _localPlayer.Id);
                            _isMyTurn = false;
                            _gameStateText = "Waiting for Opponent's Move...";
                            CenterText();
                        }
                        catch (Exception ex)
                        {
                            _gameStateText = $"Error sending move: {ex.Message}";
                            CenterText();
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            // Draw the Board
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int x = _boardOffsetX + col * _cellSize;
                    int y = _boardOffsetY + row * _cellSize;

                    // Draw Cell Background and Borders
                    _spriteBatch.Draw(_cellTexture, new Rectangle(x, y, _cellSize, _cellSize), Color.White);            // Background
                    _spriteBatch.Draw(_cellTexture, new Rectangle(x, y, _cellSize, 2), Color.Black);                    // Top Border
                    _spriteBatch.Draw(_cellTexture, new Rectangle(x, y + _cellSize - 2, _cellSize, 2), Color.Black);    // Bottom Border
                    _spriteBatch.Draw(_cellTexture, new Rectangle(x, y, 2, _cellSize), Color.Black);                    // Left Border
                    _spriteBatch.Draw(_cellTexture, new Rectangle(x + _cellSize - 2, y, 2, _cellSize), Color.Black);    // Right Border

                    // Draw Symbols (X/O)
                    var state = (TicTacToeGameState)_gameManager.GetGame(_currentGameId).State;
                    string symbol = state.Board.Cells[row, col];
                    if (!string.IsNullOrEmpty(symbol))
                    {
                        var textSize = _font.MeasureString(symbol);
                        float scale = 2.0f;  // Larger Symbols
                        _spriteBatch.DrawString(_font, symbol,
                            new Vector2(x + _cellSize / 2 - textSize.X / 2 * scale, y + _cellSize / 2 - textSize.Y / 2 * scale),
                            Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                }
            }

            // Draw Game Text
            _spriteBatch.DrawString(_font, _gameStateText, _gameStateTextPosition, Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Center Board in the Window
        private void CenterBoard()
        {
            int boardWidth = 3 * _cellSize;
            int boardHeight = 3 * _cellSize;

            _boardOffsetX = (GraphicsDevice.Viewport.Width - boardWidth) / 2;
            _boardOffsetY = (GraphicsDevice.Viewport.Height - boardHeight) / 2;
        }

        // Center Text in the Window
        private void CenterText()
        {
            var textSize = _font.MeasureString(_gameStateText);
            _gameStateTextPosition = new Vector2(
                (GraphicsDevice.Viewport.Width - textSize.X) / 2,
                _boardOffsetY - textSize.Y - 20
            );
        }
    }
}