using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.MusicRoom.Model;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Provides a base implementation of the IPlaylistItemViewModel interface.
    /// </summary>
    public abstract class BasePlaylistItemViewModel : NotifyPropertyChanged, IPlaylistItemViewModel
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name
            { get => Model.Name; set => Set(Model, nameof(Name), value); }

        /// <summary>
        /// Gets or sets the path to the item, relative to the parent or absolute.
        /// </summary>
        public string Path
        {
            get => Model.Path;
            set
            {
                if (Model.Path != value)
                {
                    Model.Path = value;
                    Notify(nameof(Path));
                    Notify(nameof(FullPath));
                }
            }
        }

        /// <summary>
        /// Gets the absolute path to the item.
        /// </summary>
        public string FullPath
            { get => Model.GetFullPath(); }

        /// <summary>
        /// Gets the underlying model item.
        /// </summary>
        public abstract IPlaylistItem Model { get; }
    }
}
