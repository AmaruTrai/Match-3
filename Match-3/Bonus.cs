using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;


namespace Match_3
{

    // Base class for bonuses.
    public class Bonus: Drawable
    {
        protected Tile _tile;
        protected GameMaster _master;

        // Constructor
        public Bonus(Tile tile, GameMaster master)
        {
            _tile = (Tile)tile.Clone();
            _master = master;
        }

        // Update function, returns true until the bonus ends.
        public virtual bool Run()
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
        public LineBonus(Tile tile, GameMaster master) : base(tile, master)
        {
            _tileList = new List<Tile>();
            IsGo = false;
        }

        // Update function, returns true until the bonus ends.
        public override bool Run()
        {
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

        // Function describing the movement of the destroyer.
        protected virtual bool MoveTile(Tile tile)
        {
            return false;
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
        public LineVerticalBonus(Tile tile, GameMaster master) : base(tile, master)
        {

        }

        // Function describing the creation of destroyers.
        protected override void FirstIter()
        {
            if (_tile.Column > 0)
            {

                _tileList.Add((Tile)_tile.Clone());
                var count = _tileList.Count - 1;
                var position = new Vector2f(_tileList[count].Position.X, _tileList[count].Position.Y - (float)Game.TileHeight);
                var animation = new MoveAnimation(_tileList[count], position);
                _master.Painter.AddAnimation(animation);
                _tileList[count].Column = _tileList[count].Column - 1;
            }
            if (_tile.Column < Game.MapSize - 1)
            {
                var newTile = (Tile)_tile.Clone();
                newTile.Position = _tile.Position;
                var position = new Vector2f(newTile.Position.X, newTile.Position.Y + (float)Game.TileHeight);
                var animation = new MoveAnimation(newTile, position);
                _master.Painter.AddAnimation(animation);
                newTile.Column = newTile.Column + 1;
                _tileList.Add(newTile);
            }
            _master.DeleteTile(_tile.Line, _tile.Column);
        }

        // Function describing the movement of the destroyer.
        protected override bool MoveTile(Tile tile)
        {
            bool reply = false;
            var dif = tile.Column -  _tile.Column;
            UpdateMap(tile);

            if (dif < 0)
            {
                if (tile.Column > 0)
                {
                    var position = new Vector2f(tile.Position.X, tile.Position.Y - (float)Game.TileHeight);
                    var animation = new MoveAnimation(tile, position);
                    _master.Painter.AddAnimation(animation);
                    tile.Column = tile.Column - 1;
                    reply = true;

                }
            }
            else if (dif > 0)
            {
                if (tile.Column < Game.MapSize - 1)
                {

                    var position = new Vector2f(tile.Position.X, tile.Position.Y + (float)Game.TileHeight);
                    var animation = new MoveAnimation(tile, position);
                    _master.Painter.AddAnimation(animation);
                    tile.Column = tile.Column + 1;
                    reply = true;

                }
            }
            return reply;
        }


    }

    // Bonus class horizontal line.
    public class LineHorizontalBonus : LineBonus
    {

        // Constructor
        public LineHorizontalBonus(Tile tile, GameMaster master) : base(tile, master)
        {
        }

        // Function describing the creation of destroyers.
        protected override void FirstIter()
        {
            if (_tile.Line > 0)
            {

                _tileList.Add((Tile)_tile.Clone());
                var count = _tileList.Count - 1;
                var position = new Vector2f(_tileList[count].Position.X - (float)Game.TileWidth, _tileList[count].Position.Y);
                var animation = new MoveAnimation(_tileList[count], position);
                _master.Painter.AddAnimation(animation);
                _tileList[count].Line = _tileList[count].Line - 1;
            }
            if (_tile.Line < Game.MapSize - 1)
            {
                var newTile = (Tile)_tile.Clone();
                newTile.Position = _tile.Position;
                var position = new Vector2f(newTile.Position.X + (float)Game.TileWidth, newTile.Position.Y);
                var animation = new MoveAnimation(newTile, position);
                _master.Painter.AddAnimation(animation);
                newTile.Line = newTile.Line + 1;
                _tileList.Add(newTile);
            }
            _master.DeleteTile(_tile.Line, _tile.Column);
        }

        // Function describing the movement of the destroyer.
        protected override bool MoveTile(Tile tile)
        {
            bool reply = false;
            var dif = tile.Line - _tile.Line;
            UpdateMap(tile);
            if (dif < 0)
            {
                if (tile.Line > 0)
                {
                    var position = new Vector2f(tile.Position.X - (float)Game.TileWidth, tile.Position.Y);
                    var animation = new MoveAnimation(tile, position);
                    _master.Painter.AddAnimation(animation);
                    tile.Line = tile.Line - 1;
                    reply = true;

                }
            }
            else if (dif > 0)
            {
                if (tile.Line < Game.MapSize - 1)
                {

                    var position = new Vector2f(tile.Position.X + (float)Game.TileWidth, tile.Position.Y);
                    var animation = new MoveAnimation(tile, position);
                    _master.Painter.AddAnimation(animation);
                    tile.Line = tile.Line + 1;
                    reply = true;

                }
            }
            return reply;
        }

    }

    // Bonus class bomb.
    public class BombBonus: Bonus
    {
        private bool IsGo;
        private Clock _clock;

        // Constructor.
        public BombBonus(Tile tile, GameMaster master) : base(tile, master)
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
        public override bool Run()
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
                    _master.Painter.AddAnimation(new BoomAnimation(_tile));
                    Boom();
                    reply = false;
                }
            }
            return reply;
        }
    }
}
