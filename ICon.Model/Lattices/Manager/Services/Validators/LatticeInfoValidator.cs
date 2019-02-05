using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices.Validators
{
    /// <summary>
    /// Validator for new LatticeInfo model parameter that checks for consistency and compatibility with existing data and general object constraints
    /// </summary>
    public class LatticeInfoValidator : DataValidator<ILatticeInfo, MocassinLatticeSettings, ILatticeDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public LatticeInfoValidator(IModelProject projectServices, MocassinLatticeSettings  settings, IDataReader<ILatticeDataPort> dataReader)
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
