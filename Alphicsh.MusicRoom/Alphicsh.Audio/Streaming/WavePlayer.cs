using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Alphicsh.Audio.Streaming
{
    /// <summary>
    /// Provides a wave stream playback and manipulation functionality.
    /// </summary>
    public class WavePlayer : IDisposable
    {
        // the inner player
        private WaveOut CurrentOut { get; set; } = null;

        /// <summary>
        /// The currently selected or playing stream.
        /// </summary>
        public WaveStream CurrentStream { get; private set; } = null;

        /// <summary>
        /// The object that created the current stream.
        /// </summary>
        private object StreamSource { get; set; } = null;

        #region Playback management

        /// <summary>
        /// Selects a track without playing it. If the stream is already selected, the player carries on.
        /// </summary>
        /// <param name="stream">The track to select.</param>
        public void Select(WaveStream stream)
        {
            if (CurrentStream != stream)
            {
                CurrentOut?.Dispose();
                CurrentStream?.Dispose();

                CurrentOut = new WaveOut(WaveCallbackInfo.FunctionCallback());      // using the function callback makes playback independent of GUI thread, apparently
                CurrentOut.PlaybackStopped += PlaybackStopped;
                CurrentOut.Volume = _Volume;
                CurrentStream = stream;
                CurrentOut.Init(stream);
                StreamSource = null;
            }
        }

        /// <summary>
        /// Builds a track created from a source without playing it. If the source is already used, the player carries on.
        /// </summary>
        /// <param name="streamSource">The object the stream is created from.</param>
        /// <param name="streamBuilder">The function that creates the stream if needed.</param>
        public void Select(object streamSource, Func<WaveStream> streamBuilder)
        {
            if (StreamSource != streamSource || streamSource == null)
            {
                Select(streamBuilder());
                StreamSource = streamSource;
            }
        }

        /// <summary>
        /// Plays the currently selected track.
        /// </summary>
        /// <param name="reset">Whether the track should be reset to initial position or not.</param>
        public void Play(bool reset = false)
        {
            if (reset) CurrentStream.Position = 0;
            CurrentOut.Play();
        }

        /// <summary>
        /// Plays a given track. If the track is already playing, the player carries on.
        /// </summary>
        /// <param name="stream">The track to play.</param>
        /// <param name="reset">Whether the track should be reset to initial position or not.</param>
        public void Play(WaveStream stream, bool reset = false)
        {
            Select(stream);
            if (reset) CurrentStream.Position = 0;
            CurrentOut.Play();
        }

        /// <summary>
        /// Plays a track created from a source. If the track from the source is already used an playing, the player carries on.
        /// </summary>
        /// <param name="streamSource">The object the stream is created from.</param>
        /// <param name="streamBuilder">The function that creates the stream if needed.</param>
        /// <param name="reset">Whether the track should be reset to initial position or not.</param>
        public void Play(object streamSource, Func<WaveStream> streamBuilder, bool reset = false)
        {
            Select(streamSource, streamBuilder);
            if (reset) CurrentStream.Position = 0;
            CurrentOut.Play();
        }

        /// <summary>
        /// Pauses the audio.
        /// </summary>
        public void Pause()
            => CurrentOut.Pause();

        /// <summary>
        /// Resumes the audio after pausing.
        /// </summary>
        public void Resume()
            => CurrentOut.Resume();

        /// <summary>
        /// Stops the track and resets the stream position.
        /// </summary>
        public void Stop()
        {
            if (CurrentOut == null)
                return;

            CurrentOut.Stop();
            CurrentStream.Position = 0;
        }

        /// <summary>
        /// Gets the current position in bytes in the played stream.
        /// </summary>
        /// <returns>The position in bytes.</returns>
        public long Position
        {
            get => CurrentStream?.Position ?? 0;
            set
            {
                var state = PlaybackState;
                if (state != PlaybackState.Stopped)
                {
                    // doing a bit of juggling here...
                    // first, the track is stopped to clear the remaining sound from buffers
                    CurrentOut.Stop();
                    
                    // then, the stream is repositioned as requested
                    CurrentStream.Position = value;

                    // then, the track is replayed/resumed from the new position
                    // the volume is temporarily changed to avoid brief glitching
                    // otherwise audible when track is repositioned in its paused state
                    var volume = CurrentOut.Volume;
                    CurrentOut.Volume = 0;
                    CurrentOut.Play();
                    CurrentOut.Volume = volume;

                    // finally, if the track has been originally paused
                    // brings it back to the paused state
                    if (state == PlaybackState.Paused)
                        CurrentOut.Pause();

                    // kinda elaborate, but works
                }
            }
        }

        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        public PlaybackState PlaybackState
            => CurrentOut?.PlaybackState ?? PlaybackState.Stopped;

        /// <summary>
        /// Gets or sets the playback volume. It should be a value between 0.0 and 1.0.
        /// </summary>
        public float Volume
        {
            get => _Volume;
            set
            {
                _Volume = value;
                CurrentOut.Volume = _Volume;
            }
        }
        private float _Volume = 1.0f;

        /// <summary>
        /// Occurs when the playback stops automatically.
        /// </summary>
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        #endregion

        #region IDisposable implementation

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CurrentOut?.Dispose();
                    CurrentStream?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

    }
}
