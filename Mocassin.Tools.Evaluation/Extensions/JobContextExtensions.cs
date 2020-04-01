using System;
using System.Linq.Expressions;
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
        ///     Get a <see cref="CrystalVector4D"/> that describes the size information of the <see cref="JobContext"/> simulation lattice
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CrystalVector4D GetLatticeSizes(this JobContext value)
        {
            if (value.JobModel.SimulationLatticeModel.LatticeInfo == null)
            {
                var marshalService = new MarshalService();
                value.JobModel.SimulationLatticeModel.ChangePropertyStatesToObjects(marshalService);
            }
            var raw = value.JobModel.SimulationLatticeModel.LatticeInfo.Structure.SizeVector;
            return new CrystalVector4D(raw.A, raw.B, raw.C, raw.D);
        }
    }
}