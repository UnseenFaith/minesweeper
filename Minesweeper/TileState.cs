using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public enum TileState
    {
        /// <summary>
        /// The tile is in a closed state
        /// </summary>
        Closed,

        /// <summary>
        /// The tile is in an opened state
        /// </summary>
        Opened,

        /// <summary>
        /// The tile is in a flagged state
        /// </summary>
        Flagged,

        /// <summary>
        /// The tile is an exploded state
        /// </summary>
        Exploded
    }
}
