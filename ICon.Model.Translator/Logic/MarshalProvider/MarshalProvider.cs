using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using ICon.Framework.Extensions;

namespace ICon.Model.Translator
{
    public class MarshalProvider : IDisposable
    {
        private Dictionary<Type, List<MarshalTarget>> MarshalTargetPool { get; }

        public MarshalProvider()
        {
            MarshalTargetPool = new Dictionary<Type, List<MarshalTarget>>();
        }

        public TStruct BytesToStructure<TStruct>(byte[] buffer, int offset) where TStruct : struct
        {
            return (TStruct)BytesToStructure(buffer, offset, typeof(TStruct));
        }

        public object BytesToStructure(byte[] buffer, int offset, Type structType)
        {
            using (var target = GetMarshalTarget(structType))
            {
                Marshal.Copy(buffer, offset, target.Pointer, target.TypeSize);
                return Marshal.PtrToStructure(target.Pointer, structType);
            }
        }

        public void StructureToBytes<TStruct>(byte[] buffer, int offset, in TStruct structure) where TStruct : struct
        {
            using (var target = GetMarshalTarget(typeof(TStruct)))
            {
                Marshal.StructureToPtr(structure, target.Pointer, true);
                Marshal.Copy(target.Pointer, buffer, offset, target.TypeSize);
            }
        }

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

        protected List<MarshalTarget> GetMarshalTargetList(Type structType)
        {
            if (!MarshalTargetPool.TryGetValue(structType, out var list))
            {
                list = new List<MarshalTarget>(4).Populate(() => GetNewMarshalTarget(structType), 4);
                MarshalTargetPool.Add(structType, list);
            }
            return list;
        }

        public MarshalTarget GetNewMarshalTarget(Type structType)
        {
            int size = Marshal.SizeOf(structType);
            return new MarshalTarget(Marshal.AllocHGlobal(size), size);
        }

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
