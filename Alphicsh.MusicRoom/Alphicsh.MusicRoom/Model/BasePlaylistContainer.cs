using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.MusicRoom.Model
{
    /// <summary>
    /// Provides a base implementation of the IPlaylistContainer interface.
    /// </summary>
    public abstract class BasePlaylistContainer : BasePlaylistItem, IPlaylistContainer
    {
        // the underlying list of playlist items
        private IList<IPlaylistItem> Items { get; } = new List<IPlaylistItem>();

        #region IPlaylistContainer implementation

        /// <summary>
        /// Indicates that the container path doesn't have filename at the end of its path.
        /// </summary>
        public virtual bool IncludesFilename => false;

        #endregion

        #region IEnumerable<IPlaylistItem> implementation

        /// <summary>
        /// Returns an enumerator that iterates through the container.
        /// </summary>
        /// <returns>The container enumerator.</returns>
        public IEnumerator<IPlaylistItem> GetEnumerator()
            => Items.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the container.
        /// </summary>
        /// <returns>The container enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => Items.GetEnumerator();

        #endregion

        #region ICollection<IPlaylistItem> implementation

        /// <summary>
        /// Gets the number of contained playlist items.
        /// </summary>
        public int Count
            => Items.Count;

        /// <summary>
        /// Gets a value indicating whether the container is read-only.
        /// </summary>
        public bool IsReadOnly
            => Items.IsReadOnly;

        /// <summary>
        /// Determines whether the container contains a specific item.
        /// </summary>
        /// <param name="item">The item to find in the container.</param>
        /// <returns>true if the container contains the item, false otherwise</returns>
        public bool Contains(IPlaylistItem item)
            => Items.Contains(item);

        /// <summary>
        /// Adds an item to the container.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(IPlaylistItem item)
        {
            RegisterItem(item);
            Items.Add(item);
        }

        /// <summary>
        /// Removes the first occurrence of an item in the container.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>true if the item has been removed, false otherwise</returns>
        public bool Remove(IPlaylistItem item)
        {
            UnregisterItem(item);
            return Items.Remove(item);
        }

        /// <summary>
        /// Removes all items from the container.
        /// </summary>
        public void Clear()
        {
            foreach (var item in Items)
                UnregisterItem(item);

            Items.Clear();
        }

        /// <summary>
        /// Copies the playlist items to an array, starting at a specified array index.
        /// </summary>
        /// <param name="array">The array to copy the items to.</param>
        /// <param name="arrayIndex">The index at which the copying should begin.</param>
        public void CopyTo(IPlaylistItem[] array, int arrayIndex)
            => Items.CopyTo(array, arrayIndex);

        #endregion

        #region IList<IPlaylistItem> implementation

        /// <summary>
        /// Gets or sets the playlist item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get or set.</param>
        /// <returns>The playlist element at the specified index.</returns>
        public IPlaylistItem this[int index]
        {
            get => Items[index];
            set
            {
                if (value == Items[index]) return;
                UnregisterItem(Items[index]);
                RegisterItem(value);
                Items[index] = value;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the container.
        /// </summary>
        /// <param name="item">The item to find in the playlist.</param>
        /// <returns>The index of the item, or -1 if the item couldn't be found.</returns>
        public int IndexOf(IPlaylistItem item)
            => Items.IndexOf(item);

        /// <summary>
        /// Inserts an item to the container at the specified index.
        /// </summary>
        /// <param name="index">The index where the item should be inserted.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, IPlaylistItem item)
        {
            RegisterItem(item);
            Items.Insert(index, item);
        }

        /// <summary>
        /// Removes the playlist item at the specified index.
        /// </summary>
        /// <param name="index">The index where the item should be removed.</param>
        public void RemoveAt(int index)
        {
            UnregisterItem(Items[index]);
            Items.RemoveAt(index);
        }

        #endregion

        #region Playlist-specific functionality

        // adds the item-side link to the container
        private void RegisterItem(IPlaylistItem item)
        {
            item.Containers.Add(this);
            item.Parent = item.Parent ?? this;
        }

        // removes the item-side link to the container
        private void UnregisterItem(IPlaylistItem item)
        {
            item.Containers.Remove(this);
            if (item.Containers.Contains(item.Parent)) item.Parent = item.Containers.FirstOrDefault();
        }

        #endregion
    }
}
