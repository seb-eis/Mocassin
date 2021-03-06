﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Non generic base class for all wrapped interop objects that can switch between object and binary unmanaged
    ///     representation
    /// </summary>
    [NotMapped]
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
        /// <param name="marshalService"></param>
        public abstract void ToBinary(byte[] targetBuffer, int offset, IMarshalService marshalService);

        /// <summary>
        ///     Read the internal object information from its unmanaged representation using the provided source buffer
        /// </summary>
        /// <param name="sourceBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="marshalService"></param>
        public abstract void FromBinary(byte[] sourceBuffer, int offset, IMarshalService marshalService);

        /// <summary>
        ///     Factory method that creates an interop object wrapper for the passed structure
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static InteropObject<TStruct> Create<TStruct>(in TStruct structure) where TStruct : struct => new InteropObject<TStruct>(structure);
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
        private T structure;

        /// <summary>
        ///     Reference access to the wrapped structure
        /// </summary>
        public ref T Structure => ref structure;

        /// <inheritdoc />
        public override int ByteCount => Marshal.SizeOf(typeof(T));

        /// <summary>
        ///     Create new interop object base wrapping the passed structure
        /// </summary>
        /// <param name="structure"></param>
        public InteropObject(T structure)
        {
            this.structure = structure;
        }

        /// <summary>
        ///     Creates an <see cref="InteropObject" />
        /// </summary>
        public InteropObject()
        {
        }

        /// <inheritdoc />
        public override void ToBinary(byte[] targetBuffer, int offset, IMarshalService marshalService)
        {
            marshalService.GetBytes(targetBuffer, offset, structure);
        }

        /// <inheritdoc />
        public override void FromBinary(byte[] sourceBuffer, int offset, IMarshalService marshalService)
        {
            structure = marshalService.GetStructure<T>(sourceBuffer, offset);
        }
    }

    /// <summary>
    ///     Represents an interop object that has no bytes
    /// </summary>
    public sealed class EmptyInteropObject : InteropObject
    {
        /// <inheritdoc />
        public override int ByteCount => 0;

        /// <inheritdoc />
        public override void ToBinary(byte[] targetBuffer, int offset, IMarshalService marshalService)
        {
        }

        /// <inheritdoc />
        public override void FromBinary(byte[] sourceBuffer, int offset, IMarshalService marshalService)
        {
        }
    }
}