using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModEditor
{
    public class SceneCollectionTab : WindowTabBase
    {
        new ModEditorWindow window;

        public SceneCollectionTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
        }

        Vector2 scroll;
        Mesh _screenMesh;
        Mesh screenMesh
        {
            get
            {
                if (_screenMesh == null)
                {
                    _screenMesh = new Mesh();
                    _screenMesh.vertices = new Vector3[]
                    {
                        new Vector3(-1, -1, 0),
                        new Vector3(-1, 1, 0),
                        new Vector3(1, 1, 0),
                        new Vector3(1, -1, 0)
                    };
                    _screenMesh.uv = new Vector2[]
                    {
                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(1, 1)
                    };
                    _screenMesh.SetIndices(new int[] { 0, 3, 2, 1 }, MeshTopology.Quads, 0);
                }
                return _screenMesh;
            }
        }
        CommandBuffer buffer;
        CameraEvent cameraEvent = CameraEvent.AfterForwardAlpha;

        public override void OnEnable()
        {
            base.OnEnable();
            window.onCameraChange += onCameraChange;
            window.onTabChanged += refreshBuffer;
            window.onRefreshTargetDic += refreshBuffer;
            window.onSceneToolChange += refreshBuffer;
            window.Tab_TextureBrush.onCurrentDrawBoardChanged += refreshBuffer;
            onCameraChange(null);
        }

        public override void OnDiable()
        {
            base.OnDiable();
            window.onCameraChange -= onCameraChange;
            window.onTabChanged -= refreshBuffer;
            window.onRefreshTargetDic -= refreshBuffer;
            window.onSceneToolChange -= refreshBuffer;
            window.Tab_TextureBrush.onCurrentDrawBoardChanged -= refreshBuffer;
            if (window.camera != null && buffer != null)
                window.camera.RemoveCommandBuffer(cameraEvent, buffer);
        }

        public override void Draw()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            drawCollection();
            EditorGUILayout.Space(20);
            drawNormalViewUtil();
            EditorGUILayout.Space(10);
            drawTangentViewUtil();
            EditorGUILayout.Space(10);
            drawGridViewUtil();
            EditorGUILayout.Space(10);
            drawUVViewUtil();
            EditorGUILayout.Space(10);
            drawVertexColorViewUtil();
            EditorGUILayout.Space(10);
            drawDepthViewUtil();
            EditorGUILayout.Space(10);
            drawNormalMapViewUtil();
            EditorGUILayout.Space(10);
            drawMapViewUtil();
            EditorGUILayout.Space(10);
            EditorGUILayout.EndScrollView();
        }

        public override void OnValidate()
        {
            base.OnValidate();
            updateMaterial();
        }

        void drawCollection()
        {
            EditorGUILayout.BeginVertical("flow background");
            EditorGUILayout.BeginHorizontal();
            if (window.Manager.Target != null && window.Manager.TargetChildren.Contains(window.Manager.Target))
            {
                bool activeable = window.Manager.ActionableDic[window.Manager.Target];
                if (GUILayout.Button(activeable ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                {
                    window.Manager.ActionableDic[window.Manager.Target] = !activeable;
                    refreshBuffer();
                }
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(window.Manager.Target, typeof(GameObject), true);
            EditorGUI.EndDisabledGroup();
            bool hasChildrenRenderer = false;
            if (window.Manager.Target != null)
            {
                if (window.Manager.TargetChildren.Contains(window.Manager.Target))
                {
                    if (window.Manager.TargetChildren.Count > 1)
                        hasChildrenRenderer = true;
                }
                else if (window.Manager.TargetChildren.Count > 0)
                    hasChildrenRenderer = true;
            }
            if (hasChildrenRenderer)
            {
                if (GUILayout.Button(window.viewContent, "ObjectPickerTab"))
                {
                    for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
                        window.Manager.ActionableDic[window.Manager.TargetChildren[i]] = true;
                    refreshBuffer();
                }
                if (GUILayout.Button(window.hiddenContent, "ObjectPickerTab"))
                {
                    for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
                        window.Manager.ActionableDic[window.Manager.TargetChildren[i]] = false;
                    refreshBuffer();
                }
                if (GUILayout.Button(window.Manager.SceneCollectionView ? window.dropdownContent : window.dropdownRightContent, "ObjectPickerTab"))
                    window.Manager.SceneCollectionView = !window.Manager.SceneCollectionView;
            }
            EditorGUILayout.EndHorizontal();

            if (hasChildrenRenderer)
            {
                if (window.Manager.SceneCollectionView)
                {
                    for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
                    {
                        GameObject obj = window.Manager.TargetChildren[i];
                        if (obj == window.Manager.Target || obj == null)
                            continue;
                        EditorGUI.indentLevel = 1;
                        Transform parent = obj.transform.parent;
                        while (parent != window.Manager.Target.transform && parent != null)
                        {
                            EditorGUI.indentLevel++;
                            parent = parent.parent;
                        }
                        EditorGUILayout.BeginHorizontal();
                        bool actionable = window.Manager.ActionableDic[obj];
                        if (GUILayout.Button(actionable ? window.viewContent : window.hiddenContent, "ObjectPickerTab"))
                        {
                            window.Manager.ActionableDic[obj] = !actionable;
                            refreshBuffer();
                        }
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel = 0;
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(16, false);
                    GUILayout.Label("······", "LODRenderersText");
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        void drawNormalViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(window.Manager.NormalView ? window.olToggleOnContent : window.olToggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
            {
                window.Manager.NormalView = !window.Manager.NormalView;
                refreshBuffer();
            }
            if (GUILayout.Button("Normal View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.NormalViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.NormalViewUnfold = !window.Manager.NormalViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.NormalViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.NormalView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Normal Color", labelStyle, GUILayout.Width(120));
                window.Manager.NormalColor = EditorGUILayout.ColorField(window.Manager.NormalColor, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Normal Length", labelStyle, GUILayout.Width(120));
                window.Manager.NormalLength = EditorGUILayout.Slider(window.Manager.NormalLength, 0.01f, 1, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawTangentViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(window.Manager.TangentView ? window.olToggleOnContent : window.olToggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
            {
                window.Manager.TangentView = !window.Manager.TangentView;
                refreshBuffer();
            }
            if (GUILayout.Button("Tangent View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.TangentViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.TangentViewUnfold = !window.Manager.TangentViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.TangentViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.TangentView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tangent Color", labelStyle, GUILayout.Width(120));
                window.Manager.TangentColor = EditorGUILayout.ColorField(window.Manager.TangentColor, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tangent Length", labelStyle, GUILayout.Width(120));
                window.Manager.TangentLength = EditorGUILayout.Slider(window.Manager.TangentLength, 0.01f, 1, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Binormal", GUI.skin.GetStyle("AnimationTimelineTick"));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Arrow Length", labelStyle, GUILayout.Width(120));
                window.Manager.ArrowLength = EditorGUILayout.Slider(window.Manager.ArrowLength, 0, 1, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Arrow Size", labelStyle, GUILayout.Width(120));
                window.Manager.ArrowSize = EditorGUILayout.Slider(window.Manager.ArrowSize, 0, 1, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawGridViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(window.Manager.GridView ? window.olToggleOnContent : window.olToggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
            {
                window.Manager.GridView = !window.Manager.GridView;
                refreshBuffer();
            }
            if (GUILayout.Button("Grid View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.GridViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.GridViewUnfold = !window.Manager.GridViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.GridViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.GridView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Grid Color", labelStyle, GUILayout.Width(120));
                window.Manager.GridColor = EditorGUILayout.ColorField(window.Manager.GridColor, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Grid With ZTest", labelStyle, GUILayout.Width(150));
                if (GUILayout.Button(window.Manager.GridWithZTest ? window.toggleOnContent : window.toggleContent, "AboutWIndowLicenseLabel"))
                    window.Manager.GridWithZTest = !window.Manager.GridWithZTest;
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawUVViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(window.Manager.UVView ? window.olToggleOnContent : window.olToggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
            {
                window.Manager.UVView = !window.Manager.UVView;
                refreshBuffer();
            }
            if (GUILayout.Button("UV View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.UVViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.UVViewUnfold = !window.Manager.UVViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.UVViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.UVView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("UV Select", labelStyle, GUILayout.Width(120));
                window.Manager.UVViewType = (UVType)EditorGUILayout.EnumPopup(window.Manager.UVViewType, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alpha", labelStyle, GUILayout.Width(120));
                window.Manager.UVAlpha = EditorGUILayout.Slider(window.Manager.UVAlpha, 0, 1, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawVertexColorViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(window.Manager.VertexColorView ? window.olToggleOnContent : window.olToggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
            {
                window.Manager.VertexColorView = !window.Manager.VertexColorView;
                refreshBuffer();
            }
            GUILayout.Label("Vertex Color View", "AboutWIndowLicenseLabel");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawDepthViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(window.Manager.DepthMapView ? window.olToggleOnContent : window.olToggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
            {
                window.Manager.DepthMapView = !window.Manager.DepthMapView;
                refreshBuffer();
            }
            if (GUILayout.Button("Depth Map View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.DepthMapViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.DepthMapViewUnfold = !window.Manager.DepthMapViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.DepthMapViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.DepthMapView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Depth Compress", labelStyle, GUILayout.Width(120));
                window.Manager.DepthCompress = EditorGUILayout.Slider(window.Manager.DepthCompress, 0, 1, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawNormalMapViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(window.Manager.NormalMapView ? window.olToggleOnContent : window.olToggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
            {
                window.Manager.NormalMapView = !window.Manager.NormalMapView;
                refreshBuffer();
            }
            GUILayout.Label("Normal Map View", "AboutWIndowLicenseLabel");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void drawMapViewUtil()
        {
            EditorGUILayout.BeginVertical("AnimationEventTooltip");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(window.Manager.MapView ? window.olToggleOnContent : window.olToggleContent, "AboutWIndowLicenseLabel", GUILayout.Width(20)))
            {
                window.Manager.MapView = !window.Manager.MapView;
                refreshBuffer();
            }
            if (GUILayout.Button("Map View", "AboutWIndowLicenseLabel", GUILayout.Width(150)) ||
                GUILayout.Button(window.Manager.MapViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 205)))
                window.Manager.MapViewUnfold = !window.Manager.MapViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.MapViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.MapView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Target Map", labelStyle, GUILayout.Width(120));
                window.Manager.MapViewTex = EditorGUILayout.ObjectField(window.Manager.MapViewTex, typeof(Texture), false, GUILayout.Width(165)) as Texture;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("View Pass", labelStyle, GUILayout.Width(120));
                window.Manager.MapViewPass = (TexViewPass)EditorGUILayout.EnumPopup(window.Manager.MapViewPass, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color Tint", labelStyle, GUILayout.Width(120));
                window.Manager.MapViewColorTint = EditorGUILayout.ColorField(window.Manager.MapViewColorTint, GUILayout.Width(165));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        private void onCameraChange(Camera obj)
        {
            if (obj != null && buffer != null)
                obj.RemoveCommandBuffer(cameraEvent, buffer);
            if (window.camera != null)
            {
                if (buffer == null)
                {
                    buffer = new CommandBuffer();
                    buffer.name = "ModEditor SceneView";
                }
                window.camera.AddCommandBuffer(cameraEvent, buffer);
                refreshBuffer();
            }
        }

        public void refreshBuffer()
        {
            if (buffer != null)
                buffer.Clear();
            window.Tab_VertexBrush.ClearCalcShaderData();
            if (window.camera == null)
                return;
            updateMaterial();
            if (window.ToolType == ModEditorToolType.VertexBrush)
                buffer.DrawMesh(screenMesh, Matrix4x4.identity, window.Mat_Util, 0, 8);
            if (window.Manager.Target == null || window.Manager.TargetChildren.Count == 0)
                return;
            if (window.ToolType == ModEditorToolType.VertexBrush)
            {
                for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
                {
                    GameObject target = window.Manager.TargetChildren[i];
                    if (target == null)
                        continue;
                    if (!window.Manager.ActionableDic[target])
                        continue;
                    Renderer renderer = target.GetComponent<Renderer>();
                    if (renderer == null)
                        continue;
                    int subCount = 1;
                    MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                        subCount = meshFilter.sharedMesh.subMeshCount;
                    SkinnedMeshRenderer skinnedMesh = renderer as SkinnedMeshRenderer;
                    if (skinnedMesh != null && skinnedMesh.sharedMesh != null)
                        subCount = skinnedMesh.sharedMesh.subMeshCount;
                    CalcManager data = window.Tab_VertexBrush.AddCalcShaderRender(renderer, meshFilter);
                    if (data == null)
                        data = window.Tab_VertexBrush.AddCalcShaderRender(skinnedMesh);
                    if (data != null)
                    {
                        for (int j = 0; j < subCount; j++)
                            buffer.DrawRenderer(data.renderer, data.Material, j);
                    }
                }
            }
            Command(buffer);
        }

        public void Command(CommandBuffer cmd)
        {
            if (buffer == null)
                return;
            if (window.Manager.Target == null || window.Manager.TargetChildren.Count == 0)
            {
                if (window.ToolType == ModEditorToolType.VertexBrush)
                    cmd.DrawMesh(screenMesh, Matrix4x4.identity, window.Mat_Util, 0, 8);
                return;
            }
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                Renderer renderer = target.GetComponent<Renderer>();
                if (renderer == null)
                    continue;
                int subCount = 1;
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                    subCount = meshFilter.sharedMesh.subMeshCount;
                SkinnedMeshRenderer skinnedMesh = renderer as SkinnedMeshRenderer;
                if (skinnedMesh != null && skinnedMesh.sharedMesh != null)
                    subCount = skinnedMesh.sharedMesh.subMeshCount;
                for (int j = 0; j < subCount; j++)
                {
                    if (window.Manager.GridView || window.Manager.UVView || window.Manager.VertexColorView || window.ToolType != ModEditorToolType.None)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 0);
                    if (window.Manager.NormalView)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 1);
                    if (window.Manager.TangentView)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 2);
                    if (window.Manager.GridView)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 3);
                    if (window.Manager.UVView)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 4);
                    if (window.Manager.VertexColorView)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 5);
                    if (window.Manager.DepthMapView)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 6);
                    if (window.Manager.NormalMapView)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 7);
                    if (window.Manager.MapView)
                        cmd.DrawRenderer(renderer, window.Mat_Util, j, 11);
                }
                if (window.ToolType == ModEditorToolType.VertexBrush)
                {
                    CalcManager data = window.Tab_VertexBrush.CalcShaderDatas.FirstOrDefault(x => x!= null && x.trans == target.transform);
                    if (data != null)
                    {
                        for (int j = 0; j < subCount; j++)
                            cmd.DrawRenderer(data.renderer, data.Material, j);
                    }
                }
                else if (window.ToolType == ModEditorToolType.TextureBrush)
                {
                    if (window.Tab_TextureBrush.TextureManager.IsAvailable && window.Tab_TextureBrush.TextureManager.trans == target.transform)
                    {
                        cmd.DrawRenderer(renderer, window.Mat_Util, window.TextureBrushTabCurrentDrawBoardSubNum, 9);
                        cmd.DrawRenderer(renderer, window.Mat_Util, window.TextureBrushTabCurrentDrawBoardSubNum, 10);
                    }
                }
            }
            if (window.ToolType == ModEditorToolType.VertexBrush)
                cmd.DrawMesh(screenMesh, Matrix4x4.identity, window.Mat_Util, 0, 8);
        }

        public void GameViewCommand(CommandBuffer cmd)
        {
            if (buffer == null)
                return;
            if (window.ToolType == ModEditorToolType.VertexBrush)
                cmd.DrawMesh(screenMesh, Matrix4x4.identity, window.Mat_Util, 0, 8);
        }

        void updateMaterial()
        {
            window.Mat_Util.SetColor("_NormalColor", window.Manager.NormalColor);
            window.Mat_Util.SetFloat("_NormalLength", window.Manager.NormalLength);

            window.Mat_Util.SetColor("_TangentColor", window.Manager.TangentColor);
            window.Mat_Util.SetFloat("_TangentLength", window.Manager.TangentLength);
            window.Mat_Util.SetFloat("_ArrowLength", window.Manager.ArrowLength);
            window.Mat_Util.SetFloat("_ArrowSize", window.Manager.ArrowSize);

            window.Mat_Util.SetColor("_GridColor", window.Manager.GridColor);
            window.Mat_Util.SetInt("_GridWithZTest", window.Manager.GridWithZTest ? (int)CompareFunction.LessEqual : (int)CompareFunction.Always);

            window.Mat_Util.SetFloat("_UVAlpha", window.Manager.UVAlpha);
            window.Mat_Util.SetFloat("_UVType", (int)window.Manager.UVViewType);

            window.Mat_Util.SetFloat("_DepthCompress", window.Manager.DepthCompress);

            window.Mat_Util.SetTexture("_MapViewTex", window.Manager.MapViewTex);
            window.Mat_Util.SetInt("_MapViewPass", (int)window.Manager.MapViewPass);
            window.Mat_Util.SetColor("_MapViewColorTint", window.Manager.MapViewColorTint);
        }
    }
}