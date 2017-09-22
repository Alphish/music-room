using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.Ston;

namespace Alphicsh.MusicRoom.Model
{
    /// <summary>
    /// Provides extension methods to various playlist item classes.
    /// </summary>
    public static class IPlaylistItem_Extensions
    {
        /// <summary>
        /// Gets the full path to the given playlist item.
        /// </summary>
        /// <param name="item">The item to get the path of.</param>
        /// <returns>The full path to the item.</returns>
        public static string GetFullPath(this IPlaylistItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var path = item.Path;
            IPlaylistContainer parent = item.Parent;
            while (parent != null)
            {
                if (Path.IsPathRooted(path)) break;
                path = Path.Combine(parent.IncludesFilename ? Path.GetDirectoryName(parent.Path) : parent.Path, path);
                parent = parent.Parent;
            }

            // when used, the playlist items must be rooted *somewhere*
            // otherwise the files they correspond to cannot be located
            if (!Path.IsPathRooted(path))
                throw new InvalidOperationException("The full path couldn't be retrieved. The root playlist must always have a rooted path.");

            return Path.GetFullPath(path);
        }
    }
}
