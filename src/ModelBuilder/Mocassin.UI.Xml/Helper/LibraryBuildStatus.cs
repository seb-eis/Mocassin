using Mocassin.Model.Translator;

#pragma warning disable 1591

namespace Mocassin.UI.Data.Helper
{
    /// <summary>
    ///     The enumeration to signal about the current status of the <see cref="ISimulationLibrary" /> building
    ///     process
    /// </summary>
    public enum LibraryBuildStatus
    {
        Unknown,
        Error,
        Cancel,
        BuildProcessCompleted,
        BuildProcessStarted,
        PreparingModelProject,
        ModelProjectPreparationError,
        BuildingModelContext,
        ModelContextBuildError,
        PreparingModelCustomization,
        ModelCustomizationPreparationError,
        PreparingLibrary,
        LibraryPreparationError,
        BuildingLibrary,
        LibraryBuildingError,
        AddingLibraryMetaData,
        MetaDataAddError,
        SavingLibraryContents,
        LibraryContentSavingError
    }
}