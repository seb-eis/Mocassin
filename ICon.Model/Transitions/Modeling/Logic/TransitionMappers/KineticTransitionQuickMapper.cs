using System;
using System.Collections.Generic;

using ICon.Symmetry.SpaceGroups;
using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Coordinates;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Quick kinetic transition mapper that uses a space group service interafce to create the kinetic mappings
    /// </summary>
    public class KineticTransitionQuickMapper
    {
        /// <summary>
        /// The space group service that provides the symmetry operations
        /// </summary>
        protected ISpaceGroupService SpaceGroupService { get; }

        /// <summary>
        /// The vector encoder to switch between fractional and 4D encoded coordinates
        /// </summary>
        protected UnitCellVectorEncoder VectorEncoder { get; }

        /// <summary>
        /// Creates new kinetic transition quick mapper that uses the provided space group service and vector encoder
        /// </summary>
        /// <param name="spaceGroupService"></param>
        /// <param name="vectorEncoder"></param>
        public KineticTransitionQuickMapper(ISpaceGroupService spaceGroupService, UnitCellVectorEncoder vectorEncoder)
        {
            SpaceGroupService = spaceGroupService ?? throw new ArgumentNullException(nameof(spaceGroupService));
            VectorEncoder = vectorEncoder ?? throw new ArgumentNullException(nameof(vectorEncoder));
        }

        /// <summary>
        /// Creates all kinetic transition mappings for the provided transition interface
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public IEnumerable<KineticMapping> GetMappings(IKineticTransition transition)
        {
            return GetMappings(transition.GetGeometrySequence(), transition);
        }

        /// <summary>
        /// Takes the provided reference geometry of a transition in fractional position information and creates all symmetry equivalent mappings
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public IEnumerable<KineticMapping> GetMappings(IEnumerable<Fractional3D> geometry, IKineticTransition transition)
        {
            foreach (var fractionalSequence in SpaceGroupService.GetAllWyckoffOriginSequences(geometry))
            {
                if (VectorEncoder.TryEncodeFractional(fractionalSequence, out List<CrystalVector4D> encodedSequence))
                {
                    yield return new KineticMapping(transition, encodedSequence.ToArray(), fractionalSequence);
                }
                else
                {
                    throw new InvalidOperationException("Mapping encoding from 3D to 4D failed. Vector encoder and space group service are not synchronized!");
                }
            }
        }
    }
}
