using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BoardGamesClient.src.Framework.GameEngine;
using BoardGamesClient.src.Games.TicTacToe;

namespace BoardGamesClient
{
    public class TicTacToe : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _cellTexture;
        private SpriteFont _font;

        private TicTacToeGameManager _gameManager;
        private string _currentGameId;
        private Player _playerX, _playerO;
        private string _gameStateText;
        private Vector2 _gameStateTextPosition;

        private int _cellSize = 200;
        private int _boardOffsetX = 200;
        private int _boardOffsetY = 200;

        public TicTacToe()
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

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            base.Initialize();
            CenterBoard();

            _gameManager = new TicTacToeGameManager();
            _currentGameId = _gameManager.CreateGame<TicTacToeGame>();

            _playerX = new Player("X", "Player X");
            _playerO = new Player("O", "Player O");
            _gameManager.AddPlayer(_currentGameId, _playerX);
            _gameManager.AddPlayer(_currentGameId, _playerO);

            // Game State Text based on current Player
            var gameState = (TicTacToeGameState)_gameManager.GetGame(_currentGameId).State;
            _gameStateText = $"Game Started! It's {gameState.CurrentPlayer.Name}'s turn.";
            CenterText();
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

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Check if Game is over
            if (_gameManager.IsGameOver(_currentGameId))
            {
                // Get Winner
                var winner = _gameManager.GetWinner(_currentGameId);
                if (winner != null)
                {
                    // Display Winner
                    _gameStateText = $"{winner.Name} wins! Press 'R' to restart!";
                    CenterText();
                }
                else
                {
                    // Display Draw Message
                    _gameStateText = "It's a draw! Press 'R' to restart!";
                    CenterText();
                }

                // Allow Game Reset by pressing 'R'
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    _gameManager.ResetGame(_currentGameId);

                    // Get starting Player
                    var gameState = (TicTacToeGameState)_gameManager.GetGame(_currentGameId).State;
                    _gameStateText = $"Game reset. It's {gameState.CurrentPlayer.Name}'s turn.";
                    CenterText();
                }
            }
            else
            {
                // Process Mouse Input for Moves
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    var mouseState = Mouse.GetState();
                    int row = (mouseState.Y - _boardOffsetY) / _cellSize;
                    int column = (mouseState.X - _boardOffsetX) / _cellSize;

                    // Check if Click is within Bounds
                    if (row >= 0 && row < 3 && column >= 0 && column < 3)
                    {
                        var currentPlayer = _gameManager.GetGame(_currentGameId).State.CurrentPlayer ?? _playerX;

                        var move = new TicTacToeMove(currentPlayer, row, column);
                        if (_gameManager.MakeMove(_currentGameId, move))
                        {
                            // Check for Game Over
                            if (_gameManager.IsGameOver(_currentGameId))
                            {
                                var winner = _gameManager.GetWinner(_currentGameId);
                                _gameStateText = winner != null ? $"{winner.Name} wins!" : "It's a draw!";
                                CenterText();
                            }
                            else
                            {
                                // Switch Turn Message
                                var nextPlayer = currentPlayer.Id == _playerX.Id ? _playerO : _playerX;
                                _gameStateText = $"It's {nextPlayer.Name}'s turn.";
                                CenterText();
                            }
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

        // Center Board and Text in Window (independent of Window Size)
        private void OnResize(object sender, System.EventArgs e)
        {
            CenterBoard();
            CenterText();
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
                (GraphicsDevice.Viewport.Width - textSize.X) / 2,  // Horizontally
                _boardOffsetY - textSize.Y - 20                    // Above the Board
            );
        }
    }
}
