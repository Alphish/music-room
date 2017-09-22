using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.Model
{
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
            Path = System.IO.Path.GetDirectoryName(exePath) + System.IO.Path.DirectorySeparatorChar + "playlist.mrpl";
        }

        /// <summary>
        /// Indicates that the playlist includes a filename at the end of its path.
        /// </summary>
        public override bool IncludesFilename => true;
    }
}
