using ICon.Mathematics.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IPairInteractionModel"/>
    public class PairInteractionModel : ModelComponentBase, IPairInteractionModel
    {
        /// <inheritdoc />
        public IPairEnergyModel PairEnergyModel { get; set; }

        /// <inheritdoc />
        public IEnvironmentModel EnvironmentModel { get; set; }

        /// <inheritdoc />
        public Fractional3D RelativeVector3D { get; set; }

        /// <inheritdoc />
        public CrystalVector4D RelativeVector4D { get; set; }
    }
}
