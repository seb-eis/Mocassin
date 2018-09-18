using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Implementation of a structure model context builder that extends the existing refernce structure data into a full model context object
    /// </summary>
    public class StructureModelContextBuilder : ModelContextBuilderBase<IStructureModelContext>
    {
        /// <summary>
        /// Create new context builder that is linked to the provided main context builder
        /// </summary>
        /// <param name="projectServices"></param>
        public StructureModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder) : base(projectModelContextBuilder)
        {
        }

        /// <summary>
        /// Popultes the currently active context
        /// </summary>
        protected override void PopulateContext()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build a new environment model for the passed unit cell position
        /// </summary>
        /// <param name="unitCellPosition"></param>
        /// <returns></returns>
        protected IEnvironmentModel BuildNewEnvironmentModel(IUnitCellPosition unitCellPosition)
        {
            var wyckoffDictionary = ProjectServices.SpaceGroupService.GetOperationDictionary(unitCellPosition.Vector);
            var environmentModel = new EnvironmentModel()
            {
                UnitCellPosition = unitCellPosition,
                TransformOperations = wyckoffDictionary
            };
            return environmentModel;
        }
    }
}
