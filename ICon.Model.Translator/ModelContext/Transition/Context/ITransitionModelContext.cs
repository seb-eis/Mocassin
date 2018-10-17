using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a transition model context that describes all existing transition models for a project
    /// </summary>
    public interface ITransitionModelContext
    {
        /// <summary>
        /// The list of existing kinetic transition models
        /// </summary>
        IList<IKineticTransitionModel> KineticTransitionModels { get; set; }

        /// <summary>
        /// THe list of existing metropolis transition models
        /// </summary>
        IList<IMetropolisTransitionModel> MetropolisTransitionModels { get; set; }

        /// <summary>
        /// The list of existing position transition models
        /// </summary>
        IList<IPositionTransitionModel> PositionTransitionModels { get; set; }
    }
}
