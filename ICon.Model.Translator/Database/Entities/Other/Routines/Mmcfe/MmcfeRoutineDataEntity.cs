namespace Mocassin.Model.Translator.Routines
{
    /// <summary>
    ///     The <see cref="RoutineDataEntity{T}" /> for providing parameter data & identification for the the MMCFE routine
    /// </summary>
    [MocsimExtensionComponent("b7f2dded-daf1-40c0-4d4d-434645000000", "mmcfe")]
    public class MmcfeRoutineDataEntity : RoutineDataEntity<CMmcfeParams>
    {
        /// <summary>
        ///     Creates new <see cref="MmcfeRoutineDataEntity" /> with default parameter settings
        /// </summary>
        public MmcfeRoutineDataEntity()
        {
            var structure = new CMmcfeParams
            {
                HistogramSize = 10000,
                AlphaCount = 50,
                AlphaMin = 0,
                AlphaMax = 1,
                AlphaCurrent = 0,
                HistogramRange = 100,
                RelaxPhaseCycleCount = 20000000,
                LogPhaseCycleCount = 100000000
            };
            InternalParameterObject = InteropObject.Create(structure);
        }
    }
}