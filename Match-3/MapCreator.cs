using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace Match_3
{
    public class MapCreator
    {
        private Tile[,] _gameMap;

        public MapCreator(Tile[,] gameMap)
        {
            _gameMap = gameMap;
        }

        public void CreateGameMap()
        {

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
        public List<Animation> GenerateTopLine(bool isDraw = true)
        {
            List<Animation> reply = new List<Animation>();
            for (int j = 0; j < Game.MapSize; ++j)
            {
                if (_gameMap[j, 0] == null)
                {
                    var positionStart = new Vector2f(j * Game.TileWidth, -1 * Game.TileHeight);
                    var positionEnd = new Vector2f(j * Game.TileWidth, 0 * Game.TileHeight);
                    if (isDraw)
                    {
                        _gameMap[j, 0] = Tile.CreateRandomTile(Game.Icon, positionStart, j, 0);
                        reply.Add(new MoveAnimation(_gameMap[j, 0], positionEnd));
                    }
                    else
                    {
                        _gameMap[j, 0] = Tile.CreateRandomTile(Game.Icon, positionEnd, j, 0);
                    }
                }
            }
            return reply;
        }




    }
}
