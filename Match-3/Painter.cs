using System;
using System.Collections.Generic;
using System.Threading;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.IO;

namespace Match_3
{
    enum GameState
    {
        MainWindow,
        GameRun,
        EndGame
    }
    public class Painter
    {
        // Private parameters.
        private RenderWindow _window;
        private RenderTexture _rTexture;
        private Sprite _renderSprite;
        private Sprite _infoSprite;
        private Sprite _buttonSprite;
        private Sprite _fontSprite;
        private Sprite _tableSprite;
        private FloatRect _rectButton;

        private GameMaster _master;
        private List<Animation> _animationsList;
        private bool AnimationStart;
        private GameState _state;
        private Clock _clock;
        private Font _font;
        private bool GameRun;
        private Vector2f _scoredPosition;

        // Constructor.
        public Painter()
        {
            // Auxiliary objects.
            string _pathToFrame = Path.Combine(Game.CurrentFolder, @"Files/Background.png");
            string _pathToButton = Path.Combine(Game.CurrentFolder, @"Files/Button.png");
            string _pathToInfo = Path.Combine(Game.CurrentFolder, @"Files/InfoPanel.png");
            string _pathToInFont = Path.Combine(Game.CurrentFolder, @"Files/Fonts.otf");
            string _pathToInTable = Path.Combine(Game.CurrentFolder, @"Files/Table.png");
            _state = GameState.MainWindow;
            _clock = new Clock();
            _animationsList = new List<Animation>();
            AnimationStart = false;
            _font = new Font(_pathToInFont);
            GameRun = false;

            // Graphic objects.
            _window = new RenderWindow(new VideoMode(Game.WindowWidth, Game.WindowHeight), Game.GameName);
            _rTexture = new RenderTexture(Game.WindowWidth, Game.WindowHeight);
             var _backgroundTexture = new Texture(_pathToFrame);
             var _buttonTexture = new Texture(_pathToButton);
             var _infoTexture = new Texture(_pathToInfo);
             var _tableTexture = new Texture(_pathToInTable);
            _infoSprite = new Sprite(_infoTexture);
            _buttonSprite = new Sprite(_buttonTexture);
            _renderSprite = new Sprite(_rTexture.Texture);
            _fontSprite = new Sprite(_backgroundTexture);
            _tableSprite = new Sprite(_tableTexture);

            var xSize = (float)(Game.WindowWidth-Game.GameFieldWidth) / _infoTexture.Size.X;
            var ySize = (float)Game.GameFieldHeight / _infoTexture.Size.Y;
            _infoSprite.Position = new Vector2f((float)Game.GameFieldWidth,0);
            _infoSprite.Scale = new Vector2f(xSize, ySize);

            xSize = (float)Game.GameFieldWidth / _backgroundTexture.Size.X;
            ySize = (float)Game.GameFieldHeight / _backgroundTexture.Size.Y;
            _fontSprite.Scale = new Vector2f(xSize, ySize);

            var wRatio = 0.3f;
            var hRatio = 0.28f;
            xSize = (float)Game.GameFieldWidth / _buttonTexture.Size.X * wRatio;
            ySize = (float)Game.GameFieldHeight / _buttonTexture.Size.Y * hRatio;
            _buttonSprite.Scale = new Vector2f(xSize, ySize);

            xSize = (float)Game.GameFieldWidth / 2 - _buttonTexture.Size.X * _buttonSprite.Scale.X / 2f;
            ySize = (float)Game.GameFieldHeight / 2 - _buttonTexture.Size.Y * _buttonSprite.Scale.Y / 2f;
            _buttonSprite.Position = new Vector2f(xSize, ySize);
            _rectButton = new FloatRect(_buttonSprite.Position.X, _buttonSprite.Position.Y, _buttonTexture.Size.X * _buttonSprite.Scale.X, _buttonTexture.Size.Y * _buttonSprite.Scale.Y);

            wRatio = 0.4f;
            hRatio = 0.4f;
            xSize = (float)Game.GameFieldWidth / _tableTexture.Size.X * wRatio;
            ySize = (float)Game.GameFieldHeight / _tableTexture.Size.Y * hRatio;
            _tableSprite.Scale = new Vector2f(xSize, ySize);

            xSize = (float)Game.GameFieldWidth / 2 - _tableTexture.Size.X * _tableSprite.Scale.X / 2f;
            ySize = (float)Game.GameFieldHeight / 2 - _tableTexture.Size.Y * _tableSprite.Scale.Y / 2f;
            _tableSprite.Position = new Vector2f(xSize, ySize);

            var x = _tableSprite.Position.X + _tableSprite.Texture.Size.X * _tableSprite.Scale.X / 8f;
            var y = _tableSprite.Position.Y + _tableSprite.Texture.Size.Y * _tableSprite.Scale.Y / 4f;
            _scoredPosition = new Vector2f(x, y);

            // Events.
            _window.Closed += _window_Closed;
            _window.MouseButtonPressed += _window_MouseButtonPressedMain;
        }

        // Method setting GameMaster.
        public bool SetMaster(GameMaster master)
        {
            if (_master == null)
            {
                _master = master;
                return true;
            }
            return false;
        }

        // Main window processing function.
        public void Run()
        {
            while (_window.IsOpen)
            {
                _window.Clear();
                _rTexture.Clear();
                _rTexture.Draw(_infoSprite);
                _rTexture.Draw(_fontSprite);

                string scoreString = null;
                int time = 0;
                if (GameRun)
                {
                    time = (int)(Game.GameDuration - _clock.ElapsedTime.AsSeconds());
                    scoreString = "Score: " + _master.Score.ToString();
                }
                else
                {
                    scoreString = "Score: 0";
                }
                var scoreText = new Text(scoreString, _font, 36);
                scoreText.Position = _scoredPosition;

                switch (_state)
                {
                    case GameState.MainWindow:
                        _rTexture.Draw(_buttonSprite);
                        break;

                    case GameState.EndGame:
                        _rTexture.Draw(_tableSprite);
                        _rTexture.Draw(scoreText);
                        break;

                    case GameState.GameRun:

                        string timeText = "Time left: " + time.ToString();
                        var text = new Text(timeText, _font, 36);
                        text.Position = new Vector2f(Game.GameFieldWidth + 25, 15);
                        scoreText.Position = new Vector2f(Game.GameFieldWidth + 25, 15+50);

                        _rTexture.Draw(scoreText);
                        _rTexture.Draw(text);
                        if(_clock.ElapsedTime.AsSeconds() < Game.GameDuration || !GameRun)
                        {
                            _rTexture.Draw(_master);
                        }
                        else
                        {
                            GameEnd();
                        }
                        Animation();
                        break;
                }

                _window.DispatchEvents();
                _rTexture.Display();
                _window.Draw(_renderSprite);
                _window.Display();
                Thread.Sleep(5);
            }
        }

        // Method describes the logic of interaction with animation.
        private void Animation()
        {
            if (_animationsList.Count == 0)
            {
                if (AnimationStart)
                {
                    AnimationStart = false;
                    _window.MouseButtonPressed += _window_MouseButtonPressedGame;
                }
                if (_master.SelectedTile == null)
                {
                    _master.UpdateMap();
                }
            }
            else
            {
                if (!AnimationStart)
                {
                    AnimationStart = true;
                    _window.MouseButtonPressed -= _window_MouseButtonPressedGame;
                }
                var animationListCopy = new List<Animation>(_animationsList);
                foreach (Animation animation in _animationsList)
                {
                    if (animation.Finished())
                    {
                        animationListCopy.Remove(animation);
                    }
                    else
                    {
                        _rTexture.Draw(animation);
                    }
                }
                _animationsList = animationListCopy;
            }
        }

        // Method adding animation to animations list
        public void AddAnimation(Animation animation)
        {
            _animationsList.Add(animation);
        }

        // SFML Events.
        // Clicking the close button.
        private void _window_Closed(object sender, EventArgs e)
        {
            _window.Close();
        }

        // Pressing the mouse button in Game.
        private void _window_MouseButtonPressedGame(object sender, MouseButtonEventArgs e)
        {
            if (_master.SelectedTile != null)
            {
                var x = Mouse.GetPosition(_window).X;
                var y  = Mouse.GetPosition(_window).Y;
                var secondSeletectedTile = _master.GetFromPos(x, y);
                if (secondSeletectedTile != null)
                {
                    var lineDif = secondSeletectedTile.Line - _master.SelectedTile.Line;
                    var columnDif = secondSeletectedTile.Column - _master.SelectedTile.Column;
                    if ((Math.Abs(lineDif) == 1 && Math.Abs(columnDif) == 0) || (Math.Abs(lineDif) == 0 && Math.Abs(columnDif) == 1))
                    {
                        if (!GameRun)
                        {
                            GameRun = true;
                            _clock.Restart();
                            _master.ResetScore();
                        }
                        _master.SwapAndFind(_master.SelectedTile, secondSeletectedTile);
                        _master.SelectedTile = null;
                    }
                    else
                    {
                        _master.SelectedTile = null;
                    }
                }
                else
                {
                    _master.SelectedTile = null;
                }
            }
            else
            {
                _master.SelectedTile = _master.GetFromPos(Mouse.GetPosition(_window).X, Mouse.GetPosition(_window).Y);
            }
        }

        // Pressing the mouse button in start window.
        private void _window_MouseButtonPressedMain(object sender, MouseButtonEventArgs e)
        {
            if(_rectButton.Contains(e.X, e.Y))
            {
                _state = GameState.GameRun;
                _window.MouseButtonPressed -= _window_MouseButtonPressedMain;
                _window.MouseButtonPressed += _window_MouseButtonPressedGame;
                GameRun = false;
                _animationsList.Clear();
                _master.GameStart();
                _clock.Restart();
            }
        }

        // Pressing the mouse button in end window.
        private void _window_MouseButtonPressedEnd(object sender, MouseButtonEventArgs e)
        {
            if (_rectButton.Contains(e.X, e.Y))
            {
                _state = GameState.MainWindow;
                _window.MouseButtonPressed -= _window_MouseButtonPressedEnd;
                _window.MouseButtonPressed += _window_MouseButtonPressedMain;    
            }
        }

        // Method containing commands to end the game.
        private void GameEnd()
        {
            _window.MouseButtonPressed -= _window_MouseButtonPressedGame;
            _window.MouseButtonPressed += _window_MouseButtonPressedEnd;
            _animationsList.Clear();
            _state = GameState.EndGame;

        }
    }
}
