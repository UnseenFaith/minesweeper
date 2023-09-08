using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Timers;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Tile[,]? _board;

        private readonly int _width = 30;

        private readonly int _height = 16;

        private readonly Random _random = new();

        private bool canRestart = false;

        private int _numOfFlagged = 0;

        private readonly int _total;

        private readonly List<Tile> _mineTiles = new();

        private readonly Timer _timer = new(1000);

        private bool _safezone = false;

        private bool _gameFinished = false;

        public MainWindow()
        {
            InitializeComponent();
            _total = _width * _height - 10;

            StartGame();
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Properties.Settings.Default.TIME_SPENT_PLAYING += TimeSpan.FromSeconds(1);
        }

        public void StartGame()
        {
            canRestart = false;
            _safezone = false;
            _gameFinished = false;

            _board = new Tile[_width, _height];
            _numOfFlagged = 0;
            _mineTiles.Clear();
            BoardGrid.Children.Clear();
            BoardGrid.ColumnDefinitions.Clear();
            BoardGrid.RowDefinitions.Clear();

            for (var i = 0; i < _height; i++) BoardGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });
            for (var i = 0; i < _width; i++) BoardGrid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var newTile = new Tile(i, j);
                    _board[i, j] = newTile;
                    newTile.PreviewMouseDown += Tile_PreviewMouseDown;
                    newTile.PreviewMouseUp += Tile_PreviewMouseUp;
                    newTile.MouseLeave += Tile_MouseLeave;

                    BoardGrid.Children.Add(newTile);
                }
            }

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var tile = _board[i, j];
                    tile.SetAdjacentTiles(_board);
                }
            }

            FlagsLeftText.Text = $"{99 - _numOfFlagged}/99";
        }

        private void Tile_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not Tile tile) return;


            if (e.XButton2 == MouseButtonState.Pressed || e.XButton1 == MouseButtonState.Pressed)
            {

                if (tile.State == TileState.Closed) tile.Image = "TileUnknown";
                foreach (var adjacent in tile.Adjacent.Where(tile => tile.State == TileState.Closed)) adjacent.Image = "TileUnknown";
            }
        }

        private void Tile_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Tile tile) return;

            switch (e.ChangedButton)
            {
                /// Reveal
                case MouseButton.Left:
                    {
                        switch (tile.State)
                        {
                            case TileState.Opened:
                                {
                                    var flaggedCount = tile.Adjacent.Count(t => t.State == TileState.Flagged);
                                    var mineCount = tile.Adjacent.Count(t => t.Type == TileType.Mine);
                                    if (mineCount > 0 && flaggedCount == mineCount)
                                    {
                                        Tile exploded = null;
                                        bool mineRevealed = false;

                                        foreach (var adjacent in tile.Adjacent.Where(t => t.State == TileState.Closed))
                                        {
                                            bool mine = adjacent.Reveal();

                                            if (mine && mineRevealed == false)
                                            {
                                                mineRevealed = true;
                                                exploded = adjacent;
                                            }

                                        }

                                        if (mineRevealed && exploded != null)
                                        {
                                            GameOver(exploded);
                                            return;
                                        }

                                        CheckForWin();
                                    }
                                    else
                                    {
                                        foreach (var adjacent in tile.Adjacent.Where(t => t.State == TileState.Closed)) adjacent.Image = "TileUnknown";
                                    }
                                }
                                break;

                            case TileState.Flagged:
                                return;
                            default:
                                {
                                    if (!_safezone)
                                    {
                                        _safezone = true;
                                        SetupMines(tile);
                                    }

                                    bool mineRevealed = tile.Reveal();


                                    if (mineRevealed)
                                    {
                                        GameOver(tile);
                                    }

                                    CheckForWin();

                                    canRestart = true;

                                    break;
                                }
                        }
                        break;
                    }

                // Double Click Functionality
                // Reveal if Flagged, Otherwise just show Adjacent
                case MouseButton.XButton2:
                case MouseButton.XButton1:
                    {
                        if (tile.State == TileState.Closed)
                        {
                            tile.Image = "TileUnknown";
                            foreach (var adjacent in tile.Adjacent.Where(tile => tile.State == TileState.Closed)) adjacent.Image = "TileUnknown";
                        }
                        else
                        {
                            var flaggedCount = tile.Adjacent.Count(t => t.State == TileState.Flagged);
                            var mineCount = tile.Adjacent.Count(t => t.Type == TileType.Mine);
                            if (mineCount > 0 && flaggedCount == mineCount)
                            {
                                Tile exploded = null;
                                bool mineRevealed = false;

                                foreach (var adjacent in tile.Adjacent.Where(t => t.State == TileState.Closed))
                                {
                                    bool mine = adjacent.Reveal();

                                    if (mine && mineRevealed == false)
                                    {
                                        mineRevealed = true;
                                        exploded = adjacent;
                                    }

                                }

                                if (mineRevealed && exploded != null)
                                {
                                    GameOver(exploded);
                                    return;
                                }

                                CheckForWin();
                            }
                            else
                            {
                                foreach (var adjacent in tile.Adjacent.Where(t => t.State == TileState.Closed)) adjacent.Image = "TileUnknown";
                            }
                        }
                        break;
                    }



                // Flag Tiles
                case MouseButton.Right:
                    {
                        if (tile.State == TileState.Opened) return;

                        if (_numOfFlagged == _mineTiles.Count && tile.State != TileState.Flagged) return;

                        var flagged = tile.ToggleFlagged();

                        if (flagged)
                        {
                            _numOfFlagged++;
                        }
                        else
                        {
                            _numOfFlagged--;
                        }

                        FlagsLeftText.Text = $"{99 - _numOfFlagged}/99";
                        break;
                    }
            }
        }

        private void Tile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Tile tile) return;

            switch (e.ChangedButton)
            {
                case MouseButton.XButton2:
                case MouseButton.XButton1:
                    {
                        /// Preview Tiles Around Clicked Tile
                        if (tile.State == TileState.Closed) tile.Image = "TileEmpty";
                        foreach (var adjacent in tile.Adjacent.Where(tile => tile.State == TileState.Closed)) adjacent.Image = "TileEmpty";

                        break;
                    }
                case MouseButton.Left:
                    {
                        if (tile.State == TileState.Opened)
                        {
                            foreach (var adjacent in tile.Adjacent.Where(tile => tile.State == TileState.Closed)) adjacent.Image = "TileEmpty";
                        }
                        break;
                    }
            }
        }

        public void GameOver(Tile bombExploded)
        {
            _timer.Stop();

            _gameFinished = true;

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var tile = _board[i, j];
                    tile.IsHitTestVisible = false;

                    if (tile.Type == TileType.Mine)
                    {
                        if (tile.State == TileState.Flagged)
                        {
                            Properties.Settings.Default.MINES_FLAGGED++;
                            tile.Image = "TileMineFlagged";
                        } else
                        {
                            if (tile != bombExploded) tile.Reveal(true);
                        }
                    }
                }
            }
        }

        public void GameWon()
        {
            _timer.Stop();

            _gameFinished = true;

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    Tile tile = _board[i, j];

                    if (tile.State == TileState.Closed) tile.Reveal(true);
                    tile.IsHitTestVisible = false;
                }
            }

            Properties.Settings.Default.MINES_FLAGGED += _mineTiles.Count;
            Properties.Settings.Default.GAMES_WON++;
        }

        private void CheckForWin()
        {
            int revealed = 0;

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    Tile tile = _board[i, j];

                    if (tile.State == TileState.Opened) revealed++;
                }
            }

            if (revealed == (_total))
            {
                GameWon();
            }
        }

        public void SetupMines(Tile safezone)
        {
            _timer.Start();
            Properties.Settings.Default.GAMES_PLAYED++;

            List<Tile> adjacent = safezone.Adjacent;

            // Add Mines
            for (var i = 0; i < 99; i++)
            {
                var j = _random.Next(_width);
                var k = _random.Next(_height);
                var tile = _board[j, k];

                if (safezone == tile || adjacent.Contains(tile) || tile.Type == TileType.Mine)
                {
                    i--;
                } else
                {
                    tile.SetMine();
                    _mineTiles.Add(tile);
                }
            }
        }

        private void SetTheme()
        {
            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    //Tile tile = board[i, j];
                    //tile.ChangeTheme();

                    _board[i, j].SwapTheme();
                }
            }

            if (Properties.Settings.Default.DARK_MODE)
            {
                Properties.Settings.Default.BACKGROUND = System.Drawing.Color.FromArgb(65, 65, 65);
                Properties.Settings.Default.FOREGROUND = "White";
            } else
            {
                Properties.Settings.Default.BACKGROUND = System.Drawing.Color.FromArgb(195, 195, 195);
                Properties.Settings.Default.FOREGROUND = "Black";
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;

            switch(e.Key)
            {
                case Key.R:
                    if (canRestart)
                    {
                        _timer.Stop();
                        StartGame();
                    }
                    break;
                case Key.T:
                    Properties.Settings.Default.DARK_MODE = !Properties.Settings.Default.DARK_MODE;
                    SetTheme();
                    break;
                case Key.A:
                    Properties.Settings.Default.EASTER_EGG = !Properties.Settings.Default.EASTER_EGG;

                    for (var i = 0; i < _width; i++)
                    {
                        for (var j = 0; j < _height; j++)
                        {
                            Tile tile = _board[i, j];

                            if (tile.State == TileState.Flagged) tile.Image = "TileFlag";

                        }
                    }
                    break;
                case Key.S:
                    if (SBar.Visibility == Visibility.Visible)
                    {
                        Height -= SBar.ActualHeight;
                        SBar.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        SBar.Visibility = Visibility.Visible;
                        Height += SBar.ActualHeight;
                    }
                    break;
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnMinButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

        }
    }
}
