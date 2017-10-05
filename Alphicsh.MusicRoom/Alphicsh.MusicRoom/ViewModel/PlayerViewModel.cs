using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

using Alphicsh.Audio.Streaming;
using Alphicsh.MusicRoom.Model;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Exposes the functionality and data of the music player.
    /// </summary>
    public class PlayerViewModel : NotifyPropertyChanged, IDisposable
    {
        // the inner music player
        private WavePlayer InnerPlayer = new WavePlayer();

        /// <summary>
        /// Gets the track to play.
        /// </summary>
        public TrackViewModel SelectedTrack
            { get => _SelectedTrack; set => Set(nameof(SelectedTrack), value); }
        private TrackViewModel _SelectedTrack = null;

        public LoopStreamProviderViewModel SelectedStreamProvider
            { get => _SelectedStreamProvider; set => Set(nameof(SelectedStreamProvider), value); }
        private LoopStreamProviderViewModel _SelectedStreamProvider;

        #region Basic playback actions

        /// <summary>
        /// Plays the currently selected track.
        /// </summary>
        public void Play()
        {
            if (SelectedTrack == null) return;

            var track = SelectedTrack.Model as Track;
            SelectedStreamProvider = SelectedTrack.StreamProvider;
            InnerPlayer.Play(SelectedStreamProvider.Model, SelectedStreamProvider.Model.CreateStream, true);
            SelectedStreamProvider.ExpectedStreamLength = Length / InnerPlayer.CurrentStream.BlockAlign;
            Notify(nameof(Length));
            Notify(nameof(Position));
            Notify(nameof(State));
        }

        /// <summary>
        /// Selects and plays a specific track.
        /// </summary>
        /// <param name="track">The track to play.</param>
        public void Play(TrackViewModel track)
        {
            SelectedTrack = track;
            Play();
        }

        /// <summary>
        /// Pauses the currently playing track.
        /// </summary>
        public void Pause()
        {
            InnerPlayer.Pause();
            Notify(nameof(State));
        }

        /// <summary>
        /// Resumes the currently paused track.
        /// </summary>
        public void Resume()
        {
            InnerPlayer.Resume();
            Notify(nameof(State));
        }

        /// <summary>
        /// Stops the track and resets the stream position.
        /// </summary>
        public void Stop()
        {
            InnerPlayer.Stop();
            Notify(nameof(Position));
            Notify(nameof(State));
        }

        #endregion

        #region Other playback management

        /// <summary>
        /// Gets the playback state of the currently played track, if any.
        /// </summary>
        public PlaybackState State
            => InnerPlayer.PlaybackState;

        /// <summary>
        /// Gets or sets the playback volume.
        /// </summary>
        public float Volume
        {
            get => InnerPlayer.Volume;
            set => Set(InnerPlayer, nameof(Volume), Math.Min(Math.Max(0f, value), 1f));
        }

        /// <summary>
        /// Gets or sets the position of the track (in bytes).
        /// </summary>
        public long Position
        {
            get => InnerPlayer.Position;
            set => Set(InnerPlayer, nameof(Position), value);
        }
        private long _LastPosition = -1;

        /// <summary>
        /// Continuously checks the current position, and notifies if it has changed.
        /// </summary>
        public void UpdatePosition()
        {
            long position = Position;
            if (position != _LastPosition)
                Notify(nameof(Position));
            _LastPosition = position;
        }

        /// <summary>
        /// Gets the length of the track, if any.
        /// </summary>
        public long Length
            => InnerPlayer.CurrentStream?.Length ?? 1;

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
