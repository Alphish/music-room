﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.MusicRoom.Model;
using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom.DataContext
{
    /// <summary>
    /// Provides a context for the entire application.
    /// </summary>
    public class MusicRoomDataContext : NotifyPropertyChanged, IDisposable
    {
        public MusicRoomDataContext(Playlist playlist)
        {
            _Playlist = new PlaylistViewModel(playlist);
        }

        /// <summary>
        /// Gets the music player instance.
        /// </summary>
        public PlayerViewModel Player { get; } = new PlayerViewModel();

        /// <summary>
        /// Gets or sets the list of selected items.
        /// </summary>
        public IEnumerable<IPlaylistItemViewModel> SelectedItems
            { get => _SelectedItems; set => Set(nameof(SelectedItems), value); }
        private IEnumerable<IPlaylistItemViewModel> _SelectedItems = new IPlaylistItemViewModel[0];

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
