using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.ProjectServices;
using ICon.Model.Structures;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Structure model context builder. Extends the reference structure data into the full structure data context
    /// </summary>
    public class StructureModelContextBuilder : ModelContextBuilderBase<IStructureModelContext>
    {
        /// <inheritdoc />
        public StructureModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder) : base(projectModelContextBuilder)
        {
        }

        /// <inheritdoc />
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
            var environmentModel = new EnvironmentModel
            {
                UnitCellPosition = unitCellPosition,
                TransformOperations = wyckoffDictionary
            };
            return environmentModel;
        }
    }
}
