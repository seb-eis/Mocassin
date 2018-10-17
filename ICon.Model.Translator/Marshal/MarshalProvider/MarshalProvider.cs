using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Translator
{
    /// <inheritdoc cref="IMarshalProvider"/>
    public class MarshalProvider : IDisposable, IMarshalProvider
    {
        /// <summary>
        /// Object to look the target pool
        /// </summary>
        private object PoolLock { get; } = new object();

        /// <summary>
        /// Marshal target pool dictionary. Holds list of unmanaged marshal targets for each type
        /// </summary>
        private Dictionary<Type, List<MarshalTarget>> MarshalTargetPool { get; }

        /// <summary>
        /// Create new default marshal provider
        /// </summary>
        public MarshalProvider()
        {
            MarshalTargetPool = new Dictionary<Type, List<MarshalTarget>>();
        }

        /// <inheritdoc/>
        public TStruct BytesToStructure<TStruct>(byte[] buffer, int offset) where TStruct : struct
        {
            return (TStruct) BytesToStructure(buffer, offset, typeof(TStruct));
        }

        /// <inheritdoc/>
        public object BytesToStructure(byte[] buffer, int offset, Type structType)
        {
            using (var target = GetMarshalTarget(structType))
            {
                Marshal.Copy(buffer, offset, target.Pointer, target.TypeSize);
                return Marshal.PtrToStructure(target.Pointer, structType);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TStruct> BytesToManyStructures<TStruct>(byte[] buffer, int offset, int upperBound)
            where TStruct : struct
        {
            using (var target = GetMarshalTarget(typeof(TStruct)))
            {
                for (int i = offset; i < upperBound; i += target.TypeSize)
                {
                    Marshal.Copy(buffer, i, target.Pointer, target.TypeSize);
                    yield return Marshal.PtrToStructure<TStruct>(target.Pointer);
                }
            }
        }

        /// <inheritdoc/>
        public void StructureToBytes<TStruct>(byte[] buffer, int offset, in TStruct structure) where TStruct : struct
        {
            using (var target = GetMarshalTarget(typeof(TStruct)))
            {
                Marshal.StructureToPtr(structure, target.Pointer, true);
                Marshal.Copy(target.Pointer, buffer, offset, target.TypeSize);
            }
        }

        /// <inheritdoc/>
        public void ManyStructuresToBytes<TStruct>(byte[] buffer, int offset, IEnumerable<TStruct> structures)
            where TStruct : struct
        {
            using (var target = GetMarshalTarget(typeof(TStruct)))
            {
                int index = offset;
                foreach (var item in structures)
                {
                    Marshal.StructureToPtr(item, target.Pointer, true);
                    Marshal.Copy(target.Pointer, buffer, index, target.TypeSize);
                    index += target.TypeSize;
                }
            }
        }

        /// <summary>
        /// Gets a free marshal target for the provided type
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public LockedMarshalTarget GetMarshalTarget(Type structType)
        {
            var targets = GetMarshalTargetList(structType);
            while (true)
            {
                foreach (var item in targets)
                {
                    var target = item.GetLocked();
                    if (target != null)
                    {
                        return target;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the marshal target list for the provided type
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        protected List<MarshalTarget> GetMarshalTargetList(Type structType)
        {
            lock (PoolLock)
            {
                if (MarshalTargetPool.TryGetValue(structType, out var list)) return list;

                list = new List<MarshalTarget>(4).Populate(() => GetNewMarshalTarget(structType), 4);
                MarshalTargetPool.Add(structType, list);
                return list;
            }
        }

        /// <summary>
        /// Get a new marshal target for the provided type
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public MarshalTarget GetNewMarshalTarget(Type structType)
        {
            var size = Marshal.SizeOf(structType);
            return new MarshalTarget(Marshal.AllocHGlobal(size), size);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var item in MarshalTargetPool)
            {
                foreach (var target in item.Value)
                {
                    Marshal.FreeHGlobal(target.Pointer);
                }
            }
        }
    }
}