using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Mathematics.Extensions;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// The basic settings for the structure managing module
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicStructureSettings
    {
        /// <summary>
        /// The maximum number of user added positions allowed (Before application of the space group position extensions)
        /// </summary>
        [DataMember(Name ="MaxBasePositions")]
        public Int32 BasePositionsLimit { get; set; }

        /// <summary>
        /// The maximum number of total positions allowed (After application of the space group position extensions)
        /// </summary>
        [DataMember(Name ="MaxPositions")]
        public Int32 TotalPositionsLimit { get; set; }

        /// <summary>
        /// The upper limit for the base vector length in arbitrary units
        /// </summary>
        [DataMember(Name ="MaxParameter")]
        public Double MaxBaseParameterLength { get; set; }

        /// <summary>
        /// The regular expression for the structure name
        /// </summary>
        [DataMember(Name ="Name")]
        public String NameRegex { get; set; }
    }
}
