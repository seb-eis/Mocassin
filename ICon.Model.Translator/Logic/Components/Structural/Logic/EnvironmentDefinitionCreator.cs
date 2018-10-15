using System;
using System.Collections.Generic;
using System.Text;
using ICon.Symmetry.SpaceGroups;
using ICon.Model.ProjectServices;
using ICon.Model.Energies;
using ICon.Model.Structures;
using ICon.Model.Simulations;

namespace ICon.Model.Translator
{
    public class EnvironmentDefinitionCreator
    {
        protected IProjectServices ProjectServices { get; set; }

        protected List<EnvironmentDefinitionEntity> EnvironmentDefinitions { get; set; }

        public EnvironmentDefinitionCreator(IProjectServices projectServices)
        {
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
        }

        public List<EnvironmentDefinitionEntity> CreateEnvironmentDefinitions(ISimulation simulationBase)
        {
            EnvironmentDefinitions = new List<EnvironmentDefinitionEntity>();

            return EnvironmentDefinitions;
        }
    }
}
