using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

using Alphicsh.Audio.Streaming;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Exposes the functionality and data of the music player.
    /// </summary>
    public class PlayerViewModel : NotifyPropertyChanged, IDisposable
    {
        // the inner music player
        private WavePlayer InnerPlayer = new WavePlayer();

        #region Basic playback management

        /// <summary>
        /// Plays the currently selected track.
        /// </summary>
        /// <param name="reset">Whether the track should be reset to initial position or not.</param>
        public void Play(bool reset = false)
            => InnerPlayer.Play(reset);

        /// <summary>
        /// Plays a given track. If the track is already playing, the player carries on.
        /// </summary>
        /// <param name="stream">The track to play.</param>
        /// <param name="reset">Whether the track should be reset to initial position or not.</param>
        public void Play(WaveStream stream, bool reset = false)
            => InnerPlayer.Play(stream, reset);

        /// <summary>
        /// Plays a track created from a source. If the track from the source is already used an playing, the player carries on.
        /// </summary>
        /// <param name="streamSource">The object the stream is created from.</param>
        /// <param name="streamBuilder">The function that creates the stream if needed.</param>
        /// <param name="reset">Whether the track should be reset to initial position or not.</param>
        public void Play(object source, Func<WaveStream> builder, bool reset = false)
            => InnerPlayer.Play(source, builder, reset);

        /// <summary>
        /// Stops the track and resets the stream position.
        /// </summary>
        public void Stop()
            => InnerPlayer.Stop();

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Diposes of the inner player resources.
        /// </summary>
        public void Dispose()
        {
            InnerPlayer.Dispose();
        }

        #endregion
    }
}
