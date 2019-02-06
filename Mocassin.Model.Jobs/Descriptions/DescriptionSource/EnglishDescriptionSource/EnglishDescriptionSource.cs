using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Mml.Descriptions
{
    /// <summary>
    ///     The default english object description source that transforms object instances into human readable string
    ///     descriptions in the english language
    /// </summary>
    public class EnglishDescriptionSource : PipelineDescriptionSource
    {
        /// <summary>
        ///     The default statement separator for multiple descriptions
        /// </summary>
        private const string StatmentSeparator = " - ";

        /// <summary>
        ///     Get the description string for a fraction vector interface
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [DescriptionCreatorMethod]
        public string GetDescription(IFractional3D vector)
        {
            return $"[Fractional {vector.A} {vector.B} {vector.C}]";
        }

        /// <summary>
        ///     Get the description string for the pair energy entry
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <returns></returns>
        [DescriptionCreatorMethod]
        public string GetDescription(PairEnergyEntry energyEntry)
        {
            return $"Pair energy in {Units.Energy}" +
                   $" for {GetIdentification(energyEntry.ParticlePair.Particle0)} at origin" +
                   $" and {GetIdentification(energyEntry.ParticlePair.Particle1)} as partner.";
        }

        /// <summary>
        ///     Get the description string for the pair energy model
        /// </summary>
        /// <param name="energyModel"></param>
        /// <returns></returns>
        [DescriptionCreatorMethod]
        public string GetDescription(IPairEnergyModel energyModel)
        {
            var builder = new StringBuilder(100);
            builder.Append($"Energy Model with {energyModel.EnergyEntries.Count} unique permutations." + StatmentSeparator);
            builder.Append(GetDescription(energyModel.PairInteraction));
            return builder.ToString();
        }

        /// <summary>
        ///     Get the description string for the pair interaction
        /// </summary>
        /// <param name="pairInteraction"></param>
        /// <returns></returns>
        [DescriptionCreatorMethod]
        public string GetDescription(IPairInteraction pairInteraction)
        {
            return $"{GetIdentification(pairInteraction)} at {AsDistance(pairInteraction.Distance)}" +
                   $" from {GetIdentification(pairInteraction.Position0)} at {GetDescription(pairInteraction.Position0.Vector)}" +
                   $" to {GetIdentification(pairInteraction.Position1)} at {GetDescription(pairInteraction.GetSecondPositionVector())}";
        }

        /// <summary>
        ///     Get the human readable default base identification string for a model object interface
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public static string GetIdentification(IModelObject modelObject)
        {
            return $"[{modelObject.GetObjectName()}, {modelObject.Index}, {modelObject.Key ?? "Unnamed"}]";
        }

        /// <summary>
        ///     Returns a string representation of a distance value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AsDistance(double value)
        {
            return $"[{value} {Units.Length}]";
        }

        /// <summary>
        ///     Returns a string representation of an energy value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AsEnergy(double value)
        {
            return $"[{value} {Units.Energy}]";
        }
    }
}