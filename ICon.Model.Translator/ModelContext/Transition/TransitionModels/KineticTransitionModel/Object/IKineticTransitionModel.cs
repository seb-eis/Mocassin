using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Model.Transitions;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a kinetic transition model that fully describes geometry and mobility information of a reference transition
    /// </summary>
    public interface IKineticTransitionModel : ITransitionModel
    {
        /// <summary>
        /// The transition the model is based upon
        /// </summary>
        IKineticTransition Transition { get; set; }

        /// <summary>
        /// The inverse transition model that describes the neutralizing transition
        /// </summary>
        /// <remarks> Is equal to itself if start and end unit cell positions are identical </remarks>
        IKineticTransitionModel InverseTransitionModel { get; set; }

        /// <summary>
        /// The list of existing mapping models that describe all possible geometries this transition model can be applied to
        /// </summary>
        IList<IKineticMappingModel> MappingModels { get; set; }

        /// <summary>
        /// The extended rule models that contain all possible rules for the transition
        /// </summary>
        IList<IKineticRuleModel> RuleModels { get; set; }

        /// <summary>
        /// Describes which effective particle describes the global tracking process
        /// </summary>
        IParticle EffectiveParticle { get; set; }

        /// <summary>
        /// Describes the general movement abstraction through integer offsets
        /// </summary>
        IList<int> AbstractMovement { get; set; }

        /// <summary>
        /// Creates an inverse kinetic transition model of the current
        /// </summary>
        /// <returns></returns>
        IKineticTransitionModel CreateInverse();

        /// <summary>
        /// Check if the mapping set contains its own inversions
        /// </summary>
        /// <returns></returns>
        bool MappingsContainInversion();

        /// <summary>
        /// Get the start unit cell position this transition model is valid for
        /// </summary>
        /// <returns></returns>
        IUnitCellPosition GetStartUnitCellPosition();
    }
}
