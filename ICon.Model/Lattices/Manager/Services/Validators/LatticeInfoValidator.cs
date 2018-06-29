using ICon.Framework.Messaging;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices.Validators
{
    /// <summary>
    /// Validator for new LatticeInfo model parameter that checks for consistency and compatibility with existing data and general object constraints
    /// </summary>
    public class LatticeInfoValidator : DataValidator<ILatticeInfo, BasicLatticeSettings, ILatticeDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public LatticeInfoValidator(IProjectServices projectServices, BasicLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new LatticeInfo parameter in terms of consistency and compatibility with existing data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(ILatticeInfo obj)
        {
            ValidationReport report = new ValidationReport();
            AddOccupationValidation(obj, report);
            return report;
        }
        
        /// <summary>
        /// Validate if the lattice size is reasonable
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="report"></param>
        protected void AddOccupationValidation(ILatticeInfo obj, ValidationReport report)
        {
            var structurePort = ProjectServices.GetManager<IStructureManager>().QueryPort;

            int positionNumber = structurePort.Query(port => port.GetLinearizedExtendedPositionList().Count);

            if (obj.Extent.A * obj.Extent.B * obj.Extent.C * positionNumber > Settings.MaxNumberOfParticles)
            {
                var detail0 = $"The specified lattice is too large";
                report.AddWarning(ModelMessages.CreateRestrictionViolationWarning(this, detail0));
            }
        }
    }
}
