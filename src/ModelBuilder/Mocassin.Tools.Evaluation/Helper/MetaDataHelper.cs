using System;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;

namespace Mocassin.Tools.Evaluation.Helper
{
    /// <summary>
    ///     Provides static methods that help with <see cref="IJobMetaData"/>
    /// </summary>
    public static class MetaDataHelper
    {
        /// <summary>
        ///     Determines the number of unit cells from an <see cref="IJobMetaData" />
        /// </summary>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public static int GetNumberOfUnitCells(IJobMetaData metaData)
        {
            var split = metaData.LatticeInfo.Split(',');
            if (split.Length < 3) throw new InvalidOperationException("The lattice info in the database is corrupt.");
            return split.Sum(int.Parse);
        }
    }
}