using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match_3
{
    // Class encapsulating algorithms for checking matches on a map.
    public class Analyzer
    {
        // Private parameters.
        private List<List<Tile>> _horizontalMatches;
        private List<List<Tile>> _verticalMatches;
        private Tile[,] _gameMap;

        private List<Tile> _bombBonusList;
        private Tile[,] _LastMovedMap;
        private List<Tile> _lineVerticalBonusList;
        private List<Tile> _lineHorizontalBonusList;

        // Constructor.
        public Analyzer(Tile[,] gameMap)
        {
            _horizontalMatches = new List<List<Tile>>();
            _verticalMatches = new List<List<Tile>>();
            _bombBonusList = new List<Tile>();
            _lineVerticalBonusList = new List<Tile>();
            _lineHorizontalBonusList = new List<Tile>();
            _gameMap = gameMap;

        }

        // Method resets the buffers
        public void Restart()
        {
            _horizontalMatches.Clear();
            _verticalMatches.Clear();
            _bombBonusList.Clear();
            _lineVerticalBonusList.Clear();
            _lineHorizontalBonusList.Clear();
        }

        // Method looks for possible matches
        public bool Find()
        {
            bool reply = false;
            for (int i = 0; i < Game.MapSize; ++i)
            {
                for (int j = 0; j < Game.MapSize; ++j)
                {
                    if (_gameMap[i, j] != null)
                    {
                        FindMatchesAtTile(_gameMap[i, j]);
                        foreach (List<Tile> list in _horizontalMatches)
                        {
                            if (list.Count > 2)
                            {
                                int num = 0;
                                foreach (Tile tile in list)
                                {
                                    if (tile != null)
                                    {
                                        num++;
                                    }
                                }
                                if (num == list.Count)
                                {
                                    reply = true;
                                }
                            }
                        }
                        foreach (List<Tile> list in _verticalMatches)
                        {
                            if (list.Count > 2)
                            {
                                int num = 0;
                                foreach (Tile tile in list)
                                {
                                    if (tile != null)
                                    {
                                        num++;
                                    }
                                }
                                if (num == list.Count)
                                {
                                    reply = true;
                                }
                            }
                        }
                    }
                }
            }
            return reply;
        }

        // Method that searches for matches starting in the tile.
        public void FindMatchesAtTile(Tile tile)
        {
            _horizontalMatches.Clear();
            _verticalMatches.Clear();

            if (CheckHorizontalMatches(tile))
            {
                foreach (Tile ftile in _horizontalMatches[0])
                {
                    if (ftile != null)
                    {
                        CheckVerticalMatches(ftile);
                    }
                }
            }
            if (CheckVerticalMatches(tile))
            {
                foreach (Tile ftile in _verticalMatches[0])
                {
                    if (ftile != null)
                    {
                        CheckHorizontalMatches(ftile);
                    }
                }
            }
        }

        // The method checks for horizontal matches relative to the tile.
        private bool CheckHorizontalMatches(Tile tile)
        {

            _horizontalMatches.Add(new List<Tile>());
            int count = _horizontalMatches.Count;
            _horizontalMatches[count - 1].Add(tile);
            CheckLeftSide(tile);
            CheckRighSide(tile);
            return _horizontalMatches[count - 1].Count > 2;
        }

        // The method checks for vertical matches relative to the tile.
        private bool CheckVerticalMatches(Tile tile)
        {
            _verticalMatches.Add(new List<Tile>());
            int count = _verticalMatches.Count;
            _verticalMatches[count - 1].Add(tile);
            CheckUpward(tile);
            CheckDownward(tile);
            return _verticalMatches[count - 1].Count > 2;
        }

        // Checks to match the tiles on the left side.
        private void CheckLeftSide(Tile tile)
        {
            int count = _horizontalMatches.Count;
            for (int i = tile.Line - 1; i >= 0; i--)
            {
                if (!CheckFunction(tile, i, tile.Column, count))
                {
                    break;
                }
            }
        }

        // Checks to match the tiles on the righ side.
        private void CheckRighSide(Tile tile)
        {
            int count = _horizontalMatches.Count;
            for (int i = tile.Line + 1; i < Game.MapSize; i++)
            {
                if (!CheckFunction(tile, i, tile.Column, count))
                {
                    break;
                }
            }
        }

        // Checks upward matches.
        private void CheckUpward(Tile tile)
        {
            int count = _verticalMatches.Count;
            for (int i = tile.Column - 1; i >= 0; i--)
            {
                if (!CheckFunction(tile, tile.Line, i, count))
                {
                    break;
                }
            }
        }

        // Checks downward matches.
        private void CheckDownward(Tile tile)
        {
            int count = _verticalMatches.Count;
            for (int i = tile.Column + 1; i < Game.MapSize; i++)
            {
                if (!CheckFunction(tile, tile.Line, i, count))
                {
                    break;
                }
            }
        }

        // Generalized tile check function.
        private bool CheckFunction(Tile tile, int stepLine, int stepColumn, int count)
        {
            if (_gameMap[stepLine, stepColumn] != null)
            {
                if (_gameMap[stepLine, stepColumn].Name == tile.Name)
                {
                    if(stepLine != tile.Line)
                    {
                        _horizontalMatches[count - 1].Add((Tile)_gameMap[stepLine, stepColumn]);
                    }
                    else if (stepColumn != tile.Column)
                    {
                        _verticalMatches[count - 1].Add((Tile)_gameMap[stepLine, stepColumn]);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Method look for possible occurrences of bonuses.
        public void FindBonus(Tile[,] lastMovedMap)
        {
            _LastMovedMap = lastMovedMap;
            NoIntersectionСheck(_horizontalMatches, _lineHorizontalBonusList);
            NoIntersectionСheck(_verticalMatches, _lineVerticalBonusList);
            IntersectionСheck();
            _lineHorizontalBonusList = FindCopy(_lineHorizontalBonusList);
            _lineVerticalBonusList = FindCopy(_lineVerticalBonusList);
            _bombBonusList = FindCopy(_bombBonusList);
        }

        // Method looks for bonuses without considering intersections
        private void NoIntersectionСheck(List<List<Tile>> listTile, List<Tile> _bonusList)
        {
            foreach (List<Tile> list in listTile)
            {
                if (list.Count > 3)
                {
                    foreach (Tile tile in list)
                    {
                        if (_LastMovedMap[tile.Line, tile.Column] != null)
                        {
                            if (list.Count == 4)
                            {
                                _bonusList.Add(_LastMovedMap[tile.Line, tile.Column]);
                                break;
                            }
                            if (list.Count > 4)
                            {
                                _bombBonusList.Add(_LastMovedMap[tile.Line, tile.Column]);
                                break;
                            }

                        }
                    }
                }
            }
        }

        // Method looks for bonuses with considering intersections
        private void IntersectionСheck()
        {
            foreach (List<Tile> list in _horizontalMatches)
            {
                if (list.Count > 2)
                {
                    foreach (Tile tile in list)
                    {
                        if (_LastMovedMap[tile.Line, tile.Column] != null)
                        {
                            foreach (List<Tile> listC in  _verticalMatches)
                            {
                                if (listC.Count > 2)
                                {
                                    foreach (Tile tileC in listC)
                                    {
                                        if (tile == tileC)
                                        {
                                            _bombBonusList.Add(_gameMap[tile.Line, tile.Column]);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }



        // Method that finds duplicate elements.
        private List<Tile> FindCopy(List<Tile> list)
        {
            var reply = new List<Tile>();
            foreach (Tile tile in list)
            {
                var allCopy = list.FindAll(item => tile == item);
                reply.Add((Tile)allCopy[0].Clone());
            }
            return reply;
        }


        // Getters and Setters
        public List<List<Tile>> HorizontalMatches
        {
            get { return _horizontalMatches; }
        }

        public List<List<Tile>> VerticalMatches
        {
            get { return _verticalMatches; }
        }

        public List<Tile> LineVerticalBonusList
        {
            get { return _lineVerticalBonusList; }
        }

        public List<Tile> LineHorizontalBonusList
        {
            get { return _lineHorizontalBonusList; }
        }

        public List<Tile> BombBonusList
        {
            get { return _bombBonusList; }
        }
    }
}
