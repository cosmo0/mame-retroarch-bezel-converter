using System.Text.Json;
using System.Xml.Serialization;

namespace BezelTools.Model
{
    public class Offset
    {
        /// <summary>
        /// The default offset
        /// </summary>
        public const double DEFAULT_OFFSET = 0;

        /// <summary>
        /// The default stretch
        /// </summary>
        public const double DEFAULT_STRETCH = 1;

        /// <summary>
        /// Gets or sets the screen horizontal offset
        /// </summary>
        [XmlAttribute("hoffset")]
        public double HOffset { get; set; }

        /// <summary>
        /// Gets or sets the screen horizontal stretch
        /// </summary>
        [XmlAttribute("hstretch")]
        public double HStretch { get; set; }

        /// <summary>
        /// Gets or sets the screen vertical offset
        /// </summary>
        [XmlAttribute("voffset")]
        public double VOffset { get; set; }

        /// <summary>
        /// Gets or sets the screen vertical stretch
        /// </summary>
        [XmlAttribute("vstretch")]
        public double VStretch { get; set; }

        /// <summary>
        /// Gets a string representation of the offset
        /// </summary>
        /// <returns>The string representation of the offset</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
