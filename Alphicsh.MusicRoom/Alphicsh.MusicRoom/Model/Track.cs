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
        public Track() { }

        /// <summary>
        /// Creates a copy of a given track.
        /// </summary>
        /// <param name="track">The track to copy the information of.</param>
        public Track(Track track)
        {
            Name = track.Name;
            Path = track.Path;
            StreamProvider = track.StreamProvider.Copy();

            Parent = track.Parent;
            foreach (var container in track.Containers)
            {
                Containers.Add(container);
            }
        }

        /// <summary>
        /// Gets or sets the stream provider that creates a playable stream appropriate for the track.
        /// </summary>
        public IStreamProvider StreamProvider { get; set; }
    }
}
