using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using ICon.Framework.Collections;
using ICon.Framework.Extensions;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Energy matrix entity that carries the pair energy information for a specific pair interaction set
    /// </summary>
    public class PairEnergyTable : EntityBase
    {
        /// <summary>
        /// The simulation object id for the unmanaged simulation context
        /// </summary>
        public int McsObjectId { get; set; }

        /// <summary>
        /// The mccs energy info entity key
        /// </summary>
        [ForeignKey(nameof(McsEnergies))]
        public int McsEnergiesId { get; set; }

        /// <summary>
        /// Navigation property for the mccs energy info this pair table belongs to
        /// </summary>
        public McsEnergies McsEnergies { get; set; }

        /// <summary>
        /// The energy matrix blob entity uid
        /// </summary>
        [ForeignKey(nameof(Matrix))]
        public int MatrixId { get; set; }

        /// <summary>
        /// The 2D energy matrix that assignes each element to element combo an energy value
        /// </summary>
        public MatrixEntity<double> Matrix { get; set; }
    }
}
