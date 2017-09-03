using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NAudio.Wave;
//using NAudio.Vorbis;
using NAudio.Flac;

namespace Alphicsh.Audio.Streaming
{
    /// <summary>
    /// Provides functionality of loading soundtracks to their basic stream or to memory.
    /// </summary>
    public static class TrackLoader
    {
        #region Format-based streams

        /// <summary>
        /// Provides the most appropriate wave stream for a given track.
        /// </summary>
        /// <param name="path">The path of the track to stream.</param>
        /// <returns>The wave stream for the given track.</returns>
        public static WaveStream LoadStream(string path)
        {
            var ext = Path.GetExtension(path);
            switch (ext)
            {
                case ".flac":
                    return new FlacReader(path);
                case ".mp3":
                    return new Mp3FileReader(path);
                case ".ogg":
                    return new VorbisWaveReader(path);
                case ".wav":
                    return new WaveFileReader(path);
                default:
                    throw new NotSupportedException($"Extension {ext} is not supported.");
            }
        }

        #endregion
    }
}
