using ICon.Framework.Operations;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Represent a data validator for the specified type that offers a validation function which create a validation report
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public interface IDataValidator<TObject>
    {
        /// <summary>
        /// Validates the object and creates a validation report containing the validation results
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        IValidationReport Validate(TObject obj);
    }
}
