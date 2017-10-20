using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.Model
{
    /// <summary>
    /// Represents a playlist item.
    /// </summary>
    public interface IPlaylistItem
    {
        /// <summary>
        /// Gets or sets the name of the playlist item.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the path to the playlist item, relative or absolute.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets the playlist container with the playlist item.
        /// </summary>
        IPlaylistContainer Parent { get; set; }

        /// <summary>
        /// Tracks the containers the item appears in.
        /// The collection should be changed from the playlist container only.
        /// </summary>
        ICollection<IPlaylistContainer> Containers { get; }
    }
}
