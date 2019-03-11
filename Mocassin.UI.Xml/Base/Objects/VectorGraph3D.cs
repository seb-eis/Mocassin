﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Mathematics.ValueTypes.IVector3D" /> data
    /// </summary>
    [XmlRoot("Vector")]
    public class VectorGraph3D : ProjectObjectGraph
    {
        /// <summary>
        ///     Get or set the A coordinate
        /// </summary>
        [XmlAttribute("A")]
        [JsonProperty("A")]
        public double A { get; set; }

        /// <summary>
        ///     Get or set the B coordinate
        /// </summary>
        [XmlAttribute("B")]
        [JsonProperty("B")]
        public double B { get; set; }

        /// <summary>
        ///     Get or set the C coordinate
        /// </summary>
        [XmlAttribute("C")]
        [JsonProperty("C")]
        public double C { get; set; }

        /// <summary>
        ///     Returns the helper object content as a <see cref="Mocassin.Mathematics.ValueTypes.DataVector3D" /> object
        /// </summary>
        /// <returns></returns>
        public DataVector3D AsDataVector3D()
        {
            return new DataVector3D(A, B, C);
        }

        /// <summary>
        /// Creates a new serializable <see cref="VectorGraph3D"/> from a passed <see cref="IVector3D"/> interface
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static VectorGraph3D Create(IVector3D vector)
        {
            return new VectorGraph3D {A = vector.Coordinates.A, B = vector.Coordinates.B, C = vector.Coordinates.C};
        }
    }
}