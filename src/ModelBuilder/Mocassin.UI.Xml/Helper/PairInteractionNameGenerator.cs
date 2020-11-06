using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Comparer;
using Mocassin.Model.Structures;
using Mocassin.UI.Data.Customization;
using Mocassin.UI.Data.ParticleModel;
using Mocassin.UI.Data.StructureModel;

namespace Mocassin.UI.Data.Helper
{
    /// <summary>
    ///     A generator that automatically names sequences of <see cref="PairEnergySetData"/> objects
    /// </summary>
    public class PairInteractionNameGenerator
    {
        /// <summary>
        ///     Get or set the tolerance for shell naming in angstrom
        /// </summary>
        public double ShellEqualityTolerance { get; set; } = 1e-3;

        /// <summary>
        ///     Get or set the component separator
        /// </summary>
        public string Separator { get; set; } = "-";

        /// <summary>
        ///     Generates and assigns the names for the passed <see cref="IEnumerable{T}"/> of <see cref="PairEnergySetData"/>
        /// </summary>
        /// <param name="pairEnergySetSequence"></param>
        public void NameSet(IEnumerable<PairEnergySetData> pairEnergySetSequence)
        {
            var pairEnergySets = pairEnergySetSequence.OrderBy(x => x.Distance).ToList();
            if (pairEnergySets.Count == 0) return;
            var stringBuilders = new StringBuilder[pairEnergySets.Count].Populate(() => new StringBuilder(50));
            AppendBaseNaming(pairEnergySets, stringBuilders);
            AppendShellNaming(pairEnergySets, stringBuilders);
            AppendDistanceNumberingWithChiralPartnerMarkers(pairEnergySets, stringBuilders);
            AssignNaming(pairEnergySets, stringBuilders);

        }

        /// <summary>
        ///     Appends the base naming (Symbol-Symbol) or (u-Symbol-Symbol) for each <see cref="PairEnergySetData"/> to its affiliated <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="pairEnergySets"></param>
        /// <param name="stringBuilders"></param>
        private void AppendBaseNaming(IList<PairEnergySetData> pairEnergySets, StringBuilder[] stringBuilders)
        {
            for (var i = 0; i < pairEnergySets.Count; i++)
            {
                var builder = stringBuilders[i];
                var site0 = (CellReferencePositionData) pairEnergySets[i].CenterPosition.Target;
                var site1 = (CellReferencePositionData) pairEnergySets[i].PartnerPosition.Target;
                var particleSet0 = (ParticleSetData) site0.Occupation.Target;
                var particleSet1 = (ParticleSetData) site1.Occupation.Target;

                if (site0.Stability == PositionStability.Unstable) builder.Append("u")
                                                                          .Append(Separator);
                builder.Append(GetFirstParticleSymbol(particleSet0))
                       .Append(Separator)
                       .Append(GetFirstParticleSymbol(particleSet1));
            }
        }

        /// <summary>
        ///     Appends the shell naming (-iNN) for each <see cref="PairEnergySetData"/> to its affiliated <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="pairEnergySets"></param>
        /// <param name="stringBuilders"></param>
        private void AppendShellNaming(IList<PairEnergySetData> pairEnergySets, StringBuilder[] stringBuilders)
        {
            var comparer = NumericComparer.CreateRanged(ShellEqualityTolerance);
            var alreadyNamed = new (PairEnergySetData, int)[pairEnergySets.Count];

            for (var i = 0; i < pairEnergySets.Count; i++)
            {
                if (alreadyNamed[i].Item1 != null) continue;

                var shellIndex = 1;
                alreadyNamed[i] = (pairEnergySets[i], shellIndex);
                for (var j = i + 1; j < pairEnergySets.Count; j++)
                {
                    if (!stringBuilders[i].Equals(stringBuilders[j])) continue;
                    if (comparer.Compare(pairEnergySets[j - 1].Distance, pairEnergySets[j].Distance) != 0) shellIndex++;
                    alreadyNamed[j] = (pairEnergySets[j], shellIndex);
                }
            }

            var decimalCount = (int) Math.Truncate(Math.Log10(alreadyNamed.Max(x => x.Item2))) + 1;

            for (var i = 0; i < alreadyNamed.Length; i++)
            {
                stringBuilders[i].Append(Separator)
                                 .Append(alreadyNamed[i].Item2.ToString($"D{decimalCount}"))
                                 .Append("NN");
            }
        }

        /// <summary>
        ///     Appends the distance numbering with chiral partner marker (-i-A/B) for each <see cref="PairEnergySetData"/> to its affiliated <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="pairEnergySets"></param>
        /// <param name="stringBuilders"></param>
        private void AppendDistanceNumberingWithChiralPartnerMarkers(IList<PairEnergySetData> pairEnergySets, StringBuilder[] stringBuilders)
        {
            var usedChiralIndices = new List<int>(10);
            for (var i = 0; i < pairEnergySets.Count - 1;)
            {
                var sameDistanceCount = 1;
                while (i + sameDistanceCount < pairEnergySets.Count && stringBuilders[i].Equals(stringBuilders[i + sameDistanceCount])) sameDistanceCount++;

                var chiralCorrection = 0;
                for (var j = 0; j < sameDistanceCount; j++)
                {
                    var index0 = i + j;
                    if (usedChiralIndices.Contains(index0))
                    {
                        chiralCorrection++;
                        continue;
                    }

                    var set = pairEnergySets[index0];
                    var distanceNumber = j + 1 - chiralCorrection;

                    if (set.ChiralPartnerModelIndex > 0)
                    {
                        var index1 = pairEnergySets.Select((x, y) => (x,y)).First(z => z.x.ChiralPartnerModelIndex == set.ModelIndex).y;
                        stringBuilders[index0].Append(Separator)
                                              .Append(distanceNumber)
                                              .Append(Separator)
                                              .Append("A");
                        stringBuilders[index1].Append(Separator)
                                              .Append(distanceNumber)
                                              .Append(Separator)
                                              .Append("B");
                        usedChiralIndices.Add(index1);
                        continue;
                    }

                    if (set.ChiralPartnerModelIndex < 0)
                    {
                        stringBuilders[index0].Append(Separator)
                                              .Append(distanceNumber);   
                    }
                }

                i += sameDistanceCount;
            }
        }

        /// <summary>
        ///     Assigns the creates names stored in the <see cref="StringBuilder"/> instances to their affiliated <see cref="PairEnergySetData"/> object
        /// </summary>
        /// <param name="pairEnergySets"></param>
        /// <param name="stringBuilders"></param>
        private static void AssignNaming(IList<PairEnergySetData> pairEnergySets, StringBuilder[] stringBuilders)
        {
            for (var i = 0; i < pairEnergySets.Count; i++)
            {
                pairEnergySets[i].Name = stringBuilders[i].ToString();
            }
        }

        /// <summary>
        ///     Get the symbol value of the first <see cref="ParticleData"/> of a <see cref="PairEnergySetData"/>
        /// </summary>
        /// <param name="particleSet"></param>
        /// <returns></returns>
        private static string GetFirstParticleSymbol(ParticleSetData particleSet)
        {
            return particleSet.Particles.Select(x => (ParticleData) x.Target).First().Symbol;
        }
    }
}