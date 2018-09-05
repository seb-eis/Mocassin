using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Represents a linearized encoded matrix that supports csv style string conversion and parsing
    /// </summary>
    public class InteropCsvMatrix<T>
    {
        /// <summary>
        /// The size list that describes the dimensions of the matrix
        /// </summary>
        public CsvList<int> SizeList { get; set; }

        /// <summary>
        /// The linearized version of the matrix
        /// </summary>
        public CsvList<T> LinearMatrix { get; set; }

        /// <summary>
        /// Creates a csv style string representation of the encoded lattice
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{SizeList}@{LinearMatrix}";
        }

        /// <summary>
        /// Parses a csv style string representation of an encoded lattice into the actual lattice object using the provided string to value converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converter"></param>
        public static InteropCsvMatrix<T> Parse(string value, Func<string,T> converter)
        {
            var encoded = new InteropCsvMatrix<T>();
            var split = value.Split('@');

            encoded.SizeList = CsvList<int>.Parse(split[0], a => int.Parse(a));
            encoded.LinearMatrix = CsvList<T>.Parse(split[1], converter);

            return encoded;
        }
    }
}
