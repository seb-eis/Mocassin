using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices.Validators
{
    public class DopingValidator : DataValidator<IDoping, MocassinLatticeSettings, ILatticeDataPort>
    {
        public DopingValidator(IModelProject modelProject, MocassinLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader) 
            : base(modelProject, settings, dataReader)
        {
        }

        public override IValidationReport Validate(IDoping obj)
        {
            return new ValidationReport();
        }
    }
}
