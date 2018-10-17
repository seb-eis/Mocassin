using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator
{
    public class EnvironmentDefinitionCreator
    {
        protected IModelProject ModelProject { get; set; }

        protected List<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        public EnvironmentDefinitionCreator(IModelProject modelProject)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
        }

        public List<EnvironmentDefinitionEntity> CreateEnvironmentDefinitions(ISimulation simulationBase)
        {
            EnvironmentDefinitions = new List<EnvironmentDefinitionEntity>();

            return EnvironmentDefinitions;
        }
    }
}
