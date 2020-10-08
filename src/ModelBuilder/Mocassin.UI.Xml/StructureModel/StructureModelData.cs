using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.StructureModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the
    ///     <see cref="Mocassin.Model.Structures.IStructureManager" />
    ///     system
    /// </summary>
    [XmlRoot]
    public class StructureModelData : ModelManagerData
    {
        private CellParametersData cellParameters;
        private ObservableCollection<CellReferencePositionData> cellReferencePositions;
        private ObservableCollection<DummyPositionData> dummyPositions;
        private SpaceGroupInfoData spaceGroupInfo;
        private StructureInfoData structureInfo;

        /// <summary>
        ///     Get or set the xml structure info
        /// </summary>
        [XmlElement]
        public StructureInfoData StructureInfo
        {
            get => structureInfo;
            set => SetProperty(ref structureInfo, value);
        }

        /// <summary>
        ///     Get or set the xml space group info
        /// </summary>
        [XmlElement]
        public SpaceGroupInfoData SpaceGroupInfo
        {
            get => spaceGroupInfo;
            set => SetProperty(ref spaceGroupInfo, value);
        }

        /// <summary>
        ///     Get or set the xml cell parameters
        /// </summary>
        [XmlElement]
        public CellParametersData CellParameters
        {
            get => cellParameters;
            set => SetProperty(ref cellParameters, value);
        }

        /// <summary>
        ///     Get or set the list of stable and unstable wyckoff positions
        /// </summary>
        [XmlArray]
        public ObservableCollection<CellReferencePositionData> CellReferencePositions
        {
            get => cellReferencePositions;
            set => SetProperty(ref cellReferencePositions, value);
        }

        /// <summary>
        ///     Get or set the list of wyckoff dummy positions that are not part of the model
        /// </summary>
        [XmlArray]
        public ObservableCollection<DummyPositionData> DummyPositions
        {
            get => dummyPositions;
            set => SetProperty(ref dummyPositions, value);
        }

        /// <summary>
        ///     Creates a new <see cref="StructureModelData" /> with empty component lists
        /// </summary>
        public StructureModelData()
        {
            StructureInfo = new StructureInfoData();
            CellParameters = new CellParametersData();
            SpaceGroupInfo = new SpaceGroupInfoData();
            CellReferencePositions = new ObservableCollection<CellReferencePositionData>();
            DummyPositions = new ObservableCollection<DummyPositionData>();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield return StructureInfo.GetInputObject();
            yield return SpaceGroupInfo.GetInputObject();
            yield return CellParameters.GetInputObject();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return CellReferencePositions
                   .Select(x => x.GetInputObject())
                   .Concat(DummyPositions.Select(x => x.GetInputObject()));
        }
    }
}