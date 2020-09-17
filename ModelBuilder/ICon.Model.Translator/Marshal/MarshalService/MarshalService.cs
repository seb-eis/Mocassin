using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Translator
{
    /// <inheritdoc cref="Mocassin.Model.Translator.IMarshalService" />
    public class MarshalService : IMarshalService
    {
        /// <summary>
        ///     Object to look the target pool
        /// </summary>
        private object PoolLock { get; } = new object();

        /// <summary>
        ///     Marshal target pool dictionary. Holds list of unmanaged marshal memory providers for each type
        /// </summary>
        private Dictionary<Type, List<MarshalMemoryProvider>> MarshalTargetPool { get; }

        /// <summary>
        ///     Create new default marshal provider
        /// </summary>
        public MarshalService()
        {
            MarshalTargetPool = new Dictionary<Type, List<MarshalMemoryProvider>>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var item in MarshalTargetPool)
            {
                foreach (var target in item.Value)
                    target.Dispose();
            }
        }

        /// <inheritdoc />
        public TStruct GetStructure<TStruct>(byte[] buffer, int offset) where TStruct : struct => (TStruct) GetStructure(buffer, offset, typeof(TStruct));

        /// <inheritdoc />
        public object GetStructure(byte[] buffer, int offset, Type structType)
        {
            using var target = GetMemoryProvider(structType);
            Marshal.Copy(buffer, offset, target.Pointer, target.TypeSize);
            return Marshal.PtrToStructure(target.Pointer, structType);
        }

        /// <inheritdoc />
        public IEnumerable<TStruct> GetStructures<TStruct>(byte[] buffer, int offset, int upperBound)
            where TStruct : struct
        {
            using var target = GetMemoryProvider(typeof(TStruct));
            for (var i = offset; i < upperBound; i += target.TypeSize)
            {
                Marshal.Copy(buffer, i, target.Pointer, target.TypeSize);
                yield return Marshal.PtrToStructure<TStruct>(target.Pointer);
            }
        }

        /// <inheritdoc />
        public void GetBytes<TStruct>(byte[] buffer, int offset, in TStruct structure) where TStruct : struct
        {
            using var target = GetMemoryProvider(typeof(TStruct));
            Marshal.StructureToPtr(structure, target.Pointer, true);
            Marshal.Copy(target.Pointer, buffer, offset, target.TypeSize);
        }

        /// <inheritdoc />
        public void GetBytes<TStruct>(byte[] buffer, int offset, IEnumerable<TStruct> structures)
            where TStruct : struct
        {
            using var target = GetMemoryProvider(typeof(TStruct));
            var index = offset;
            foreach (var item in structures)
            {
                Marshal.StructureToPtr(item, target.Pointer, true);
                Marshal.Copy(target.Pointer, buffer, index, target.TypeSize);
                index += target.TypeSize;
            }
        }

        /// <summary>
        ///     Gets a free marshal memory provider for the provided type
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public LockedMarshalMemory GetMemoryProvider(Type structType)
        {
            // ToDo: Change to a system that can be canceled on request 
            var targets = GetMemoryProviderList(structType);
            while (true)
            {
                foreach (var item in targets)
                {
                    var target = item.GetLocked();
                    if (target != null)
                        return target;
                }
            }
        }

        /// <summary>
        ///     Gets the marshal target list for the provided type
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        protected List<MarshalMemoryProvider> GetMemoryProviderList(Type structType)
        {
            lock (PoolLock)
            {
                if (MarshalTargetPool.TryGetValue(structType, out var list))
                    return list;

                list = new List<MarshalMemoryProvider>(4).Populate(() => GetNewMarshalMemoryProvider(structType), 4);
                MarshalTargetPool.Add(structType, list);
                return list;
            }
        }

        /// <summary>
        ///     Get a new marshal memory provider for the provided type
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public MarshalMemoryProvider GetNewMarshalMemoryProvider(Type structType)
        {
            var size = Marshal.SizeOf(structType);
            return new MarshalMemoryProvider(Marshal.AllocHGlobal(size), size);
        }
    }
}