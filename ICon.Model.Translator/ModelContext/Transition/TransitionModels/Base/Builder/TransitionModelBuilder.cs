using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Abstract base class for transition model builder implementations
    /// </summary>
    public abstract class TransitionModelBuilder : ModelBuilderBase
    {
        /// <inheritdoc />
        protected TransitionModelBuilder(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Creates the charge transport matrix 1xN for a set of start state particles
        /// </summary>
        /// <param name="startState"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected Matrix2D CreateChargeTransportMatrix(IList<IParticle> startState, IComparer<double> comparer)
        {
            var result = new Matrix2D(1, startState.Count, comparer);
            for (var i = 0; i < startState.Count; i++)
                result[0, i] = startState[i].Charge;

            return result;
        }

        /// <summary>
        ///     Creates the rule model of the specified type and sets the basis information
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transitionRule"></param>
        /// <returns></returns>
        protected T CreateRuleModelBasis<T>(ITransitionRule transitionRule) where T : TransitionRuleModel, new()
        {
            var ruleModel = new T
            {
                StartState = transitionRule.GetStartStateOccupation().ToList(),
                FinalState = transitionRule.GetFinalStateOccupation().ToList(),
                EndIndexingDeltas = CreateEndIndexingDeltas(transitionRule)
            };
            return ruleModel;
        }

        /// <summary>
        ///     Creates all inversion links and simulation codes for a prepared list of rule models
        /// </summary>
        /// <param name="ruleModels"></param>
        protected void CreateCodesAndLinkInverseRuleModels(IReadOnlyList<ITransitionRuleModel> ruleModels)
        {
            CreateAllCodesOnRuleModels(ruleModels);

            for (var i = 0; i < ruleModels.Count; i++)
            {
                if (ruleModels[i].InverseIsSet)
                    continue;

                for (var j = i; j < ruleModels.Count; j++)
                {
                    if (ruleModels[j].InverseIsSet)
                        continue;

                    if (ruleModels[i].LinkIfInverseMatch(ruleModels[j]))
                        break;
                }
            }
        }

        /// <summary>
        ///     Creates all codes on a sequence of rule models
        /// </summary>
        /// <param name="ruleModels"></param>
        protected void CreateAllCodesOnRuleModels(IEnumerable<ITransitionRuleModel> ruleModels)
        {
            var buffer = new byte[8];
            foreach (var ruleModel in ruleModels)
            {
                ruleModel.StartStateCode = Create64BitIndexCode(ruleModel.StartState.Select(a => a.Index), buffer);
                ruleModel.FinalStateCode = Create64BitIndexCode(ruleModel.FinalState.Select(a => a.Index), buffer);
                ruleModel.FinalTrackerOrderCode = CreateFinalTrackerOrderCode(ruleModel.EndIndexingDeltas, buffer);

                if (ruleModel is IKineticRuleModel kineticModel)
                {
                    kineticModel.TransitionStateCode =
                        Create64BitIndexCode(kineticModel.TransitionState.Select(a => a.Index), buffer);
                }
            }
        }

        /// <summary>
        ///     Creates a 64 long code for the simulation from the passed set of int values
        /// </summary>
        /// <param name="values"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected long Create64BitIndexCode(IEnumerable<int> values, byte[] buffer)
        {
            var index = -1;
            foreach (var item in values) 
                buffer[++index] = (byte) item;

            var code = BitConverter.ToInt64(buffer, 0);
            for (; index >= 0; index--)
                buffer[index] = 0;
            return code;
        }

        /// <summary>
        ///     Translates the end indexing deltas into the final tracker 64 bit order code for the simulation
        /// </summary>
        /// <param name="endIndexingDeltas"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected long CreateFinalTrackerOrderCode(IList<int> endIndexingDeltas, byte[] buffer)
        {
            var orderIndexing = new List<int>(endIndexingDeltas.Count);
            orderIndexing.AddRange(endIndexingDeltas.Select((element, index) => index + element));
            var result = Create64BitIndexCode(orderIndexing, buffer);
            return result;
        }

        /// <summary>
        ///     Converts the movement description of a transition rule into the end indexing delta value list
        /// </summary>
        /// <param name="transitionRule"></param>
        /// <returns></returns>
        protected IList<int> CreateEndIndexingDeltas(ITransitionRule transitionRule)
        {
            var index = 0;
            var result = new List<int>(transitionRule.PathLength).Populate(() => index++, transitionRule.PathLength);

            foreach (var (start, end) in transitionRule.GetMovementDescription().SelectConsecutivePairs((a, b) => (a, b)))
                result.Swap(start, end);

            for (var i = 0; i < result.Count; i++) 
                result[i] -= i;

            return result;
        }

        /// <summary>
        ///     Takes a set of rule models and creates the selectable particle set
        /// </summary>
        /// <param name="ruleModels"></param>
        /// <returns></returns>
        protected IParticleSet CreateSelectableParticleSet(IEnumerable<ITransitionRuleModel> ruleModels)
        {
            var comparer = Comparer<IParticle>.Create((a, b) => a.Index.CompareTo(b.Index));
            var uniqueParticles = ruleModels.Select(a => a.SelectableParticle).ToSetList(comparer);
            return new ParticleSet {Index = -1, Particles = uniqueParticles.ToList()};
        }

        /// <summary>
        /// Adds the mobility information (selectable particle set, particle mask ...) to the passed transition model
        /// </summary>
        /// <param name="transitionModel"></param>
        protected void AddBasicMobilityInformation(ITransitionModel transitionModel)
        {
            var buffer = new byte[8];
            transitionModel.SelectableParticles = CreateSelectableParticleSet(transitionModel.GetRuleModels());
            transitionModel.SelectableParticleMask = Create64BitIndexCode(transitionModel.SelectableParticles.Select(a => a.Index), buffer);
        }
    }
}