using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IGroupInteraction" />
    public class GroupInteraction : ModelObject, IGroupInteraction
    {
        /// <inheritdoc />
        public int GroupSize => GeometryVectors.Count + 1;

        /// <inheritdoc />
        [UseTrackedData]
        public ICellSite CenterCellSite { get; set; }

        /// <summary>
        ///     The list of 3D fractional vectors that describe the group geometry
        /// </summary>
        public List<Fractional3D> GeometryVectors { get; set; }

        /// <summary>
        ///     The energy dictionaries for each center particle and surrounding occupation state (Auto managed by model)
        /// </summary>
        public Dictionary<IParticle, Dictionary<OccupationState, double>> EnergyDictionarySet { get; set; }

        /// <inheritdoc />
        public override string ObjectName => "Group Interaction";

        /// <inheritdoc />
        public IEnumerable<Fractional3D> GetSurroundingGeometry() => (GeometryVectors ?? new List<Fractional3D>()).AsEnumerable();

        /// <inheritdoc />
        public IEnumerable<Fractional3D> GetFullGeometry() => CenterCellSite.Vector.AsSingleton().Concat(GetSurroundingGeometry());

        /// <inheritdoc />
        public IReadOnlyDictionary<IParticle, IReadOnlyDictionary<OccupationState, double>> GetEnergyDictionarySet()
        {
            var result = new Dictionary<IParticle, IReadOnlyDictionary<OccupationState, double>>();
            if (EnergyDictionarySet == null)
                return result;

            foreach (var item in EnergyDictionarySet)
                result.Add(item.Key, item.Value);

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<GroupEnergyEntry> GetEnergyEntries()
        {
            foreach (var outerItem in EnergyDictionarySet)
            {
                foreach (var innerItem in outerItem.Value)
                    yield return new GroupEnergyEntry(outerItem.Key, innerItem.Key, innerItem.Value);
            }
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IGroupInteraction>(obj) is { } interaction))
                return null;

            CenterCellSite = interaction.CenterCellSite;
            GeometryVectors = interaction.GetSurroundingGeometry().ToList();
            EnergyDictionarySet = new Dictionary<IParticle, Dictionary<OccupationState, double>>();

            foreach (var item in interaction.GetEnergyDictionarySet())
            {
                EnergyDictionarySet.Add(item.Key, new Dictionary<OccupationState, double>(item.Value.Count));
                foreach (var inner in item.Value) EnergyDictionarySet[item.Key].Add(inner.Key, inner.Value);
            }

            return this;
        }

        /// <summary>
        ///     Tries to set the passed energy entry in the energy dictionary. Returns false if the value cannot be set
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <returns></returns>
        public bool TrySetEnergyEntry(in GroupEnergyEntry energyEntry)
        {
            if (!EnergyDictionarySet.TryGetValue(energyEntry.CenterParticle, out var innerDictionary))
                return false;

            var searchEntry = energyEntry.GroupOccupation as OccupationState ?? new OccupationState(energyEntry.GroupOccupation);

            if (!innerDictionary.ContainsKey(searchEntry))
                return false;

            innerDictionary[searchEntry] = energyEntry.Energy;
            return true;
        }
    }
}