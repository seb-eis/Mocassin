using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Mathematics.Extensions;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// The basic settings for the structure managing module data validations
    /// </summary>
    [DataContract]
    public class BasicStructureSettings
    {
        /// <summary>
        /// The value restriction setting for the structure base position count
        /// </summary>
        [DataMember]
        public ValueSetting<int> BasePositionCount { get; set; }

        /// <summary>
        /// The value restriction setting for the structure total position count (After application of space group wyckoff extension)
        /// </summary>
        [DataMember]
        public ValueSetting<int> TotalPositionCount { get; set; }

        /// <summary>
        /// The value restriction for the structure cell parameter length in [Angstrom]
        /// </summary>
        [DataMember]
        public ValueSetting<double> CellParameter { get; set; }

        /// <summary>
        /// The string restriction pattern for structure naming
        /// </summary>
        [DataMember]
        public string NameStringPattern { get; set; }
    }
}
