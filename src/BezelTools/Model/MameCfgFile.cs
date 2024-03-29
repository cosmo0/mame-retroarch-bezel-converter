﻿using System.Xml.Serialization;

namespace BezelTools.Model
{
    /// <summary>
    /// A MAME config file
    /// </summary>
    [XmlRoot("mameconfig")]
    public class MameCfgFile
    {
        /// <summary>
        /// Gets or sets the configured system
        /// </summary>
        [XmlElement("system")]
        public System SystemConfig { get; set; }

        /// <summary>
        /// A configured system
        /// </summary>
        public class System
        {
            /// <summary>
            /// Gets or sets the system name (ex: altbeast)
            /// </summary>
            [XmlAttribute("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the video configuration
            /// </summary>
            [XmlElement("video")]
            public Video VideoConfig { get; set; }

            /// <summary>
            /// The video configuration
            /// </summary>
            public class Video
            {
                /// <summary>
                /// Gets or sets the video screen configuration
                /// </summary>
                [XmlElement("screen")]
                public Screen VideoScreen { get; set; }

                /// <summary>
                /// A screen configuration
                /// </summary>
                public class Screen : Offset
                {
                    /// <summary>
                    /// Gets or sets the screen index
                    /// </summary>
                    [XmlAttribute("index")]
                    public int Index { get; set; }
                }
            }
        }
    }
}
