using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using BoardGamesClient.src.Framework.GameEngine;
using BoardGamesClient.src.Games.Chess;

namespace BoardGamesClient
{
    public class Chess : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Dictionary<string, Texture2D> _pieceTextures;
        private ChessGame _game;
        private ChessGameState _state;

        private Point? _selectedCell;
        private MouseState _previousMouseState;

        public Chess()
        {
            _graphics = new GraphicsDeviceManager(this);

            // Window size
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1000;

            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Allow Maximizing Window
            Window.AllowUserResizing = true;
            base.Initialize();

            // Initialize the game
            _game = new ChessGame();
            _game.Initialize();
            _state = (ChessGameState)_game.State;

            // Add players
            var player1 = new Player("W", "White");
            var player2 = new Player("B", "Black");
            _state.Players.Add(player1);
            _state.Players.Add(player2);
            _state.CurrentPlayer = player1;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load chess piece textures
            _pieceTextures = new Dictionary<string, Texture2D>
            {
                { "WP", Content.Load<Texture2D>("WP") },
                { "WR", Content.Load<Texture2D>("WR") },
                { "WN", Content.Load<Texture2D>("WN") },
                { "WB", Content.Load<Texture2D>("WB") },
                { "WQ", Content.Load<Texture2D>("WQ") },
                { "WK", Content.Load<Texture2D>("WK") },
                { "BP", Content.Load<Texture2D>("BP") },
                { "BR", Content.Load<Texture2D>("BR") },
                { "BN", Content.Load<Texture2D>("BN") },
                { "BB", Content.Load<Texture2D>("BB") },
                { "BQ", Content.Load<Texture2D>("BQ") },
                { "BK", Content.Load<Texture2D>("BK") }
            };

            _font = Content.Load<SpriteFont>("Arial");
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle input
            var keyboardState = Keyboard.GetState();

            // Restart the game when "R" is pressed
            if (keyboardState.IsKeyDown(Keys.R))
            {
                Restart();
            }

            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                HandleMouseClick(mouseState.Position);
            }

            _previousMouseState = mouseState;

            base.Update(gameTime);
        }

        private void HandleMouseClick(Point mousePosition)
        {
            // Calculate offsets to center the board
            int boardWidth = _state.Board.Columns * 100;
            int boardHeight = _state.Board.Rows * 100;
            int offsetX = (GraphicsDevice.Viewport.Width - boardWidth) / 2;
            int offsetY = (GraphicsDevice.Viewport.Height - boardHeight) / 2;

            // Adjust the mouse position to account for the offsets
            int adjustedX = mousePosition.X - offsetX;
            int adjustedY = mousePosition.Y - offsetY;

            // Determine the clicked cell based on the adjusted position
            int column = adjustedX / 100;
            int row = adjustedY / 100;

            // Check if the click is outside the board
            if (column < 0 || column >= _state.Board.Columns || row < 0 || row >= _state.Board.Rows)
                return;

            if (_selectedCell == null)
            {
                // First click: Select a piece
                string piece = _state.Board.BoardState[row, column];
                if (!string.IsNullOrEmpty(piece) && piece[0] == _state.CurrentPlayer.Id[0])
                {
                    _selectedCell = new Point(column, row);
                }
            }
            else
            {
                // Second click: Attempt to move the piece
                var source = _selectedCell.Value;
                var move = new ChessMove(_state.CurrentPlayer, source.Y, source.X, row, column);

                if (_game.IsMoveValid(move))
                {
                    _game.PlayMove(move);
                }

                // Deselect the cell and reset
                _selectedCell = null;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            // Calculate offsets to center the board
            int boardWidth = _state.Board.Columns * 100;
            int boardHeight = _state.Board.Rows * 100;
            int offsetX = (GraphicsDevice.Viewport.Width - boardWidth) / 2;
            int offsetY = (GraphicsDevice.Viewport.Height - boardHeight) / 2;

            // Draw the turn indicator text or "Game Over"
            if (_state.IsGameOver)
            {
                DrawGameOverText(offsetX, offsetY, boardWidth);
            }
            else
            {
                DrawTurnIndicator(offsetX, offsetY, boardWidth);
            }

            // Draw the board grid
            DrawGrid(offsetX, offsetY);

            // Draw pieces
            for (int row = 0; row < _state.Board.Rows; row++)
            {
                for (int col = 0; col < _state.Board.Columns; col++)
                {
                    string piece = _state.Board.BoardState[row, col];
                    if (!string.IsNullOrEmpty(piece) && _pieceTextures.ContainsKey(piece))
                    {
                        _spriteBatch.Draw(
                            _pieceTextures[piece],
                            new Rectangle(offsetX + col * 100, offsetY + row * 100, 100, 100), // Apply offsets
                            Color.White
                        );
                    }
                }
            }

            // Highlight the selected cell
            if (_selectedCell != null)
            {
                var highlightRect = new Rectangle(
                    offsetX + _selectedCell.Value.X * 100,
                    offsetY + _selectedCell.Value.Y * 100,
                    100,
                    100
                );
                _spriteBatch.Draw(CreateHighlightTexture(), highlightRect, Color.Yellow * 0.5f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGrid(int offsetX, int offsetY)
        {
            var texture = CreateGridTexture();

            for (int row = 0; row < _state.Board.Rows; row++)
            {
                for (int col = 0; col < _state.Board.Columns; col++)
                {
                    _spriteBatch.Draw(
                        texture,
                        new Rectangle(offsetX + col * 100, offsetY + row * 100, 100, 100),
                        (row + col) % 2 == 0 ? Color.White : Color.LightGray
                    );
                }
            }
        }

        private void DrawTurnIndicator(int offsetX, int offsetY, int boardWidth)
        {
            string currentPlayerText = $"It's {_state.CurrentPlayer.Name}'s turn";

            // Measure the width of the text to center it
            var textSize = _font.MeasureString(currentPlayerText);
            var textX = offsetX + (boardWidth - textSize.X) / 2; // Center the text horizontally above the board
            var textY = offsetY - textSize.Y - 10; // Position above the board with a small margin

            var textPosition = new Vector2(textX, textY);

            // Draw the text
            _spriteBatch.DrawString(_font, currentPlayerText, textPosition, Color.Black);
        }

        private void Restart()
        {
            // Reinitialize the game
            _game.Initialize();
            _state = (ChessGameState)_game.State;

            // Add players again
            var player1 = new Player("W", "White");
            var player2 = new Player("B", "Black");
            _state.Players.Add(player1);
            _state.Players.Add(player2);
            _state.CurrentPlayer = player1;

            // Clear selection and reset any other necessary states
            _selectedCell = null;
        }

        private void DrawGameOverText(int offsetX, int offsetY, int boardWidth)
        {
            
            string gameOverText = $"{_state.CurrentPlayer.Name} wins! Press 'R' to Restart";

            // Measure the width of the text to center it
            var textSize = _font.MeasureString(gameOverText);
            var textX = offsetX + (boardWidth - textSize.X) / 2; // Center the text
            var textY = offsetY - textSize.Y - 10; // Position above the board with a small margin

            var textPosition = new Vector2(textX, textY);

            // Draw the text
            _spriteBatch.DrawString(_font, gameOverText, textPosition, Color.Black);
        }

        private Texture2D CreateGridTexture()
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }

        private Texture2D CreateHighlightTexture()
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }

    }
}
