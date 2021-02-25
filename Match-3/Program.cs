using System;
using SFML.Graphics;
using System.Configuration.Assemblies;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;

namespace Match_3
{
    class Game
    {
        // Images and Font resourse
        public static Texture Background;
        public static Texture Bomb;
        public static Texture Button;
        public static Font Fonts;
        public static Texture Frame;
        public static Texture Icon;
        public static Texture InfoPanel;
        public static Texture Line;
        public static Texture LineV;
        public static Texture Table;

        // Static properties.
        public static uint WindowWidth;
        public static uint WindowHeight;
        public static uint GameFieldWidth;
        public static uint GameFieldHeight;
        public static uint MapSize;
        public static uint TileWidth;
        public static uint TileHeight;
        public static uint StepInImage;
        public static string GameName;
        public static float TimeStep;
        public static float GameDuration;

        // Static constructor, load files from Resource.
        static Game()
        {
            // Image resourse loading
            Icon        = new Texture(Resource.Icon);
            Background  = new Texture(Resource.Background);
            Bomb        = new Texture(Resource.Bomb);
            Button      = new Texture(Resource.Button); 
            Frame       = new Texture(Resource.Frame);
            Icon        = new Texture(Resource.Icon);
            InfoPanel   = new Texture(Resource.InfoPanel);
            Line        = new Texture(Resource.Line);
            LineV       = new Texture(Resource.LineV);
            Table       = new Texture(Resource.Table);

            // Fonts resourse loading
            Fonts       = new Font(Resource.Fonts);

            // Loading settings
            var settings    = new ConfigurationBuilder().AddXmlFile("Settings.xml").Build();
            WindowWidth     = Convert.ToUInt32(settings["WindowWidth"]);
            WindowHeight    = Convert.ToUInt32(settings["WindowHeight"]);
            GameFieldWidth  = Convert.ToUInt32(settings["GameFieldWidth"]);
            GameFieldHeight = Convert.ToUInt32(settings["GameFieldHeight"]);
            MapSize         = Convert.ToUInt32(settings["MapSize"]);
            StepInImage     = Convert.ToUInt32(settings["StepInImage"]);
            GameName        = Convert.ToString( settings["GameName"]);
            GameDuration    = (float)Convert.ToInt32(settings["GameDuration"]);
            TileWidth       = GameFieldWidth / MapSize;
            TileHeight      = GameFieldHeight / MapSize;
            TimeStep = (float)TileWidth * 0.2f;

        }

        static void Main(string[] args)
        {
            Painter UI = new Painter();
            GameMaster Logic = new GameMaster(UI);
            UI.Run();

        }

    }
}
