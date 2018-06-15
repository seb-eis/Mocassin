using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ICon.Framework.Extensions;
using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Conversion
{
    /// <summary>
    /// String value converter that handles the conversion of mathematical valu types into separated strings
    /// </summary>
    public static class ValueSerializer
    {
        /// <summary>
        /// Translate any vector into a linearized string with format 'A,B,C'
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static string ConvertFromVector3D<T1>(T1 vector) where T1 : IVector3D
        {
            var coordinates = vector.Coordinates;
            return $"{coordinates.A},{coordinates.B},{coordinates.C}";
        }

        /// <summary>
        /// Creates a vector of specified type from the string with format 'A,B,C'
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="linearized"></param>
        /// <returns></returns>
        public static T1 ConvertToVector3D<T1>(string linearized) where T1 : IVector3D
        {
            var splitted = linearized.Split(',').Select(value => Convert.ToDouble(value)).ToArray();
            if (splitted.Length != 3)
            {
                throw new ArgumentException("Passed string cannot be split into 3 values", nameof(linearized));
            }
            return (T1)Activator.CreateInstance(typeof(T1), splitted[0], splitted[1], splitted[2]);
        }

        /// <summary>
        /// Translates a series of vectors into a linearized string of the format 'A,B,C;A,B,C;...'
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static string ConvertFromManyVector3D<T1>(IEnumerable<T1> vectors) where T1 : IVector3D
        {
            var builder = new StringBuilder(100);
            builder.AppendSeparatedToString(vectors.Select(value => ConvertFromVector3D(value)), ";");
            return builder.ToString();
        }

        /// <summary>
        /// Converts a linearizes form of many 3D vectors (A,B,C;A,B,C;...) into a sequence of vectors
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="linearizedVectors"></param>
        /// <returns></returns>
        public static IEnumerable<T1> ConvertToManyVector3D<T1>(string linearizedVectors) where T1 : IVector3D
        {
            foreach (var item in linearizedVectors.Split(';'))
            {
                yield return ConvertToVector3D<T1>(item);
            }
        }

        /// <summary>
        /// Converts any 4D lineraized vector into a string of the form 'A,B,C,D'
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static string ConvertFromVector4D<T1>(T1 vector) where T1 : ILinearVector4D
        {
            var coordinates = vector.Coordinates;
            return $"{coordinates.A},{coordinates.B},{coordinates.C},{coordinates.D}";
        }

        /// <summary>
        /// Takes a string of a 4D vector with format 'A,B,C,D' and converts it to a vector of specfified type. Vector has to implement a constructor that takes 4 integer values
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="linearizedVector"></param>
        /// <returns></returns>
        public static T1 ConvertToVector4D<T1>(string linearizedVector) where T1 : ILinearVector4D
        {
            var values = linearizedVector.Split(',').Select(value => Convert.ToInt32(value)).ToArray();
            if (values.Length != 4)
            {
                throw new ArgumentException("Passed string could not be separated into 4 integer values");
            }
            return (T1)Activator.CreateInstance(typeof(T1), values[0], values[1], values[2], values[3]);
        }

        /// <summary>
        /// Converts a seqeunce of 4D vectors into a single string of the format 'A,B,C,D;A,B,C,D;...'
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static string ConvertFromManyVector4D<T1>(IEnumerable<T1> vectors) where T1 : ILinearVector4D
        {
            var builder = new StringBuilder(100);
            builder.AppendSeparatedToString(vectors.Select(value => ConvertFromVector4D(value)), ";");
            return builder.ToString();
        }

        /// <summary>
        /// Takes string that represents a seqeunce of linear vectors in the format 'A,B,C,D;A,B,C,D;...' and converts it to a sequence of vectors of the specififed type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="linearizedVectors"></param>
        /// <returns></returns>
        public static IEnumerable<T1> ConvertToManyVector4D<T1>(string linearizedVectors) where T1 : ILinearVector4D
        {
            foreach (var item in linearizedVectors.Split(';'))
            {
                yield return ConvertToVector4D<T1>(item);
            }
        }

        /// <summary>
        /// Converts a sequence of primitive types to a separated string using the provided separator
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ConvertFromPrimitive<T1>(IEnumerable<T1> sequence, char separator) where T1 : struct
        {
            if (!typeof(T1).IsPrimitive)
            {
                throw new ArgumentException("Passed seqeunce is not of primitive type");
            }
            var builder = new StringBuilder(100);
            builder.AppendSeparatedToString(sequence, separator.ToString());
            return builder.ToString();
        }

        /// <summary>
        /// Converts a linearized string representation of primitive types that are sepratated by the provided separator into the sequence of types
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="linearized"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<T1> ConvertToPrimitive<T1>(string linearized, char separator) where T1 : IConvertible
        {
            return linearized.Split(separator).Select(value => (T1)Convert.ChangeType(value, typeof(T1)));
        }
    }
}
