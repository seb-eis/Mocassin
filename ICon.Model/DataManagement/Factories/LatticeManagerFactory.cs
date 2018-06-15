﻿using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Lattices;

namespace ICon.Model.DataManagement
{
    /// <summary>
    /// Fatory for new lattice manager systems
    /// </summary>
    public class LatticeManagerFactory : IModelManagerFactory
    {
        /// <summary>
        /// Tries to create a new energy manager with the provided project service. Returns the used data object as an out parameter
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public IModelManager CreateNew(IProjectServices projectServices, out object dataObject)
        {
            var data = LatticeModelData.CreateNew();
            dataObject = data;
            return new LatticeManager(projectServices, data);
        }
    }
}
