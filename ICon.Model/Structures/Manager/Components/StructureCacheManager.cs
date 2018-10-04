using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Collections;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.ValueTypes;
using ICon.Symmetry.Analysis;
using ICon.Model.Basic;
using ICon.Framework.Extensions;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Basic implementation of the structure cache manager that provides read only access to the extended 'on demand' structure data
    /// </summary>
    internal class StructureCacheManager : ModelCacheManager<StructureDataCache, IStructureCachePort>, IStructureCachePort
    {
        /// <inheritdoc />
        public StructureCacheManager(StructureDataCache cache, IProjectServices projectServices) : base(cache, projectServices)
        {

        }

        /// <inheritdoc />
        public SetList<FractionalPosition> GetExtendedPositionList(int index)
        {
            return GetExtendedPositionLists()[index];
        }

        /// <inheritdoc />
        public IList<SetList<FractionalPosition>> GetExtendedPositionLists()
        {
            return GetResultFromCache(CreateExtendedPositionLists);
        }

        /// <inheritdoc />
        public SetList<FractionalPosition> GetLinearizedExtendedPositionList()
        {
            return GetResultFromCache(CreateLinearizedExtendedPositionList);
        }

        /// <inheritdoc />
        public SetList<CrystalVector4D> GetEncodedExtendedPositionList(int index)
        {
            return GetEncodedExtendedPositionLists()[index];
        }

        /// <inheritdoc />
        public IList<SetList<CrystalVector4D>> GetEncodedExtendedPositionLists()
        {
            return GetResultFromCache(CreateExtendedEncodedPositionLists);
        }

        /// <inheritdoc />
        public IUnitCellVectorEncoder GetVectorEncoder()
        {
            return GetResultFromCache(CreateVectorEncoder);
        }

        /// <inheritdoc />
        public IUnitCellProvider<FractionalPosition> GetUnitCellProvider()
        {
            return GetResultFromCache(CreateUnitCellProvider);
        }

        /// <inheritdoc />
        public IUnitCellProvider<int> GetOccupationUnitCellProvider()
        {
            return GetResultFromCache(CreateOccupationUnitCellProvider);
        }

        /// <inheritdoc />
        public IUnitCellProvider<IUnitCellPosition> GetFullUnitCellProvider()
        {
            return GetResultFromCache(CreateFullUnitCellProvider);
        }

        /// <inheritdoc />
        public SetList<FractionalPosition> GetExtendedPositionList(in CrystalVector4D vector)
        {
            if (!GetVectorEncoder().TryDecode(vector, out Fractional3D decoded))
            {
                throw new ArgumentException("4D vector cannot be decoded into a valid 3D equivalent", nameof(vector));
            }
            return GetExtendedPositionLists()
                .SkipWhile(set => !set.Contains(new FractionalPosition(decoded, 0, PositionStatus.Undefined)))
                .First();
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<int, IUnitCellPosition> GetExtendedIndexToPositionDictionary()
        {
            return GetResultFromCache(CreateExtendedIndexToPositionDictionary);
        }

        /// <inheritdoc />
        public SetList<Fractional3D> GetExtendedDummyPositionList(int index)
        {
            return GetExtendedDummyPositionLists()[index];
        }

        /// <inheritdoc />
        public IList<SetList<Fractional3D>> GetExtendedDummyPositionLists()
        {
            return GetResultFromCache(CreateExtendedDummyPositionLists);
        }

        /// <summary>
        /// Creates the extended wyckoff position lists for all dummy positions. Deprecated dummies produce an empty list
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<SetList<Fractional3D>> CreateExtendedDummyPositionLists()
        {
            var positions = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetPositionDummies());
            return IConTaskingExtensions
                .RunAndGetResults(positions
                    .Select(value => MakeWyckoffExtensionDelegate(value.Vector, value.IsDeprecated)))
                .ToList();
        }

        /// <summary>
        /// Creates the wyckoff extended position lists for all unit cell positions
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<SetList<FractionalPosition>> CreateExtendedPositionLists()
        {
            var positions = ProjectServices.GetManager<IStructureManager>().QueryPort.Query(port => port.GetUnitCellPositions());
            return IConTaskingExtensions
                .RunAndGetResults(positions
                    .Select(value => MakeWyckoffExtensionDelegate(value.AsPosition(), value.IsDeprecated)))
                .ToList(); 
        }

        /// <summary>
        /// Creates the vector encoder for transformations between 3D and 4D systems
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IUnitCellVectorEncoder CreateVectorEncoder()
        {
            var sortedList = new SetList<Fractional3D>(new VectorComparer3D<Fractional3D>(ProjectServices.GeometryNumerics.RangeComparer))
            {
                GetLinearizedExtendedPositionList().Select(pos => new Fractional3D(pos.Coordinates))
            };
            return new UnitCellVectorEncoder(sortedList, ProjectServices.CrystalSystemService.VectorTransformer);
        }

        /// <summary>
        /// Creates the linearized version of the extended position list
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected SetList<FractionalPosition> CreateLinearizedExtendedPositionList()
        {
            var result = new SetList<FractionalPosition>(new VectorComparer3D<FractionalPosition>(ProjectServices.GeometryNumerics.RangeComparer));
            foreach (var subList in GetExtendedPositionLists())
            {
                result.Add(subList);
            }
            return result;
        }

        /// <summary>
        /// Creates all encoded versions of the extended position lists
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IList<SetList<CrystalVector4D>> CreateExtendedEncodedPositionLists()
        {
            var result = new List<SetList<CrystalVector4D>>();
            var encoder = GetVectorEncoder();
            foreach (var positionSet in GetExtendedPositionLists())
            {
                var list = new SetList<CrystalVector4D>(Comparer<CrystalVector4D>.Default);
                foreach (var position in positionSet)
                {
                    if (!encoder.TryEncode(position, out var encoded))
                    {
                        throw new InvalidOperationException("Encoding of basic unit cell vectors did not yield a valid 4D position");
                    }
                    list.Add(encoded);
                }
                result.Add(list);
            }
            return result;
        }

        /// <summary>
        /// Creates a sorted dictionary that assigns each extended position index the unit cell position that belongs to it
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected SortedDictionary<int, IUnitCellPosition> CreateExtendedIndexToPositionDictionary()
        {
            var result = new SortedDictionary<int, IUnitCellPosition>();
            var ucpList = ProjectServices
                .GetManager<IStructureManager>()
                .QueryPort
                .Query(port => port.GetUnitCellPositions());

            int index = 0;
            foreach (var extendedList in GetEncodedExtendedPositionLists())
            {
                foreach (var vector in extendedList)
                {
                    result.Add(vector.P, ucpList[index]);
                }
                index++;
            }
            return result;
        }

        /// <summary>
        /// Creates the unit cell provider for the extended unit cell
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IUnitCellProvider<FractionalPosition> CreateUnitCellProvider()
        {
            return CellWrapperFactory.CreateUnitCell(GetLinearizedExtendedPositionList(), GetVectorEncoder());
        }

        /// <summary>
        /// Creates the full unit cell provider
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IUnitCellProvider<IUnitCellPosition> CreateFullUnitCellProvider()
        {
            return CellWrapperFactory.CreateUnitCell(GetExtendedIndexToPositionDictionary().Values.ToList(), GetVectorEncoder());
        }

        /// <summary>
        /// Creates a unit cell provider that carries only occupations as entries
        /// </summary>
        /// <returns></returns>
        [CacheMethodResult]
        protected IUnitCellProvider<int> CreateOccupationUnitCellProvider()
        {
            return CellWrapperFactory.CreateUnitCell(GetLinearizedExtendedPositionList().Select(value => value.OccupationIndex).ToList(), GetVectorEncoder());
        }

        /// <summary>
        /// Creates a call delegate to create the extended position list of the provided vector or an empty set if the deprecated flag is passed
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="isDeprecated"></param>
        /// <returns></returns>
        protected Func<SetList<Fractional3D>> MakeWyckoffExtensionDelegate(Fractional3D vector, bool isDeprecated)
        {
            SetList<Fractional3D> Create()
            {
                return isDeprecated 
                    ? new SetList<Fractional3D>(ProjectServices.SpaceGroupService.Comparer)
                    : ProjectServices.SpaceGroupService.GetAllWyckoffPositions(vector);
            }

            return Create;
        }

        /// <summary>
        /// Creates a call delegate to create the extended position list of the provided fractional position or an empty set if the depreacted flag is passed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="isDeprecated"></param>
        /// <returns></returns>
        protected Func<SetList<FractionalPosition>> MakeWyckoffExtensionDelegate(FractionalPosition position, bool isDeprecated)
        {
            SetList<FractionalPosition> Create()
            {
                return isDeprecated 
                    ? new SetList<FractionalPosition>(ProjectServices.SpaceGroupService.GetSpecialVectorComparer<FractionalPosition>())
                    : ProjectServices.SpaceGroupService.GetAllWyckoffPositions(position);
            }

            return Create;
        }
    }
}
