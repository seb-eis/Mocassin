using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Generic relation entity class for many to many relations between two other <see cref="XmlEntity" /> types
    /// </summary>
    public class XmlEntityRelation<T1, T2> : IEquatable<XmlEntityRelation<T1, T2>> where T1 : XmlEntity where T2 : XmlEntity
    {
        /// <summary>
        ///     Get or set the primary context key
        /// </summary>
        [Column("Id"), Key]
        public int ContextId { get; set; }

        /// <summary>
        ///     Get or set context key of the first entity
        /// </summary>
        public int ContextId1 { get; set; }

        /// <summary>
        ///     Get or set the navigation property for the first entity
        /// </summary>
        [ForeignKey(nameof(ContextId1))]
        public T1 Entity1 { get; set; }

        /// <summary>
        ///     Get or set context key of the second entity
        /// </summary>
        public int ContextId2 { get; set; }

        /// <summary>
        ///     Get or set the navigation property for the second entity
        /// </summary>
        [ForeignKey(nameof(ContextId2))]
        public T2 Entity2 { get; set; }

        /// <inheritdoc />
        public bool Equals(XmlEntityRelation<T1, T2> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ReferenceEquals(Entity1, other.Entity1) && ReferenceEquals(Entity2, other.Entity2);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((XmlEntityRelation<T1, T2>) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return Entity1.GetHashCode() * 397 ^ Entity2.GetHashCode();
            }
        }
    }
}