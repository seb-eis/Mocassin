using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Non generic base class for all wrapped interop objects that can switch between objet and binary unmanaged representation
    /// </summary>
    public abstract class InteropObjectBase
    {
        /// <summary>
        /// Write the internal object information to its unmanaged representation using the provided target buffer
        /// </summary>
        /// <param name="targetBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="marshalProvider"></param>
        public abstract void ToBinary(byte[] targetBuffer, int offset, IMarshalProvider marshalProvider);

        /// <summary>
        /// Read the internal object information from its unmanaged representation using the provided source buffer
        /// </summary>
        /// <param name="sourceBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="marshalProvider"></param>
        public abstract void FromBinary(byte[] sourceBuffer, int offset, IMarshalProvider marshalProvider);
    }

    /// <summary>
    /// Generic base class for wrapping interop structures into a net object. Supports marshalling of teh wrapped structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [NotMapped]
    public abstract class InteropObjectBase<T> : InteropObjectBase where T : struct
    {
        /// <summary>
        /// The wrapped structure
        /// </summary>
        private T structure;

        /// <summary>
        /// Create new interop object base wrapping the passed structure
        /// </summary>
        /// <param name="structure"></param>
        protected InteropObjectBase(T structure)
        {
            this.structure = structure;
        }

        protected InteropObjectBase()
        {
        }

        /// <summary>
        /// Refernce access to the wrapped structure
        /// </summary>
        public ref T Structure => ref structure;

        /// <summary>
        /// Writes the byte representation of the wrapped struct to the target buffer at specififed offset
        /// </summary>
        /// <param name="targetBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="marshalProvider"></param>
        public override void ToBinary(byte[] targetBuffer, int offset, IMarshalProvider marshalProvider)
        {
            marshalProvider.StructureToBytes(targetBuffer, offset, structure);
        }

        /// <summary>
        /// Loads the wrapped structure from the source buffer starting at the specififed offset
        /// </summary>
        /// <param name="sourceBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="marshalProvider"></param>
        public override void FromBinary(byte[] sourceBuffer, int offset, IMarshalProvider marshalProvider)
        {
            structure = marshalProvider.BytesToStructure<T>(sourceBuffer, offset);
        }
    }
}
