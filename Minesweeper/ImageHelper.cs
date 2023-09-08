using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    public static class ImageHelper
    {
        private readonly static Dictionary<string, BitmapImage> LightSprites = new()
        {
            { "AniTileFlag", new BitmapImage(new Uri("/Sprites/AniTileFlag.png", UriKind.Relative)) },
            { "Tile1", new BitmapImage(new Uri("/Sprites/Tile1.png", UriKind.Relative)) },
            { "Tile2", new BitmapImage(new Uri("/Sprites/Tile2.png", UriKind.Relative)) },
            { "Tile3", new BitmapImage(new Uri("/Sprites/Tile3.png", UriKind.Relative)) },
            { "Tile4", new BitmapImage(new Uri("/Sprites/Tile4.png", UriKind.Relative)) },
            { "Tile5", new BitmapImage(new Uri("/Sprites/Tile5.png", UriKind.Relative)) },
            { "Tile6", new BitmapImage(new Uri("/Sprites/Tile6.png", UriKind.Relative)) },
            { "Tile7", new BitmapImage(new Uri("/Sprites/Tile7.png", UriKind.Relative)) },
            { "Tile8", new BitmapImage(new Uri("/Sprites/Tile8.png", UriKind.Relative)) },
            { "TileEmpty", new BitmapImage(new Uri("/Sprites/TileEmpty.png", UriKind.Relative)) },
            { "TileFlag", new BitmapImage(new Uri("/Sprites/TileFlag.png", UriKind.Relative)) },
            { "TileMine", new BitmapImage(new Uri("/Sprites/TileMine.png", UriKind.Relative)) },
            { "TileMineFlagged", new BitmapImage(new Uri("/Sprites/TileMineFlagged.png", UriKind.Relative)) },
            { "TileUnknown", new BitmapImage(new Uri("/Sprites/TileUnknown.png", UriKind.Relative)) },
            { "TileExploded", new BitmapImage(new Uri("/Sprites/TileExploded.png", UriKind.Relative)) },

        };

        private readonly static Dictionary<string, BitmapImage> DarkSprites = new()
        {
            { "AniTileFlag", new BitmapImage(new Uri("/Sprites/AniDarkTileFlag.png", UriKind.Relative)) },
            { "Tile1", new BitmapImage(new Uri("/Sprites/DarkTile1.png", UriKind.Relative)) },
            { "Tile2", new BitmapImage(new Uri("/Sprites/DarkTile2.png", UriKind.Relative)) },
            { "Tile3", new BitmapImage(new Uri("/Sprites/DarkTile3.png", UriKind.Relative)) },
            { "Tile4", new BitmapImage(new Uri("/Sprites/DarkTile4.png", UriKind.Relative)) },
            { "Tile5", new BitmapImage(new Uri("/Sprites/DarkTile5.png", UriKind.Relative)) },
            { "Tile6", new BitmapImage(new Uri("/Sprites/DarkTile6.png", UriKind.Relative)) },
            { "Tile7", new BitmapImage(new Uri("/Sprites/DarkTile7.png", UriKind.Relative)) },
            { "Tile8", new BitmapImage(new Uri("/Sprites/DarkTile8.png", UriKind.Relative)) },
            { "TileEmpty", new BitmapImage(new Uri("/Sprites/DarkTileEmpty.png", UriKind.Relative)) },
            { "TileFlag", new BitmapImage(new Uri("/Sprites/DarkTileFlag.png", UriKind.Relative)) },
            { "TileMine", new BitmapImage(new Uri("/Sprites/DarkTileMine.png", UriKind.Relative)) },
            { "TileMineFlagged", new BitmapImage(new Uri("/Sprites/DarkTileMineFlagged.png", UriKind.Relative)) },
            { "TileUnknown", new BitmapImage(new Uri("/Sprites/DarkTileUnknown.png", UriKind.Relative)) },
            { "TileExploded", new BitmapImage(new Uri("/Sprites/TileExploded.png", UriKind.Relative)) },
        };

        public static BitmapImage GetImage(string image)
        {
            if (image == "TileFlag" && Properties.Settings.Default.EASTER_EGG)
            {
                image = "Ani" + image;
            }

            if (Properties.Settings.Default.DARK_MODE)
            {
                return DarkSprites[image];
            }

            return LightSprites[image];
        }
    }
}
