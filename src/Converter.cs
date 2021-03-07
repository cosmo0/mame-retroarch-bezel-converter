using System;
using Converter.Model;

namespace Converter
{
    public class Converter
    {
        /// <summary>
        /// Applies the specified offset to the specified bounds
        /// </summary>
        /// <param name="sourcePosition">The source screen position</param>
        /// <param name="offset">The offset to apply</param>
        /// <param name="sourceResolution">The source resolution</param>
        /// <returns>The new bounds</returns>
        public static Bounds ApplyOffset(Bounds sourcePosition, Offset offset, Bounds sourceResolution)
        {
            if (offset == null)
            {
                return sourcePosition;
            }

            // TODO
            return sourcePosition;
        }
    }
}
