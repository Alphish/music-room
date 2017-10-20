/*
This file contains a modified portion of NAudio.Vorbis library source, that portion being VorbisWaveReader class.
The Length and Position properties have been modified for the sake of a correct wave alignment. The rest of class' contents remain the same.

Including the license for VorbisWaveReader, as per license terms:

Copyright (c) 2015 Andrew Ward, Ms-PL

Microsoft Public License (Ms-PL)

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.
Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees, or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.Audio.Streaming
{
    /// <summary>
    /// VorbisWaveReader, as copied from NAudio.Vorbis project.
    /// </summary>
    public class VorbisWaveReader : NAudio.Wave.WaveStream, IDisposable, NAudio.Wave.ISampleProvider, NAudio.Wave.IWaveProvider
    {
        NVorbis.VorbisReader _reader;
        NAudio.Wave.WaveFormat _waveFormat;

        public VorbisWaveReader(string fileName)
        {
            _reader = new NVorbis.VorbisReader(fileName);

            _waveFormat = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(_reader.SampleRate, _reader.Channels);
        }

        public VorbisWaveReader(System.IO.Stream sourceStream)
        {
            _reader = new NVorbis.VorbisReader(sourceStream, false);

            _waveFormat = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(_reader.SampleRate, _reader.Channels);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            base.Dispose(disposing);
        }

        public override NAudio.Wave.WaveFormat WaveFormat
        {
            get { return _waveFormat; }
        }

        public override long Length
        {
            get { return _reader.TotalSamples * _waveFormat.BlockAlign; }
        }

        public override long Position
        {
            get
            {
                return _reader.DecodedPosition * _waveFormat.BlockAlign;
            }
            set
            {
                if (value < 0 || value > Length) throw new ArgumentOutOfRangeException("value");

                _reader.DecodedPosition = value / _waveFormat.BlockAlign;
            }
        }

        // This buffer can be static because it can only be used by 1 instance per thread
        [ThreadStatic]
        static float[] _conversionBuffer = null;

        public override int Read(byte[] buffer, int offset, int count)
        {
            // adjust count so it is in floats instead of bytes
            count /= sizeof(float);

            // make sure we don't have an odd count
            count -= count % _reader.Channels;

            // get the buffer, creating a new one if none exists or the existing one is too small
            var cb = _conversionBuffer ?? (_conversionBuffer = new float[count]);
            if (cb.Length < count)
            {
                cb = (_conversionBuffer = new float[count]);
            }

            // let Read(float[], int, int) do the actual reading; adjust count back to bytes
            int cnt = Read(cb, 0, count) * sizeof(float);

            // move the data back to the request buffer
            Buffer.BlockCopy(cb, 0, buffer, offset, cnt);

            // done!
            return cnt;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            return _reader.ReadSamples(buffer, offset, count);
        }

        public bool IsParameterChange { get { return _reader.IsParameterChange; } }

        public void ClearParameterChange()
        {
            _reader.ClearParameterChange();
        }

        public int StreamCount
        {
            get { return _reader.StreamCount; }
        }

        public int? NextStreamIndex { get; set; }

        public bool GetNextStreamIndex()
        {
            if (!NextStreamIndex.HasValue)
            {
                var idx = _reader.StreamCount;
                if (_reader.FindNextStream())
                {
                    NextStreamIndex = idx;
                    return true;
                }
            }
            return false;
        }

        public int CurrentStream
        {
            get { return _reader.StreamIndex; }
            set
            {
                if (!_reader.SwitchStreams(value))
                {
                    throw new System.IO.InvalidDataException("The selected stream is not a valid Vorbis stream!");
                }

                if (NextStreamIndex.HasValue && value == NextStreamIndex.Value)
                {
                    NextStreamIndex = null;
                }
            }
        }

        /// <summary>
        /// Gets the encoder's upper bitrate of the current selected Vorbis stream
        /// </summary>
        public int UpperBitrate { get { return _reader.UpperBitrate; } }

        /// <summary>
        /// Gets the encoder's nominal bitrate of the current selected Vorbis stream
        /// </summary>
        public int NominalBitrate { get { return _reader.NominalBitrate; } }

        /// <summary>
        /// Gets the encoder's lower bitrate of the current selected Vorbis stream
        /// </summary>
        public int LowerBitrate { get { return _reader.LowerBitrate; } }

        /// <summary>
        /// Gets the encoder's vendor string for the current selected Vorbis stream
        /// </summary>
        public string Vendor { get { return _reader.Vendor; } }

        /// <summary>
        /// Gets the comments in the current selected Vorbis stream
        /// </summary>
        public string[] Comments { get { return _reader.Comments; } }

        /// <summary>
        /// Gets the number of bits read that are related to framing and transport alone
        /// </summary>
        public long ContainerOverheadBits { get { return _reader.ContainerOverheadBits; } }

        /// <summary>
        /// Gets stats from each decoder stream available
        /// </summary>
        public NVorbis.IVorbisStreamStatus[] Stats { get { return _reader.Stats; } }
    }
}
