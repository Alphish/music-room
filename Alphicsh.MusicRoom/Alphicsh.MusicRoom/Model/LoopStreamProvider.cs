using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

using Alphicsh.Audio.Streaming;

namespace Alphicsh.MusicRoom.Model
{
    /// <summary>
    /// Provides a looping music stream for a given track.
    /// </summary>
    public class LoopStreamProvider : IStreamProvider
    {
        /// <summary>
        /// Gets the track to be streamed.
        /// </summary>
        public Track Track { get; }

        /// <summary>
        /// Creates a looping stream provider for a given track.
        /// </summary>
        /// <param name="track">The track to be streamed.</param>
        public LoopStreamProvider(Track track)
        {
            Track = track;
        }

        /// <summary>
        /// Creates a copy of the given looping stream provider.
        /// </summary>
        /// <param name="provider">The provider to be copied.</param>
        public LoopStreamProvider(LoopStreamProvider provider)
            : this(provider.Track)
        {
            TrackStart = provider.TrackStart;
            StreamLoopStart = provider.StreamLoopStart;
            StreamLoopEnd = provider.StreamLoopEnd;
            TrackEnd = provider.TrackEnd;
            Loops = provider.Loops;
        }

        /// <summary>
        /// Gets or sets the track beginning position in samples, in relation to the track's original stream.
        /// If it's set to -1, the stream beginning position will be used.
        /// </summary>
        public long TrackStart { get; set; } = -1;

        /// <summary>
        /// Gets or sets the loop beginning position in samples, in relation to the track's original stream.
        /// If it's set to -1, the track start position will be used.
        /// </summary>
        public long StreamLoopStart { get; set; } = -1;

        /// <summary>
        /// Gets or sets the loop end position in samples, in relation to the track's original stream.
        /// If it's set to -1, the track end position will be used.
        /// </summary>
        public long StreamLoopEnd { get; set; } = -1;

        /// <summary>
        /// Gets or sets the track end position in samples, in relation to the track's original stream.
        /// If it's set to -1, the stream end position will be used.
        /// </summary>
        public long TrackEnd { get; set; } = -1;

        /// <summary>
        /// Gets or sets the number of loops to play.
        /// For a value of -1, the track is set to loop indefinitely.
        /// </summary>
        public int Loops { get; set; } = -1;

        /// <summary>
        /// Creates the looping stream using earlier set looping values.
        /// </summary>
        /// <returns>The looping music stream.</returns>
        public WaveStream CreateStream()
        {
            var stream = new LoopStream(TrackLoader.LoadStream(Track.GetFullPath()));
            stream.SetStreamPoints(
                TrackStart,
                StreamLoopStart != -1 ? StreamLoopStart : TrackStart,
                StreamLoopEnd != -1 ? StreamLoopEnd : TrackEnd,
                TrackEnd
                );
            stream.TotalLoops = Loops;
            return stream;
        }
    }
}
