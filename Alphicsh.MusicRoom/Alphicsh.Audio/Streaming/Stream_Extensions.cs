using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphicsh.Audio.Streaming
{
    /// <summary>
    /// Provides extension methods for various streams.
    /// </summary>
    public static class Stream_Extensions
    {
        /// <summary>
        /// Attempts to read a specific number of bytes from the given stream, until no more bytes can be read.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="buffer">The bytes buffer to copy the stream contents to.</param>
        /// <param name="offset">The position where the bytes should be copied.</param>
        /// <param name="count">The number of bytes requested.</param>
        /// <returns>The number of bytes actually read.</returns>
        public static int ReadMax(this Stream stream, byte[] buffer, int offset, int count)
        {
            int result = 0;
            int read;
            while (count > 0 && (read = stream.Read(buffer, offset, count)) != 0)
            {
                result += read;
                offset += read;
                count -= read;
            }
            return result;
        }
    }
}
