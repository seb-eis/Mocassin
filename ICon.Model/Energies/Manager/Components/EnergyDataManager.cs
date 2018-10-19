using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Energy data manager that provides safe read only access to the energy base model data
    /// </summary>
    internal class EnergyDataManager : ModelDataManager<EnergyModelData>, IEnergyDataPort
    {
        /// <inheritdoc />
        public EnergyDataManager(EnergyModelData modelData)
            : base(modelData)
        {
        }
        
        /// <inheritdoc />
        public IStableEnvironmentInfo GetStableEnvironmentInfo()
        {
            return Data.StableEnvironmentInfo;
        }
        
        /// <inheritdoc />
        public IGroupInteraction GetGroupInteraction(int index)
        {
            return Data.GroupInteractions[index];
        }
        
        /// <inheritdoc />
        public ReadOnlyListAdapter<IGroupInteraction> GetGroupInteractions()
        {
            return ReadOnlyListAdapter<IGroupInteraction>.FromEnumerable(Data.GroupInteractions);
        }
        
        /// <inheritdoc />
        public ISymmetricPairInteraction GetStablePairInteraction(int index)
        {
            return Data.StablePairInteractions[index];
        }
        
        /// <inheritdoc />
        public ReadOnlyListAdapter<ISymmetricPairInteraction> GetStablePairInteractions()
        {
            return ReadOnlyListAdapter<ISymmetricPairInteraction>.FromEnumerable(Data.StablePairInteractions);
        }
        
        /// <inheritdoc />
        public IUnstableEnvironment GetUnstableEnvironment(int index)
        {
            return Data.UnstableEnvironmentInfos[index];
        }
        
        /// <inheritdoc />
        public ReadOnlyListAdapter<IUnstableEnvironment> GetUnstableEnvironments()
        {
            return ReadOnlyListAdapter<IUnstableEnvironment>.FromEnumerable(Data.UnstableEnvironmentInfos);
        }
        
        /// <inheritdoc />
        public IAsymmetricPairInteraction GetUnstablePairInteractions(int index)
        {
            return Data.UnstablePairInteractions[index];
        }
        
        /// <inheritdoc />
        public ReadOnlyListAdapter<IAsymmetricPairInteraction> GetUnstablePairInteractions()
        {
            return ReadOnlyListAdapter<IAsymmetricPairInteraction>.FromEnumerable(Data.UnstablePairInteractions);
        }

        /// <inheritdoc />
        public IEnergySetterProvider GetEnergySetterProvider()
        {
            return new EnergySetterProvider(Data);
        }
    }
}