using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Acts as the view model of the entire application.
    /// </summary>
    public class MusicRoomViewModel : NotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Creates a default application view model.
        /// </summary>
        public MusicRoomViewModel()
        {
            Player = new PlayerViewModel();
            _Playlist = new PlaylistViewModel(new Model.Playlist() { Name = "Playlist", Path = ""} );
        }

        /// <summary>
        /// Gets the music player instance.
        /// </summary>
        public PlayerViewModel Player { get; }

        /// <summary>
        /// Gets or sets the list of selected items.
        /// </summary>
        public IEnumerable<IPlaylistItemViewModel> SelectedItems
            { get => _SelectedItems; set => Set(nameof(SelectedItems), value); }
        private IEnumerable<IPlaylistItemViewModel> _SelectedItems = null;

        /// <summary>
        /// Gets or sets the current playlist.
        /// </summary>
        public PlaylistViewModel Playlist
            { get => _Playlist; set => Set(nameof(Playlist), value); }
        private PlaylistViewModel _Playlist;

        /// <summary>
        /// Frees the resources used in application.
        /// Should be used only upon closing the application.
        /// </summary>
        public void Dispose()
        {
            Player.Dispose();
        }
    }
}
