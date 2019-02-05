using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICon.Framework.Messaging;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;
=======
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
>>>>>>> origin/s.eisele@dev

namespace Mocassin.Model.Lattices.Validators
{
    /// <summary>
    /// Validator for new BuildingBlock model objects that checks for consistency and compatibility with existing data and general object constraints
    /// </summary>
    public class BuildingBlockValidator : DataValidator<IBuildingBlock, BasicLatticeSettings, ILatticeDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public BuildingBlockValidator(IModelProject modelProject, MocassinLatticeSettings settings, IDataReader<ILatticeDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new BuildingBlock object in terms of consistency and compatibility with existing data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IBuildingBlock obj)
        {
            ValidationReport report = new ValidationReport();
            AddOccupationValidation(obj, report);
            return report;
        }

        /// <summary>
        /// Validate matching particles and unit cell positions
        /// </summary>
        /// <param name="buildingBlock"></param>
        /// <param name="report"></param>
        protected void AddOccupationValidation(IBuildingBlock buildingBlock, ValidationReport report)
        {
            var structurePort = ProjectServices.GetManager<IStructureManager>().QueryPort;

            var occupationList = structurePort.Query(port => port.GetExtendedIndexToPositionDictionary());

            for (int i = 0; i < occupationList.Count; i++)
            {
                if (occupationList[i].OccupationSet.GetParticles().Contains(buildingBlock.CellEntries[i]) == false)
                {
                    var detail0 = $"A Particle cannot be placed at the specified position";
                    report.AddWarning(ModelMessages.CreateContentMismatchWarning(this, detail0));
                }
            }
        }
    }
}
