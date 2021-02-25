using System;
using SFML.Graphics;
using SFML.System;


namespace Match_3
{

    // Base class for all animations
    public abstract class Animation : Drawable
    {
        // Method returning true if animation is complete.
        abstract public bool Finished();

        //  Interface Drawable implementation.
        abstract public void Draw(RenderTarget target, RenderStates state);

    }

    // Tile motion animation
    public class MoveAnimation : Animation
    {
        private bool _isDone = false;
        private Tile _tile;
        private Vector2f _targetPosition;
        private Clock _clock;

        // Constructor without target position, fall down.
        public MoveAnimation(Tile targetTile)
        {
            _tile = targetTile;
            _targetPosition = new Vector2f(_tile.Position.X, _tile.Position.Y + Game.TileHeight);
            _clock = new Clock();
            _clock.Restart();
        }

        // Constructor with target position.
        public MoveAnimation(Tile targetTile, Vector2f position)
        {
            _tile = targetTile;
            _targetPosition = new Vector2f(position.X, position.Y);
            _clock = new Clock();
            _clock.Restart();
        }

        //  Interface Drawable implementation.
        public override void Draw(RenderTarget target, RenderStates state)
        {
            var time = _clock.ElapsedTime.AsSeconds();
            if (_tile.Position.Y != _targetPosition.Y)
            {
                var dif = _targetPosition.Y-  _tile.Position.Y;
                _tile.Position = _tile.Position + new Vector2f(0, Math.Sign(dif) * Game.TimeStep* time);
            }
            if (_tile.Position.X != _targetPosition.X)
            {
                var dif = _targetPosition.X - _tile.Position.X;
                _tile.Position = _tile.Position + new Vector2f(Math.Sign(dif) * Game.TimeStep* time, 0);
            }
            var errX = Math.Abs(_tile.Position.X - _targetPosition.X);
            var errY = Math.Abs(_tile.Position.Y - _targetPosition.Y);
            if (errX < 1 && errY < 1)
            {
                _tile.Position = _targetPosition;
                _isDone = true;
            }
            target.Draw(_tile);
        }

        // Method returning true if animation is complete.
        public override bool Finished()
        {
            return _isDone;
        }

    }

    // Tile disappearance animation.
    public class DestroyAnimation : Animation
    {
        private float widthScale = (float)Game.TileWidth / 250f;
        private float heightScale = (float)Game.TileHeight / 250f;
        private Sprite _sprite;
        private Clock _clock = new Clock();
        private bool _isDone = false;
        private Tile _targetTile;

        // Constructor
        public DestroyAnimation(Tile targetTile)
        {
            _targetTile = targetTile;
            if (targetTile == null)
            {
                _isDone = true;
            }
            else
            {
                _sprite = new Sprite(targetTile.Sprite);
                _clock.Restart();
            }
        }

        //  Interface Drawable implementation.
        public override void Draw(RenderTarget target, RenderStates state)
        {
            float time = _clock.ElapsedTime.AsSeconds();
            if (time > 0.4f)
            {
                _isDone = true;
            }
            else
            {
                var timefactor = time * 1.75f;
                var widthBias = widthScale - widthScale * timefactor;
                var heighBias = heightScale - heightScale * timefactor;
                if (widthBias  < 0)
                {
                    widthBias = 0;
                }
                if (heighBias < 0)
                {
                    heighBias = 0;
                }
                _sprite.Scale = new Vector2f(widthBias, heighBias);

                _sprite.Position = new Vector2f(_sprite.Position.X + widthScale * timefactor*2, _sprite.Position.Y + heightScale * timefactor*2);
                target.Draw(_sprite);
            }

        }

        // Method returning true if animation is complete.
        public override bool Finished()
        {
            return _isDone;
        }
    }


    // Animation of swapping two tiles
    public class SwapAnimation: Animation
    {
        private bool _isDone = false;
        private bool _returnBack = false;
        private MoveAnimation _selectedAnimation;
        private MoveAnimation _targetAnimation;
        private MoveAnimation _selectedAnimationBack;
        private MoveAnimation _targetAnimationBack;

        // Constructor
        public SwapAnimation(Tile selectedTile, Tile targetTile, bool returnBack = false)
        {
            _returnBack = returnBack;
            _selectedAnimation = new MoveAnimation(selectedTile, targetTile.Position);
            _targetAnimation = new MoveAnimation(targetTile, selectedTile.Position);
            if (_returnBack)
            {
                _selectedAnimationBack = new MoveAnimation(selectedTile, selectedTile.Position);
                _targetAnimationBack = new MoveAnimation(targetTile, targetTile.Position);
            }
        }

        //  Interface Drawable implementation.
        public override void Draw(RenderTarget target, RenderStates state)
        {
            if (_selectedAnimation.Finished() && _targetAnimation.Finished())
            {
                if (_returnBack)
                {
                    if (_selectedAnimationBack.Finished() && _targetAnimationBack.Finished())
                    {
                        _returnBack = false;
                    }
                    else
                    {
                        target.Draw(_selectedAnimationBack);
                        target.Draw(_targetAnimationBack);
                    }
                }
                else
                {
                    _isDone = true;
                }
            }
            else
            {
                target.Draw(_selectedAnimation);
                target.Draw(_targetAnimation);
            }
        }

        // Method returning true if animation is complete.
        public override bool Finished()
        {
            return _isDone;
        }
    }

    // Bomb explosion animation
    public class BoomAnimation: Animation
    {
        private bool _isDone = false;
        private Tile _tile;
        private Clock _clock;
        private float widthScale = (float)Game.TileWidth / 250f;
        private float heightScale = (float)Game.TileHeight / 250f;

        // Constructor
        public BoomAnimation(Tile tile)
        {
        _tile = tile;
            _clock = new Clock();
            _clock.Restart();
        }

        //  Interface Drawable implementation.
        public override void Draw(RenderTarget target, RenderStates state)
        {
            float time = _clock.ElapsedTime.AsSeconds();
            if (time < 0.5f)
            {
                var timefactor = time * 4f;
                var widthBias = widthScale + widthScale * timefactor;
                var heighBias = heightScale + heightScale * timefactor;
                _tile.Position = new Vector2f(_tile.Position.X - timefactor, _tile.Position.Y - timefactor);
                _tile.Sprite.Scale = new Vector2f(widthBias, heighBias);
                target.Draw(_tile);
            }
            else
            {
                _isDone = true;
            }

        }

        // Method returning true if animation is complete.
        public override bool Finished()
        {
            return _isDone;
        }
    }




}
