using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.Model
{
    /// <summary>
    /// Represents a playable soundtrack.
    /// </summary>
    public class Track : BasePlaylistItem
    {
        /// <summary>
        /// Gets or sets the stream provider that creates a playable stream appropriate for the track.
        /// </summary>
        public IStreamProvider StreamProvider { get; set; }
    }
}
