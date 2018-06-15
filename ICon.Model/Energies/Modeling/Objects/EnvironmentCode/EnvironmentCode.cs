using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Environment code that specifies the used position environment through the position index 'P' and an additional environment index
    /// </summary>
    public readonly struct EnvironmentCode
    {
        /// <summary>
        /// Index of the position 'P' in the extended and sorted wyckoff position set of the whole unit cell
        /// </summary>
        public int PositionIndex { get; }

        /// <summary>
        /// Index of the environment within the position index. Differs from 0 only if multiple transition environments exist for an unstable position
        /// </summary>
        public int EnvironmentIndex { get; }

        /// <summary>
        /// Creates new environment code from position index and envrionment index
        /// </summary>
        /// <param name="positionIndex"></param>
        /// <param name="environmentIndex"></param>
        public EnvironmentCode(int positionIndex, int environmentIndex) : this()
        {
            PositionIndex = positionIndex;
            EnvironmentIndex = environmentIndex;
        }
    }
}
