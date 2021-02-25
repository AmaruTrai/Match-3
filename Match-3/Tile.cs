using System;
using SFML.Graphics;
using SFML.System;

namespace Match_3
{
    // Class describing the tile of the game field.
    public class Tile : Drawable, ICloneable
    {
        // Private parameters.
        protected Sprite _sprite;
        protected string _id;

        public int Line { get; set; }
        public int Column { get; set; }

        // Constructor.
        public Tile(Sprite sprite, string id, int line, int column)
        {
            _sprite = new Sprite(sprite);
            _id = id;
            Line = line;
            Column = column;
        }

        // Returns a tile of a random type
        static public Tile CreateRandomTile(Texture texture, Vector2f position, int line, int column)
        {
            int random = new Random().Next(1, 5);
            IntRect rect;
            string name;
            var width = (int)Game.StepInImage;
            var height = (int)texture.Size.Y;
            switch (random)
            {
                case 1:
                    rect = new IntRect(0, 0, width, height);
                    name = "Red";
                    break;
                case 2:
                    rect = new IntRect(width, 0, width, height);
                    name = "Purple";
                    break;
                case 3:
                    rect = new IntRect(width*2, 0, width, height);
                    name = "Blue";
                    break;
                case 4:
                    rect = new IntRect(width*3, 0, width, height);
                    name = "Orange";
                    break;
                default:
                    rect = new IntRect(width*4, 0, width, height);
                    name = "Green";
                    break;
            }
            Sprite sprite = new Sprite(texture, rect);
            sprite.Scale = new Vector2f((float)Game.TileWidth /(float) width, (float)Game.TileHeight / (float)height);
            sprite.Position = position;
            return new Tile(sprite, name, line, column) ;
        }

        //  Interface Drawable implementation.
        public void Draw(RenderTarget target, RenderStates state)
        {
            target.Draw(_sprite);
        }

        // Interface ICloneable implementation.
        public object Clone()
        {
            return new Tile(_sprite, _id, Line, Column);
        }

        // Getters and Setters
        public Vector2f Position
        {
            get { return _sprite.Position; }
            set { _sprite.Position = value; }
        }

        public string Name
        {
            get { return _id; }
        }

        public Sprite Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }
    }


    // Class describing tiles containing bonuses
    public class BonusTile : Tile
    {
        private Bonus _bonus;

        // Constructors
        public BonusTile(Sprite sprite, string id, int line, int column, Bonus bonus) : base(sprite,id,line,column)
        {
            _bonus = bonus;
        }

        public BonusTile(Texture texture, Tile tile, Bonus bonus) : this(tile.Sprite, tile.Name, tile.Line, tile.Column, bonus)
        {
            var width = (int)Game.StepInImage;
            var height = (int)texture.Size.Y;
            IntRect rect;
            switch (_id)
            {
                case "Red":
                    rect = new IntRect(0, 0, width, height);
                    break;
                case "Purple":
                    rect = new IntRect(width, 0, width, height);
                    break;
                case "Blue":
                    rect = new IntRect(width * 2, 0, width, height);
                    break;
                case "Orange":
                    rect = new IntRect(width * 3, 0, width, height);
                    break;
                default:
                    rect = new IntRect(width * 4, 0, width, height);
                    break;
            }
            _sprite = new Sprite(texture, rect);
            _sprite.Scale = new Vector2f((float)Game.TileWidth / (float)width, (float)Game.TileHeight / (float)height);
            _sprite.Position = tile.Position;
        }

        // Getters and Setters
        public Bonus Bonus
        {
            get
            {
                _bonus.Tile = this;
                return _bonus;
            }
        }
    }

}

