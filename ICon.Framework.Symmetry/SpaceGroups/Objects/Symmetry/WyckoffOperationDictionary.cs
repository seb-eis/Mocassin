using System;
using System.Linq;
using System.Collections.Generic;

using ICon.Mathematics.ValueTypes;
using ICon.Framework.Collections;
using System.Collections;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// A wyckoff operation overlay that describes which operations belong to which symmetry equivalent position
    /// </summary>
    public class WyckoffOperationDictionary : IWyckoffOperationDictionary
    {
        /// <summary>
        /// The space group the operation dictionary belongs to
        /// </summary>
        public ISpaceGroup SpaceGroup { get; set; }

        /// <summary>
        /// The source position that was used to create the overlay
        /// </summary>
        public Fractional3D SourcePosition { get; set; }

        /// <summary>
        /// Sorted position dictionary that holds a list of operations for each of the wyckoff 1 positions
        /// </summary>
        public SortedDictionary<Fractional3D, SetList<ISymmetryOperation>> OperationDictionary { get; set; }

        /// <summary>
        /// Get all dictionary keys
        /// </summary>
        public IEnumerable<Fractional3D> Keys => OperationDictionary.Keys;

        /// <summary>
        /// Get all dictionary operation sequences
        /// </summary>
        public IEnumerable<IEnumerable<ISymmetryOperation>> Values => OperationDictionary.Values;

        /// <summary>
        /// Get the number of dictionary entries
        /// </summary>
        public int Count => OperationDictionary.Count;

        /// <summary>
        /// Get the operations for the specfifed position
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<ISymmetryOperation> this[Fractional3D key] => OperationDictionary[key].AsEnumerable();

        /// <summary>
        /// Creates new wyckoff operation dictionary with the passed source position and operation dictionary
        /// </summary>
        /// <param name="sourcePosition"></param>
        /// <param name="operationDictionary"></param>
        public WyckoffOperationDictionary(Fractional3D sourcePosition, ISpaceGroup spaceGroup, SortedDictionary<Fractional3D, SetList<ISymmetryOperation>> operationDictionary)
        {
            SourcePosition = sourcePosition;
            SpaceGroup = spaceGroup ?? throw new ArgumentNullException(nameof(spaceGroup));
            OperationDictionary = operationDictionary ?? throw new ArgumentNullException(nameof(operationDictionary));
        }

        /// <summary>
        /// Checks if the key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Fractional3D key)
        {
            return OperationDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Tries to get the value with the specfified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the generic enumerator for the dictionary
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<Fractional3D, IEnumerable<ISymmetryOperation>>> GetEnumerator()
        {
            foreach (var item in OperationDictionary)
            {
                yield return new KeyValuePair<Fractional3D, IEnumerable<ISymmetryOperation>>(item.Key, item.Value.AsEnumerable());
            }
        }

        /// <summary>
        /// Returns the enumerator for the dictionary
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
