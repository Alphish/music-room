using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.Model
{
    using FilePath = System.IO.Path;

    /// <summary>
    /// Represents a Music Room playlist.
    /// </summary>
    public class Playlist : BasePlaylistContainer
    {
        public Playlist()
        {
            Name = "Playlist";

            // by default, the playlist saving location is assumed to be in the same directory as Music Room executable
            var exePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            Path = FilePath.GetDirectoryName(exePath) + FilePath.DirectorySeparatorChar + "playlist.mrpl";
        }

        /// <summary>
        /// Gets or sets the path to the playlist item, relative or absolute.
        /// </summary>
        public override string Path
        {
            get => base.Path;
            set
            {
                var previous = base.Path;
                base.Path = value;

                // reverting if the new path is invalid
                if (!Directory.Exists(FilePath.GetDirectoryName(this.GetFullPath())))
                    base.Path = previous;
            }
        }

        /// <summary>
        /// Indicates that the playlist includes a filename at the end of its path.
        /// </summary>
        public override bool IncludesFilename => true;

        /// <summary>
        /// Saves the playlist at the given location.
        /// </summary>
        /// <param name="path">The path to save the playlist at.</param>
        public void Save(string path)
            => Playlist_IO.Save(this, path);

        /// <summary>
        /// Loads a playlist from a given path.
        /// </summary>
        /// <param name="path">The path to load the playlist from.</param>
        /// <returns>The loaded playlist.</returns>
        public static Playlist Load(string path)
            => Playlist_IO.Load(path);
    }
}
