using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.MusicRoom.ViewModel;

namespace Alphicsh.MusicRoom.DataContext
{
    /// <summary>
    /// Provides a context for the soundtrack editing window.
    /// </summary>
    public class TrackEditWindowDataContext : NotifyPropertyChanged
    {
        /// <summary>
        /// Creates a soundtrack editing window, given the application context and edited soundtrack.
        /// </summary>
        /// <param name="mainContext">The application context.</param>
        /// <param name="track">The soundtrack to be edited.</param>
        public TrackEditWindowDataContext(MusicRoomDataContext mainContext, TrackViewModel track)
        {
            MainContext = mainContext;
            SourceTrack = track;
            CopyTrack = new TrackViewModel(track);
        }

        // the stored main application context
        private MusicRoomDataContext MainContext;

        // the track being edited
        public TrackViewModel SourceTrack { get; }

        // the working copy of the track
        // storing the changes before they are applied
        public TrackViewModel CopyTrack { get; }
    }
}
