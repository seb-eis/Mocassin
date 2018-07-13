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
            return report;
        }
        
    }
}
