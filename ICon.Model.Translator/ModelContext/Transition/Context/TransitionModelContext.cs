﻿using ICon.Model.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class TransitionModelContext : ITransitionModelContext
    {
        /// <inheritdoc />
        public IList<IKineticTransitionModel> KineticTransitionModels { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisTransitionModel> MetropolisTransitionModels { get; set; }

        /// <inheritdoc />
        public IList<IPositionTransitionModel> PositionTransitionModels { get; set; }
    }
}
