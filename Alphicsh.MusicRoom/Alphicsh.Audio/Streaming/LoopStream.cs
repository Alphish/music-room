using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Alphicsh.Audio.Streaming
{
    /// <summary>
    /// Stream for playing soundtrack between two specified points.
    /// </summary>
    public class LoopStream : WaveStream
    {
        // The underlying stream
        private WaveStream InnerStream;

        #region Loop setup

        /// <summary>
        /// Creates a new loop stream, with given starting and ending points (provided as sample indices or byte positions).
        /// </summary>
        public LoopStream(WaveStream innerStream)
        {
            // assigning the stream
            InnerStream = innerStream;

            TrackStartByte = 0;
            LoopStartByte = 0;
            LoopEndByte = InnerStream.Length;
            TrackEndByte = InnerStream.Length;

            RemainingLoops = -1;
        }

        /// <summary>
        /// Changes the track range and looping range in the stream.
        /// All values are provided as sample positions in the underlying wave stream.
        /// </summary>
        /// <param name="streamTrackStart">The position of the track beginning in samples, in relation to the underlying wave stream.</param>
        /// <param name="streamLoopStart">The position of the loop beginning in samples, in relation to the underlying wave stream.</param>
        /// <param name="streamLoopEnd">The position of the loop end in samples, in relation to the underlying wave stream.</param>
        /// <param name="streamTrackEnd">The position of the track end in samples, in relation to the underlying wave stream.</param>
        public void SetStreamPoints(long streamTrackStart = -1, long streamLoopStart = -1, long streamLoopEnd = -1, long streamTrackEnd = -1)
        {
            // calculating the undefined stream points
            if (streamTrackStart < 0)
                streamTrackStart = TrackStart;
            if (streamLoopStart < 0)
                streamLoopStart = StreamLoopStart;
            if (streamLoopEnd < 0)
                streamLoopEnd = StreamLoopEnd;
            if (streamTrackEnd < 0)
                streamTrackEnd = TrackEnd;

            // validating the points
            if (streamLoopStart < streamTrackStart)
                throw new InvalidOperationException("The loop beginning position cannot be lower than the track beginning position.");
            if (streamLoopEnd <= streamLoopStart)
                throw new InvalidOperationException("The loop end position must come after the loop beginning position.");
            if (streamTrackEnd < streamLoopEnd)
                throw new InvalidOperationException("The track ending position cannot be lower than the loop ending position.");

            // assigning the stream points
            TrackStart = streamTrackStart;
            StreamLoopStart = streamLoopStart;
            StreamLoopEnd = streamLoopEnd;
            TrackEnd = streamTrackEnd;

            // repositioning the stream if needed
            if (TrackStartByte > InnerStream.Position) InnerStream.Position = TrackStartByte;
            if (TrackEndByte < InnerStream.Position) InnerStream.Position = TrackEndByte;
        }

        /// <summary>
        /// Changes the loop position within the track.
        /// The loop points are given within the range between track start and end, not the underlying wave stream.
        /// </summary>
        /// <param name="loopStart">The position of the loop beginning in samples, in relation to the track.</param>
        /// <param name="loopEnd">The position of the loop end in samples, in relation to the track.</param>
        public void SetLoopPoints(long loopStart = -1, long loopEnd = -1)
        {
            // calculating the undefined looping points
            if (loopStart < 0)
                loopStart = LoopStart;
            if (loopEnd < 0)
                loopEnd = LoopEnd;

            // setting the points in the stream
            SetStreamPoints(-1, TrackStart + loopStart, TrackStart + loopEnd, -1);
        }

        /// <summary>
        /// Changes the actual track range in the stream.
        /// The looping points in the stream remain unchanged, so you must make sure they still fall within the track range.
        /// </summary>
        /// <param name="trackStart">The position of the track beginning in samples, in relation to the underlying wave stream.</param>
        /// <param name="trackEnd">The position of the track end in samples, in relation to the underlying wave stream.</param>
        public void SetTrackPoints(long trackStart = -1, long trackEnd = -1)
        {
            // calculating the undefined track points
            if (trackStart < 0)
                trackStart = TrackStart;
            if (trackEnd < 0)
                trackEnd = TrackEnd;

            // setting the points in the stream
            SetStreamPoints(trackStart, -1, -1, trackEnd);
        }

        #endregion

        #region Loop-specific members

        /// <summary>
        /// Gets or sets the track beginning position in samples, in relation to the underlying wave stream.
        /// </summary>
        public long TrackStart
        {
            get => TrackStartByte / WaveFormat.BlockAlign;
            private set => TrackStartByte = value * WaveFormat.BlockAlign;
        }
        private long TrackStartByte;



        /// <summary>
        /// Gets or sets the loop beginning position in samples, in relation to the underlying wave stream; returned to once the loop is reached.
        /// </summary>
        public long StreamLoopStart
        {
            get => LoopStartByte / WaveFormat.BlockAlign;
            private set => LoopStartByte = value * WaveFormat.BlockAlign;
        }
        /// <summary>
        /// Gets or sets the loop beginning position in samples; returned to once the loop is reached.
        /// The loop point is given within the range between track start and end, not the underlying wave stream.
        /// </summary>
        public long LoopStart
        {
            get => StreamLoopStart - TrackStart;
            private set => StreamLoopStart = TrackStart + value;
        }
        private long LoopStartByte;



        /// <summary>
        /// Gets or sets the loop end position in samples, in relation to the underlying wave stream; when reached, stream returns to the loop beginning.
        /// </summary>
        public long StreamLoopEnd
        {
            get => LoopEndByte / WaveFormat.BlockAlign;
            private set => LoopEndByte = value * WaveFormat.BlockAlign;
        }
        /// <summary>
        /// Gets or sets the loop end position in samples; when reached, stream returns to the loop beginning.
        /// The loop point is given within the range between track start and end, not the underlying wave stream.
        /// </summary>
        public long LoopEnd
        {
            get => StreamLoopEnd - TrackStart;
            private set => StreamLoopEnd = TrackStart + value;
        }
        private long LoopEndByte;



        /// <summary>
        /// Gets or sets the track end position in samples, in relation to the underlying wave stream.
        /// </summary>
        public long TrackEnd
        {
            get => TrackEndByte / WaveFormat.BlockAlign;
            private set => TrackEndByte = value * WaveFormat.BlockAlign;
        }
        private long TrackEndByte;



        /// <summary>
        /// Number of loops remaining to play. If set to -1, the track loops indefinitely.
        /// </summary>
        public int RemainingLoops { get; set; }

        /// <summary>
        /// Gets or sets the sample index at the current track position.
        /// </summary>
        public long CurrentSample
        {
            get => Position / WaveFormat.BlockAlign;
            set => Position = value * WaveFormat.BlockAlign;
        }

        /// <summary>
        /// Occurs when the soundtrack loops.
        /// </summary>
        public event EventHandler Looped;

        #endregion

        #region Inherited properties

        /// <summary>
        /// Gets the wave format of the underlying stream.
        /// </summary>
        public override WaveFormat WaveFormat => InnerStream.WaveFormat;

        /// <summary>
        /// Gets the length of an entire track.
        /// Note that it's different from underlying stream length if track start/end values are different than default.
        /// </summary>
        public override long Length => TrackEndByte - TrackStartByte;

        /// <summary>
        /// Gets or sets the current position in the stream.
        /// Note that it's offset by the track start position.
        /// </summary>
        public override long Position
        {
            get => InnerStream.Position - TrackStartByte;
            set => InnerStream.Position = TrackStartByte + value - (value % WaveFormat.BlockAlign);
        }

        #endregion

        #region Inherited methods

        /// <summary>
        /// Reads the bytes from the stream to a provided buffer, taking the looping into account.
        /// </summary>
        /// <param name="buffer">The buffer to write the bytes read from the stream to.</param>
        /// <param name="offset">The position in the buffer to start writing the bytes to.</param>
        /// <param name="count">The maximum number of bytes to read from the stream.</param>
        /// <returns>The number of bytes read from the stream.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // foolproofing the function against the likes of me
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Number must be non-negative.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Number must be non-negative.");

            // setting up variables
            int totalBytesRead = 0;
            int subcount, bytesRead;

            // looping back to loop beginning if the track is at or after the loop point
            // this may happen when the stream points (track start, loop start, loop end, track end)
            // are changed when the track is being streamed
            if (RemainingLoops != 0 && InnerStream.Position >= LoopEndByte)
            {
                InnerStream.Position = LoopStartByte;
                if (RemainingLoops > 0) RemainingLoops--;
                Looped?.Invoke(this, new EventArgs());
            }

            // reading bytes within the loop
            while (count > 0 && RemainingLoops != 0)
            {
                // reading bytes up to the next looping point
                subcount = (int)Math.Min(LoopEndByte - InnerStream.Position, count);

                bytesRead = InnerStream.ReadMax(buffer, offset, subcount);

                // if the requested number of bytes couldn't be read
                // the number of bytes read so far is returned
                if (bytesRead != subcount)
                    return totalBytesRead + bytesRead;

                // tracking bytes read
                totalBytesRead += bytesRead;
                offset += bytesRead;
                count -= bytesRead;

                // updating the loop status as needed
                if (InnerStream.Position == LoopEndByte)
                {
                    InnerStream.Position = LoopStartByte;
                    if (RemainingLoops > 0) RemainingLoops--;
                    Looped?.Invoke(this, new EventArgs());
                }
            }
            if (count == 0)
                return totalBytesRead;

            // reading bytes up to the track end
            subcount = (int)Math.Min(TrackEndByte - InnerStream.Position, count);
            bytesRead = InnerStream.ReadMax(buffer, offset, subcount);
            return totalBytesRead + bytesRead;
        }

        /// <summary>
        /// Releases the inner stream resources.
        /// </summary>
        /// <param name="disposing">true to dispose both managed and unmanaged resources, false to dispose only the unmanaged resources</param>
        protected override void Dispose(bool disposing)
        {
            InnerStream.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}
