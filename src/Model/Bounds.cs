using System.Text.Json;
using System.Xml.Serialization;

namespace BezelTools.Model
{
    /// <summary>
    /// An element bounds (position)
    /// </summary>
    public class Bounds
    {
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
        /// Gets or sets the height
        /// </summary>
        [XmlAttribute("height")]
        public double Height { get; set; }

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
        /// Gets or sets the width
        /// </summary>
        [XmlAttribute("width")]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the X position
        /// </summary>
        [XmlAttribute("x")]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y position
        /// </summary>
        [XmlAttribute("y")]
        public double Y { get; set; }

        /// <summary>
        /// Create a clone of this instance
        /// </summary>
        /// <returns>The cloned bounds</returns>
        public Bounds Clone()
        {
            return new Bounds
            {
                X = this.X,
                Y = this.Y,
                Width = this.Width,
                Height = this.Height
            };
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
