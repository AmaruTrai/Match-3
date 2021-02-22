using System;


namespace Match_3
{
    class Game
    {
        // Static properties.
        public static uint WindowWidth = 742;
        public static uint WindowHeight = 512;
        public static uint GameFieldWidth = 512;
        public static uint GameFieldHeight = 512;
        public static uint MapSize = 8;
        public static uint TileWidth = GameFieldWidth / 8;
        public static uint TileHeight = GameFieldHeight / 8;
        public static string GameName = "Match-3";
        public static string CurrentFolder = AppDomain.CurrentDomain.BaseDirectory;
        public static float TimeStep = 4f;
        public static float GameDuration = 60f;


        static void Main(string[] args)
        {
            Painter UI = new Painter();
            GameMaster Logic = new GameMaster(UI);
            UI.Run();

        }

    }
}
