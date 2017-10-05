using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

using Alphicsh.MusicRoom.DataContext;
using Alphicsh.MusicRoom.Model;
using Alphicsh.MusicRoom.View;
using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom
{
    using Path = System.IO.Path;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MusicRoom : Window
    {
        public MusicRoom(Playlist playlist)
        {
            var Context = new MusicRoomDataContext(playlist);
            DataContext = Context;
            InitializeComponent();
        }

        private MusicRoomDataContext Context => DataContext as MusicRoomDataContext;

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

        // starting a new playlist
        private void NewPlaylistButton_Click(object sender, RoutedEventArgs e)
            => Context.Playlist = new PlaylistViewModel(new Playlist());

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
                InitialDirectory = Path.GetDirectoryName(Context.Playlist.Path),
                DefaultFileName = Path.GetFileName(Context.Playlist.Path),
            };
            dialog.Filters.Add(new CommonFileDialogFilter("Music Room Playlist", "*.mrpl"));

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                Context.Playlist.Save(dialog.FileName);
        }

        // managing the current playlist
        private void ManagePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new PlaylistEditWindow(Context.Playlist);
            window.ShowDialog();
        }

        #endregion

        #region Playlist selection and dragging

        // used for determining how far the mouse went
        // from the position of clicking
        private Point? DragPoint;

        // indicates whether the list should reselect the items or wait
        private int ReselectionState = 1;

        // stores the possible selection changed caused by the most recent click
        // if the mouse is released before initiating a drag and drop operation
        // the selection will be handled instead
        private IEnumerable<IPlaylistItemViewModel> Preselection;

        // updating the selection variable of the context
        // also, handling reselection shenanigans
        private void PlaylistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // a code-based selection happens, ignore
            if (ReselectionState == -1)
                return;

            // the click might be a drag and drop operation on existing selection, wait
            if (ReselectionState == 0)
            {
                Preselection = PlaylistBox.SelectedItems.Cast<IPlaylistItemViewModel>().ToList();
                SelectPlaylistItems(Context.SelectedItems);
            }
            // the click is definitely a selection, change
            else
            {
                Context.SelectedItems = (sender as ListBox).SelectedItems.Cast<IPlaylistItemViewModel>().ToList();
                Preselection = null;
            }
        }

        // manually selects the playlist box items
        private void SelectPlaylistItems(IEnumerable<IPlaylistItemViewModel> items)
        {
            ReselectionState = -1;
            PlaylistBox.SelectedItems.Clear();
            foreach (var item in items)
                PlaylistBox.SelectedItems.Add(item);
            ReselectionState = 1;
        }

        // stores the dragging point
        // doesn't begin the dragging process yet, so that dragging doesn't begin upon the slightest mouse move
        // also, if the clicked item is already selected, suppresses the selection change
        private void PlaylistItem_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            DragPoint = e.GetPosition(this);

            var element = sender as ListBoxItem;
            var item = PlaylistBox.ItemContainerGenerator.ItemFromContainer(element) as IPlaylistItemViewModel;

            // suppresses the selection change
            if (Context.SelectedItems.Contains(item))
                ReselectionState = 0;
        }

        // resets the dragging point
        // also, performs the stored selection if one is available
        private void MusicRoom_MouseUp(object sender, MouseEventArgs e)
        {
            if (DragPoint != null && Preselection != null)
            {
                SelectPlaylistItems(Preselection);
                Context.SelectedItems = Preselection;
                Preselection = null;
            }

            DragPoint = null;
            ReselectionState = 1;
        }

        // starts the drag and drop operation
        // as soon as mouse moves a significant distance outside the clicking point
        private void PlaylistItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (DragPoint == null)
                return;

            var point = e.GetPosition(this);
            double dx = point.X - DragPoint.Value.X;
            double dy = point.Y - DragPoint.Value.Y;

            if (DragPoint != null && dx * dx + dy * dy >= 25)
            {
                DragPoint = null;
                DragDrop.DoDragDrop(PlaylistBox, new DataObject(typeof(IEnumerable<IPlaylistItemViewModel>), Context.SelectedItems), DragDropEffects.Move);
            }
        }

        // drops the selected items at a specific position
        private void PlaylistBox_Drop(object sender, DragEventArgs e)
        {
            var dropPoint = e.GetPosition(PlaylistBox);

            var element = GetAncestorElement<ListBoxItem>(PlaylistBox.InputHitTest(dropPoint) as DependencyObject, PlaylistBox);
            int index;

            if (element == null)
                index = PlaylistBox.Items.Count;
            else
                index = PlaylistBox.ItemContainerGenerator.IndexFromContainer(element) + (e.GetPosition(element).Y > element.ActualHeight / 2 ? 1 : 0);

            var items = e.Data.GetData(typeof(IEnumerable<IPlaylistItemViewModel>));
            Context.Playlist.Move(index, items as IEnumerable<IPlaylistItemViewModel>);
        }

        // retrieves the innermost ancestor element of a specific type
        // the scope element may be provided to end the search earlier
        // if the innermost ancestor contains the scope, it won't be returned
        private T GetAncestorElement<T>(DependencyObject element, DependencyObject scope = null) where T : DependencyObject
        {
            Type type = typeof(T);

            while (element != null)
            {
                element = VisualTreeHelper.GetParent(element) ?? (element as FrameworkElement)?.Parent as DependencyObject;

                if (element == scope)
                    return null;

                if (element is T)
                    return element as T;
            }
            return null;
        }

        #endregion

        #region Playlist item context menu

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
        private void PlaylistItem_DoubleClick(object sender, RoutedEventArgs e)
        {
            var item = Context.SelectedItems.OfType<TrackViewModel>().FirstOrDefault();
            if (item != null)
            {
                Context.Player.SelectedTrack = item;
                try
                {
                    Context.Player.Play();
                }
                catch (IOException ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    DragPoint = null;
                    Preselection = null;
                    MessageBox.Show($"Could not find track at the following path:\n{item.FullPath}\n\nRight-click on the track and edit it to change its location.");
                }
            }
        }

        #endregion
    }
}
