<<<<<<< HEAD
﻿using ICon.Framework.Messaging;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;
using System;
=======
﻿using System;
>>>>>>> origin/s.eisele@dev
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices.Validators
{
<<<<<<< HEAD
    /// <summary>
    /// Validator for new LatticeInfo model parameter that checks for consistency and compatibility with existing data and general object constraints
    /// </summary>
    public class LatticeInfoValidator : DataValidator<ILatticeInfo, BasicLatticeSettings, ILatticeDataPort>
=======
    public class LatticeInfoValidator : DataValidator<ILatticeInfo, MocassinLatticeSettings, ILatticeDataPort>
>>>>>>> origin/s.eisele@dev
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public LatticeInfoValidator(IModelProject modelProject, MocassinLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(modelProject, settings, dataReader)
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
