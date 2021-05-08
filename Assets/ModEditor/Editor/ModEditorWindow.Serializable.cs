using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class ModEditorWindow
    {
        #region Copy
        [SerializeField]
        Copy.DataType copyCurrentData = Copy.DataType.VertexColor;
        public Copy.DataType CopyCurrentData
        {
            get => copyCurrentData;
            set
            {
                if (value == copyCurrentData)
                    return;
                Undo.RecordObject(this, "Copy CurrentData Changed");
                copyCurrentData = value;
            }
        }
        #endregion

        #region RecalcMesh
        [SerializeField]
        RecalcMesh.CalcType recalcMeshCalcType = RecalcMesh.CalcType.Normals;
        public RecalcMesh.CalcType RecalcMeshCalcType
        {
            get => recalcMeshCalcType;
            set
            {
                if (value == recalcMeshCalcType)
                    return;
                Undo.RecordObject(this, "RecalcMesh CalcType Changed");
                recalcMeshCalcType = value;
            }
        }
        #endregion

        #region DirectionPull
        [SerializeField]
        DirectionPull.DirectionType directionPullDirectionType = DirectionPull.DirectionType.Normal;
        public DirectionPull.DirectionType DirectionPullDirectionType
        {
            get => directionPullDirectionType;
            set
            {
                if (value == directionPullDirectionType)
                    return;
                Undo.RecordObject(this, "DirectionPull DirectionType Changed");
                directionPullDirectionType = value;
            }
        }

        [SerializeField]
        bool directionPullCalcNormal;
        public bool DirectionPullCalcNormal
        {
            get => directionPullCalcNormal;
            set
            {
                if (value == directionPullCalcNormal)
                    return;
                Undo.RecordObject(this, "DirectionPull CalcNormal Changed");
                directionPullCalcNormal = value;
            }
        }
        #endregion

        #region LocalRemap
        [SerializeField]
        Vector3 coord;
        public Vector3 Coord
        {
            get => coord;
            set
            {
                if (value == coord)
                    return;
                Undo.RecordObject(this, "LoaclRemap Coord Changed");
                coord = value;
            }
        }
        #endregion
    }
}