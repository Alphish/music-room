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

        /// <summary>
        /// Gets the relative path to the given playlist item, using its parent full path as a reference.
        /// </summary>
        /// <param name="item">The item to get the relative path of.</param>
        /// <returns>The path relative to the parent's full path.</returns>
        public static string GetRelativePath(this IPlaylistItem item)
        {
            if (item.Parent == null) return item.Path;
            if (item.Path == "") return item.Path;

            var parentPath = item.Parent.GetFullPath();
            if (item.Parent.IncludesFilename) parentPath = Path.GetDirectoryName(parentPath);
            var ownPath = Path.GetFullPath(Path.Combine(parentPath, item.Path));

            var parentSegments = parentPath.Split(Path.DirectorySeparatorChar).ToList();
            var ownSegments = ownPath.Split(Path.DirectorySeparatorChar).ToList();

            // if the playlist item and parent are rooted at different places
            // the only possible relative path is item's own path
            if (parentSegments.First() != ownSegments.First())
                return ownPath;

            // going through the common segments
            while (parentSegments.Any() && ownSegments.Any() && parentSegments.First() == ownSegments.First())
            {
                parentSegments.RemoveAt(0);
                ownSegments.RemoveAt(0);
            }

            return string.Join("", Enumerable.Repeat(".." + Path.DirectorySeparatorChar, parentSegments.Count)) + string.Join(Path.DirectorySeparatorChar.ToString(), ownSegments);
        }
    }
}
