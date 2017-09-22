using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.MusicRoom.Model;
using System.ComponentModel;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Acts as a view model for a soundtrack.
    /// </summary>
    public class TrackViewModel : BasePlaylistItemViewModel
    {
        /// <summary>
        /// Creates a new track view model from a given model track.
        /// </summary>
        /// <param name="track">The inner model track.</param>
        public TrackViewModel(Track track)
        {
            InnerTrack = track;
        }

        /// <summary>
        /// Creates a basic track view model based on a file at a given path.
        /// A complete track loop is used by default.
        /// </summary>
        /// <param name="path">The path of the soundtrack file.</param>
        public TrackViewModel(string path)
        {
            var file = new FileInfo(path);
            InnerTrack = new Track();
            InnerTrack.Path = file.FullName;
            InnerTrack.Name = System.IO.Path.GetFileNameWithoutExtension(InnerTrack.Path);

            InnerTrack.StreamProvider = new LoopStreamProvider(InnerTrack);
        }

        // the soundtrack file used
        private Track InnerTrack { get; }

        /// <summary>
        /// Gets the underlying soundtrack model.
        /// </summary>
        public override IPlaylistItem Model => InnerTrack;

        /// <summary>
        /// Represents a soundtrack as a string with its assigned name.
        /// </summary>
        /// <returns>The name of the soundtrack.</returns>
        public override string ToString() => Name;
    }
}
