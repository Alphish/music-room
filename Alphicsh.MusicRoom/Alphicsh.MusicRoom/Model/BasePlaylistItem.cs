using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.Model
{
    /// <summary>
    /// Provides a base implementation of the IPlaylistItem interface.
    /// </summary>
    public abstract class BasePlaylistItem : IPlaylistItem
    {
        /// <summary>
        /// Gets or sets the name of the playlist item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path to the playlist item, relative or absolute.
        /// </summary>
        public virtual string Path
        {
            get => _Path;
            set => _Path = value ?? throw new ArgumentNullException("Playlist item path cannot be null.");
        }
        private string _Path = "";

        /// <summary>
        /// Gets or sets the primary playlist container with the playlist item.
        /// In general, it should be one of containers elements.
        /// </summary>
        public IPlaylistContainer Parent { get; set; }

        /// <summary>
        /// Tracks the containers the item appears in.
        /// Be careful when modifying.
        /// </summary>
        public ICollection<IPlaylistContainer> Containers { get; } = new List<IPlaylistContainer>();
    }
}
