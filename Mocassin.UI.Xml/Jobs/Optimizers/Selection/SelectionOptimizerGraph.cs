using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator.Optimization;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable data object that enables manual definition of <see cref="JumpSelectionOptimizer" /> objects to speed
    ///     up simulation
    /// </summary>
    [XmlRoot("SelectionOptimizer")]
    public class SelectionOptimizerGraph : ManualOptimizerGraph
    {
        /// <summary>
        ///     Get or set <see cref="IUnitCellPosition" /> key that the remove is defined for
        /// </summary>
        [XmlAttribute("OnWyckoff")]
        public string UnitCellPositionKey { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IParticle" /> key that should be removed from the selection mask
        /// </summary>
        [XmlAttribute("RemoveParticle")]
        public string ParticleKey { get; set; }

        /// <inheritdoc />
        public override IPostBuildOptimizer ToInternal(IModelProject modelProject)
        {
            var particle = modelProject.DataTracker.FindObjectByKey<IParticle>(ParticleKey);
            var unitCellPosition = modelProject.DataTracker.FindObjectByKey<IUnitCellPosition>(UnitCellPositionKey);
            return new JumpSelectionOptimizer
            {
                RemoveCombinations = new List<(IParticle, IUnitCellPosition)> {(particle, unitCellPosition)}
            };
        }
    }
}