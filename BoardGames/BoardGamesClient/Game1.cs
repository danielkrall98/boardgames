using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoardGamesClient
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private string[] _menuOptions = { "Play Chess", "Play TicTacToe", "Play Chess Online", "Play TicTacToe Online", "Exit" };
        private int _selectedIndex = 0;
        private KeyboardState _previousKeyboardState;

        private bool _isLaunchingGame = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Arial");
        }

        protected override void Update(GameTime gameTime)
        {
            if (_isLaunchingGame) return;

            var keyboardState = Keyboard.GetState();
            var previousKeyboardState = _previousKeyboardState; // Store the previous state for comparison

            // Navigate menu: Register key press only when transitioning from Released to Pressed
            if (keyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
            {
                _selectedIndex = (_selectedIndex - 1 + _menuOptions.Length) % _menuOptions.Length;
            }

            if (keyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
            {
                _selectedIndex = (_selectedIndex + 1) % _menuOptions.Length;
            }

            // Confirm selection: Register Enter key press only when transitioning from Released to Pressed
            if (keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                switch (_selectedIndex)
                {
                    case 0:
                        _isLaunchingGame = true;
                        LaunchGame(new Chess());
                        break;

                    case 1:
                        _isLaunchingGame = true;
                        LaunchGame(new TicTacToe());
                        break;

                    case 2:
                        _isLaunchingGame = true;
                        LaunchGame(new ChessOnline());
                        break;

                    case 3:
                        _isLaunchingGame = true;
                        LaunchGame(new TicTacToeOnline());
                        break;

                    case 4:
                        Exit();
                        break;
                }
            }

            _previousKeyboardState = keyboardState; // Update the previous state at the end of the frame
            base.Update(gameTime);
        }

        private void LaunchGame(Game gameInstance)
        {
            gameInstance.Run();
            Exit(); // Close the menu once a game is launched
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            // Draw the menu options
            for (int i = 0; i < _menuOptions.Length; i++)
            {
                var color = (i == _selectedIndex) ? Color.Black : Color.White;
                var position = new Vector2(350, 200 + i * 50);
                _spriteBatch.DrawString(_font, _menuOptions[i], position, color);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
