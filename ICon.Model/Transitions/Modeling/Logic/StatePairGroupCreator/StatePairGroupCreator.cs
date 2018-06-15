using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Collections;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Creates state pair groups from property groups and property state pairs for processing operations
    /// </summary>
    public class StatePairGroupCreator
    {
        /// <summary>
        /// Projects a sequence of property groups onto a state pair pool and particle index pool and returns a single state pair group containing all unique possible state pairs
        /// </summary>
        /// <param name="propertyGroups"></param>
        /// <param name="statePairPool"></param>
        /// <param name="particleIndexPool"></param>
        /// <returns></returns>
        public StatePairGroup MakeMergedGroup(IEnumerable<IPropertyGroup> propertyGroups, IEnumerable<IPropertyStatePair> statePairPool, IEnumerable<int> particleIndexPool)
        {
            return MergeGroups(propertyGroups.Select(group => MakeGroup(group, statePairPool, particleIndexPool)));
        }

        /// <summary>
        /// Projects a single property group onto a pool of particle indices and state pairs and returns a state pair group cotaining all possible state pairs
        /// </summary>
        /// <param name="propertyGroup"></param>
        /// <param name="statePairPool"></param>
        /// <param name="particleIndexPool"></param>
        /// <returns></returns>
        public StatePairGroup MakeGroup(IPropertyGroup propertyGroup, IEnumerable<IPropertyStatePair> statePairPool, IEnumerable<int> particleIndexPool)
        {
            var statePairs = FilterByParticles(FilterByStatePairs(propertyGroup, statePairPool), particleIndexPool);
            return new StatePairGroup(statePairs.ToArray());
        }

        /// <summary>
        /// Projects a single property group onto a possible pool of state pairs and retruns a state pair group that contains only the found state pairs
        /// </summary>
        /// <param name="propertyGroup"></param>
        /// <param name="statePairPool"></param>
        /// <returns></returns>
        public StatePairGroup MakeGroup(IPropertyGroup propertyGroup, IEnumerable<IPropertyStatePair> statePairPool)
        {
            return new StatePairGroup(FilterByStatePairs(propertyGroup, statePairPool).Select(pair => pair.AsIndexTuple()).ToArray());
        }

        /// <summary>
        /// Merges a sequence of state pair groups into a single group that contains the uniques
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public StatePairGroup MergeGroups(IEnumerable<StatePairGroup> groups)
        {
            IComparer<(int, int)> comparer = new TupleContentComparer<int, int>();
            var result = ContainerFactory.CreateSetList(comparer, groups.SelectMany(group => group.StatePairs));
            return new StatePairGroup(result.ToArray());
        }

        /// <summary>
        /// Filters the state pairs by particle index pool that restricts the possible state pairs and the deprecation status
        /// </summary>
        /// <param name="statePairs"></param>
        /// <param name="particleIndexPool"></param>
        /// <returns></returns>
        protected IEnumerable<(int, int)> FilterByParticles(IEnumerable<IPropertyStatePair> statePairs, IEnumerable<int> particleIndexPool)
        {
            foreach (var statePair in statePairs)
            {
                if (!statePair.IsDeprecated && particleIndexPool.Contains(statePair.DonorParticle.Index) && particleIndexPool.Contains(statePair.AcceptorParticle.Index))
                {
                    yield return statePair.AsIndexTuple();
                }
            }
        }

        /// <summary>
        /// Filters a property group for all state pairs that are not deprecated and allowed within the provided state pair pool
        /// </summary>
        /// <param name="propertyGroup"></param>
        /// <param name="statePairPool"></param>
        /// <returns></returns>
        protected IEnumerable<IPropertyStatePair> FilterByStatePairs(IPropertyGroup propertyGroup, IEnumerable<IPropertyStatePair> statePairPool)
        {
            foreach (var statePair in statePairPool)
            {
                if (!statePair.IsDeprecated && propertyGroup.GetPropertyStatePairs().Select(a => a.Index).Contains(statePair.Index))
                {
                    yield return statePair;
                }
            }
        }
    }
}
