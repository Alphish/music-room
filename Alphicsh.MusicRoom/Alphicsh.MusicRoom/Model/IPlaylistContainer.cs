using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.Model
{
    /// <summary>
    /// Represents a playlist item that lists other items.
    /// </summary>
    public interface IPlaylistContainer : IPlaylistItem, IList<IPlaylistItem>
    {
        /// <summary>
        /// Indicates whether the container path includes a filename at the end or not.
        /// </summary>
        bool IncludesFilename { get; }
    }
}
