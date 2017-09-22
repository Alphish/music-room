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
using Alphicsh.MusicRoom.Model;
using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MusicRoom : Window
    {
        MusicRoomViewModel ViewModel { get; } = new MusicRoomViewModel();

        public MusicRoom()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }

        // disposing of the application resources
        // among other things, it includes closing the player if it plays
        // so that wave output isn't left hanging and crashing
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            ViewModel.Dispose();
        }

        #region Playlist controls

        // adding new tracks
        private void AddButton_Click(object sender, RoutedEventArgs e)
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
                    ViewModel.Playlist.Add(new TrackViewModel(filename));
            }
        }

        // removing the currently selected tracks
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
            => ViewModel.Playlist.Remove(ViewModel.SelectedItems.Cast<IPlaylistItemViewModel>());

        // playing the currently selected track (one of these, anyway)
        // if the track is playing already, it's reset
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ViewModel.SelectedItems.OfType<TrackViewModel>().First();
            if (item != null)
            {
                var track = (item as TrackViewModel).Model as Track;
                ViewModel.Player.Play(track, track.StreamProvider.CreateStream, true);
            }
        }

        // stopping the currently played track
        private void StopButton_Click(object sender, RoutedEventArgs e)
            => ViewModel.Player.Stop();

        #endregion

        // updating the selected items list in the view model
        // apparently, ListBox SelectedItems can't be easily bound to
        // so I'll just operate on these manually
        // what could possibly go wrong...
        private void PlaylistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ViewModel.SelectedItems = (sender as ListBox).SelectedItems.Cast<IPlaylistItemViewModel>().ToList();
    }
}
