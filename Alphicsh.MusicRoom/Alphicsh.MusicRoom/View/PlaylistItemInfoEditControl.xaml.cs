using System;
using System.Collections.Generic;
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

using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom.View
{
    /// <summary>
    /// Interaction logic for PlaylistItemInfoEditControl.xaml
    /// </summary>
    public partial class PlaylistItemInfoEditControl : UserControl
    {
        // to be created through XAML
        public PlaylistItemInfoEditControl()
        {
            InitializeComponent();
        }

        // provides the data context as a playlist item
        // the control makes no sense for other times
        private IPlaylistItemViewModel PlaylistItem => DataContext as IPlaylistItemViewModel;

        // selects the track path from an existing file
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog()
            {
                Title = "Select the track",
            };

            dialog.Filters.Add(new CommonFileDialogFilter("All supported formats", "*.flac;*.mp3;*.ogg;*.wav"));
            dialog.Filters.Add(new CommonFileDialogFilter("Free Lossless Audio Codec", "*.flac"));
            dialog.Filters.Add(new CommonFileDialogFilter("MPEG layer 3", "*.mp3"));
            dialog.Filters.Add(new CommonFileDialogFilter("Ogg Vorbis", "*.ogg"));
            dialog.Filters.Add(new CommonFileDialogFilter("Waveform Audio", "*.wav"));

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                PlaylistItem.Path = dialog.FileName;
            }
            Window.GetWindow(this).Activate();
        }

        // sets the stored path to the relative path
        // useful for making shareable playlists
        private void UseRelativePathButton_Click(object sender, RoutedEventArgs e)
            => PlaylistItem.Path = PlaylistItem.RelativePath;
        
        // sets the stored path to the full path
        // makes the track independent from parent container
        // which might be a good or a bad thing, depending on the context
        private void UseFullPathButton_Click(object sender, RoutedEventArgs e)
            => PlaylistItem.Path = PlaylistItem.FullPath;
        
    }
}
