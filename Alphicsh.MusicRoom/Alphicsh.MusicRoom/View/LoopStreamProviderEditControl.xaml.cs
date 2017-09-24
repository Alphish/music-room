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
    using RangeLock = LoopStreamProviderViewModel.RangeLock;

    /// <summary>
    /// Interaction logic for LoopStreamProviderEditControl.xaml
    /// </summary>
    public partial class LoopStreamProviderEditControl : UserControl
    {
        public LoopStreamProviderEditControl()
        {
            InitializeComponent();
        }

        // the loop stream provider being edited
        private LoopStreamProviderViewModel Provider => DataContext as LoopStreamProviderViewModel;

        #region Loop section events

        private void LockLoopStartButton_Click(object sender, RoutedEventArgs e)
            => Provider.LoopLock = ToggleLock(Provider.LoopLock, RangeLock.Start);

        private void LockLoopLengthButton_Click(object sender, RoutedEventArgs e)
            => Provider.LoopLock = ToggleLock(Provider.LoopLock, RangeLock.Length);

        private void LockLoopEndButton_Click(object sender, RoutedEventArgs e)
            => Provider.LoopLock = ToggleLock(Provider.LoopLock, RangeLock.End);

        #endregion

        #region Track section events

        private void LockTrackStartButton_Click(object sender, RoutedEventArgs e)
            => Provider.TrackLock = ToggleLock(Provider.TrackLock, RangeLock.Start);

        private void LockTrackLengthButton_Click(object sender, RoutedEventArgs e)
            => Provider.TrackLock = ToggleLock(Provider.TrackLock, RangeLock.Length);

        private void LockTrackEndButton_Click(object sender, RoutedEventArgs e)
            => Provider.TrackLock = ToggleLock(Provider.TrackLock, RangeLock.End);

        #endregion

        #region Helper methods

        private RangeLock ToggleLock(RangeLock currentValue, RangeLock lockValue)
            => currentValue == lockValue ? RangeLock.None : lockValue;

        #endregion
    }
}
