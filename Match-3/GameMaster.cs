using System.Collections.Generic;
using SFML.Graphics;
using System.IO;
using SFML.System;
using System.Collections;


namespace Match_3
{

    public class GameMaster : Drawable
    {

        // Private parameters.
        private List<Bonus> bonusList;
        private Tile[,] _gameMap;
        private Tile[,] _LastMovedMap;
        private Tile _selectedTile;
        private Painter _painter;
        private Sprite _frame;
        private double _score;
        private Analyzer _analyzer;

        // Constructor.
        public GameMaster(Painter painter)
        {
            bonusList = new List<Bonus>();
            _painter = painter;
            _painter.SetMaster(this);
            _LastMovedMap = new Tile[Game.MapSize, Game.MapSize];
            _gameMap = new Tile[Game.MapSize, Game.MapSize];
            _analyzer = new Analyzer(_gameMap);
            _frame = new Sprite(Game.Frame);
            _frame.Scale = new Vector2f((float)Game.TileWidth / _frame.Texture.Size.X, (float)Game.TileHeight / _frame.Texture.Size.Y);
            _score = 0;

        }

        // Method initializing the game after pressing the play button.
        public void GameStart()
        {
            _selectedTile = null;
            bonusList.Clear();
            _analyzer.Restart();
            GenerateMap();
            while (UpdateMap(false)) { }
            _score = 0;
        }

        // Game map generating method
        private void GenerateMap()
        {
            for (int i = 0; i < Game.MapSize; ++i)
            {
                for (int j = 0; j < Game.MapSize; ++j)
                {
                    var position = new Vector2f(i * Game.TileWidth, j * Game.TileHeight);
                    _gameMap[i, j] = Tile.CreateRandomTile(Game.Icon, position, i, j);
                }
            }
        }

        // The method generates new elements on the top line, return true, if create any Tile.
        private bool GenerateTopLine(bool isDraw = true)
        {
            bool reply = false;
            for (int j = 0; j < Game.MapSize; ++j)
            {
                if (_gameMap[j, 0] == null)
                {
                    var positionStart = new Vector2f(j * Game.TileWidth, -1 * Game.TileHeight);
                    var positionEnd = new Vector2f(j * Game.TileWidth, 0 * Game.TileHeight);
                    if (isDraw)
                    {
                        _gameMap[j, 0] = Tile.CreateRandomTile(Game.Icon, positionStart, j, 0);
                        _painter.AddAnimation(new MoveAnimation(_gameMap[j, 0], positionEnd));
                    }
                    else
                    {
                        _gameMap[j, 0] = Tile.CreateRandomTile(Game.Icon, positionEnd, j, 0);
                    }
                    reply = true;
                }
             }
            return reply;
        }

        // Defining an interface Drawable method.
        public virtual void Draw(RenderTarget target, RenderStates state)
        {
            for (int i = 0; i < Game.MapSize; ++i)
            {
                for (int j = 0; j < Game.MapSize; ++j)
                {
                    if (_gameMap[i, j] != null)
                    {
                        target.Draw(_gameMap[i, j]);
                    }
                }
            }
            if (_selectedTile != null)
            {
                _frame.Position = _selectedTile.Position;
                target.Draw(_selectedTile);
                target.Draw(_frame);
            }
            if(bonusList.Count > 0)
            {
                foreach(Bonus bonus in bonusList)
                {
                    target.Draw(bonus);
                }
            }
        }

        // Helper get and set methods.
        // Method that returns the object at position () as selected.
        public Tile GetFromPos(float x, float y)
        {
            Tile reply = null;
            var i = (int)(x / Game.TileWidth);
            var j = (int)(y / Game.TileHeight);
            if (i < Game.MapSize && i >= 0 && j < Game.MapSize && j >= 0)
            {
                reply = _gameMap[i, j];
            }
            return reply;
        }

        // Sets the selected object to the position (i,j) related in the window.
        public void Swap(Tile left, Tile right)
        {

            if (left != null && right != null && left != right)
            {
                var i = right.Line;
                var j = right.Column;
                var position = new Vector2f(right.Position.X, right.Position.Y);
                _gameMap[left.Line, left.Column] = right;
                _gameMap[left.Line, left.Column].Position = left.Position;
                _gameMap[left.Line, left.Column].Line = left.Line;
                _gameMap[left.Line, left.Column].Column = left.Column;
                _gameMap[i, j] = left;
                _gameMap[i, j].Position = position;
                _gameMap[i, j].Line = i;
                _gameMap[i, j].Column = j;
            }
        }

        // Sets the selected object to the position (i,j) related in the window and finds matches, return true, if find any match.
        public bool SwapAndFind (Tile left, Tile right)
        {
            if (left != null && right != null)
            {
                Swap(left, right);

                if (_analyzer.Find())
                {
                    var position = new Vector2f(right.Position.X, right.Position.Y);
                    _LastMovedMap[right.Line, right.Column] = right;
                    _LastMovedMap[left.Line, left.Column] = left;
                    right.Position = left.Position;
                    left.Position = position;
                    _painter.AddAnimation(new SwapAnimation(left, right));
                    return true;
                }
                else
                {
                    Swap(left, right);
                    _painter.AddAnimation(new SwapAnimation(left, right, true));

                }
            }
            return false;
        }

        // The main method in which the state of the playing field is updated
        public bool UpdateMap(bool isDraw = true)
        {
            bool reply = true;
            if (!BonusRun(isDraw))
            {
                if (!Falling(isDraw))
                {
                    if (!FindAndDestroy(isDraw))
                    {
                        GenerateTopLine(isDraw);
                        reply = false;
                    }
                }
                else
                {
                    GenerateTopLine(isDraw);
                }
            }
            return reply;
        }

        // Method describing falling tiles on the map, return true, if any Tile fall down.
        private bool Falling(bool isDraw = true)
        {
            bool reply = false;
            for (int i = 0; i < Game.MapSize; ++i)
            {
                for (int j = (int)Game.MapSize - 2; j >= 0; --j)
                {
                    if (_gameMap[i, j] != null && _gameMap[i, j + 1] == null)
                    {
                        reply = true;
                        _LastMovedMap[i, j + 1] = (Tile)_gameMap[i, j].Clone();
                        if (isDraw)
                        {
                            _painter.AddAnimation(new MoveAnimation(_gameMap[i, j]));
                        }
                        else
                        {
                            _gameMap[i, j].Position = new Vector2f(_gameMap[i, j].Position.X, _gameMap[i, j].Position.Y + Game.TileHeight);
                        }
                        _gameMap[i, j].Column = j + 1;
                        _gameMap[i, j + 1] = _gameMap[i, j];
                        _gameMap[i, j] = null;
                    }
                }
            }
            return reply;
        }

        // The method finds matches and removes them, return true, if remove any match.
        public bool FindAndDestroy(bool isDraw = true)
        {
            bool reply = false;
            for (int i = 0; i < Game.MapSize; ++i)
            {
                for (int j = 0; j < Game.MapSize; ++j)
                {
                    if (_gameMap[i,j] != null)
                    {
                        _analyzer.FindMatchesAtTile(_gameMap[i, j]);
                        _analyzer.FindBonus(_LastMovedMap);
                        if (DeleteTile(_analyzer.HorizontalMatches, isDraw))
                        {
                            reply = true;
                        }

                        if(DeleteTile(_analyzer.VerticalMatches, isDraw))
                        {
                            reply = true;
                        }
                    }
                }
            }
            _LastMovedMap = new Tile[Game.MapSize, Game.MapSize];
            CreateBonusTiles();
            return reply;
        }


        // Method describing the behavior of activated bonuses.
        private bool BonusRun(bool isDraw = true)
        {
            bool reply = true;
            if (bonusList.Count > 0)
            {
                var bonusListCopy = new List<Bonus>(bonusList);
                var bonusListForDelete = new List<Bonus>();
                foreach (Bonus bonus in bonusListCopy)
                {
                    if (!bonus.Run(isDraw))
                    {
                        bonusListForDelete.Add(bonus);
                    }
                }
                foreach (Bonus bonus in bonusListForDelete)
                {
                    bonusList.Remove(bonus);
                }

            }
            else
            {
                reply = false;
            }


            return reply;
        }

        // Method creates files containing bonuses.
        private void CreateBonusTiles()
        {
            var lineHorizontalBonusList = _analyzer.LineHorizontalBonusList;
            var lineVerticalBonusList = _analyzer.LineVerticalBonusList;
            var bombBonusList = _analyzer.BombBonusList;
            if (lineHorizontalBonusList.Count > 0 || lineVerticalBonusList.Count > 0 || bombBonusList.Count > 0)
            {
                foreach(Tile tile in lineVerticalBonusList)
                {
                    _gameMap[tile.Line, tile.Column] = new BonusTile(Game.LineV, tile, new LineVerticalBonus(tile, this));
                    _gameMap[tile.Line, tile.Column].Position = tile.Position;
                }
                foreach (Tile tile in lineHorizontalBonusList)
                {
                    _gameMap[tile.Line, tile.Column] = new BonusTile(Game.Line, tile, new LineHorizontalBonus(tile, this));
                    _gameMap[tile.Line, tile.Column].Position = tile.Position;
                }
                foreach (Tile tile in bombBonusList)
                {
                    _gameMap[tile.Line, tile.Column] = new BonusTile(Game.Bomb, tile, new BombBonus(tile, this));
                    _gameMap[tile.Line, tile.Column].Position = tile.Position;
                }
            }
            lineHorizontalBonusList.Clear();
            lineVerticalBonusList.Clear();
            bombBonusList.Clear();
        }


        // Method removes the list of tiles from the map, calls animation for each
        private bool DeleteTile(List<List<Tile>> listTile, bool isDraw = true)
        {
            bool reply = false;
            foreach (List<Tile> list in listTile)
            {
                if (list.Count >= 3)
                {
                    _score += list.Count * 10;
                    foreach (Tile tile in list)
                    {
                        if (tile != null)
                        {
                            var bonus = tile as BonusTile;
                            if(bonus != null)
                            {
                                bonusList.Add(bonus.Bonus);
                            }
                            if (isDraw)
                            {
                                _painter.AddAnimation(new DestroyAnimation(_gameMap[tile.Line, tile.Column]));
                            }
                            _gameMap[tile.Line, tile.Column] = null;
                            reply = true;
                        }
                    }
                }
            }
            return reply;
        }

        // Method removes a tile from the map, does not trigger an animation
        public bool DeleteTile(int Line, int Column)
        {
            bool reply = false;
            if (_gameMap[Line,Column] != null)
            {
                _score += 10;
                var bonusTile = _gameMap[Line, Column] as BonusTile;
                if (bonusTile != null)
                {
                    bonusList.Add(bonusTile.Bonus);
                }
                _gameMap[Line, Column] = null;
                reply = true;
            }
            return reply;
        }


        // Getters and Setters
        public Tile[,] GameMap
        {
            get { return _gameMap; }
        }

        public Tile SelectedTile
        {
            get { return _selectedTile; }
            set { _selectedTile = value; }
        }

        public double Score
        {
            get { return _score; }
        }

        public Painter Painter
        {
            get { return _painter; }

        }

    }
}

