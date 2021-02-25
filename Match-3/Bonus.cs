using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using System;

namespace Match_3
{

    // Base class for bonuses.
    public class Bonus: Drawable
    {
        protected Tile _tile;
        protected GameMaster _master;
        protected bool _isDraw;

        // Constructor
        public Bonus(Tile tile, GameMaster master, bool isDraw = true)
        {
            _tile = (Tile)tile.Clone();
            _master = master;
            _isDraw = isDraw;
        }

        // Update function, returns true until the bonus ends.
        public virtual bool Run(bool isDraw = true)
        {
            return true;
        }

        //  Interface Drawable implementation.
        public virtual void Draw(RenderTarget target, RenderStates state)
        {
            target.Draw(_tile);
        }

        // Getters and Setters.
        public Tile Tile
        {
            get { return _tile; }
            set { _tile= value; }
        }

    }

    // Base class for line bonus.
    public class LineBonus : Bonus
    {
        protected bool IsGo;
        protected List<Tile> _tileList;

        // Constructor.
        public LineBonus(Tile tile, GameMaster master, bool isDraw = true) : base(tile, master, isDraw)
        {
            _tileList = new List<Tile>();
            IsGo = false;
        }

        // Update function, returns true until the bonus ends.
        public override bool Run(bool isDraw = true)
        {
            _isDraw = isDraw;
            if (!IsGo)
            {
                FirstIter();
                IsGo = true;
            }
            else
            {
                var listCopy = new List<Tile>(_tileList);
                if (_tileList.Count == 0)
                {
                    return false;
                }
                else
                {
                    foreach (Tile tile in _tileList)
                    {
                        if (!MoveTile(tile))
                        {
                            listCopy.Remove(tile);
                        }
                    }
                    _tileList = listCopy;
                }
            }
            return true;
        }

        protected void FirstTileMove(Vector2f targetPosition)
        {
            var newTile = (Tile)_tile.Clone();
            if (_isDraw)
            {
                newTile.Position = _tile.Position;
                var animation = new MoveAnimation(newTile, targetPosition);
                _master.Painter.AddAnimation(animation);
            }
            else
            {
                newTile.Position = targetPosition;
            }
            var errorPosition = targetPosition - _tile.Position;
            newTile.Column = newTile.Column + (int)(errorPosition.Y / (float)Game.TileWidth);
            newTile.Line = newTile.Line + (int)(errorPosition.X / (float)Game.TileHeight);
            _tileList.Add(newTile);
        }

        // Function describing the movement of the destroyer.
        protected virtual bool MoveTile(Tile tile)
        {
            bool reply = false;
            var dif = tile.Position - _tile.Position;
            UpdateMap(tile);
            if (dif.X < 0)
            {
                if (tile.Line > 0 )
                {
                    Move(tile, dif);
                    reply = true;
                }
            }
            else if (dif.X > 0)
            {
                if (tile.Line < Game.MapSize - 1)
                {
                    Move(tile, dif);
                    reply = true;
                }
            }
            else if (dif.Y < 0)
            {
                if (tile.Column > 0)
                {
                    Move(tile, dif);
                    reply = true;
                }
            }
            else if (dif.Y > 0)
            {
                if (tile.Column < Game.MapSize - 1)
                {
                    Move(tile, dif);
                    reply = true;
                }
            }

            return reply;
        }

        private void Move(Tile tile, Vector2f errorPos)
        {
            var position = new Vector2f(tile.Position.X + Math.Sign(errorPos.X) * (float)Game.TileWidth, tile.Position.Y + Math.Sign(errorPos.Y) * (float)Game.TileHeight);
            if (_isDraw)
            {
                var animation = new MoveAnimation(tile, position);
                _master.Painter.AddAnimation(animation);
            }
            else
            {
                tile.Position = position;
            }
            tile.Line = tile.Line + Math.Sign(errorPos.X);
            tile.Column = tile.Column + Math.Sign(errorPos.Y);
        }

        // Function describing the creation of destroyers.
        protected virtual void FirstIter()
        {

        }

        // Helper function.
        protected void UpdateMap(Tile tile)
        {
            _master.DeleteTile(tile.Line, tile.Column);
        }

        //  Interface Drawable implementation.
        public override void Draw(RenderTarget target, RenderStates state)
        {
            foreach (Tile tile in _tileList)
            {
                target.Draw(tile);
            }
        }
    }

    // Bonus class vertical line.
    public class LineVerticalBonus: LineBonus
    {
        // Constructor.
        public LineVerticalBonus(Tile tile, GameMaster master, bool isDraw = true) : base(tile, master, isDraw)
        {

        }

        // Function describing the creation of destroyers.
        protected override void FirstIter()
        {
            if (_tile.Column > 0)
            {
                var position = new Vector2f(_tile.Position.X, _tile.Position.Y - (float)Game.TileHeight);
                FirstTileMove(position);
            }
            if (_tile.Column < Game.MapSize - 1)
            {
                var position = new Vector2f(_tile.Position.X, _tile.Position.Y + (float)Game.TileHeight);
                FirstTileMove(position);
            }
            _master.DeleteTile(_tile.Line, _tile.Column);
        }
    }

    // Bonus class horizontal line.
    public class LineHorizontalBonus : LineBonus
    {

        // Constructor
        public LineHorizontalBonus(Tile tile, GameMaster master, bool isDraw = true) : base(tile, master, isDraw)
        {
        }

        // Function describing the creation of destroyers.
        protected override void FirstIter()
        {
            if (_tile.Line > 0)
            {
                var position = new Vector2f(_tile.Position.X - (float)Game.TileWidth, _tile.Position.Y);
                FirstTileMove(position);
            }
            if (_tile.Line < Game.MapSize - 1)
            {
                var position = new Vector2f(_tile.Position.X + (float)Game.TileWidth, _tile.Position.Y);
                FirstTileMove(position);
            }
            _master.DeleteTile(_tile.Line, _tile.Column);
        }
    }

    // Bonus class bomb.
    public class BombBonus: Bonus
    {
        private bool IsGo;
        private Clock _clock;

        // Constructor.
        public BombBonus(Tile tile, GameMaster master, bool isDraw = true) : base(tile, master, isDraw)
        {
            _clock = new Clock();
            IsGo = false;
        }

        // Method describes the removal of tiles around the bomb
        private void Boom()
        {
            var i = _tile.Line;
            var j = _tile.Column;
            if(i > 0)
            {
                _master.DeleteTile(i - 1, j);
                if (j > 0)
                {
                    _master.DeleteTile(i - 1, j-1);
                    _master.DeleteTile(i, j - 1);
                }
                if (j < Game.MapSize - 1)
                {
                    _master.DeleteTile(i - 1, j + 1);
                    _master.DeleteTile(i, j + 1);
                }
            }
            if(i < Game.MapSize-1)
            {
                _master.DeleteTile(i + 1, j);
                if (j > 0)
                {
                    _master.DeleteTile(i + 1, j - 1);
                }
                if (j < Game.MapSize - 1)
                {
                    _master.DeleteTile(i + 1, j + 1);
                }
            }
            _master.DeleteTile(i, j);
        }

        // Update function, returns true until the bonus ends.
        public override bool Run(bool isDraw = true)
        {
            bool reply = true;
            if (!IsGo)
            {
                IsGo = true;
                _clock.Restart();
            }
            else
            {
                if (_clock.ElapsedTime.AsSeconds() < 0.5f)
                {
                    if (isDraw)
                    {
                        _master.Painter.AddAnimation(new BoomAnimation(_tile));
                    }
                    Boom();
                    reply = false;
                }
            }
            return reply;
        }
    }
}
