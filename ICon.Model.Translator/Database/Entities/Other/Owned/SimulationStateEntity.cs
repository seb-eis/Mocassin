namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Simulation state entity that can switch between the .NET object and binary simulation state representation
    /// </summary>
    public class SimulationStateEntity : BlobEntityBase
    {
        /// <inheritdoc />
        public override void ChangeStateToBinary(IMarshalProvider marshalProvider)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void ChangeStateToObject(IMarshalProvider marshalProvider)
        {
            throw new System.NotImplementedException();
        }
    }
}