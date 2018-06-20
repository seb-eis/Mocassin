using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices.Validators
{
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

        public override IValidationReport Validate(ILatticeInfo obj)
        {
            return new ValidationReport();
        }
    }
}
