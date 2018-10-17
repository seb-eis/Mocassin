using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Represents a marshal provider that handles conversion between structures and byte array representations
    /// </summary>
    public interface IMarshalProvider
    {
        /// <summary>
        /// Translates the bytes starting at the provided buffer offset to the defined structure
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        TStruct BytesToStructure<TStruct>(byte[] buffer, int offset) where TStruct : struct;

        /// <summary>
        /// Translates the bytes starting at the provided buffer offset to the defined structure type
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="structType"></param>
        /// <returns></returns>
        object BytesToStructure(byte[] buffer, int offset, Type structType);

        /// <summary>
        /// Translates the provided structure to its byte representation intto the provided buffer starting at an offset
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="structure"></param>
        void StructureToBytes<TStruct>(byte[] buffer, int offset, in TStruct structure) where TStruct : struct;

        /// <summary>
        /// Marshal all the passed structures to the passed byte buffer stating at the specified offset
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="structures"></param>
        void ManyStructuresToBytes<TStruct>(byte[] buffer, int offset, IEnumerable<TStruct> structures) where TStruct : struct;

        /// <summary>
        /// Marshals the contents of the passed byte buffer enclosed by offset and upper bound to many structures
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        IEnumerable<TStruct> BytesToManyStructures<TStruct>(byte[] buffer, int offset, int upperBound) where TStruct : struct;
    }
}
