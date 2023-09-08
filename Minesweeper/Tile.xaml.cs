using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class Tile : Button
    {
        /// <summary>
        /// The row on the grid where this tile is located
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// The column on the grid where this tile is located
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// The state of the tile
        /// </summary>
        public TileState State { get; private set; }

        /// <summary>
        /// The type of tile 
        /// </summary>
        public TileType Type { get; private set; }

        /// <summary>
        /// The adjacent tiles next to this tile
        /// </summary>
        public List<Tile> Adjacent { get; }

        private string? _image;
        /// <summary>
        /// The image tag this tile should use for it's content
        /// </summary>
        public string Image
        {
            get { return "/Sprites/" + _image + ".png"; }
            set
            {
                _image = value;
                Content = new Image()
                {
                    Source = ImageHelper.GetImage(value)
                };
            }
        }

        public int AdjacentMines { get; private set; }


        public Tile(int column, int row)
        {
            InitializeComponent();
            DataContext = this;

            Column = column;
            Row = row;
            State = TileState.Closed;
            Type = TileType.Empty;
            Adjacent = new();
            Image = "TileUnknown";
        }

        /// <summary>
        /// forces a call to ImageBuilder to swap the image based on the current theme of the app
        /// </summary>
        public void SwapTheme()
        {
            if (_image != null)
            {
                Image = _image;
            }
        }

        public void SetMine()
        {
            Type = TileType.Mine;
        }

        public bool ToggleFlagged()
        {
            if (State == TileState.Flagged)
            {
                State = TileState.Closed;
                Image = "TileUnknown";
            }
            else
            {
                State = TileState.Flagged;
                Image = Properties.Settings.Default.EASTER_EGG ? "AniTileFlag" : "TileFlag";
                return true;
            }

            return false;
        }

        public bool Reveal(bool gameOver = false)
        {
            State = TileState.Opened;

            if (Type == TileType.Mine)
            {
                if (State == TileState.Flagged)
                {
                    if (gameOver) Image = "TileMineFlagged";
                }
                else
                {
                    if (gameOver) Image = "TileMine";
                    else Image = "TileExploded";
                }
                State = TileState.Exploded;
                return true;
            }
            else
            {
                if (!gameOver) Properties.Settings.Default.TILES_REVEALED++;
                var count = Adjacent.Count(t => t.Type == TileType.Mine);
                if (count > 0)
                {
                    Image = $"Tile{count}";
                }
                else
                {
                    Image = "TileEmpty";
                    foreach (var tile in Adjacent.Where(tile => tile.State == TileState.Closed)) tile.Reveal();
                }
            }
            return false;
        }

        internal void SetAdjacentTiles(Tile[,] board)
        {
            int[,] directions = { { 0, -1 }, { 0, 1 }, { -1, 0 }, { 1, 0 }, { 1, -1 }, { -1, 1 }, { 1, 1 }, { -1, -1 } };

            for (int i = 0; i < 8; i++)
            {
                int x = Column + directions[i, 0];
                if (x < 0 || x > 29) continue;

                int y = Row + directions[i, 1];
                if (y < 0 || y > 15) continue;

                var tile = board[x, y];

                if (tile.Type == TileType.Mine)
                {
                    Type = TileType.Numbered;
                    AdjacentMines++;
                }

                Adjacent.Add(tile);
            }
        }
    }
}
