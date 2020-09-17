using System.Runtime.Serialization;
using Mocassin.Model.Structures;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     The basic settings for the structure managing module data validations
    /// </summary>
    [DataContract, ModuleSettings(typeof(IStructureManager))]
    public class MocassinStructureSettings : MocassinModuleSettings
    {
        /// <summary>
        ///     The value restriction setting for the structure base position count
        /// </summary>
        [DataMember]
        public ValueSetting<int> BasePositionCount { get; set; }

        /// <summary>
        ///     The value restriction setting for the structure total position count (After application of space group wyckoff
        ///     extension)
        /// </summary>
        [DataMember]
        public ValueSetting<int> TotalPositionCount { get; set; }

        /// <summary>
        ///     The value restriction for the structure cell parameter length in [Angstrom]
        /// </summary>
        [DataMember]
        public ValueSetting<double> CellParameter { get; set; }


        /// <inheritdoc />
        public override void InitAsDefault()
        {
            BasePositionCount = new ValueSetting<int>("Base Position Count", 0, 1000);
            TotalPositionCount = new ValueSetting<int>("Total Position Count", 0, 10000);
            CellParameter = new ValueSetting<double>("Cell Parameter Length", 0.1, 1000);
        }
    }
}