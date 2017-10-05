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

using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom.View
{
    using PlaybackState = NAudio.Wave.PlaybackState;

    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        public PlayerControl()
        {
            InitializeComponent();
        }

        private PlayerViewModel Player => DataContext as PlayerViewModel;

        // attempts to play the currently selected track
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.State == PlaybackState.Playing)
                Player.Pause();
            else if (Player.State == PlaybackState.Paused)
                Player.Resume();
            else
            {
                try
                {
                    Player.Play();
                }
                catch (System.IO.IOException ex) when (ex is System.IO.FileNotFoundException || ex is System.IO.DirectoryNotFoundException)
                {
                    MessageBox.Show($"Could not find track at the following path:\n{Player.SelectedTrack.FullPath}\n\nRight-click on the track and edit it to change its location.");
                }
            }
        }

        // stops the currently playing track
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
        }

        // toggles the volume between 0 and 1
        // if the volume is neither of these values, it's set to 0
        private void VolumeIcon_Click(object sender, MouseButtonEventArgs e)
            => Player.Volume = Player.Volume > 0 ? 0 : 1;

        // sets the position of the currently played track
        private void PlayerBar_Click(object sender, MouseButtonEventArgs e)
        {
            var playerBar = sender as ProgressBar;
            if (Player.State != PlaybackState.Stopped)
            {
                var position = e.GetPosition(playerBar).X;
                Player.Position = (long)Math.Min(Player.Length-1, Math.Round(Player.Length * (position / playerBar.ActualWidth)));
            }
        }
    }
}
