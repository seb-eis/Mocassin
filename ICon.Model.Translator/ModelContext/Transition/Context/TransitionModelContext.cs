using ICon.Model.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Implementation of a transition model context that contains all extended transition context data for simulation generation/evaluation
    /// </summary>
    public class TransitionModelContext : ITransitionModelContext
    {
        /// <summary>
        /// The list of existing kinetic transition models
        /// </summary>
        public IList<IKineticTransitionModel> KineticTransitionModels { get; set; }

        /// <summary>
        /// THe list of existing metropolis transition models
        /// </summary>
        public IList<IMetropolisTransitionModel> MetropolisTransitionModels { get; set; }

        /// <summary>
        /// The list of existing position transition models
        /// </summary>
        public IList<IPositionTransitionModel> PositionTransitionModels { get; set; }
    }
}
