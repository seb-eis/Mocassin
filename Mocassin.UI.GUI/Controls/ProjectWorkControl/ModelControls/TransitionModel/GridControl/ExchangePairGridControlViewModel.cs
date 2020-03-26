using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Particles;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" />
    /// </summary>
    public class ExchangePairGridControlViewModel : CollectionControlViewModel<StateExchangePairData>,
        IContentSupplier<MocassinProject>
    {
        /// <summary>
        ///     Get or set the <see cref="ICollection{T}" /> of all available particles
        /// </summary>
        private ICollection<ParticleData> BaseParticleOptions => ContentSource?.ProjectModelData?.ParticleModelData?.Particles;

        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <summary>
        ///     Get a <see cref="IEnumerable{T}" /> of possible donor <see cref="ParticleData" /> options for the currently
        ///     selected <see cref="StateExchangePairData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<Particle>> CurrentDonorOptions => EnumerateCurrentDonorOptions();

        /// <summary>
        ///     Get a <see cref="IEnumerable{T}" /> of possible donor <see cref="ParticleData" /> options for the currently
        ///     selected <see cref="StateExchangePairData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<Particle>> CurrentAcceptorOptions => EnumerateCurrentAcceptorOptions();

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.TransitionModelData?.StateExchangePairs);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> for the donor <see cref="ModelObjectReference{T}" /> options of the currently
        ///     selected <see cref="StateExchangePairData" />
        /// </summary>
        /// <returns></returns>
        /// <remarks> This method ensures that on loading (Selection is null) the full collection is returned </remarks>
        private IEnumerable<ModelObjectReference<Particle>> EnumerateCurrentDonorOptions()
        {
            if (BaseParticleOptions == null) yield break;
            foreach (var particleData in BaseParticleOptions)
            {
                if (!PairIsAlreadyDefined(Items, particleData.Key, SelectedItem?.AcceptorParticle?.Key))
                    yield return new ModelObjectReference<Particle>(particleData);
            }
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> for the acceptor <see cref="ModelObjectReference{T}" /> options of the
        ///     currently selected <see cref="StateExchangePairData" />
        /// </summary>
        /// <returns></returns>
        /// <remarks> This method ensures that on loading (Selection is null) the full collection is returned </remarks>
        private IEnumerable<ModelObjectReference<Particle>> EnumerateCurrentAcceptorOptions()
        {
            if (BaseParticleOptions == null) yield break;
            foreach (var particleData in BaseParticleOptions.Concat(ParticleData.VoidParticle.AsSingleton()))
            {
                if (!PairIsAlreadyDefined(Items, SelectedItem?.DonorParticle?.Key, particleData?.Key))
                    yield return new ModelObjectReference<Particle>(particleData);
            }
        }

        /// <summary>
        ///     Checks if th passed <see cref="ParticleData" /> key combination is already defined as a
        ///     <see cref="StateExchangePairData" />
        /// </summary>
        /// <param name="defined"></param>
        /// <param name="donorKey"></param>
        /// <param name="acceptorKey"></param>
        /// <returns></returns>
        private bool PairIsAlreadyDefined(ICollection<StateExchangePairData> defined, string donorKey, string acceptorKey)
        {
            if (donorKey == null || acceptorKey == null) return false;
            return donorKey == acceptorKey
                   || defined != null && defined
                       .Where(x => x != SelectedItem)
                       .Any(x => x.AcceptorParticle?.Key == acceptorKey && x.DonorParticle?.Key == donorKey);
        }
    }
}