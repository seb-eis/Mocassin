using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Energies;
using Mocassin.Model.Lattices;
using Mocassin.Model.Particles;
using Mocassin.Model.Simulations;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Base class for all translator components
    /// </summary>
    public abstract class TranslatorBase
    {
        protected ITranslationContext TranslationContext { get; set; }

        protected IStructureManager StructureManager { get; set; }

        protected ITransitionManager TransitionManager { get; set; }

        protected IParticleManager ParticleManager { get; set; }

        protected IEnergyManager EnergyManager { get; set; }

        protected ISimulationManager SimulationManager { get; set; }

        protected ILatticeManager LatticeManager { get; set; }

        protected void CheckAndPrepareModelContext(ITranslationContext translationContext)
        {
            TranslationContext = translationContext ?? throw new ArgumentNullException(nameof(translationContext));

            if ((ParticleManager = TranslationContext.ModelProject.GetManager<IParticleManager>()) == null)
            {
                throw new InvalidOperationException("Required particle model context does not exist");
            }

            if ((StructureManager = TranslationContext.ModelProject.GetManager<IStructureManager>()) == null)
            {
                throw new InvalidOperationException("Required structure model context does not exist");
            }

            if ((TransitionManager = TranslationContext.ModelProject.GetManager<ITransitionManager>()) == null)
            {
                throw new InvalidOperationException("Required transition model context does not exist");
            }

            if ((EnergyManager = TranslationContext.ModelProject.GetManager<IEnergyManager>()) == null)
            {
                throw new InvalidOperationException("Required energy model context does not exist");
            }

            if ((SimulationManager = TranslationContext.ModelProject.GetManager<ISimulationManager>()) == null)
            {
                throw new InvalidOperationException("Required energy model context does not exist");
            }

            //if ((LatticeManager = TranslationContext.ProjectServices.GetManager<ILatticeManager>()) == null)
            //{
            //    throw new InvalidOperationException("Required energy model context does not exist");
            //}
        }
    }
}
