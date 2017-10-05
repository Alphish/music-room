using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.Ston;


namespace Alphicsh.MusicRoom.Model
{
    internal static class Playlist_IO
    {
        #region Saving playlists

        /// <summary>
        /// Saves the playlist at the given location.
        /// </summary>
        /// <param name="playlist">The playlist to save.</param>
        /// <param name="path">The path to save the playlist at.</param>
        internal static void Save(Playlist playlist, string path)
        {
            playlist.ToSton().SaveCanonicalForm(path);
            playlist.Path = path;
        }

        // builds a STON entity from a playlist item
        private static IStonEntity ToSton(this IPlaylistItem item)
        {
            string typeName;
            var properties = new List<KeyValuePair<IStonBindingKey, IStonEntity>>();
            IEnumerable<IStonEntity> subitems = null;

            properties.Add(MakeStonProperty("Name", MakeStonSimpleEntity(item.Name)));

            // the path of the playlist depends on its filesystem location
            // and thus is excluded from playlist item properties
            if (!(item is Playlist))
                properties.Add(MakeStonProperty("Path", MakeStonSimpleEntity(item.Path)));

            // preparing subitems collection if the item is container
            if (item is IPlaylistContainer)
                subitems = (item as IPlaylistContainer).Select(i => i.ToSton());

            // determining the type, and adding type specific variables
            switch (item)
            {
                case Playlist playlist:
                    typeName = "Playlist";
                    break;

                case Track track:
                    typeName = "Track";
                    properties.Add(MakeStonProperty("Stream", track.StreamProvider.ToSton()));
                    break;

                default:
                    throw new NotSupportedException();
            }

            // building the result
            return new StonComplexEntity(null, new StonMemberInit(properties), subitems != null ? new StonCollectionInit(subitems) : null, new StonNamedType(typeName));
        }

        // builds a STON entity from a stream provider
        private static IStonEntity ToSton(this IStreamProvider provider)
        {
            // except for tracks, providers have no properties in common
            // so this just switches on each supported provider type
            switch (provider)
            {
                case LoopStreamProvider loopProvider:
                    return loopProvider.ToSton();
                default:
                    throw new NotSupportedException();
            }
        }

        // builds a STON entity from a looping stream provider
        private static IStonEntity ToSton(this LoopStreamProvider provider)
        {
            var properties = new List<KeyValuePair<IStonBindingKey, IStonEntity>>();

            if (provider.TrackStart >= 0)
                properties.Add(MakeStonProperty("TrackStart", MakeStonSimpleEntity(provider.TrackStart)));
            if (provider.StreamLoopStart >= 0)
                properties.Add(MakeStonProperty("LoopStart", MakeStonSimpleEntity(provider.StreamLoopStart)));
            if (provider.StreamLoopEnd >= 0)
                properties.Add(MakeStonProperty("LoopEnd", MakeStonSimpleEntity(provider.StreamLoopEnd)));
            if (provider.TrackEnd >= 0)
                properties.Add(MakeStonProperty("TrackEnd", MakeStonSimpleEntity(provider.TrackEnd)));
            if (provider.Loops >= 0)
                properties.Add(MakeStonProperty("Loops", MakeStonSimpleEntity(provider.Loops)));

            return new StonComplexEntity(
                type: new StonNamedType("Loop"),
                memberInit: new StonMemberInit(properties)
                );
        }

        // general function to create a proper key-value pair of STON properties
        private static KeyValuePair<IStonBindingKey, IStonEntity> MakeStonProperty(string name, IStonEntity entity)
        {
            return new KeyValuePair<IStonBindingKey, IStonEntity>(new StonBindingName(name), entity);
        }

        // general function to create a STON simple entity from a given object
        private static IStonSimpleEntity MakeStonSimpleEntity(object value)
        {
            if (value is int || value is long)
            {
                var lval = (long)value;
                string content;
                if (lval == 0) content = "0";
                else
                {
                    int exponent = 0;
                    while (lval % 10 == 0)
                    {
                        exponent++;
                        lval /= 10;
                    }
                    content = lval.ToString() + "e" + exponent.ToString();
                }
                return new StonSimpleEntity(new StonSimpleValue(StonDataType.Number, content));
            }
            else if (value is string)
            {
                return new StonSimpleEntity(new StonSimpleValue(StonDataType.Text, value.ToString()));
            }
            else throw new NotSupportedException();
        }

        #endregion

        #region Loading playlists

        // loads a playlist from a given path
        internal static Playlist Load(string path)
        {
            var document = RegularStonReader.Default.LoadDocument(path);
            var builtObjects = new Dictionary<IStonValuedEntity, object>();

            var playlist = FromSton<IPlaylistItem>(document, document.Core, builtObjects) as Playlist;
            playlist.Path = path;

            return playlist;
        }

        // reads a usable object from a STON value
        private static TObject FromSton<TObject>(IStonDocument document, IStonValuedEntity entity, IDictionary<IStonValuedEntity, object> builtObjects)
        {
            if (entity == null)
                return default(TObject);

            if (!builtObjects.ContainsKey(entity))
            {
                var obj = CreateFromSton<TObject>(document, entity, builtObjects);
                builtObjects.Add(entity, obj);

                if (entity is IStonComplexEntity)
                    InitializeFromSton(obj, document, entity as IStonComplexEntity, builtObjects);
            }
            return (TObject)builtObjects[entity];
        }

        // creates an object from a STON value
        private static object CreateFromSton<TObject>(IStonDocument document, IStonValuedEntity entity, IDictionary<IStonValuedEntity, object> builtObjects)
        {
            var clrType = typeof(TObject);

            if (clrType == typeof(string))
            {
                return (entity as IStonSimpleEntity).Value.Content;
            }
            else if (clrType == typeof(int) || clrType == typeof(int?) || clrType == typeof(long) || clrType == typeof(long?))
            {
                // miiiight wanna make this a built-in part of STON library at some point
                IStonSimpleValue value = (entity as IStonSimpleEntity).Value;
                int eidx = value.Content.IndexOf('e');
                string sstr = value.Content.Remove(eidx);
                string estr = value.Content.Substring(eidx + 1);

                var result = long.Parse(sstr);
                var exponent = int.Parse(estr);
                while (exponent-- > 0)
                    result *= 10;
                return result;
            }
            else if (clrType == typeof(IPlaylistItem))
            {
                switch ((entity.Type as IStonNamedType).Name)
                {
                    case "Playlist":
                        return new Playlist();
                    case "Track":
                        return new Track();
                    default:
                        throw new NotSupportedException();
                }
            }
            else if (clrType == typeof(IStreamProvider))
            {
                switch ((entity.Type as IStonNamedType).Name)
                {
                    case "Loop":
                        return new LoopStreamProvider(builtObjects[document.GetParentContext(entity)] as Track);
                    default:
                        throw new NotSupportedException();
                }
            }
            else
                throw new NotSupportedException();
        }

        // initializes an object using a corresponding STON value
        private static void InitializeFromSton(object obj, IStonDocument document, IStonComplexEntity entity, IDictionary<IStonValuedEntity, object> builtObjects)
        {
            switch (obj)
            {
                case IPlaylistItem item:
                    InitializeFromSton(item, document, entity, builtObjects);
                    break;

                case IStreamProvider provider:
                    InitializeFromSton(provider, document, entity, builtObjects);
                    break;
            }
        }

        // initializes a playlist item using a corresponding STON value
        private static void InitializeFromSton(IPlaylistItem item, IStonDocument document, IStonComplexEntity entity, IDictionary<IStonValuedEntity, object> builtObjects)
        {
            item.Name = FromSton<string>(document, GetValue(document, document.GetMember(entity, new StonBindingName("Name"))), builtObjects) ?? item.GetType().Name;

            // playlist never declares its path explicitly
            // in general, it should be based on the playlist location instead
            if (!(item is Playlist))
                item.Path = FromSton<string>(document, GetValue(document, document.GetMember(entity, new StonBindingName("Path"))), builtObjects) ?? "";

            // loading subitems of a container
            if (item is IPlaylistContainer)
            {
                var container = item as IPlaylistContainer;
                foreach (var subitemData in entity.CollectionInit.Elements)
                {
                    var subitem = FromSton<IPlaylistItem>(document, GetValue(document, subitemData), builtObjects);
                    container.Add(subitem);
                }
            }

            // loading the loop provider of a playlist
            if (item is Track)
                (item as Track).StreamProvider = FromSton<IStreamProvider>(document, GetValue(document, document.GetMember(entity, new StonBindingName("Stream"))), builtObjects);
        }

        // initializes a stream provider using a corresponding STON value
        private static void InitializeFromSton(IStreamProvider provider, IStonDocument document, IStonComplexEntity entity, IDictionary<IStonValuedEntity, object> builtObjects)
        {
            if (provider is LoopStreamProvider)
            {
                var loop = provider as LoopStreamProvider;
                loop.TrackStart = FromSton<long?>(document, GetValue(document, document.GetMember(entity, new StonBindingName("TrackStart"))), builtObjects) ?? -1;
                loop.StreamLoopStart = FromSton<long?>(document, GetValue(document, document.GetMember(entity, new StonBindingName("LoopStart"))), builtObjects) ?? -1;
                loop.StreamLoopEnd = FromSton<long?>(document, GetValue(document, document.GetMember(entity, new StonBindingName("LoopEnd"))), builtObjects) ?? -1;
                loop.TrackEnd = FromSton<long?>(document, GetValue(document, document.GetMember(entity, new StonBindingName("TrackEnd"))), builtObjects) ?? -1;
                loop.Loops = FromSton<int?>(document, GetValue(document, document.GetMember(entity, new StonBindingName("Loops"))), builtObjects) ?? -1;
            }
        }

        // general function to retrieve a valued entity from a STON document
        // it really should appear in the original STON library...
        private static IStonValuedEntity GetValue(IStonDocument document, IStonEntity entity)
        {
            if (entity == null)
                return null;
            else if (entity is IStonValuedEntity)
                return entity as IStonValuedEntity;
            else
                return document.GetReferencedValue(entity as IStonReferenceEntity);
        }

        #endregion
    }
}
