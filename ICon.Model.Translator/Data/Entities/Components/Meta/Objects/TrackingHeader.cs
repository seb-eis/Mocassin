using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// The tracking header information for the simulation that describes required trackers and access index offsets
    /// </summary>
    public class TrackingHeader : EntityBase
    {
        /// <summary>
        /// The mccs metra info entity id
        /// </summary>
        public int MccsMetaInfoId { get; set; }

        /// <summary>
        /// Navigation property for the mccs meta info this tracking header belongs to
        /// </summary>
        public MccsMetaInfo MccsMetaInfo { get; set; }

        /// <summary>
        /// Defines the total number of dynamic trackers that are required
        /// </summary>
        public int DynamicTrackerCount { get; set; }

        /// <summary>
        /// Defines the number of static trackers required per unit cell
        /// </summary>
        public int TrackersPerUnitCell { get; set; }

        /// <summary>
        /// Defines the total number of trackers for transition types that are required
        /// </summary>
        public int TypeTrackerCount { get; set; }

        /// <summary>
        /// Static index skips blob entity id
        /// </summary>
        [ForeignKey(nameof(StaticIndexMatrix))]
        public int StaticIndexMatrixId { get; set; }

        /// <summary>
        /// Static tracker indexing offsets for position (row) and particle (column) combination within their respective unit cell array
        /// </summary>
        public MatrixEntity<int> StaticIndexMatrix { get; set; }
    }
}
