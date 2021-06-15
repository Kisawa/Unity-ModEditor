using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public partial class ModEditorWindow
    {
        void targetChanged_serializableData()
        {
            LocalRemapCoordClear();
            LocalRemapRotationClear();
            if (Manager.Target != null)
            {
                LocalRemapCoord[0] = Manager.Target.transform.position;
                LocalRemapRotation[0] = Manager.Target.transform.rotation;
            }
        }

        #region DrawUtil
        public List<Transform> DrawUtilTransCache = new List<Transform>();
        public List<DrawUtil.Cache> DrawUtilCache = new List<DrawUtil.Cache>();
        #endregion

        #region TextureBrushTab
        [SerializeField]
        bool textureBrushTabTextureView = false;
        public bool TextureBrushTabTextureView
        {
            get => textureBrushTabTextureView;
            set
            {
                if (value == textureBrushTabTextureView)
                    return;
                Undo.RecordObject(this, "TextureBrushTab TextureView Changed");
                textureBrushTabTextureView = value;
            }
        }

        [SerializeField]
        Transform textureBrushTabCurrentDrawBoard;
        public Transform TextureBrushTabCurrentDrawBoard
        {
            get => textureBrushTabCurrentDrawBoard;
            set
            {
                if (value == textureBrushTabCurrentDrawBoard)
                    return;
                Undo.RecordObject(this, "TextureBrushTab CurrentDrawBoard Changed");
                textureBrushTabCurrentDrawBoard = value;
            }
        }

        [SerializeField]
        float textureBrushTabBrushRotation;
        public float TextureBrushTabBrushRotation
        {
            get => textureBrushTabBrushRotation;
            set
            {
                if (value == textureBrushTabBrushRotation)
                    return;
                Undo.RecordObject(this, "TextureBrushTab BrushRotation Changed");
                textureBrushTabBrushRotation = value;
            }
        }

        [SerializeField]
        Texture textureBrushTabBaseTex;
        public Texture TextureBrushTabBaseTex
        {
            get => textureBrushTabBaseTex;
            set
            {
                if (value == textureBrushTabBaseTex)
                    return;
                Undo.RecordObject(this, "TextureBrushTab BaseTex Changed");
                textureBrushTabBaseTex = value;
            }
        }

        [SerializeField]
        TargetTextureType textureBrushTabTargetTextureType;
        public TargetTextureType TextureBrushTabTargetTextureType
        {
            get => textureBrushTabTargetTextureType;
            set
            {
                if (value == textureBrushTabTargetTextureType)
                    return;
                Undo.RecordObject(this, "TextureBrushTab TargetTextureType Changed");
                textureBrushTabTargetTextureType = value;
            }
        }

        [SerializeField]
        Texture textureBrushTabUtilCustomOriginTex;
        public Texture TextureBrushTabUtilCustomOriginTex
        {
            get => textureBrushTabUtilCustomOriginTex;
            set
            {
                if (value == textureBrushTabUtilCustomOriginTex)
                    return;
                Undo.RecordObject(this, "TextureBrushTab UtilCustomOriginTex Changed");
                textureBrushTabUtilCustomOriginTex = value;
            }
        }

        public RenderTexture textureBrushTabUtilCustomResultTex;
        public RenderTexture TextureBrushTabUtilCustomResultTex
        {
            get => textureBrushTabUtilCustomResultTex;
            set
            {
                if (value == textureBrushTabUtilCustomResultTex)
                    return;
                Undo.RecordObject(this, "TextureBrushTab UtilCustomResultTex Changed");
                textureBrushTabUtilCustomResultTex = value;
            }
        }
        #endregion

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
        public List<Vector3> LocalRemapCoord = new List<Vector3>();

        public void LocalRemapCoordSet(int index, Vector3 coord)
        {
            if (index < LocalRemapCoord.Count)
            {
                if (LocalRemapCoord[index] != coord)
                {
                    Undo.RecordObject(this, "LocalRemap Coord Changed");
                    LocalRemapCoord[index] = coord;
                }
            }
            else if (index == LocalRemapCoord.Count)
            {
                Undo.RecordObject(this, "LocalRemap Coord Changed");
                LocalRemapCoord.Add(coord);
            }
            else
            {
                throw new System.IndexOutOfRangeException("ModEditor: LocalRemap Coord Index out of range.");
            }
        }

        public void LocalRemapCoordRemove(int index)
        {
            Undo.RecordObject(this, "LocalRemap Coord Changed");
            LocalRemapCoord.RemoveAt(index);
            if(LocalRemapCoord.Count == 0)
                LocalRemapCoord.Add(Vector3.zero);
        }

        public void LocalRemapCoordClear()
        {
            Undo.RecordObject(this, "LocalRemap Coord Changed");
            LocalRemapCoord.Clear();
            LocalRemapCoord.Add(Vector3.zero);
        }

        public List<Quaternion> LocalRemapRotation = new List<Quaternion>();

        public void LocalRemapRotationSet(int index, Quaternion rotation)
        {
            if (index < LocalRemapRotation.Count)
            {
                if (LocalRemapRotation[index] != rotation)
                {
                    Undo.RecordObject(this, "LocalRemap Rotation Changed");
                    LocalRemapRotation[index] = rotation;
                }
            }
            else if (LocalRemapRotation.Count == index)
            {
                Undo.RecordObject(this, "LocalRemap Rotation Changed");
                LocalRemapRotation.Add(rotation);
            }
            else
            {
                throw new System.IndexOutOfRangeException("ModEditor: LocalRemap Rotation Index out of range.");
            }
        }

        public void LocalRemapRotationRemove(int index)
        {
            Undo.RecordObject(this, "LocalRemap Rotation Changed");
            LocalRemapRotation.RemoveAt(index);
            if (LocalRemapRotation.Count == 0)
                LocalRemapRotation.Add(Quaternion.identity);
        }

        public void LocalRemapRotationClear()
        {
            Undo.RecordObject(this, "LocalRemap Rotation Changed");
            LocalRemapRotation.Clear();
            LocalRemapRotation.Add(Quaternion.identity);
        }
        #endregion

        #region Blur
        [SerializeField]
        int blurDownSample = 1;
        public int BlurDownSample
        {
            get => blurDownSample;
            set
            {
                if (value == blurDownSample)
                    return;
                Undo.RecordObject(this, "Blur DownSample Changed");
                blurDownSample = value;
            }
        }

        [SerializeField]
        int blurIterations = 5;
        public int BlurIterations
        {
            get => blurIterations;
            set
            {
                if (value == blurIterations)
                    return;
                Undo.RecordObject(this, "Blur BlurIterations Changed");
                blurIterations = value;
            }
        }

        [SerializeField]
        int blurSpread = 1;
        public int BlurSpread
        {
            get => blurSpread;
            set
            {
                if (value == blurSpread)
                    return;
                Undo.RecordObject(this, "Blur Spread Changed");
                blurSpread = value;
            }
        }
        #endregion
    }
}