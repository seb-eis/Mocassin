﻿using System.Text.RegularExpressions;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.Validators
{
    /// <summary>
    /// Validator for new structure info model parametr that checks for compatibility with existing data and general object constraints
    /// </summary>
    public class StructureInfoValidator : DataValidator<IStructureInfo, MocassinStructureSettings, IStructureDataPort>
    {
        /// <summary>
        /// Creates new validator with the provided project services, settings object and data reader
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        public StructureInfoValidator(IModelProject modelProject, MocassinStructureSettings settings, IDataReader<IStructureDataPort> dataReader)
            : base(modelProject, settings, dataReader)
        {
        }

        /// <summary>
        /// Validate a new structure info parameter object in terms of compatibility with existing data and constraint settings and creates the affiliated validaton report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(IStructureInfo obj)
        {
            var report = new ValidationReport();
            AddGenericContentEqualityValidation(DataReader.Access.GetStructureInfo(), obj, report);
            AddNameValidation(obj, report);
            return report;
        }
        /// <summary>
        /// Validates the structure name and adds the results to the validation report
        /// </summary>
        /// <param name="info"></param>
        /// <param name="report"></param>
        private void AddNameValidation(IStructureInfo info, ValidationReport report)
        {
            if (!Settings.StructureName.ParseValue(info.Name, out var warnings))
            {
                report.AddWarnings(warnings);
            }
        }
    }
}
