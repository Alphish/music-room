using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Alphicsh.MusicRoom.Model
{
    /// <summary>
    /// Provides methods to create a playable stream from a given track.
    /// </summary>
    public interface IStreamProvider
    {
        /// <summary>
        /// Gets the track to be streamed.
        /// </summary>
        Track Track { get; }

        /// <summary>
        /// The method of creating the playable stream from the given track.
        /// </summary>
        /// <returns>A playable soundtrack stream.</returns>
        WaveStream CreateStream();
    }
}
