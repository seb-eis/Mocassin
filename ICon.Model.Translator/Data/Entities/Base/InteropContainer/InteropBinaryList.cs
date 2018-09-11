using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using ICon.Framework.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Interop list of structs to exchange linear lists of values between managed and unmanaged code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InteropBinaryList<T> : BlobEntityBase where T : struct
    {
        /// <summary>
        /// Wrapped list of objects used to create the array
        /// </summary>
        [NotMapped]
        public IList<T> Values { get; set; }

        public override int BlobSize { get => GetBinarySizeOfValueList(); protected set => base.BlobSize = value; }

        /// <summary>
        /// Get or set the heaedr size. Return always 0 and throws on set
        /// </summary>
        public override int HeaderSize
        {
            get { return 0; }
            protected set { throw new InvalidOperationException("Header size is read only"); }
        }

        /// <summary>
        /// Changes the warpped list into a binary array format
        /// </summary>
        public override void ChangeStateToBinary(IMarshalProvider marshalProvider)
        {
            if (Values == null)
            {
                throw new InvalidOperationException("Translation list is null");
            }

            BinaryState = new byte[GetBinarySizeOfValueList()];
            marshalProvider.ManyStructuresToBytes(BinaryState, 0, Values);
        }

        /// <summary>
        /// Changes the set binary array format into a list of values
        /// </summary>
        public override void ChangeStateToObject(IMarshalProvider marshalProvider)
        {
            if (BinaryState == null)
            {
                throw new InvalidOperationException("Binary state is null");
            }

            Values = marshalProvider.BytesToManyStructures<T>(BinaryState, 0, BinaryState.Length).ToList();
        }

        /// <summary>
        /// Calculates the binary object size for the current value list
        /// </summary>
        /// <returns></returns>
        public int GetBinarySizeOfValueList()
        {
            return Marshal.SizeOf(typeof(T)) * (Values?.Count ?? 0);
        }
         
        /// <summary>
        /// Calculates the number of objects in the current binary state
        /// </summary>
        /// <returns></returns>
        public int GetItemCountOfBinaryState()
        {
            return (BinaryState?.Length ?? 0) / Marshal.SizeOf<T>();
        }
    }
}
