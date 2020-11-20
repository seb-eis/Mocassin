using System;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.Evaluation.Queries.Data;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Query to extract the <see cref="LatticeMetaData" /> from a <see cref="IEvaluableJobSet" />
    /// </summary>
    public class LatticeMetaEvaluation : JobEvaluation<LatticeMetaData>
    {
        /// <inheritdoc />
        public LatticeMetaEvaluation(IEvaluableJobSet jobSet)
            : base(jobSet)
        {
        }

        /// <inheritdoc />
        protected override LatticeMetaData GetValue(JobContext jobContext)
        {
            var vectorEncoder = jobContext.ModelContext.GetUnitCellVectorEncoder();
            var sizeInfo = GetLatticeSizeInfo(jobContext, vectorEncoder);
            var volume = vectorEncoder.GetCellVolume() * sizeInfo.A * sizeInfo.B * sizeInfo.C *
                         Math.Pow(UnitConversions.Length.AngstromToMeter, 3);

            return new LatticeMetaData(sizeInfo, volume);
        }

        /// <summary>
        ///     Extracts the size information of the lattice from the passed <see cref="JobContext" /> and
        ///     <see cref="IUnitCellVectorEncoder" />
        /// </summary>
        /// <param name="jobContext"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        protected Vector4I GetLatticeSizeInfo(JobContext jobContext, IUnitCellVectorEncoder vectorEncoder)
        {
            var split = jobContext.JobModel.JobMetaData.LatticeInfo.Split(',');
            if (split.Length < 3) throw new InvalidOperationException("Invalid format of lattice size string.");
            return new Vector4I(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), vectorEncoder.PositionCount);
        }
    }
}