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
using System.Windows.Shapes;

using Alphicsh.MusicRoom.DataContext;
using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom.View
{
    /// <summary>
    /// Interaction logic for TrackEditWindow.xaml
    /// </summary>
    public partial class TrackEditWindow : Window
    {
        private TrackEditWindowDataContext Context;
        
        public TrackEditWindow(MusicRoomDataContext mainContext, TrackViewModel viewModel)
        {
            Context = new TrackEditWindowDataContext(mainContext, viewModel);
            DataContext = Context;
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            TrackViewModel source, copy;
            source = Context.SourceTrack;
            copy = Context.CopyTrack;
            source.Name = copy.Name;
            source.Path = copy.Path;
            if (copy.StreamProvider.IsValid)
                source.StreamProvider = copy.StreamProvider;
            else
                MessageBox.Show("The loop provided is not valid. The previous loop will remain.");

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
