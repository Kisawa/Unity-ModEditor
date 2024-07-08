using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class ModEditorManager : ScriptableObject
    {
        [SerializeField]
        bool lockTarget = true;
        public bool LockTarget
        {
            get => lockTarget;
            set {
                if (value == lockTarget)
                    return;
                Undo.RecordObject(this, "ModEditor LockTarget");
                lockTarget = value;
            }
        }

        [SerializeField]
        ExposedReference<GameObject> target;
        public GameObject Target
        {
            get => target.Resolve(ModEditorWindow.ExposedManagement);
            set
            {
                if (LockTarget)
                    return;
                target = ModEditorWindow.ExposedManagement.LinkExposedReference(value);
                EditorUtility.SetDirty(this);
            }
        }

        [SerializeField]
        bool gameCameraFollow;
        public bool GameCameraFollow
        {
            get => gameCameraFollow;
            set
            {
                if (value == gameCameraFollow)
                    return;
                Undo.RecordObject(this, "ModEditor GameCameraFollow");
                gameCameraFollow = value;
            }
        }

        [SerializeField]
        SerializableClass.Dictionary_Obj_Bool actionableDic;
        public SerializableClass.Dictionary_Obj_Bool ActionableDic { get => actionableDic; }

        [SerializeField]
        SerializableClass.Dictionary_Obj_Mesh meshDic;
        public SerializableClass.Dictionary_Obj_Mesh MeshDic { get => meshDic; }

        public List<GameObject> TargetChildren { get; set; } = new List<GameObject>();

        [SerializeField]
        bool sceneCollectionView = true;
        public bool SceneCollectionView
        {
            get => sceneCollectionView;
            set
            {
                if (value == sceneCollectionView)
                    return;
                Undo.RecordObject(this, "ModEditor SceneCollectionView");
                sceneCollectionView = value;
            }
        }

        #region Vertex Shading
        [SerializeField]
        Color unselectedVertexColor = Color.white;
        public Color UnselectedVertexColor
        {
            get => unselectedVertexColor;
            set
            {
                if (value == unselectedVertexColor)
                    return;
                Undo.RecordObject(this, "ModEditor UnselectedVertexColor");
                unselectedVertexColor = value;
            }
        }

        [SerializeField]
        Color selectedVertexColor = Color.black;
        public Color SelectedVertexColor
        {
            get => selectedVertexColor;
            set
            {
                if (value == selectedVertexColor)
                    return;
                Undo.RecordObject(this, "ModEditor SelectedVertexColor");
                selectedVertexColor = value;
            }
        }

        [SerializeField]
        float vertexScale = 0.5f;
        public float VertexScale
        {
            get => vertexScale;
            set
            {
                if (value == vertexScale)
                    return;
                Undo.RecordObject(this, "ModEditor VertexScale");
                vertexScale = value;
            }
        }

        [SerializeField]
        bool vertexWithZTest = true;
        public bool VertexWithZTest
        {
            get => vertexWithZTest;
            set
            {
                if (value == vertexWithZTest)
                    return;
                Undo.RecordObject(this, "ModEditor VertexWithZTest");
                vertexWithZTest = value;
            }
        }

        [SerializeField]
        bool hideUnselectedVertex = false;
        public bool HideUnselectedVertex
        {
            get => hideUnselectedVertex;
            set
            {
                if (value == hideUnselectedVertex)
                    return;
                Undo.RecordObject(this, "ModEditor HideUnselectedVertex");
                hideUnselectedVertex = value;
            }
        }
        #endregion

        #region Normal Shading
        [SerializeField]
        bool normalView;
        public bool NormalView
        {
            get => normalView;
            set
            {
                if (value == normalView)
                    return;
                Undo.RecordObject(this, "ModEditor NormalView");
                normalView = value;
            }
        }

        [SerializeField]
        bool normalViewUnfold = true;
        public bool NormalViewUnfold
        {
            get => normalViewUnfold;
            set
            {
                if (value == normalViewUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor NormalViewUnfold");
                normalViewUnfold = value;
            }
        }

        [SerializeField]
        Color normalColor = Color.red;
        public Color NormalColor
        {
            get => normalColor;
            set
            {
                if (value == normalColor)
                    return;
                Undo.RecordObject(this, "ModEditor NormalColor");
                normalColor = value;
            }
        }

        [SerializeField]
        float normalLength = 0.5f;
        public float NormalLength
        {
            get => normalLength;
            set
            {
                if (value == normalLength)
                    return;
                Undo.RecordObject(this, "ModEditor NormalLength");
                normalLength = value;
            }
        }
        #endregion

        #region Tangent Shading
        [SerializeField]
        bool tangentView;
        public bool TangentView
        {
            get => tangentView;
            set
            {
                if (value == tangentView)
                    return;
                Undo.RecordObject(this, "ModEditor TangentView");
                tangentView = value;
            }
        }

        [SerializeField]
        bool tangentViewUnfold = true;
        public bool TangentViewUnfold
        {
            get => tangentViewUnfold;
            set
            {
                if (value == tangentViewUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor TangentViewUnfold");
                tangentViewUnfold = value;
            }
        }

        [SerializeField]
        Color tangentColor = Color.green;
        public Color TangentColor
        {
            get => tangentColor;
            set
            {
                if (value == tangentColor)
                    return;
                Undo.RecordObject(this, "ModEditor TangentColor");
                tangentColor = value;
            }
        }

        [SerializeField]
        float tangentLength = 0.5f;
        public float TangentLength
        {
            get => tangentLength;
            set
            {
                if (value == tangentLength)
                    return;
                Undo.RecordObject(this, "ModEditor TangentLength");
                tangentLength = value;
            }
        }

        [SerializeField]
        float arrowLength = 0.2f;
        public float ArrowLength
        {
            get => arrowLength;
            set
            {
                if (value == arrowLength)
                    return;
                Undo.RecordObject(this, "ModEditor ArrowLength");
                arrowLength = value;
            }
        }

        [SerializeField]
        float arrowSize = 0.1f;
        public float ArrowSize
        {
            get => arrowSize;
            set
            {
                if (value == arrowSize)
                    return;
                Undo.RecordObject(this, "ModEditor ArrowSize");
                arrowSize = value;
            }
        }
        #endregion

        #region Grid Shading
        [SerializeField]
        bool gridView;
        public bool GridView
        {
            get => gridView;
            set
            {
                if (value == gridView)
                    return;
                Undo.RecordObject(this, "ModEditor GridView");
                gridView = value;
            }
        }

        [SerializeField]
        bool gridViewUnfold = true;
        public bool GridViewUnfold
        {
            get => gridViewUnfold;
            set
            {
                if (value == gridViewUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor GridViewUnfold");
                gridViewUnfold = value;
            }
        }

        [SerializeField]
        Color gridColor = Color.white;
        public Color GridColor
        {
            get => gridColor;
            set
            {
                if (value == gridColor)
                    return;
                Undo.RecordObject(this, "ModEditor GridColor");
                gridColor = value;
            }
        }

        [SerializeField]
        bool gridWithZTest = false;
        public bool GridWithZTest
        {
            get => gridWithZTest;
            set
            {
                if (value == gridWithZTest)
                    return;
                Undo.RecordObject(this, "ModEditor GridWithZTest");
                gridWithZTest = value;
            }
        }
        #endregion

        #region UV Shading
        [SerializeField]
        bool uvView;
        public bool UVView
        {
            get => uvView;
            set
            {
                if (value == uvView)
                    return;
                Undo.RecordObject(this, "ModEditor UVView");
                uvView = value;
            }
        }

        [SerializeField]
        UVType uvViewType;
        public UVType UVViewType
        {
            get => uvViewType;
            set
            {
                if (value == uvViewType)
                    return;
                Undo.RecordObject(this, "ModEditor UVViewType");
                uvViewType = value;
            }
        }

        [SerializeField]
        bool uvViewUnfold = true;
        public bool UVViewUnfold
        {
            get => uvViewUnfold;
            set
            {
                if (value == uvViewUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor UVViewUnfold");
                uvViewUnfold = value;
            }
        }

        [SerializeField]
        float uvAlpha = 1;
        public float UVAlpha
        {
            get => uvAlpha;
            set
            {
                if (value == uvAlpha)
                    return;
                Undo.RecordObject(this, "ModEditor UVAlpha");
                uvAlpha = value;
            }
        }
        #endregion

        #region VertexColor Shading
        [SerializeField]
        bool vertexColorView;
        public bool VertexColorView
        {
            get => vertexColorView;
            set
            {
                if (value == vertexColorView)
                    return;
                Undo.RecordObject(this, "ModEditor VertexColorView");
                vertexColorView = value;
            }
        }
        #endregion

        #region DepthMap Shading
        [SerializeField]
        bool depthMapView;
        public bool DepthMapView
        {
            get => depthMapView;
            set
            {
                if (value == depthMapView)
                    return;
                Undo.RecordObject(this, "ModEditor DepthMapView");
                depthMapView = value;
            }
        }

        [SerializeField]
        bool depthMapViewUnfold = true;
        public bool DepthMapViewUnfold
        {
            get => depthMapViewUnfold;
            set
            {
                if (value == depthMapViewUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor DepthMapViewUnfold");
                depthMapViewUnfold = value;
            }
        }

        [SerializeField]
        float depthCompress = 0.1f;
        public float DepthCompress
        {
            get => depthCompress;
            set
            {
                if (value == depthCompress)
                    return;
                Undo.RecordObject(this, "ModEditor DepthCompress");
                depthCompress = value;
            }
        }
        #endregion

        #region NormalMap Shading
        [SerializeField]
        bool normalMapView;
        public bool NormalMapView
        {
            get => normalMapView;
            set
            {
                if (value == normalMapView)
                    return;
                Undo.RecordObject(this, "ModEditor NormalMapView");
                normalMapView = value;
            }
        }
        #endregion

        #region Map Shading
        [SerializeField]
        bool mapView;
        public bool MapView
        {
            get => mapView;
            set
            {
                if (value == mapView)
                    return;
                Undo.RecordObject(this, "ModEditor MapView");
                mapView = value;
            }
        }

        [SerializeField]
        bool mapViewUnfold = true;
        public bool MapViewUnfold
        {
            get => mapViewUnfold;
            set
            {
                if (value == mapViewUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor MapViewUnfold");
                mapViewUnfold = value;
            }
        }

        [SerializeField]
        Texture mapViewTex;
        public Texture MapViewTex
        {
            get => mapViewTex;
            set
            {
                if (value == mapViewTex)
                    return;
                Undo.RecordObject(this, "ModEditor MapViewTex");
                mapViewTex = value;
            }
        }

        [SerializeField]
        TexViewPass mapViewPass;
        public TexViewPass MapViewPass
        {
            get => mapViewPass;
            set
            {
                if (value == mapViewPass)
                    return;
                Undo.RecordObject(this, "ModEditor MapViewPass");
                mapViewPass = value;
            }
        }

        [SerializeField]
        Color mapViewColorTint = Color.white;
        public Color MapViewColorTint
        {
            get => mapViewColorTint;
            set
            {
                if (value == mapViewColorTint)
                    return;
                Undo.RecordObject(this, "ModEditor MapViewColorTint");
                mapViewColorTint = value;
            }
        }
        #endregion

        #region VertexBrush
        [SerializeField]
        bool vertexBrushUnfold = true;
        public bool VertexBrushUnfold
        {
            get => vertexBrushUnfold;
            set
            {
                if (value == vertexBrushUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushUnfold");
                vertexBrushUnfold = value;
            }
        }

        [SerializeField]
        VertexBrushType vertexBrushType = VertexBrushType.Color;
        public VertexBrushType VertexBrushType
        {
            get => vertexBrushType;
            set
            {
                if (value == vertexBrushType)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushType");
                vertexBrushType = value;
            }
        }

        [SerializeField]
        float vertexBrushStrength = 1;
        public float VertexBrushStrength
        {
            get => vertexBrushStrength;
            set
            {
                if (value == vertexBrushStrength)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushStrength");
                vertexBrushStrength = value;
            }
        }

        [SerializeField]
        Color vertexBrushColor = Color.white;
        public Color VertexBrushColor
        {
            get => vertexBrushColor;
            set
            {
                if (value == vertexBrushColor)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushColor");
                vertexBrushColor = value;
            }
        }

        [SerializeField]
        Color vertexBrushColorFrom = Color.red;
        public Color VertexBrushColorFrom
        {
            get => vertexBrushColorFrom;
            set
            {
                if (value == vertexBrushColorFrom)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushColorFrom");
                vertexBrushColorFrom = value;
            }
        }

        [SerializeField]
        float vertexBrushColorFromStep = 0;
        public float VertexBrushColorFromStep
        {
            get => vertexBrushColorFromStep;
            set
            {
                if (value < 0)
                    value = 0;
                if (value > vertexBrushColorToStep - 0.001f)
                    value = vertexBrushColorToStep - 0.001f;
                if (value == vertexBrushColorFromStep)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushColorFromStep");
                vertexBrushColorFromStep = value;
            }
        }

        [SerializeField]
        Color vertexBrushColorTo = Color.red;
        public Color VertexBrushColorTo
        {
            get => vertexBrushColorTo;
            set
            {
                if (value == vertexBrushColorTo)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushColorTo");
                vertexBrushColorTo = value;
            }
        }

        [SerializeField]
        float vertexBrushColorToStep = 1;
        public float VertexBrushColorToStep
        {
            get => vertexBrushColorToStep;
            set
            {
                if (value < vertexBrushColorFromStep + 0.001f)
                    value = vertexBrushColorFromStep + 0.001f;
                if (value > 1)
                    value = 1;
                if (value == vertexBrushColorToStep)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushColorToStep");
                vertexBrushColorToStep = value;
            }
        }

        [SerializeField]
        Color vertexBrushViewColor = Color.black * 0.3f;
        public Color VertexBrushScopeViewColor
        {
            get => vertexBrushViewColor;
            set
            {
                if (value == vertexBrushViewColor)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushScopeViewColor");
                vertexBrushViewColor = value;
            }
        }

        [SerializeField]
        float vertexBrushSize = 0.05f;
        public float VertexBrushSize
        {
            get => vertexBrushSize;
            set
            {
                if (value == vertexBrushSize)
                    return;
                if (value < 0.01 || value > 0.5)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushSize");
                vertexBrushSize = value;
            }
        }

        [SerializeField]
        float vertexBrushDepth = 10;
        public float VertexBrushDepth
        {
            get => vertexBrushDepth;
            set
            {
                if (value == vertexBrushDepth)
                    return;
                if (value < 0)
                    value = 0;
                if (value > ModEditorConstants.VertexBrushMaxDepth)
                    value = ModEditorConstants.VertexBrushMaxDepth;
                Undo.RecordObject(this, "ModEditor VertexBrushDepth");
                vertexBrushDepth = value;
            }
        }

        [SerializeField]
        bool vertexBrushClamp = false;
        public bool VertexBrushClamp
        {
            get => vertexBrushClamp;
            set
            {
                if (value == vertexBrushClamp)
                    return;
                Undo.RecordObject(this, "ModEditor VertexBrushClamp");
                vertexBrushClamp = value;
            }
        }

        public float vertexBrushClampMin = 0;
        public float VertexBrushClampMin
        {
            get => vertexBrushClampMin;
            set
            {
                if (value == vertexBrushClampMin)
                    return;
                if (value > vertexBrushClampMax)
                    value = vertexBrushClampMax;
                Undo.RecordObject(this, "ModEditor VertexBrushClampMin");
                vertexBrushClampMin = value;
            }
        }

        public float vertexBrushClampMax = 1;
        public float VertexBrushClampMax
        {
            get => vertexBrushClampMax;
            set
            {
                if (value == vertexBrushClampMax)
                    return;
                if (value < vertexBrushClampMin)
                    value = vertexBrushClampMin;
                Undo.RecordObject(this, "ModEditor VertexBrushClampMax");
                vertexBrushClampMax = value;
            }
        }
        #endregion

        #region Write
        [SerializeField]
        bool writeUnfold = true;
        public bool WriteUnfold
        {
            get => writeUnfold;
            set
            {
                if (value == writeUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor WriteUnfold");
                writeUnfold = value;
            }
        }

        [SerializeField]
        WriteType writeType = WriteType.Replace;
        public WriteType WriteType
        {
            get => writeType;
            set
            {
                if (value == writeType)
                    return;
                Undo.RecordObject(this, "ModEditor WriteType");
                writeType = value;
            }
        }

        [SerializeField]
        WriteTargetType writeTargetType = WriteTargetType.VertexColor;
        public WriteTargetType WriteTargetType
        {
            get => writeTargetType;
            set
            {
                if (value == writeTargetType)
                    return;
                Undo.RecordObject(this, "ModEditor WriteTargetType");
                writeTargetType = value;
            }
        }

        [SerializeField]
        CustomTargetType customTargetType_X = CustomTargetType.None;
        public CustomTargetType CustomTargetType_X
        {
            get => customTargetType_X;
            set
            {
                if (value == customTargetType_X)
                    return;
                Undo.RecordObject(this, "ModEditor CustomTargetType_X");
                if ((value == CustomTargetType.Vertex || value == CustomTargetType.Normal) && CustomTargetPass_X == TargetPassType.W)
                    customTargetPass_X = TargetPassType.X;
                customTargetType_X = value;
            }
        }

        [SerializeField]
        TargetPassType customTargetPass_X = TargetPassType.X;
        public TargetPassType CustomTargetPass_X
        {
            get => customTargetPass_X;
            set
            {
                if (value == customTargetPass_X)
                    return;
                Undo.RecordObject(this, "ModEditor CustomTargetPass_X");
                customTargetPass_X = value;
            }
        }

        [SerializeField]
        CustomTargetType customTargetType_Y = CustomTargetType.None;
        public CustomTargetType CustomTargetType_Y
        {
            get => customTargetType_Y;
            set
            {
                if (value == customTargetType_Y)
                    return;
                Undo.RecordObject(this, "ModEditor CustomTargetType_Y");
                if ((value == CustomTargetType.Vertex || value == CustomTargetType.Normal) && CustomTargetPass_Y == TargetPassType.W)
                    customTargetPass_Y = TargetPassType.X;
                customTargetType_Y = value;
            }
        }

        [SerializeField]
        TargetPassType customTargetPass_Y = TargetPassType.Y;
        public TargetPassType CustomTargetPass_Y
        {
            get => customTargetPass_Y;
            set
            {
                if (value == customTargetPass_Y)
                    return;
                Undo.RecordObject(this, "ModEditor CustomTargetPass_Y");
                customTargetPass_Y = value;
            }
        }

        [SerializeField]
        CustomTargetType customTargetType_Z = CustomTargetType.None;
        public CustomTargetType CustomTargetType_Z
        {
            get => customTargetType_Z;
            set
            {
                if (value == customTargetType_Z)
                    return;
                Undo.RecordObject(this, "ModEditor CustomTargetType_Z");
                if ((value == CustomTargetType.Vertex || value == CustomTargetType.Normal) && CustomTargetPass_Z == TargetPassType.W)
                    customTargetPass_Z = TargetPassType.X;
                customTargetType_Z = value;
            }
        }

        [SerializeField]
        TargetPassType customTargetPass_Z = TargetPassType.Z;
        public TargetPassType CustomTargetPass_Z
        {
            get => customTargetPass_Z;
            set
            {
                if (value == customTargetPass_Z)
                    return;
                Undo.RecordObject(this, "ModEditor CustomTargetPass_Z");
                customTargetPass_Z = value;
            }
        }

        [SerializeField]
        CustomTargetType customTargetType_W = CustomTargetType.None;
        public CustomTargetType CustomTargetType_W
        {
            get => customTargetType_W;
            set
            {
                if (value == customTargetType_W)
                    return;
                Undo.RecordObject(this, "ModEditor CustomTargetType_W");
                if ((value == CustomTargetType.Vertex || value == CustomTargetType.Normal) && CustomTargetPass_W == TargetPassType.W)
                    customTargetPass_W = TargetPassType.X;
                customTargetType_W = value;
            }
        }

        [SerializeField]
        TargetPassType customTargetPass_W = TargetPassType.W;
        public TargetPassType CustomTargetPass_W
        {
            get => customTargetPass_W;
            set
            {
                if (value == customTargetPass_W)
                    return;
                Undo.RecordObject(this, "ModEditor CustomTargetPass_W");
                customTargetPass_W = value;
            }
        }
        #endregion

        #region VertexUtil
        [SerializeField]
        bool calcUtilUnfold = true;
        public bool CalcUtilUnfold
        {
            get => calcUtilUnfold;
            set
            {
                if (value == calcUtilUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor CalcUtilUnfold");
                calcUtilUnfold = value;
            }
        }

        public int calcUtilIndex;
        public int CalcUtilIndex
        {
            get => calcUtilIndex;
            set
            {
                if (value == calcUtilIndex)
                    return;
                Undo.RecordObject(this, "ModEditor CalcUtilIndex");
                calcUtilIndex = value;
            }
        }

        public int brushUtilIndex;
        public int BrushUtilIndex
        {
            get => brushUtilIndex;
            set
            {
                if (value == brushUtilIndex)
                    return;
                Undo.RecordObject(this, "ModEditor BrushUtilIndex");
                brushUtilIndex = value;
            }
        }
        #endregion

        #region TextureBrush
        [SerializeField]
        Vector2Int texBrushTexelSize = Vector2Int.one * 1024;
        public Vector2Int TexBrushTexelSize
        {
            get => texBrushTexelSize;
            set
            {
                if (value == texBrushTexelSize)
                    return;
                Undo.RecordObject(this, "ModEditor TexBrushTexelSize");
                texBrushTexelSize = value;
            }
        }

        [SerializeField]
        bool texBrushUnfold = true;
        public bool TexBrushUnfold
        {
            get => texBrushUnfold;
            set
            {
                if (value == texBrushUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor TexBrushUnfold");
                texBrushUnfold = value;
            }
        }

        [SerializeField]
        bool texBrushForegroundUnfold = true;
        public bool TexBrushForegroundUnfold
        {
            get => texBrushForegroundUnfold;
            set
            {
                if (value == texBrushForegroundUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor TexBrushForegroundUnfold");
                texBrushForegroundUnfold = value;
            }
        }

        [SerializeField]
        Color texBrushRangeViewColor = Color.green;
        public Color TexBrushRangeViewColor
        {
            get => texBrushRangeViewColor;
            set
            {
                if (value == texBrushRangeViewColor)
                    return;
                Undo.RecordObject(this, "ModEditor TexBrushRangeViewColor");
                texBrushRangeViewColor = value;
            }
        }

        [SerializeField]
        Color textureBaseColor = Color.clear;
        public Color TextureBaseColor
        {
            get => textureBaseColor;
            set
            {
                if (value == textureBaseColor)
                    return;
                Undo.RecordObject(this, "ModEditor TextureBaseColor");
                textureBaseColor = value;
            }
        }

        [SerializeField]
        Color textureBrushColor = Color.white;
        public Color TextureBrushColor
        {
            get => textureBrushColor;
            set
            {
                if (value == textureBrushColor)
                    return;
                Undo.RecordObject(this, "ModEditor TextureBrushColor");
                textureBrushColor = value;
            }
        }

        [SerializeField]
        Vector3 texBrushRange;
        public Vector3 TexBrushRange
        {
            get => texBrushRange;
            set
            {
                if (value.x <= 0)
                    value.x = 0.001f;
                if (value.y <= 0)
                    value.y = 0.001f;
                if (value.z < 0)
                    value.z = 0;
                if (value.x > 0.5f)
                    value.x = 0.5f;
                if (value.y > 0.5f)
                    value.y = 0.5f;
                if (value.z > 1)
                    value.z = 1;
                if (value == texBrushRange)
                    return;
                Undo.RecordObject(this, "ModEditor TexBrushRange");
                texBrushRange = value;
            }
        }
        #endregion

        #region TextureUtil
        [SerializeField]
        bool texUtilUnfold = true;
        public bool TexUtilUnfold
        {
            get => texUtilUnfold;
            set
            {
                if (value == texUtilUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor TexUtilUnfold");
                texUtilUnfold = value;
            }
        }

        public int texUtilIndex;
        public int TexUtilIndex
        {
            get => texUtilIndex;
            set
            {
                if (value == texUtilIndex)
                    return;
                Undo.RecordObject(this, "ModEditor TexUtilIndex");
                texUtilIndex = value;
            }
        }
        #endregion

        #region TextureColorMask
        [SerializeField]
        bool colorMaskUnfold = true;
        public bool ColorMaskUnfold
        {
            get => colorMaskUnfold;
            set
            {
                if (value == colorMaskUnfold)
                    return;
                Undo.RecordObject(this, "ModEditor ColorMaskUnfold");
                colorMaskUnfold = value;
            }
        }

        [SerializeField]
        Color colorMask = Color.white;
        public Color ColorMask
        {
            get => colorMask;
            set
            {
                if (value == colorMask)
                    return;
                Undo.RecordObject(this, "ModEditor ColorMask");
                colorMask = value;
            }
        }

        [SerializeField]
        bool texturePassView = false;
        public bool TexturePassView
        {
            get => texturePassView;
            set
            {
                if (value == texturePassView)
                    return;
                Undo.RecordObject(this, "ModEditor TexturePassView");
                texturePassView = value;
            }
        }

        [SerializeField]
        TexViewPass textureViewPass = TexViewPass.R;
        public TexViewPass TextureViewPass
        {
            get => textureViewPass;
            set
            {
                if (value == textureViewPass)
                    return;
                Undo.RecordObject(this, "ModEditor TextureViewPass");
                textureViewPass = value;
            }
        }
        #endregion

        private void Awake()
        {
            if (actionableDic == null)
                actionableDic = new SerializableClass.Dictionary_Obj_Bool(this);
            if (meshDic == null)
                meshDic = new SerializableClass.Dictionary_Obj_Mesh();
        }

        public void CheckAndClearExposed()
        {
            if (ModEditorWindow.ExposedManagement.CheckAndClearExposed(target.exposedName))
                target = default;
            ActionableDic.CheckAndClearExposed();
            MeshDic.CheckAndClearExposed();
            EditorUtility.SetDirty(this);
        }
    }
}