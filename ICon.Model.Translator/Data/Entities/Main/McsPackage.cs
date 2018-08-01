using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Mccs package entity that bundles multiple mccs parents and their jobs to describe a full simulation series
    /// </summary>
    public class McsPackage : EntityBase
    {
        /// <summary>
        /// The list of mccs parents affiliated wit the package
        /// </summary>
        public List<McsParent> MccsParents { get; set; }

        /// <summary>
        /// Creation time meta information about the package
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Description string for the simulation package
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The project Guid. Assigned by the project used to create the package
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// The package guid. Assigned by the project on creation of the package
        /// </summary>
        public string PackageGuid { get; set; }
    }
}
