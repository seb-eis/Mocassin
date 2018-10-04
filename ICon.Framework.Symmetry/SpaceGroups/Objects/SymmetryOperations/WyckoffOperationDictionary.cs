using System;
using System.Linq;
using System.Collections.Generic;

using ICon.Mathematics.ValueTypes;
using ICon.Framework.Collections;
using System.Collections;

namespace ICon.Symmetry.SpaceGroups
{
    /// <inheritdoc />
    public class WyckoffOperationDictionary : IWyckoffOperationDictionary
    {
        /// <inheritdoc />
        public ISpaceGroup SpaceGroup { get; set; }

        /// <inheritdoc />
        public Fractional3D SourcePosition { get; set; }

        /// <summary>
        /// Sorted position dictionary that holds a list of operations for each of the wyckoff 1 positions
        /// </summary>
        public SortedDictionary<Fractional3D, SetList<ISymmetryOperation>> OperationDictionary { get; set; }

        /// <inheritdoc />
        public IEnumerable<Fractional3D> Keys => OperationDictionary.Keys;

        /// <inheritdoc />
        public IEnumerable<IEnumerable<ISymmetryOperation>> Values => OperationDictionary.Values;

        /// <inheritdoc />
        public int Count => OperationDictionary.Count;


        /// <inheritdoc />
        public IEnumerable<ISymmetryOperation> this[Fractional3D key] => OperationDictionary[key].AsEnumerable();

        /// <summary>
        /// Creates new wyckoff operation dictionary with the passed source position and operation dictionary
        /// </summary>
        /// <param name="sourcePosition"></param>
        /// <param name="spaceGroup"></param>
        /// <param name="operationDictionary"></param>
        public WyckoffOperationDictionary(Fractional3D sourcePosition, ISpaceGroup spaceGroup, SortedDictionary<Fractional3D, SetList<ISymmetryOperation>> operationDictionary)
        {
            SourcePosition = sourcePosition;
            SpaceGroup = spaceGroup ?? throw new ArgumentNullException(nameof(spaceGroup));
            OperationDictionary = operationDictionary ?? throw new ArgumentNullException(nameof(operationDictionary));
        }

        /// <inheritdoc />
        public bool ContainsKey(Fractional3D key)
        {
            return OperationDictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(Fractional3D key, out IEnumerable<ISymmetryOperation> value)
        {
            if (OperationDictionary.TryGetValue(key, out var results))
            {
                value = results.AsEnumerable();
                return true;
            }
            value = default;
            return false;
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Fractional3D, IEnumerable<ISymmetryOperation>>> GetEnumerator()
        {
            foreach (var item in OperationDictionary)
            {
                yield return new KeyValuePair<Fractional3D, IEnumerable<ISymmetryOperation>>(item.Key, item.Value.AsEnumerable());
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
