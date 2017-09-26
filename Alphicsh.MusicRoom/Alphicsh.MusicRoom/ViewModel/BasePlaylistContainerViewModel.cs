using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.MusicRoom.Model;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Provides a base implementation of the IPlaylistContainerViewModel interface.
    /// </summary>
    public abstract class BasePlaylistContainerViewModel : BasePlaylistItemViewModel, IPlaylistContainerViewModel
    {
        public BasePlaylistContainerViewModel(IPlaylistContainer innerContainer)
        {
            InnerContainer = innerContainer;
            foreach (var item in InnerContainer)
            {
                switch (item)
                {
                    case Track track:
                        Items.Add(new TrackViewModel(track));
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Gets the underlying playlist container model.
        /// </summary>
        protected IPlaylistContainer InnerContainer { get; }

        /// <summary>
        /// Gets the underlying playlist container model.
        /// </summary>
        public override IPlaylistItem Model => InnerContainer;

        // the list of view-model items corresponding to the model ones
        private IList<IPlaylistItemViewModel> Items = new List<IPlaylistItemViewModel>();

        #region Collection changes

        /// <summary>
        /// Raised when the contents of the collection have changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        // notifies about the contents being drastically changed
        private void CollectionReset()
            => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        #region Adding

        // notifies about the new item being added to the container
        private void CollectionAdded(int index, IPlaylistItemViewModel item)
            => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));

        // notifies about the new items being added to the container
        private void CollectionAdded(int index, IEnumerable<IPlaylistItemViewModel> items)
            => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList(), index));

        /// <summary>
        /// Inserts an item to the container at the specified index.
        /// </summary>
        /// <param name="index">The index where the item should be inserted.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, IPlaylistItemViewModel item)
        {
            Items.Insert(index, item);
            InnerContainer.Insert(index, item.Model);

            Notify(nameof(Count));
            Notify(IndexerName);
            CollectionAdded(index, item);
        }

        /// <summary>
        /// Inserts items to the container at the specified index.
        /// </summary>
        /// <param name="index">The index where the items should be inserted.</param>
        /// <param name="items">The items to insert.</param>
        public void Insert(int index, IEnumerable<IPlaylistItemViewModel> items)
        {
            foreach (var item in items)
            {
                Items.Insert(index, item);
                InnerContainer.Insert(index, item.Model);
            }
            Notify(nameof(Count));
            Notify(IndexerName);
            CollectionAdded(index, items);
        }

        /// <summary>
        /// Adds an item to the container.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(IPlaylistItemViewModel item)
            => Insert(Items.Count, item);

        /// <summary>
        /// Adds items to the container.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void Add(IEnumerable<IPlaylistItemViewModel> items)
            => Insert(Items.Count, items);

        #endregion

        #region Removal

        // notifies about an item being removed from the container
        private void CollectionRemoved(int index, IPlaylistItemViewModel item)
            => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));

        // notifies about items being removed from the container
        private void CollectionRemoved(int index, IEnumerable<IPlaylistItemViewModel> items)
            => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items.ToList(), index));

        /// <summary>
        /// Removes the playlist item at the specified index.
        /// </summary>
        /// <param name="index">The index where the item should be removed.</param>
        public void RemoveAt(int index)
        {
            IPlaylistItemViewModel item = Items[index];
            Items.RemoveAt(index);
            InnerContainer.RemoveAt(index);

            Notify(nameof(Count));
            Notify(IndexerName);
            CollectionRemoved(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of an item in the container.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>true if the item has been removed, false otherwise</returns>
        public bool Remove(IPlaylistItemViewModel item)
        {
            int index = Items.IndexOf(item);
            if (index < 0) return false;

            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes the specified items from the container.
        /// </summary>
        /// <param name="items">The items to remove.</param>
        /// <returns>true if all items have been removed, false otherwise</returns>
        public bool Remove(IEnumerable<IPlaylistItemViewModel> items)
        {
            if (!items.Any())
                return true;

            if (!items.Skip(1).Any())
                return Remove(items.First());

            // setting up useful variables
            var set = new HashSet<IPlaylistItemViewModel>(items);
            IPlaylistItemViewModel item;

            // actually removing the items
            for (var i = Count-1; i >= 0; i--)
            {
                item = this[i];
                if (set.Contains(item))
                {
                    set.Remove(item);
                    Items.RemoveAt(i);
                    InnerContainer.RemoveAt(i);
                }

                if (!set.Any()) break;
            }

            // notification squad strikes again
            Notify(nameof(Count));
            Notify(IndexerName);
            CollectionReset();

            return !set.Any();
        }

        /// <summary>
        /// Removes all items from the container.
        /// </summary>
        public void Clear()
        {
            var items = new List<IPlaylistItemViewModel>(Items);
            Items.Clear();
            InnerContainer.Clear();

            Notify(nameof(Count));
            Notify(IndexerName);
            CollectionReset();
        }

        #endregion

        #region Replacement

        // notifies about the items being replaced in the container
        private void CollectionReplaced(int index, IPlaylistItemViewModel oldItem, IPlaylistItemViewModel newItem)
            => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));

        /// <summary>
        /// Gets or sets the playlist item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get or set.</param>
        /// <returns>The playlist element at the specified index.</returns>
        [System.Runtime.CompilerServices.IndexerName("Item")]
        public IPlaylistItemViewModel this[int index]
        {
            get => Items[index];
            set
            {
                IPlaylistItemViewModel oldValue = Items[index];
                Items[index] = value;
                InnerContainer[index] = value.Model;

                Notify(IndexerName);
                CollectionReplaced(index, oldValue, value);
            }
        }

        // used for indexer change notifications
        private const string IndexerName = "Item[]";

        #endregion

        #endregion

        #region Other IList<IPlaylistItemViewModel> members

        /// <summary>
        /// Gets the number of contained playlist items.
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// Gets a value indicating whether the container is read-only.
        /// </summary>
        public bool IsReadOnly => Items.IsReadOnly;

        /// <summary>
        /// Determines the index of a specific item in the container.
        /// </summary>
        /// <param name="item">The item to find in the playlist.</param>
        /// <returns>The index of the item, or -1 if the item couldn't be found.</returns>
        public int IndexOf(IPlaylistItemViewModel item) => Items.IndexOf(item);

        /// <summary>
        /// Determines whether the container contains a specific item.
        /// </summary>
        /// <param name="item">The item to find in the container.</param>
        /// <returns>true if the container contains the item, false otherwise</returns>
        public bool Contains(IPlaylistItemViewModel item)
            => Items.Contains(item);

        /// <summary>
        /// Copies the playlist items to an array, starting at a specified array index.
        /// </summary>
        /// <param name="array">The array to copy the items to.</param>
        /// <param name="arrayIndex">The index at which the copying should begin.</param>
        public void CopyTo(IPlaylistItemViewModel[] array, int arrayIndex)
            => Items.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the container.
        /// </summary>
        /// <returns>The container enumerator.</returns>
        public IEnumerator<IPlaylistItemViewModel> GetEnumerator()
            => Items.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the container.
        /// </summary>
        /// <returns>The container enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => Items.GetEnumerator();

        #endregion
    }
}
