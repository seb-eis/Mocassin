using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
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
    }
}
