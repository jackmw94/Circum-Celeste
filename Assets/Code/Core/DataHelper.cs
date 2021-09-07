using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Code.Debugging;
using Code.Level.Player;
using UnityEngine;
using UnityExtras.Code.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Core
{
    public static class DataHelper
    {
        /// <summary>
        /// Compresses a string and returns a deflate compressed, Base64 encoded string.
        /// </summary>
        /// <param name="uncompressedString">String to compress</param>
        public static string Compress(this string uncompressedString)
        {
            byte[] compressedBytes;

            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
            {
                using (var compressedStream = new MemoryStream())
                {
                    // setting the leaveOpen parameter to true to ensure that compressedStream will not be closed when compressorStream is disposed
                    // this allows compressorStream to close and flush its buffers to compressedStream and guarantees that compressedStream.ToArray() can be called afterward
                    // although MSDN documentation states that ToArray() can be called on a closed MemoryStream, I don't want to rely on that very odd behavior should it ever change
                    using (var compressorStream = new DeflateStream(compressedStream, System.IO.Compression.CompressionLevel.Fastest, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }

                    // call compressedStream.ToArray() after the enclosing DeflateStream has closed and flushed its buffer to compressedStream
                    compressedBytes = compressedStream.ToArray();
                }
            }

            return Convert.ToBase64String(compressedBytes);
        }

        /// <summary>
        /// Decompresses a deflate compressed, Base64 encoded string and returns an uncompressed string.
        /// </summary>
        /// <param name="compressedString">String to decompress.</param>
        public static string Decompress(this string compressedString)
        {
            byte[] decompressedBytes;

            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);

                    decompressedBytes = decompressedStream.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decompressedBytes);
        }

        /// <summary>
        /// WARNING: makes assumptions. result not guaranteed
        /// </summary>
        public static bool SaveDataIsProbablyJson(this string str)
        {
            // Bit of a hack, checks that the first and last characters are curly braces.
            //
            // Given that save data will either be json or a compressed string it's highly unlikely
            // that the compressed string happens to randomly have a curly brace at the start and end.
            
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }
            
            string firstChar = str.Substring(0, 1);
            string lastChar = str.Substring(str.Length - 1, 1);
            
            return firstChar.EqualsIgnoreCase("{") && lastChar.EqualsIgnoreCase("}");
        }
    }
}