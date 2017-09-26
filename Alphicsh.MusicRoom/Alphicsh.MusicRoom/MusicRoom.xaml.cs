using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.WindowsAPICodePack.Dialogs;

using Alphicsh.Audio.Streaming;
using Alphicsh.MusicRoom.DataContext;
using Alphicsh.MusicRoom.Model;
using Alphicsh.MusicRoom.View;
using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MusicRoom : Window
    {
        private MusicRoomDataContext Context { get; } = new MusicRoomDataContext();

        public MusicRoom()
        {
            DataContext = Context;
            InitializeComponent();
        }

        // disposing of the application resources
        // among other things, it includes closing the player if it plays
        // so that wave output isn't left hanging and crashing
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Context.Dispose();
        }

        #region Files menu

        // adding new tracks
        private void AddTracksButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog()
            {
                Multiselect = true,
                Title = "Add tracks",
            };

            dialog.Filters.Add(new CommonFileDialogFilter("All supported formats", "*.flac;*.mp3;*.ogg;*.wav"));
            dialog.Filters.Add(new CommonFileDialogFilter("Free Lossless Audio Codec", "*.flac"));
            dialog.Filters.Add(new CommonFileDialogFilter("MPEG layer 3", "*.mp3"));
            dialog.Filters.Add(new CommonFileDialogFilter("Ogg Vorbis", "*.ogg"));
            dialog.Filters.Add(new CommonFileDialogFilter("Waveform Audio", "*.wav"));

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                foreach (var filename in dialog.FileNames)
                    Context.Playlist.Add(new TrackViewModel(filename));
            }
        }

        // loading a previously saved playlist
        private void LoadPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog()
            {
                Title = "Load playlist",
            };
            dialog.Filters.Add(new CommonFileDialogFilter("Music Room Playlist", "*.mrpl"));

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                Context.Playlist = new PlaylistViewModel(dialog.FileName);
        }

        // saving the current playlist
        private void SavePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonSaveFileDialog()
            {
                Title = "Save playlist",
            };
            dialog.Filters.Add(new CommonFileDialogFilter("Music Room Playlist", "*.mrpl"));

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                Context.Playlist.Save(dialog.FileName);
        }

        #endregion

        #region Playlist area

        // updating the selected items list in the view model
        // apparently, ListBox SelectedItems can't be easily bound to
        // so I'll just operate on these manually
        // what could possibly go wrong...
        private void PlaylistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => Context.SelectedItems = (sender as ListBox).SelectedItems.Cast<IPlaylistItemViewModel>().ToList();

        // editing the currently selected track
        private void PlaylistItem_EditMenu_Click(object sender, RoutedEventArgs e)
        {
            var item = Context.SelectedItems.OfType<TrackViewModel>().FirstOrDefault();
            if (item != null)
            {
                var window = new TrackEditWindow(Context, item);
                window.ShowDialog();
            }
        }

        // removing the currently selected tracks
        private void PlaylistItem_DeleteMenu_Click(object sender, RoutedEventArgs e)
            => Context.Playlist.Remove(Context.SelectedItems.Cast<IPlaylistItemViewModel>());

        #endregion

        #region Playback controls

        // playing the currently selected track (one of these, anyway)
        // if the track is playing already, it's reset
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var item = Context.SelectedItems.OfType<TrackViewModel>().FirstOrDefault();
            if (item != null)
            {
                var track = (item as TrackViewModel).Model as Track;
                Context.Player.Play(track.StreamProvider, track.StreamProvider.CreateStream, true);
            }
        }

        // stopping the currently played track
        private void StopButton_Click(object sender, RoutedEventArgs e)
            => Context.Player.Stop();

        #endregion
    }
}
