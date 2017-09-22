using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.MusicRoom.Model;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Represents a view model of a playlist item.
    /// </summary>
    public interface IPlaylistItemViewModel
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets own part of path of the playlist item.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Retrieves the full path of the playlist item.
        /// </summary>
        string FullPath { get; }

        /// <summary>
        /// Gets the underlying model playlist item.
        /// </summary>
        IPlaylistItem Model { get; }
    }
}
