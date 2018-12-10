namespace Mocassin.Model.Translator.DbBuilder
{
    /// <summary>
    ///     The job package db model builder that translates job package creation instructions into the simulation database
    ///     context
    /// </summary>
    public interface IJobPackageDbModelBuilder
    {
        /// <summary>
        ///     Get the simulation database context that the builder uses
        /// </summary>
        ISimulationDbContext SimulationDbContext { get; set; }

        /// <summary>
        ///     Get or set the structure database model builder the builder uses
        /// </summary>
        IStructureDbModelBuilder StructureDbModelBuilder { get; set; }

        /// <summary>
        ///     Get or set the energy database model builder the builder uses
        /// </summary>
        IEnergyDbModelBuilder EnergyDbModelBuilder { get; set; }

        /// <summary>
        ///     Get or set the transition database model builder the builder uses
        /// </summary>
        ITransitionDbModelBuilder TransitionDbModelBuilder { get; set; }
    }
}