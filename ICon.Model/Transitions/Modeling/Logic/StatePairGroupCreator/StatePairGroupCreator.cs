using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Create index based state pair groups from state exchange groups and state exchange pairs for processing operations
    /// </summary>
    public class StatePairGroupCreator
    {
        /// <summary>
        ///     Projects a sequence of state exchange groups onto a state pair pool and particle index pool and returns a single
        ///     state pair group containing all unique possible state pairs
        /// </summary>
        /// <param name="stateGroups"></param>
        /// <param name="statePairPool"></param>
        /// <param name="particleIndexPool"></param>
        /// <returns></returns>
        public StatePairGroup MakeMergedGroup(IEnumerable<IStateExchangeGroup> stateGroups, IEnumerable<IStateExchangePair> statePairPool,
            IEnumerable<int> particleIndexPool)
        {
            return MergeGroups(stateGroups.Select(group => MakeGroup(group, statePairPool, particleIndexPool)));
        }

        /// <summary>
        ///     Creates a list interface of state pair groups where indices that are not present in the passed state exchange group
        ///     pool are filled with blank entries
        /// </summary>
        /// <param name="stateGroups"></param>
        /// <returns></returns>
        public IList<StatePairGroup> MakeGroupsWithBlanks(IEnumerable<IStateExchangeGroup> stateGroups)
        {
            var stateGroupCollection = stateGroups.ToCollection();

            var maxIndex = 0;
            foreach (var item in stateGroupCollection)
                maxIndex = maxIndex < item.Index ? item.Index : maxIndex;

            var result = new List<StatePairGroup>(maxIndex);
            for (var i = 0; i <= maxIndex; i++)
            {
                var propertyGroup = stateGroupCollection.FirstOrDefault(a => a.Index == i);
                result.Add(propertyGroup != null ? MakeGroup(propertyGroup) : StatePairGroup.CreateEmpty());
            }

            return result;
        }

        /// <summary>
        ///     Creates the state pair group for the provided state exchange group
        /// </summary>
        /// <param name="stateGroup"></param>
        /// <returns></returns>
        public StatePairGroup MakeGroup(IStateExchangeGroup stateGroup)
        {
            return new StatePairGroup(stateGroup.GetStateExchangePairs().Select(pair => pair.AsIndexTuple()).ToArray());
        }

        /// <summary>
        ///     Projects a single state exchange group onto a pool of particle indices and state pairs and returns a state pair
        ///     group containing all possible state pairs
        /// </summary>
        /// <param name="stateGroup"></param>
        /// <param name="statePairPool"></param>
        /// <param name="particleIndexPool"></param>
        /// <returns></returns>
        public StatePairGroup MakeGroup(IStateExchangeGroup stateGroup, IEnumerable<IStateExchangePair> statePairPool,
            IEnumerable<int> particleIndexPool)
        {
            var statePairs = FilterByParticles(FilterByStatePairs(stateGroup, statePairPool), particleIndexPool);
            return new StatePairGroup(statePairs.ToArray());
        }

        /// <summary>
        ///     Projects a single state exchange group onto a possible pool of state pairs and returns a state pair group that
        ///     contains only the found state pairs
        /// </summary>
        /// <param name="stateGroup"></param>
        /// <param name="statePairPool"></param>
        /// <returns></returns>
        public StatePairGroup MakeGroup(IStateExchangeGroup stateGroup, IEnumerable<IStateExchangePair> statePairPool)
        {
            return new StatePairGroup(FilterByStatePairs(stateGroup, statePairPool).Select(pair => pair.AsIndexTuple()).ToArray());
        }

        /// <summary>
        ///     Merges a sequence of state pair groups into a single group that contains the uniques
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public StatePairGroup MergeGroups(IEnumerable<StatePairGroup> groups)
        {
            IComparer<(int, int)> comparer = new TupleComparer<int, int>();
            var result = ContainerFactory.CreateSetList(comparer, groups.SelectMany(group => group.StatePairs));
            return new StatePairGroup(result.ToArray());
        }

        /// <summary>
        ///     Filters the state pairs by particle index pool that restricts the possible state pairs and the deprecation status
        /// </summary>
        /// <param name="statePairs"></param>
        /// <param name="particleIndexPool"></param>
        /// <returns></returns>
        protected IEnumerable<(int, int)> FilterByParticles(IEnumerable<IStateExchangePair> statePairs, IEnumerable<int> particleIndexPool)
        {
            var indexPoolCollection = particleIndexPool.ToCollection();

            foreach (var statePair in statePairs)
            {
                if (!statePair.IsDeprecated && indexPoolCollection.Contains(statePair.DonorParticle.Index) &&
                    indexPoolCollection.Contains(statePair.AcceptorParticle.Index))
                    yield return statePair.AsIndexTuple();
            }
        }

        /// <summary>
        ///     Filters a state exchange group for all state pairs that are not deprecated and allowed within the provided state
        ///     pair pool
        /// </summary>
        /// <param name="stateGroup"></param>
        /// <param name="statePairPool"></param>
        /// <returns></returns>
        protected IEnumerable<IStateExchangePair> FilterByStatePairs(IStateExchangeGroup stateGroup,
            IEnumerable<IStateExchangePair> statePairPool)
        {
            foreach (var statePair in statePairPool)
            {
                if (!statePair.IsDeprecated && stateGroup.GetStateExchangePairs().Select(a => a.Index).Contains(statePair.Index))
                    yield return statePair;
            }
        }
    }
}