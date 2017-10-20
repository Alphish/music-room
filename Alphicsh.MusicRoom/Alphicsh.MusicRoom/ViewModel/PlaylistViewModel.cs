using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.MusicRoom.Model;

namespace Alphicsh.MusicRoom.ViewModel
{
    public sealed class PlaylistViewModel : BasePlaylistContainerViewModel
    {
        /// <summary>
        /// Creates a playlist view model from a given playlist model.
        /// </summary>
        /// <param name="playlist">The playlist model to wrap.</param>
        public PlaylistViewModel(Playlist playlist)
            : base(playlist) { }

        /// <summary>
        /// Loads a playlist view model from a given file.
        /// </summary>
        /// <param name="path">The path to load the playlist from.</param>
        public PlaylistViewModel(string path)
            : base(Playlist.Load(path)) { }

        // the underlying playlist, of course
        private Playlist InnerPlaylist => InnerContainer as Playlist;

        /// <summary>
        /// Saves the playlist at the given location.
        /// </summary>
        /// <param name="path">The path to save the playlist at.</param>
        public void Save(string path)
            => InnerPlaylist.Save(path);
    }
}
