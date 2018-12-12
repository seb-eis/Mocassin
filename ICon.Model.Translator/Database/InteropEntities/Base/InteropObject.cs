using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Non generic base class for all wrapped interop objects that can switch between object and binary unmanaged
    ///     representation
    /// </summary>
    public abstract class InteropObject
    {
        /// <summary>
        ///     Get the size of the binary state of the object
        /// </summary>
        public abstract int ByteCount { get; }

        /// <summary>
        ///     Write the internal object information to its unmanaged representation using the provided target buffer
        /// </summary>
        /// <param name="targetBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="marshalProvider"></param>
        public abstract void ToBinary(byte[] targetBuffer, int offset, IMarshalProvider marshalProvider);

        /// <summary>
        ///     Read the internal object information from its unmanaged representation using the provided source buffer
        /// </summary>
        /// <param name="sourceBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="marshalProvider"></param>
        public abstract void FromBinary(byte[] sourceBuffer, int offset, IMarshalProvider marshalProvider);

        /// <summary>
        ///     Factory method that creates an interop object wrapper for the passed structure
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static InteropObject<TStruct> Create<TStruct>(in TStruct structure) where TStruct : struct
        {
            return new InteropObject<TStruct>(structure);
        }
    }

    /// <summary>
    ///     Generic adapter for wrapping interop structures into a net object. Supports marshalling of the wrapped structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [NotMapped]
    public class InteropObject<T> : InteropObject where T : struct
    {
        /// <summary>
        ///     The wrapped structure
        /// </summary>
        private T _structure;

        /// <summary>
        ///     Create new interop object base wrapping the passed structure
        /// </summary>
        /// <param name="structure"></param>
        public InteropObject(T structure)
        {
            _structure = structure;
        }

        public InteropObject()
        {
        }

        /// <summary>
        ///     Reference access to the wrapped structure
        /// </summary>
        public ref T Structure => ref _structure;

        /// <inheritdoc />
        public override int ByteCount => Marshal.SizeOf(typeof(T));

        /// <inheritdoc />
        public override void ToBinary(byte[] targetBuffer, int offset, IMarshalProvider marshalProvider)
        {
            marshalProvider.StructureToBytes(targetBuffer, offset, _structure);
        }

        /// <inheritdoc />
        public override void FromBinary(byte[] sourceBuffer, int offset, IMarshalProvider marshalProvider)
        {
            _structure = marshalProvider.BytesToStructure<T>(sourceBuffer, offset);
        }
    }
}