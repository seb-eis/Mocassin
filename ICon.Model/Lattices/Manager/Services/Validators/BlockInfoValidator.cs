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
            AddUnfittingBlockSizeValidation(obj, report);
            AddDefaultBlockExtentValidation(obj, report);
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
            if (blockInfo.Origin.GetVolume() < 0)
            {
                var detail0 = $"A component of the Origin vector is smaller than 0";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }

            if (blockInfo.Extent.GetVolume() <= 0)
            {
                var detail0 = $"A component of the Extent vector is smaller than 0";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }

            if (blockInfo.Size.GetVolume() <= 0)
            {
                var detail0 = $"A component of the Size vector is smaller than 0";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }

            var latticeSize = DataReader.Access.GetLatticeInfo().Extent;

            if ((latticeSize - blockInfo.Origin).GetVolume() <= 0)
            {
                var detail0 = $"A component of the Origin vector is greater than the corresponding component of the lattice size vector";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
            }

            if ((latticeSize - blockInfo.Extent).GetVolume() < 0)
            {
                var detail0 = $"A component of the Extent vector is greater than the corresponding component of the lattice size vector";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
            }

            if ((latticeSize - blockInfo.Size).GetVolume() < 0)
            {
                var detail0 = $"A component of the Size vector is greater than the corresponding component of the lattice size vector";
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
            if ((blockInfo.Extent - blockInfo.Origin).GetVolume() <= 0)
            {
                var detail0 = $"A component of the extent vector is not greater than the corresponding component of the origin vector";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validate if the super block can fill the specified block extent without cutting off parts of the super block
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="report"></param>
        protected void AddUnfittingBlockSizeValidation(IBlockInfo blockInfo, ValidationReport report)
        {
            if (blockInfo.Size.GetVolume() <= 0) return;

            if(((blockInfo.Extent - blockInfo.Origin) % blockInfo.Size).Equals(new CartesianInt3D(0,0,0)) == false)
            {
                var detail0 = $"The super block cannot fill the specified space without cutoff";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
            }
        }

        /// <summary>
        /// Validate default (index = 0) BlockInfo for coherence with lattice size
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="report"></param>
        protected void AddDefaultBlockExtentValidation(IBlockInfo blockInfo, ValidationReport report)
        {
            var blockInfoCount = DataReader.Access.GetBlockInfos().Count;

            if (blockInfoCount == 0)
            {
                var latticeInfo = DataReader.Access.GetLatticeInfo();

                if (blockInfo.Origin.Equals(new CartesianInt3D(0, 0, 0)) == false)
                {
                    var detail0 = $"Extent of default building block does not originate at 0, 0, 0";
                    report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
                }

                if (blockInfo.Extent.Equals(latticeInfo.Extent) == false)
                {
                    var detail0 = $"Default block has different extent than lattice size";
                    report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
                }
            }
        }

        /// <summary>
        /// Validate if the 
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <param name="report"></param>
        protected void AddUnfittingSuperBlockSizeValidation(IBlockInfo blockInfo, ValidationReport report)
        {
            if (blockInfo.BlockAssembly.Count != blockInfo.Size.A * blockInfo.Size.B * blockInfo.Size.C)
            {
                var detail0 = $"Number of super block elements does not match the provided BuildingBlocks";
                report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
            }
        }




    }
}
