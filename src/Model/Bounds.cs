using System.Text.Json;
using System.Xml.Serialization;

namespace Converter.Model
{
    /// <summary>
    /// An element bounds (position)
    /// </summary>
    public class Bounds
    {
        /// <summary>
        /// Gets or sets the X position
        /// </summary>
        [XmlAttribute("x")]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y position
        /// </summary>
        [XmlAttribute("y")]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        [XmlAttribute("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        [XmlAttribute("height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets the orientation (vertical/horizontal)
        /// </summary>
        [XmlIgnore]
        public Orientation Orientation
        {
            get
            {
                return this.Width > this.Height ? Orientation.Horizontal : Orientation.Vertical;
            }
        }

        /// <summary>
        /// Gets the center point
        /// </summary>
        [XmlIgnore]
        public Point Center
        {
            get
            {
                return new Point
                {
                    X = this.X + (this.Width / 2),
                    Y = this.Y + (this.Height / 2)
                };
            }
        }

        /// <summary>
        /// Gets a string representation of the object
        /// </summary>
        /// <returns>The string representation</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
