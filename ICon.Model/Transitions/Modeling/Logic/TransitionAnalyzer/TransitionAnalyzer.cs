using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.Comparers;
using ICon.Framework.Collections;
using ICon.Symmetry.SpaceGroups;
using ICon.Framework.Extensions;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Bundles analysis functions for both kinetic and metropolis transitions
    /// </summary>
    public class TransitionAnalyzer
    {
        /// <summary>
        /// Checks if a transition geometry described by 4D vectors contains a cycle or ring transition
        /// </summary>
        /// <param name="geometryVectors"></param>
        /// <returns></returns>
        public bool ContainsRingTransition(IEnumerable<CrystalVector4D> geometryVectors)
        {
            CrystalVector4D relative = new CrystalVector4D(0, 0, 0, 0), last = new CrystalVector4D(0, 0, 0, 0);
            foreach (var vector in geometryVectors)
            {
                relative += vector - last;
                if (relative.Equals(CrystalVector4D.NullVector))
                {
                    return true;
                }
                last = vector;
            }
            return false;
        }

        /// <summary>
        /// Cehcks if a seqeunce of position vectors describe or contain a ring transition with the provided vector comparer
        /// </summary>
        /// <param name="positionGeometry"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public bool ContainsRingTransition(IEnumerable<Fractional3D> positionGeometry, IComparer<Fractional3D> equalityComparer)
        {
            var current = new Fractional3D(0, 0, 0);
            foreach (var vector in positionGeometry.ConsecutivePairSelect((a, b) => b - a))
            {
                current += vector;
                if (equalityComparer.Compare(current, Fractional3D.NullVector) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Takes a set of fractional refernce positions and creates a 2D map of all symmetry equivalent intermediate positions between each two consecutive vectors
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="spaceGroupService"></param>
        /// <returns></returns>
        public IList<SetList<Fractional3D>> GetEquivalentIntermediatePositions(IEnumerable<Fractional3D> geometry, ISpaceGroupService spaceGroupService)
        {
            var result = new List<SetList<Fractional3D>>();
            foreach (var item in geometry.ConsecutivePairSelect((first, second) => Fractional3D.GetMiddle(second, first)))
            {
                result.Add(spaceGroupService.GetAllWyckoffPositions(item));
            }
            return result;
        }
    }
}
