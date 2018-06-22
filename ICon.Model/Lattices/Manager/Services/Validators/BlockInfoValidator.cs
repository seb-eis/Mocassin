using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Framework.Messaging;
using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Lattices.Validators
{
    /// <summary>
    /// Validator for new BlockInfo model objects that checks for consistency and compatibility with existing data and general object constraints
    /// </summary>
    public class BlockInfoValidator : DataValidator<IBlockInfo, BasicLatticeSettings, ILatticeDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public BlockInfoValidator(IProjectServices projectServices, BasicLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(projectServices, settings, dataReader)
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
        protected void AddNegativeVectorValidation(DataIntVector3D vector, ValidationReport report)
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
        protected void AddNegativeSizeValidation(DataIntVector3D origin, DataIntVector3D extent, ValidationReport report)
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
        protected void AddOverhangValidation(DataIntVector3D vector, ValidationReport report)
        {
            var latticeInfo = DataReader.Access.GetLatticeInfo();

            if (vector.A > latticeInfo.Extent.A || vector.B > latticeInfo.Extent.B || vector.B > latticeInfo.Extent.B)
            {
                var message = new WarningMessage(this, "BlockInfo vector validation failure") { IsCritical = true };
                message.Details.Add($"A component of a BlockInfo vector is larger than the corresponding component of the super cell vector");
                report.AddWarning(message);
            }
        }
    }
}
