using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices.Validators
{
    public class DopingCombinationValidator : DataValidator<IDopingCombination, BasicLatticeSettings, ILatticeDataPort>
    {
        public DopingCombinationValidator(IProjectServices projectServices, BasicLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {
        }

        public override IValidationReport Validate(IDopingCombination obj)
        {
            return new ValidationReport();
        }
    }
}
