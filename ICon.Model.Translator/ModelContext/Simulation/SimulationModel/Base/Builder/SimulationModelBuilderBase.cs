using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Base class for the simulation model builder that supplies shared generic simulation model build functionality
    /// </summary>
    public abstract class SimulationModelBuilderBase : ModelBuilderBase
    {
        /// <inheritdoc />
        protected SimulationModelBuilderBase(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Creates the mapping assign matrix for the passed set of transition models using the set generic type
        ///     specializations
        /// </summary>
        /// <param name="transitionModels"></param>
        /// <returns></returns>
        protected TMappingModel[,,] CreateMappingAssignMatrix<TMappingModel, TTransitionModel>(
            IEnumerable<TTransitionModel> transitionModels)
            where TMappingModel : ITransitionMappingModel
            where TTransitionModel : ITransitionModel
        {
            var listResult = new List<IList<IList<TMappingModel>>>();

            foreach (var transitionModel in transitionModels)
            {
                var mappingModels = transitionModel.GetMappingModels()
                    .Cast<TMappingModel>()
                    .ToList();

                foreach (var particle in transitionModel.MobileParticles)
                    InsertMappingModelsIntoRawMatrix(listResult, mappingModels, particle);
            }

            return ConvertRawAssignListsToMatrix(listResult);
        }

        /// <summary>
        ///     Inserts the passed list of mapping models into the list based raw mapping matrix
        /// </summary>
        /// <param name="rawMatrix"></param>
        /// <param name="mappingModels"></param>
        /// <param name="particle"></param>
        protected void InsertMappingModelsIntoRawMatrix<TMappingModel>(IList<IList<IList<TMappingModel>>> rawMatrix,
            IList<TMappingModel> mappingModels, IParticle particle)
            where TMappingModel : ITransitionMappingModel
        {
            var maxP = mappingModels.Select(a => a.StartVector4D.P).Max();
            while (rawMatrix.Count <= maxP)
                rawMatrix.Add(new List<IList<TMappingModel>>(particle.Index + 1));

            foreach (var list in rawMatrix.Where(a => a.Count <= particle.Index))
            {
                while (list.Count <= particle.Index)
                    list.Add(new List<TMappingModel>());
            }

            foreach (var mappingModel in mappingModels)
                rawMatrix[mappingModel.StartVector4D.P][particle.Index].Add(mappingModel);
        }

        /// <summary>
        ///     Converts the raw list based mapping assign matrix into a fixed size 3D array and fills the placeholder spots with
        ///     an invalid mapping
        /// </summary>
        /// <param name="rawMatrix"></param>
        /// <returns></returns>
        protected TMappingModel[,,] ConvertRawAssignListsToMatrix<TMappingModel>(IList<IList<IList<TMappingModel>>> rawMatrix)
            where TMappingModel : ITransitionMappingModel
        {
            var (sizeA, sizeB, sizeC) = GetRawAssignListSizeInfo(rawMatrix);
            var matrix = new TMappingModel[sizeA, sizeB, sizeC];

            for (var a = 0; a < rawMatrix.Count; a++)
            {
                for (var b = 0; b < rawMatrix[a].Count; b++)
                {
                    for (var c = 0; c < rawMatrix[a][b].Count; c++)
                        matrix[a, b, c] = rawMatrix[a][b][c];
                }
            }

            return matrix;
        }

        /// <summary>
        ///     Get the size information in three dimensions for the list based mapping assign matrix
        /// </summary>
        /// <param name="rawMatrix"></param>
        /// <returns></returns>
        protected (int, int, int) GetRawAssignListSizeInfo<TMappingModel>(IList<IList<IList<TMappingModel>>> rawMatrix)
            where TMappingModel : ITransitionMappingModel
        {
            return (rawMatrix.Count, rawMatrix[0].Count, rawMatrix.SelectMany(a => a.Select(b => b).Select(c => c.Count)).Max());
        }
    }
}