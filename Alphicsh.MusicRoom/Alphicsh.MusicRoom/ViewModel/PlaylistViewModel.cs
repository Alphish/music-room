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
        {
            InnerPlaylist = playlist;
        }

        // the underlying playlist, of course
        private Playlist InnerPlaylist { get; }

        // this is the underlying playlist, too
        protected override IPlaylistContainer InnerContainer => InnerPlaylist;

        /// <summary>
        /// Gets the underlying playlist model.
        /// </summary>
        public override IPlaylistItem Model => InnerPlaylist;
    }
}
