using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Lattices.Validators
{
    public class DopingValidator : DataValidator<IDoping, BasicLatticeSettings, ILatticeDataPort>
    {
        public DopingValidator(IProjectServices projectServices, BasicLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader) 
            : base(projectServices, settings, dataReader)
        {
        }

        public override IValidationReport Validate(IDoping obj)
        {
            return new ValidationReport();
        }
    }
}
