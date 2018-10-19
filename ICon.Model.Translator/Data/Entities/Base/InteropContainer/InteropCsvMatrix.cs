using System;
using Mocassin.Framework.Collections;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Represents a linearized encoded matrix that supports csv style string conversion and parsing
    /// </summary>
    public class InteropCsvMatrix<T>
    {
        /// <summary>
        ///     The size list that describes the dimensions of the matrix
        /// </summary>
        public CsvSerializableList<int> SizeList { get; set; }

        /// <summary>
        ///     The linearized version of the matrix
        /// </summary>
        public CsvSerializableList<T> LinearMatrix { get; set; }

        /// <summary>
        ///     Creates a csv style string representation of the encoded lattice
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{SizeList}@{LinearMatrix}";
        }

        /// <summary>
        ///     Parses a csv style string representation of an encoded lattice into the actual lattice object using the provided
        ///     string to value converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converter"></param>
        public static InteropCsvMatrix<T> Parse(string value, Func<string, T> converter)
        {
            var encoded = new InteropCsvMatrix<T>();
            var split = value.Split('@');

            encoded.SizeList = CsvSerializableList<int>.Parse(split[0], int.Parse);
            encoded.LinearMatrix = CsvSerializableList<T>.Parse(split[1], converter);

            return encoded;
        }
    }
}