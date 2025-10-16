// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.Extensions
{
    #region usings

    using System.IO;
    using System.Text;

    #endregion

    /// <summary>
    ///     MemoryStream Extension Methods that provide conversions to and from strings
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Returns the content of the stream as a string
        /// </summary>
        /// <param name="ms">
        /// </param>
        /// <param name="encoding">
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AsString(this MemoryStream ms, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Unicode;
            }

            return encoding.GetString(ms.ToArray());
        }

        /// <summary>
        /// The as string.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AsString(this Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Unicode;
            }

            return encoding.GetString(stream.ToByteArray(encoding));
        }

        /// <summary>
        /// Writes the specified string into the memory stream
        /// </summary>
        /// <param name="ms">
        /// </param>
        /// <param name="inputString">
        /// </param>
        /// <param name="encoding">
        /// </param>
        public static void FromString(this MemoryStream ms, string inputString, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Unicode;
            }

            var buffer = encoding.GetBytes(inputString);
            ms.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// The to byte array.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] ToByteArray(this Stream stream, Encoding encoding = null)
        {
            if (stream == null) return null;

            stream.Position = 0;
            using (var memstream = new MemoryStream())
            {
                var buffer = new byte[512];
                int bytesRead;

                if (encoding != null)
                {
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memstream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memstream.Write(buffer, 0, bytesRead);
                }

                return memstream.ToArray();
            }
        }
    }
}