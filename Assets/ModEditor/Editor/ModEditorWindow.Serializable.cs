using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class ModEditorWindow
    {
        void targetChanged_serializableRefresh()
        {
            if (Manager.Target != null)
            {
                localRemapCoord = Manager.Target.transform.position;
                localRemapRotation = Manager.Target.transform.rotation;
            }
        }

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
        public Vector3 localRemapCoord;
        public Vector3 LocalRemapCoord
        {
            get => localRemapCoord;
            set
            {
                if (value == localRemapCoord)
                    return;
                Undo.RecordObject(this, "LoaclRemap Coord Changed");
                localRemapCoord = value;
            }
        }

        public Quaternion localRemapRotation = Quaternion.identity;
        public Quaternion LocalRemapRotation
        {
            get => localRemapRotation;
            set
            {
                if (value == localRemapRotation)
                    return;
                Undo.RecordObject(this, "LoaclRemap Rotation Changed");
                localRemapRotation = value;
            }
        }
        #endregion
    }
}