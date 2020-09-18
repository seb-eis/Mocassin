using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Doping information that describes the element, concentration, sublattice which is substituted.
    ///     May also contain information about counter doping which is described in the same manner.
    /// </summary>
    public class Doping : ModelObject, IDoping
    {
        /// <summary>
        ///     Information about the doping (particles and sublattice)
        /// </summary>
        [UseTrackedData]
        public IDopingCombination PrimaryDoping { set; get; }

        /// <summary>
        ///     Information about the counter doping (particles and sublattice)
        /// </summary>
        [UseTrackedData]
        public IDopingCombination CounterDoping { set; get; }

        /// <summary>
        ///     Building Block in which the doping should take place
        /// </summary>
        [UseTrackedData]
        public IBuildingBlock BuildingBlock { get; set; }

        /// <summary>
        ///     Flag to indicate whether a counter doping should be applied
        /// </summary>
        public bool UseCounterDoping { get; set; }

        /// <summary>
        ///     Doping Group for simutaneous doping
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///     Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string ObjectName => "Doping";

        /// <summary>
        ///     Copies the information from the provided model object interface and returns the object (Returns null if type
        ///     mismatch)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IDoping>(obj) is {} doping)) return null;
            PrimaryDoping = doping.PrimaryDoping;
            CounterDoping = doping.CounterDoping;
            BuildingBlock = doping.BuildingBlock;
            UseCounterDoping = doping.UseCounterDoping;
            Priority = doping.Priority;
            return this;
        }
    }
}