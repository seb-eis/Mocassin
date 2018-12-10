using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Interop list of structs to exchange linear lists of values between managed and unmanaged code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InteropList<T> : BlobEntityBase where T : struct
    {
        /// <summary>
        ///     Wrapped list of objects used to create the array
        /// </summary>
        [NotMapped]
        public IList<T> Values { get; set; }

        /// <inheritdoc />
        public override int BlobByteCount
        {
            get => GetBinarySizeOfValueList();
            protected set => base.BlobByteCount = value;
        }

        /// <inheritdoc />
        public override int HeaderByteCount
        {
            get => 0;
            protected set => throw new InvalidOperationException("Header size is read only");
        }

        /// <inheritdoc />
        public override void ChangeStateToBinary(IMarshalProvider marshalProvider)
        {
            if (Values == null)
                throw new InvalidOperationException("Translation list is null");

            BinaryState = new byte[GetBinarySizeOfValueList()];
            marshalProvider.ManyStructuresToBytes(BinaryState, 0, Values);
        }

        /// <inheritdoc />
        public override void ChangeStateToObject(IMarshalProvider marshalProvider)
        {
            if (BinaryState == null)
                throw new InvalidOperationException("Binary state is null");

            Values = marshalProvider.BytesToManyStructures<T>(BinaryState, 0, BinaryState.Length).ToList();
        }

        /// <summary>
        ///     Calculates the binary object size for the current value list
        /// </summary>
        /// <returns></returns>
        public int GetBinarySizeOfValueList()
        {
            return Marshal.SizeOf(typeof(T)) * (Values?.Count ?? 0);
        }

        /// <summary>
        ///     Calculates the number of objects in the current binary state
        /// </summary>
        /// <returns></returns>
        public int GetItemCountOfBinaryState()
        {
            return (BinaryState?.Length ?? 0) / Marshal.SizeOf<T>();
        }
    }
}