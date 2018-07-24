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
    public class PairEnergyMatrix : EntityBase
    {
        /// <summary>
        /// The mccs energy info entity key
        /// </summary>
        [ForeignKey(nameof(MccsEnergyInfo))]
        public int MccsEnergyInfoId { get; set; }

        /// <summary>
        /// Navigation property for the mccs energy info this pair table belongs to
        /// </summary>
        public McsEnergies MccsEnergyInfo { get; set; }

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
