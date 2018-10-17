using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices.Validators
{
    /// <summary>
    /// Validator for new BlockInfo model objects that checks for consistency and compatibility with existing data and general object constraints
    /// </summary>
    public class BlockInfoValidator : DataValidator<IBlockInfo, MocassinLatticeSettings, ILatticeDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public BlockInfoValidator(IModelProject modelProject, MocassinLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new BlockInfo object in terms of consistency and compatibility with existing data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IBlockInfo obj)
        {
            ValidationReport report = new ValidationReport();
            AddNegativeVectorValidation(obj.Origin, report);
            AddNegativeVectorValidation(obj.Extent, report);
            AddNegativeSizeValidation(obj.Origin, obj.Extent, report);
            AddOverhangValidation(obj.Origin, report);
            AddOverhangValidation(obj.Extent, report);

            return report;
        }

        /// <summary>
        /// Validate if any component of a vector is smaller than zero
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="report"></param>
        protected void AddNegativeVectorValidation(DataIntegralVector3D vector, ValidationReport report)
        {
            if (vector.A < 0 || vector.B < 0 || vector.C < 0)
            {
                var message = new WarningMessage(this, "BuildingInfo vector validation failure") { IsCritical = true};
                message.Details.Add($"A component of a BlockInfo vector is smaller than 0");
                report.AddWarning(message);
            }
        }

        /// <summary>
        /// Validate if the origin vector is larger than the extent vector in any direction
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="extent"></param>
        /// <param name="report"></param>
        protected void AddNegativeSizeValidation(DataIntegralVector3D origin, DataIntegralVector3D extent, ValidationReport report)
        {
            if (extent.A < origin.A || extent.B < origin.B || extent.C < origin.C)
            {
                var message = new WarningMessage(this, "BlockInfo vector validation failure");
                message.Details.Add($"A component of the extent vector is smaller than the corresponding component of the origin vector");
                report.AddWarning(message);
            }
        }

        /// <summary>
        /// Validate if a vector is larger than the supercell in any direction
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="report"></param>
        protected void AddOverhangValidation(DataIntegralVector3D vector, ValidationReport report)
        {
            var latticeInfo = DataReader.Access.GetLatticeInfo();

            if (vector.A > latticeInfo.Extent.A || vector.B > latticeInfo.Extent.B || vector.B > latticeInfo.Extent.B)
            {
                var message = new WarningMessage(this, "BlockInfo vector validation failure") { IsCritical = true };
                message.Details.Add($"A component of a BlockInfo vector is larger than the corresponding component of the super cell vector");
                report.AddWarning(message);
            }
        }

        /// <summary>
        /// Validate the BlockID of the BlockInfo
        /// </summary>
        /// <param name="blockID"></param>
        /// <param name="report"></param>
        protected void AddBlockIDValidation(int blockID, ValidationReport report)
        {
            if (blockID < 0)
            {
                var message = new WarningMessage(this, "BlockInfo BlockID validation failure") { IsCritical = true };
                message.Details.Add($"The BuildingBlock ID of the BlockInfo is smaller than 0");
                report.AddWarning(message);
            }

            var buildingBlockCount = DataReader.Access.GetBuildingBlocks().Count;

            if (blockID > buildingBlockCount)
            {
                var message = new WarningMessage(this, "BlockInfo BlockID validation failure") { IsCritical = true };
                message.Details.Add($"The BuildingBlock ID of the BlockInfo is greater than the number of BuildingBlocks");
                report.AddWarning(message);
            }
        }
    }
}
