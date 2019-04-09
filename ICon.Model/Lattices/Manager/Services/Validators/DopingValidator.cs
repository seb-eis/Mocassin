using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Moccasin.Model.ProjectServices;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices.Validators
{
    /// <summary>
    /// Validator for new Doping model objects that checks for consistency and compatibility with existing data and general object constraints
    /// </summary>
    public class DopingValidator : DataValidator<IDoping, MocassinLatticeSettings, ILatticeDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public DopingValidator(IModelProject projectServices, MocassinLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader) 
            : base(projectServices, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new Doping object in terms of consistency and compatibility with existing data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IDoping obj)
        {
            ValidationReport report = new ValidationReport();
            //AddNegativeDopingValidation(obj.Concentration, report);
            //AddOverdopingValidation(obj.Concentration, report);
            return report;
        }

        /// <summary>
        /// Validate if doping concentration is positive
        /// </summary>
        /// <param name="concentration"></param>
        /// <param name="report"></param>
        protected void AddNegativeDopingValidation(double concentration, ValidationReport report)
        {
            if (concentration < 0.0)
            {
                var detail0 = $"The doping concentration cannot be smaller than zero";
                report.AddWarning(WarningMessage.CreateCritical(this, detail0));
            }
        }

        /// <summary>
        /// Validate if doping concentration is greater than one
        /// </summary>
        /// <param name="concentration"></param>
        /// <param name="report"></param>
        protected void AddOverdopingValidation(double concentration, ValidationReport report)
        {
            if (concentration > 1.0)
            {
                var detail0 = $"The doping concentration should not be larger than one";
                report.AddWarning(WarningMessage.CreateCritical(this, detail0));
            }
        }
    }
}
