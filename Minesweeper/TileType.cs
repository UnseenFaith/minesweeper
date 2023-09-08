using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public enum TileType
    {
        /// <summary>
        /// The tile is empty
        /// </summary>
        Empty,

        /// <summary>
        /// The tile is numbered, representing adjacent mines
        /// </summary>
        Numbered,

        /// <summary>
        /// The tile is a mine
        /// </summary>
        Mine
    }
}
