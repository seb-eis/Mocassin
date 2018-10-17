using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices.Validators
{
    public class DopingCombinationValidator : DataValidator<IDopingCombination, MocassinLatticeSettings, ILatticeDataPort>
    {
        public DopingCombinationValidator(IModelProject modelProject, MocassinLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        public override IValidationReport Validate(IDopingCombination obj)
        {
            return new ValidationReport();
        }
    }
}
