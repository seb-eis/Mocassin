using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ICon.Framework.Messaging;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Factory class for basic message objects that are required throughout the model process
    /// </summary>
    public static class ModelMessages
    {
        /// <summary>
        /// Creates a non-critical raw warning message for cases where a not recommended settings is detected during validation
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateNotRecommendedWarning(object sender)
        {
            return new WarningMessage(sender, "Setting Not Recommended");
        }

        /// <summary>
        /// Creates a non-critical warning message for cases where a not recommended settings is detected during validation (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateNotRecommendedWarning(object sender, params string[] details)
        {
            var message = CreateNotRecommendedWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a new raw warning message without further details for cases where modeling objects have missing content during validation
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateMissingOrEmptyContentWarning(object sender)
        {
            return WarningMessage.CreateCritical(sender, "Content Missing Or Empty");
        }

        /// <summary>
        /// Creates a new warning message for cases where modeling objects have missing content during validation (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateMissingOrEmptyContentWarning(object sender, params string[] details)
        {
            var message = CreateMissingOrEmptyContentWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a non critical raw warning that possbly redundant content was detected
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateRedundantContentWarning(object sender)
        {
            return new WarningMessage(sender, "Possibly Redundant Content");
        }

        /// <summary>
        /// Creates a non critical raw warning that possbly redundant content was detected with the provided set of details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateRedundantContentWarning(object sender, params string[] details)
        {
            var message = CreateRedundantContentWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a non critical raw warning that informs that a warning limit was reached or surpassed
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateWarningLimitReachedWarning(object sender)
        {
            return new WarningMessage(sender, "Value Over Warning Limit");
        }

        /// <summary>
        /// Creates a non critical raw warning that redundant content was detected with the provided set of details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateWarningLimitReachedWarning(object sender, params string[] details)
        {
            var message = CreateWarningLimitReachedWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a new raw warning message for cases where a model data mismatch is detected during validation
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateContentMismatchWarning(object sender)
        {
            return WarningMessage.CreateCritical(sender, "Content Mismatch");
        }

        /// <summary>
        /// Creates a new warning message for cases where a model data mismatch is detected during validation (with arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateContentMismatchWarning(object sender, params string[] details)
        {
            var message = CreateContentMismatchWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates new raw warning message for cases where model content restriction violation is detected during validation
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateRestrictionViolationWarning(object sender)
        {
            return WarningMessage.CreateCritical(sender, "Content Restriction Violation");
        }

        /// <summary>
        /// Creates new warning message for cases where model content restriction violation is detected during validation (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateRestrictionViolationWarning(object sender, params string[] details)
        {
            var message = CreateRestrictionViolationWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates new raw warning message for cases where a model content naming violation is detected during validation
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateNamingViolationWarning(object sender)
        {
            return WarningMessage.CreateCritical(sender, "Naming Violation");
        }

        /// <summary>
        /// Creates a new warning message for cases where a model content naming violation is detected during validation (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateNamingViolationWarning(object sender, params string[] details)
        {
            var message = CreateNamingViolationWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a new raw warning message for cases where a model object duplicate is detected during validation
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateModelDuplicateWarning(object sender)
        {
            return WarningMessage.CreateCritical(sender, "Model Object Duplicate");
        }

        /// <summary>
        /// Creates a new warning message for cases where a model object duplicate is detected during validation (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateModelDuplicateWarning(object sender, params string[] details)
        {
            var message = CreateModelDuplicateWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a new raw warning message for cases where a model object causes an implicit dependency e.g. matter conversion rule erforcement
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateImplicitDependencyWarning(object sender)
        {
            return new WarningMessage(sender, "Implicit Model Dependency");
        }

        /// <summary>
        /// Creates a new warning message for cases where a model object has implicit model object dependencies due to consistency (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateImplicitDependencyWarning(object sender, params string[] details)
        {
            var message = CreateImplicitDependencyWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a new raw warning message for cases where a model definition is supported but does implicitly break or reduce feature support
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateFeatureBreakingInputWarning(object sender)
        {
            return new WarningMessage(sender, "Feature Breaking Input");
        }

        /// <summary>
        /// Creates a new raw warning message for cases where a model definition is supported but does implicitly break or reduce feature support (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateFeatureBreakingInputWarning(object sender, params string[] details)
        {
            var message = CreateFeatureBreakingInputWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a new raw warning message for cases where an identical model parameter replacement is detected during validation
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateParameterIdenticalWarning(object sender)
        {
            return WarningMessage.CreateCritical(sender, "Identical Parameter");
        }

        /// <summary>
        /// Creates a new warning message for cases where an identical model parameter replacement is detected during validation (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateParameterIdenticalWarning(object sender, params string[] details)
        {
            var message = CreateParameterIdenticalWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a new raw warning message that a conflict was successfully resolved by a conflict resolver
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateConflictHandlingWarning(object sender)
        {
            return new WarningMessage(sender, "Content Auto-Update (SUCCESS)");
        }

        /// <summary>
        /// Creates a new warning message that a conflict was successfully resolved by a conflict resolver (With arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateConflictHandlingWarning(object sender, params string[] details)
        {
            var message = CreateConflictHandlingWarning(sender);
            message.AddDetails(details);
            return message;
        }

        /// <summary>
        /// Creates a new raw warning message for cases where a model data conflict resolvers had to auto correct data by loading a default value
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static WarningMessage CreateContentResetWarning(object sender)
        {
            return new WarningMessage(sender, "Content Auto-Update (RESET)");
        }

        /// <summary>
        /// Creates a new raw warning message for cases where a model data conflict resolvers had to auto correct data (with arbitrary number of details)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static WarningMessage CreateContentResetWarning(object sender, params string[] details)
        {
            var message = CreateContentResetWarning(sender);
            message.AddDetails(details);
            return message;
        }
    }
}
