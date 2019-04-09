using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Moccasin.Model.ProjectServices;
using Mocassin.Framework.Messaging;

namespace Mocassin.Model.Lattices.Validators
{
    /// <summary>
    /// Validator for new DopingCombination model objects that checks for consistency and compatibility with existing data and general object constraints
    /// </summary>
    public class DopingCombinationValidator : DataValidator<IDopingCombination, MocassinLatticeSettings, ILatticeDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public DopingCombinationValidator(IModelProject modelProject, MocassinLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new DopingCombination object in terms of consistency and compatibility with existing data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IDopingCombination obj)
        {
            ValidationReport report = new ValidationReport();
            AddOccupationValidation(obj.Dopant, obj.UnitCellPosition, report);
            AddOccupationValidation(obj.DopedParticle, obj.UnitCellPosition, report);
            return report;
        }

        /// <summary>
        /// Validate matching particles and unit cell positions
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="position"></param>
        /// <param name="report"></param>
        protected void AddOccupationValidation(IParticle particle, IUnitCellPosition position, ValidationReport report)
        {
            if (position.OccupationSet.GetParticles().Contains(particle) == false)
            {
                var detail0 = $"A Particle cannot be placed at the specified position";
                report.AddWarning(WarningMessage.CreateCritical(this, detail0));
            }
        }
    }
}
