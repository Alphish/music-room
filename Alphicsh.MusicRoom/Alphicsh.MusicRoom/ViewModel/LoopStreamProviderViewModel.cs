using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphicsh.MusicRoom.Model;

namespace Alphicsh.MusicRoom.ViewModel
{
    /// <summary>
    /// Acts as a view model for a loop stream provider.
    /// </summary>
    public class LoopStreamProviderViewModel : NotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new view model from a given loop stream provider.
        /// </summary>
        /// <param name="provider">The inner model provider.</param>
        public LoopStreamProviderViewModel(LoopStreamProvider provider)
        {
            Model = provider;
        }

        /// <summary>
        /// Gets the underlying provider model.
        /// </summary>
        public LoopStreamProvider Model { get; }

        #region Track properties

        // UI property, determines if and which track property is uneditable
        public RangeLock TrackLock
            { get => _TrackLock; set => Set(nameof(TrackLock), value); }
        private RangeLock _TrackLock = RangeLock.None;

        /// <summary>
        /// Gets or sets the track beginning position in samples, in relation to the track's original stream.
        /// If it's set to -1, the stream beginning position will be used.
        /// </summary>
        public long TrackStart
        {
            get => Model.TrackStart;
            set
            {
                if (_TrackLock == RangeLock.Start)
                    return;

                if (Model.TrackStart != value)
                {
                    if (_TrackLock == RangeLock.Length)
                    {
                        var length = TrackLength;
                        Model.TrackStart = value;
                        Model.TrackEnd = value + length;
                        Notify(nameof(TrackStart));
                        Notify(nameof(TrackEnd));
                    }
                    else
                    {
                        Model.TrackStart = value;
                        Notify(nameof(TrackStart));
                        Notify(nameof(TrackStart));
                    }
                    Notify(nameof(IsValid));
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the length of the track range in samples.
        /// </summary>
        public long TrackLength
        {
            get => (Model.TrackEnd == -1 || Model.TrackStart == -1) ? -1 : Model.TrackEnd - Model.TrackStart;
            set
            {
                if (_TrackLock == RangeLock.Length)
                    return;

                // you cannot explicitly set a track to be of unknown length
                // that's just silly
                if (value == -1)
                    return;

                // if neither start or end are provided
                // there is no reference point to base the length on
                if (Model.TrackEnd == -1 && Model.TrackStart == -1)
                    return;
                // attempting to base the length on either of existing values
                // if the other is unknown
                else if (Model.TrackStart == -1)
                {
                    if (_TrackLock == RangeLock.Start || value > TrackEnd)
                        return;
                    Model.TrackStart = Model.TrackEnd - value;
                    Notify(nameof(TrackStart));
                    Notify(nameof(TrackLength));
                    Notify(nameof(IsValid));
                    return;
                }
                else if (Model.TrackEnd == -1)
                {
                    if (_TrackLock == RangeLock.End)
                        return;
                    Model.TrackEnd = Model.TrackStart + value;
                    Notify(nameof(TrackEnd));
                    Notify(nameof(TrackLength));
                    Notify(nameof(IsValid));
                    return;
                }

                // setting the track length with both values known
                if (TrackLength != value)
                {
                    if (_TrackLock == RangeLock.End)
                    {
                        Model.TrackStart = Model.TrackEnd - value;
                        Notify(nameof(TrackStart));
                        Notify(nameof(TrackLength));
                    }
                    else
                    {
                        Model.TrackEnd = Model.TrackStart + value;
                        Notify(nameof(TrackEnd));
                        Notify(nameof(TrackLength));
                    }
                    Notify(nameof(IsValid));
                }
            }
        }

        /// <summary>
        /// Gets or sets the track end position in samples, in relation to the track's original stream.
        /// If it's set to -1, the stream end position will be used.
        /// </summary>
        public long TrackEnd
        {
            get => Model.TrackEnd;
            set
            {
                if (_TrackLock == RangeLock.End)
                    return;

                if (Model.TrackEnd != value)
                {
                    if (_TrackLock == RangeLock.Length)
                    {
                        var length = TrackLength;
                        Model.TrackEnd = value;
                        Model.TrackStart = value - length;
                        Notify(nameof(TrackStart));
                        Notify(nameof(TrackEnd));
                    }
                    else
                    {
                        Model.TrackEnd = value;
                        Notify(nameof(TrackEnd));
                        Notify(nameof(TrackLength));
                    }
                    Notify(nameof(IsValid));
                }
            }
        }

        #endregion

        #region Loop properties

        // UI property, determines if and which loop property is uneditable
        public RangeLock LoopLock
            { get => _LoopLock; set => Set(nameof(LoopLock), value); }
        private RangeLock _LoopLock = RangeLock.None;

        /// <summary>
        /// Gets or sets the loop beginning position in samples, in relation to the track's original stream.
        /// If it's set to -1, the track start position will be used.
        /// </summary>
        public long StreamLoopStart
        {
            get => Model.StreamLoopStart;
            set
            {
                if (_LoopLock == RangeLock.Start)
                    return;

                if (Model.StreamLoopStart != value)
                {
                    if (_LoopLock == RangeLock.Length)
                    {
                        var length = LoopLength;
                        Model.StreamLoopStart = value;
                        Model.StreamLoopEnd = value + length;
                        Notify(nameof(StreamLoopStart));
                        Notify(nameof(StreamLoopEnd));
                    }
                    else
                    {
                        Model.StreamLoopStart = value;
                        Notify(nameof(StreamLoopStart));
                        Notify(nameof(LoopLength));
                    }
                    Notify(nameof(IsValid));
                }
            }
        }

        /// <summary>
        /// Gets or sets the length of the loop in samples.
        /// </summary>
        public long LoopLength
        {
            get => (Model.StreamLoopEnd == -1 || Model.StreamLoopStart == -1) ? -1 : Model.StreamLoopEnd - Model.StreamLoopStart;
            set
            {
                if (_LoopLock == RangeLock.Length)
                    return;

                // you cannot explicitly set a loop to be of unknown length
                // that's just silly
                if (value == -1)
                    return;

                // if neither start or end are provided
                // there is no reference point to base the length on
                if (Model.StreamLoopEnd == -1 && Model.StreamLoopStart == -1)
                    return;
                // attempting to base the length on either of existing values
                // if the other is unknown
                else if (Model.StreamLoopStart == -1)
                {
                    if (_LoopLock == RangeLock.Start || value > StreamLoopEnd)
                        return;
                    Model.StreamLoopStart = Model.StreamLoopEnd - value;
                    Notify(nameof(StreamLoopStart));
                    Notify(nameof(LoopLength));
                    Notify(nameof(IsValid));
                    return;
                }
                else if (Model.StreamLoopEnd == -1)
                {
                    if (_LoopLock == RangeLock.End)
                        return;
                    Model.StreamLoopEnd = Model.StreamLoopStart + value;
                    Notify(nameof(StreamLoopEnd));
                    Notify(nameof(LoopLength));
                    Notify(nameof(IsValid));
                    return;
                }

                // setting the loop length with both values known
                if (LoopLength != value)
                {
                    if (_LoopLock == RangeLock.End)
                    {
                        Model.StreamLoopStart = Model.StreamLoopEnd - value;
                        Notify(nameof(StreamLoopStart));
                        Notify(nameof(LoopLength));
                    }
                    else
                    {
                        Model.StreamLoopEnd = Model.StreamLoopStart + value;
                        Notify(nameof(StreamLoopEnd));
                        Notify(nameof(LoopLength));
                    }
                    Notify(nameof(IsValid));
                }
            }
        }

        /// <summary>
        /// Gets or sets the loop end position in samples, in relation to the track's original stream.
        /// If it's set to -1, the track end position will be used.
        /// </summary>
        public long StreamLoopEnd
        {
            get => Model.StreamLoopEnd;
            set
            {
                if (_LoopLock == RangeLock.End)
                    return;

                if (Model.StreamLoopEnd != value)
                {
                    if (_LoopLock == RangeLock.Length)
                    {
                        var length = LoopLength;
                        Model.StreamLoopEnd = value;
                        Model.StreamLoopStart = value - length;
                        Notify(nameof(StreamLoopStart));
                        Notify(nameof(StreamLoopEnd));
                    }
                    else
                    {
                        Model.StreamLoopEnd = value;
                        Notify(nameof(StreamLoopEnd));
                        Notify(nameof(LoopLength));
                    }
                    Notify(nameof(IsValid));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of loops to play.
        /// For a value of -1, the track is set to loop indefinitely.
        /// </summary>
        public int Loops
            { get => Model.Loops; set => Set(Model, nameof(Loops), value); }

        #endregion

        #region Lengths

        /// <summary>
        /// Gets or sets the assumed length of the track stream.
        /// Based on it, the track/loop positions can be fully determined.
        /// </summary>
        public long ExpectedStreamLength
        {
            get => _ExpectedStreamLength;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("The expected stream length must have a positive value.");

                if (_ExpectedStreamLength != value)
                {
                    _ExpectedStreamLength = value;
                    Notify(nameof(ExpectedStreamLength));

                    Notify(nameof(ExpectedTrackStart));
                    Notify(nameof(ExpectedLoopStart));
                    Notify(nameof(ExpectedLoopEnd));
                    Notify(nameof(ExpectedTrackEnd));

                    Notify(nameof(ExpectedIntroLength));
                    Notify(nameof(ExpectedLoopLength));
                    Notify(nameof(ExpectedOutroLength));
                }
            }
        }
        private long _ExpectedStreamLength = 1;

        /// <summary>
        /// Gets the expected track start position, based on expected stream length and given track start value.
        /// </summary>
        public long ExpectedTrackStart
            => TrackStart < 0 ? 0 : TrackStart >= ExpectedStreamLength ? ExpectedStreamLength - 1 : TrackStart;

        /// <summary>
        /// Gets the expected loop start position, based on expected stream length and given loop start value.
        /// </summary>
        public long ExpectedLoopStart
            => StreamLoopStart < ExpectedTrackStart ? ExpectedTrackStart : StreamLoopStart >= ExpectedStreamLength ? ExpectedStreamLength - 1 : StreamLoopStart;

        /// <summary>
        /// Gets the expected loop end position, based on expected stream length amd given loop end value.
        /// </summary>
        public long ExpectedLoopEnd
            => StreamLoopEnd < 0 ? ExpectedTrackEnd : StreamLoopEnd > ExpectedTrackEnd ? ExpectedTrackEnd : StreamLoopEnd <= ExpectedLoopStart ? ExpectedLoopStart + 1 : StreamLoopEnd;

        /// <summary>
        /// Gets the expected track start position, based on expected stream length and given track end value.
        /// </summary>
        public long ExpectedTrackEnd
            => TrackEnd < 0 ? ExpectedStreamLength : TrackEnd <= ExpectedLoopStart ? ExpectedLoopStart + 1 : TrackEnd > ExpectedStreamLength ? ExpectedStreamLength : TrackEnd;

        /// <summary>
        /// Gets the length of the track intro (the part before the loop).
        /// </summary>
        public long ExpectedIntroLength => ExpectedLoopStart - ExpectedTrackStart;

        /// <summary>
        /// Gets the length of the loop.
        /// </summary>
        public long ExpectedLoopLength => ExpectedLoopEnd - ExpectedLoopStart;

        /// <summary>
        /// Gets the length of the track outro (the part after the loop).
        /// </summary>
        public long ExpectedOutroLength => ExpectedTrackEnd - ExpectedLoopEnd;

        #endregion

        /// <summary>
        /// Indicates whether all track and loop properties are in a correct relation. It does not take into account the length of the original stream.
        /// </summary>
        public bool IsValid
        {
            get
            {
                long ts = TrackStart < 0 ? 0 : TrackStart;
                long ls = StreamLoopStart < 0 ? ts : StreamLoopStart;
                long te = TrackEnd < 0 ? long.MaxValue : TrackEnd;
                long le = StreamLoopEnd < 0 ? te : StreamLoopEnd;
                return ts <= ls && ls < le && le <= te;
            }
        }

        /// <summary>
        /// Represents possible states of track/loop range properties locking.
        /// </summary>
        public enum RangeLock : byte
        {
            None,
            Start,
            Length,
            End
        }
    }
}
