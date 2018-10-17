using Mocassin.Framework.Messaging;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Factory class for basic message objects that are required throughout the model process
    /// </summary>
    public static class ModelMessageSource
    {
        /// <summary>
        ///     Creates a non-critical warning message for cases where a not recommended settings is detected during validation
        ///     (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateNotRecommendedWarning(object sender, params string[] details)
        {
            var message = new WarningMessage(sender, "Setting Not Recommended");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new warning message for cases where modeling objects have missing content during validation (With
        ///     arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateMissingOrEmptyContentWarning(object sender, params string[] details)
        {
            var message = WarningMessage.CreateCritical(sender, "Content Missing Or Empty");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a non critical raw warning that possibly redundant content was detected with the provided set of details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateRedundantContentWarning(object sender, params string[] details)
        {
            var message = new WarningMessage(sender, "Possibly Redundant Content");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a non critical raw warning that redundant content was detected with the provided set of details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateWarningLimitReachedWarning(object sender, params string[] details)
        {
            var message = new WarningMessage(sender, "Value Over Warning Limit");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new warning message for cases where a model data mismatch is detected during validation (with arbitrary
        ///     number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateContentMismatchWarning(object sender, params string[] details)
        {
            var message = WarningMessage.CreateCritical(sender, "Content Mismatch");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates new warning message for cases where model content restriction violation is detected during validation (With
        ///     arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateRestrictionViolationWarning(object sender, params string[] details)
        {
            var message = WarningMessage.CreateCritical(sender, "Content Restriction Violation");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new warning message for cases where a model content naming violation is detected during validation (With
        ///     arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateNamingViolationWarning(object sender, params string[] details)
        {
            var message = WarningMessage.CreateCritical(sender, "Naming Violation");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new warning message for cases where a model object duplicate is detected during validation (With
        ///     arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateModelDuplicateWarning(object sender, params string[] details)
        {
            var message = WarningMessage.CreateCritical(sender, "Model Object Duplicate");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new warning message for cases where a model object has implicit model object dependencies due to
        ///     consistency (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateImplicitDependencyWarning(object sender, params string[] details)
        {
            var message = new WarningMessage(sender, "Implicit Model Dependency");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new raw warning message for cases where a model definition is supported but does implicitly break or
        ///     reduce feature support (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateFeatureBreakingInputWarning(object sender, params string[] details)
        {
            var message = new WarningMessage(sender, "Feature Breaking Input");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new warning message for cases where an identical model parameter replacement is detected during
        ///     validation (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateParameterIdenticalWarning(object sender, params string[] details)
        {
            var message = WarningMessage.CreateCritical(sender, "Identical Parameter");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new warning message that a conflict was successfully resolved by a conflict resolver (With arbitrary
        ///     number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateConflictHandlingWarning(object sender, params string[] details)
        {
            var message = new WarningMessage(sender, "Content Auto-Update (SUCCESS)");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new raw warning message for cases where a model data conflict resolvers had to auto correct data (with
        ///     arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateContentResetWarning(object sender, params string[] details)
        {
            var message = new WarningMessage(sender, "Content Auto-Update (RESET)");
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        ///     Creates a new raw critical warning for cases where user data is validated that can lead to exceptions (With
        ///     arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateUserInducedExceptionWarning(object sender, params string[] details)
        {
            var message = WarningMessage.CreateCritical(sender, "User Induced Exception");
            message.AddDetails(details);
            return message;
        }
    }
}