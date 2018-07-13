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
            AddBasicVectorValidation(obj, report);
            AddExtentGreaterOriginValidation(obj, report);
            AddUnfittingSuperBlockSizeValidation(obj, report);

            return report;
        }

        /// <summary>
        /// Validate if the vectors of the BlockInfo follow the basic restrictions
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="report"></param>
        protected void AddBasicVectorValidation(IBlockInfo blockInfo, ValidationReport report)
        {
            if (blockInfo.Origin.GetCoordinateProduct() < 0)
            {
                var detail0 = $"A component of the Origin vector is smaller than 0";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }

            if (blockInfo.Extent.GetCoordinateProduct() <= 0)
            {
                var detail0 = $"A component of the Extent vector is smaller than 0";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }

            if (blockInfo.Size.GetCoordinateProduct() <= 0)
            {
                var detail0 = $"A component of the Size vector is smaller than 0";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validate if the extent vector is larger than the origin vector in all directions
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="report"></param>
        protected void AddExtentGreaterOriginValidation(IBlockInfo blockInfo, ValidationReport report)
        {
            if ((blockInfo.Extent - blockInfo.Origin).GetCoordinateProduct() <= 0)
            {
                var detail0 = $"A component of the extent vector is not greater than the corresponding component of the origin vector";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validate if the 
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="report"></param>
        protected void AddUnfittingSuperBlockSizeValidation(IBlockInfo blockInfo, ValidationReport report)
        {
            if (blockInfo.BlockGrouping.Count != blockInfo.Size.A * blockInfo.Size.B * blockInfo.Size.C)
            {
                var detail0 = $"Number of super block elements does not match the provided BuildingBlocks";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
            }
        }




    }
}
