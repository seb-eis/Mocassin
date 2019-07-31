using System.Collections.Generic;
using System.Linq;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" />
    /// </summary>
    public class ExchangePairGridControlViewModel : CollectionControlViewModel<StateExchangePairGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <summary>
        ///     Get or set the <see cref="ICollection{T}" /> of all available particles
        /// </summary>
        private ICollection<ParticleGraph> BaseParticleOptions => ContentSource?.ProjectModelGraph?.ParticleModelGraph?.Particles;

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get a <see cref="IEnumerable{T}" /> of possible donor <see cref="ParticleGraph" /> options for the currently
        ///     selected <see cref="StateExchangePairGraph" />
        /// </summary>
        public IEnumerable<ParticleGraph> CurrentDonorOptions => GetCurrentDonorOptions();

        /// <summary>
        ///     Get a <see cref="IEnumerable{T}" /> of possible donor <see cref="ParticleGraph" /> options for the currently
        ///     selected <see cref="StateExchangePairGraph" />
        /// </summary>
        public IEnumerable<ParticleGraph> CurrentAcceptorOptions => GetCurrentAcceptorOptions();

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelGraph?.TransitionModelGraph?.StateExchangePairs);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of the donor <see cref="ParticleGraph" /> options for the currently selected
        ///     <see cref="StateExchangePairGraph" />
        /// </summary>
        /// <returns></returns>
        /// <remarks> This method ensures that on loading (Selection is null) the full collection is returned </remarks>
        private IEnumerable<ParticleGraph> GetCurrentDonorOptions()
        {
            if (BaseParticleOptions == null) yield break;
            foreach (var particleGraph in BaseParticleOptions)
            {
                if (!PairIsAlreadyDefined(Items, particleGraph.Key, SelectedItem?.AcceptorParticleKey))
                    yield return particleGraph;
            }
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of the acceptor <see cref="ParticleGraph" /> options for the currently
        ///     selected <see cref="StateExchangePairGraph" />
        /// </summary>
        /// <returns></returns>
        /// <remarks> This method ensures that on loading (Selection is null) the full collection is returned </remarks>
        private IEnumerable<ParticleGraph> GetCurrentAcceptorOptions()
        {
            if (BaseParticleOptions == null) yield break;
            foreach (var particleGraph in BaseParticleOptions)
            {
                if (!PairIsAlreadyDefined(Items, SelectedItem?.DonorParticleKey, particleGraph.Key))
                    yield return particleGraph;
            }

            yield return ParticleGraph.VoidParticle;
        }

        /// <summary>
        ///     Checks if th passed <see cref="ParticleGraph" /> key combination is already defined as a
        ///     <see cref="StateExchangePairGraph" />
        /// </summary>
        /// <param name="defined"></param>
        /// <param name="donorKey"></param>
        /// <param name="acceptorKey"></param>
        /// <returns></returns>
        private bool PairIsAlreadyDefined(ICollection<StateExchangePairGraph> defined, string donorKey, string acceptorKey)
        {
            return donorKey == acceptorKey 
                   || defined != null && defined
                       .Where(x => x != SelectedItem)
                       .Any(x => x.AcceptorParticleKey == acceptorKey && x.DonorParticleKey == donorKey);
        }
    }
}