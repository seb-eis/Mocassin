using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Job property that carries a user defined raw job property overwrite through simple string definitions
    /// </summary>
    [XmlRoot("JobProperty")]
    public class MmlJobProperty : IEquatable<MmlJobProperty>
    {
        /// <summary>
        ///     The .NET property name on the simulation interface that should be overwritten
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     The property value string. Requires parsing to correct type
        /// </summary>
        [XmlAttribute("Value")]
        public string Value { get; set; }

        /// <summary>
        /// Create new empty job property
        /// </summary>
        public MmlJobProperty()
        {
        }

        /// <summary>
        /// Create new job property with name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public MmlJobProperty(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public bool Equals(MmlJobProperty other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }
    }
}