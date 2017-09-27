using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

using Microsoft.WindowsAPICodePack.Dialogs;

using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom.View
{
    using Path = System.IO.Path;

    /// <summary>
    /// Interaction logic for PlaylistEditWindow.xaml
    /// </summary>
    public partial class PlaylistEditWindow : Window
    {
        public PlaylistEditWindow(PlaylistViewModel playlist)
        {
            DataContext = playlist;
            InitializeComponent();
        }

        private PlaylistViewModel Playlist => DataContext as PlaylistViewModel;

        // choosing a save location for the playlist
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonSaveFileDialog()
            {
                Title = "Choose playlist location",
                InitialDirectory = Path.GetDirectoryName(Playlist.Path),
                DefaultFileName = Path.GetFileName(Playlist.Path),
            };
            dialog.Filters.Add(new CommonFileDialogFilter("Music Room Playlist", "*.mrpl"));

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                Playlist.Path = dialog.FileName;

            Activate();
        }

        // making the entire playlist shareable
        // more specifically, finding the innermost common ancestor for all playlist items
        // setting the playlist save location to that ancestor
        // and updating each item with a relative path that would preserve the full path
        private void MakeShareableButton_Click(object sender, RoutedEventArgs e)
        {
            // finding the full paths to all elements the playlist contains
            var allPaths = new Dictionary<IPlaylistItemViewModel, string>();
            string commonString = null;

            foreach (var item in Playlist)
            {
                if (!AddToPath(item, Path.GetDirectoryName(Playlist.Path)))
                    break;
            }

            // trying to update the paths, if shareable playlist is possible
            if (commonString == null)
                MessageBox.Show("There are no items to make the shareable playlist of.");
            else if (commonString == "")
                MessageBox.Show("The playlist items are rooted in multiple locations. No shareable playlist could be created.");
            else
            {
                // confirmation that the user really wants to update the playlist
                string newPath = commonString + Path.GetFileName(Playlist.Path);
                var result = MessageBox.Show($"All playlist items will have paths relative to the main playlist.\n\nThe playlist will be located at:\n{newPath}\n\nThis action cannot be undone. Proceed?", "Make shareable playlist", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // actually updating the playlist
                    Playlist.Path = newPath;
                    int len = commonString.Length;

                    foreach (var item in Playlist)
                        UpdatePaths(item, commonString);
                }
            }

            /*******************
             * INNER FUNCTIONS *
             *******************/ 

            // building the list of full paths for each item
            // if the common path becomes empty, returns false
            bool AddToPath(IPlaylistItemViewModel item, string basePath)
            {
                // comparing own path with the rest
                string ownPath = Path.GetFullPath(Path.Combine(basePath, item.Path));
                UpdateCommonString(ownPath);
                if (commonString == "")
                    return false;
                allPaths.Add(item, ownPath);

                // adding contained paths
                if (item is IPlaylistContainerViewModel)
                {
                    string ownBasePath = (item.Model as Model.IPlaylistContainer).IncludesFilename ? Path.GetDirectoryName(ownPath) : ownPath;
                    foreach (var subitem in (item as IPlaylistContainerViewModel))
                        AddToPath(subitem, ownBasePath);
                }
                return true;
            }

            // updating the common string shared among paths
            void UpdateCommonString(string newPath)
            {
                if (commonString == null)
                {
                    commonString = Path.GetDirectoryName(newPath) + Path.DirectorySeparatorChar;
                    return;
                }

                int len = Math.Min(commonString.Length, newPath.Length);
                int cut = 0;
                int i = 0;
                for (; i < len; i++)
                {
                    if (commonString[i] != newPath[i])
                        break;

                    if (commonString[i] == Path.DirectorySeparatorChar)
                        cut = i+1;
                }

                // ensuring that a separator char of difference doesn't cause a path to go a directory up
                // for example, a common path of "C:\Music\" and a new path of "C:\Music"
                // should result in common path staying "C:\Music\", not becoming "C:\"
                if (i == len && commonString.Length <= len + 1)
                    return;

                commonString = commonString.Remove(cut);
            }

            // updating the paths in items to be relative instead of absolute
            void UpdatePaths(IPlaylistItemViewModel item, string basePath)
            {
                string fullPath = allPaths[item];
                if (fullPath.Length > basePath.Length)
                    item.Path = fullPath.Substring(basePath.Length);

                if (item is IPlaylistContainerViewModel)
                {
                    string ownBasePath = (item.Model as Model.IPlaylistContainer).IncludesFilename ? Path.GetDirectoryName(fullPath) : fullPath;
                    foreach (var subitem in (item as IPlaylistContainerViewModel))
                        UpdatePaths(subitem, ownBasePath + Path.DirectorySeparatorChar);
                }
            }
        }

        // making the entire playlist portable
        // more specifically, setting paths of direct playlist items to their full paths
        private void MakePortableButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"All items directly contained in the playlist will have absolute paths.\n\nThis action cannot be undone. Proceed?", "Make portable playlist", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                foreach (var item in Playlist)
                    item.Path = Path.GetFullPath(Path.Combine(Playlist.Path, item.Path));
            }
        }
    }
}
