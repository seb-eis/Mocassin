using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Translator;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Extensions
{
    /// <summary>
    ///     Provides useful extension methods for the
    /// </summary>
    public static class JobContextExtensions
    {
        /// <summary>
        ///     Get a <see cref="Vector4I" /> that describes the size information of the <see cref="JobContext" /> simulation
        ///     lattice
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector4I GetLatticeSizes(this JobContext value)
        {                   
            if (value.JobModel.SimulationLatticeModel.LatticeInfo == null)
            {
                var marshalService = new MarshalService();
                value.JobModel.SimulationLatticeModel.ChangePropertyStatesToObjects(marshalService);
            }

            var raw = value.JobModel.SimulationLatticeModel.LatticeInfo.Structure.SizeVector;
            return new Vector4I(raw.A, raw.B, raw.C, raw.D);
        }
    }
}