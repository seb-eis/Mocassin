using System;
using System.Reflection;
using System.Text;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Routine data class that is used to encode a custom routine identification and parameters
    /// </summary>
    public abstract class RoutineDataEntity : BlobEntityBase
    {
        /// <summary>
        ///     Get the <see cref="Guid" /> that identifies the routine
        /// </summary>
        public abstract Guid RoutineGuid { get; }

        /// <summary>
        ///     Get or set the <see cref="InteropObject{T}" /> that contains the routine parameter data
        /// </summary>
        protected InteropObject InternalParameterObject { get; set; }

        /// <inheritdoc />
        public override int BlobByteCount => InternalParameterObject?.ByteCount ?? 0;

        /// <inheritdoc />
        public override int HeaderByteCount => 16;

        /// <inheritdoc />
        public override void ChangeStateToBinary(IMarshalService marshalService)
        {
            if (InternalParameterObject == null) throw new InvalidOperationException("Parameter object is null.");

            var buffer = new byte[BlobByteCount];
            Buffer.BlockCopy(RoutineGuid.ToByteArray(), 0, buffer, 0, 16);
            InternalParameterObject.ToBinary(buffer, 16, marshalService);
            BinaryState = buffer;

            InternalParameterObject = null;
        }

        /// <inheritdoc />
        public override void ChangeStateToObject(IMarshalService marshalService)
        {
            if (BinaryState == null) throw new InvalidOperationException("Binary state is null.");
            InternalParameterObject = GetInteropObjectFromBinaryState(marshalService);
            BinaryState = null;
        }

        /// <summary>
        ///     Method to extract the actual <see cref="InteropObject" /> from the binary state using the provided
        ///     <see cref="IMarshalService" />
        /// </summary>
        /// <param name="marshalService"></param>
        /// <returns></returns>
        protected abstract InteropObject GetInteropObjectFromBinaryState(IMarshalService marshalService);
    }

    /// <summary>
    ///     Generic routine data class that is used to encode a custom routine identification and parameters
    /// </summary>
    public abstract class RoutineDataEntity<T> : RoutineDataEntity where T : struct
    {
        /// <inheritdoc />
        public override Guid RoutineGuid => GetGuidInternal();

        /// <summary>
        ///     Get or set the <see cref="InteropObject{T}"/> that stores the routine parameters
        /// </summary>
        public InteropObject<T> ParameterObject
        {
            get => (InteropObject<T>) InternalParameterObject;
            set => InternalParameterObject = value;
        }

        /// <inheritdoc />
        protected override InteropObject GetInteropObjectFromBinaryState(IMarshalService marshalService)
        {
            return InteropObject.Create(marshalService.GetStructure<T>(BinaryState, 16));
        }

        /// <summary>
        ///     Internal GUI lookup that uses the <see cref="MocsimExtensionAttribute"/> marker, returns the empty <see cref="Guid"/> if the marker is missing
        /// </summary>
        /// <returns></returns>
        protected virtual Guid GetGuidInternal()
        {
            return GetType().GetCustomAttribute<MocsimExtensionAttribute>()?.ExtensionGuid ?? Guid.Empty;
        }
    }
}