using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Represents a view model of a playlist container, with added collection changes notifications features.
    /// </summary>
    interface IPlaylistContainerViewModel : IPlaylistItemViewModel, IList<IPlaylistItemViewModel>, INotifyCollectionChanged
    {
    }
}
