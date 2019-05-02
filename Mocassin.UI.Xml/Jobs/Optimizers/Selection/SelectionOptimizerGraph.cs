using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator.Optimization;
using Mocassin.UI.Xml.Base;

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
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}"/> to the <see cref="UnitCellPosition"/> that the optimizer targets
        /// </summary>
        [XmlElement("Wyckoff")]
        public ModelObjectReferenceGraph<UnitCellPosition> StartUnitCellPosition { get; set; }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}"/> to the <see cref="Particle"/> that the optimizers removes
        /// </summary>
        [XmlElement("Particle")]
       public ModelObjectReferenceGraph<Particle> RemovedParticle { get; set; }

        /// <inheritdoc />
        public override IPostBuildOptimizer ToInternal(IModelProject modelProject)
        {
            var particle = modelProject.DataTracker.FindObjectByKey<IParticle>(RemovedParticle.Key);
            var unitCellPosition = modelProject.DataTracker.FindObjectByKey<IUnitCellPosition>(StartUnitCellPosition.Key);
            return new JumpSelectionOptimizer
            {
                RemoveCombinations = new List<(IParticle, IUnitCellPosition)> {(particle, unitCellPosition)}
            };
        }
    }
}