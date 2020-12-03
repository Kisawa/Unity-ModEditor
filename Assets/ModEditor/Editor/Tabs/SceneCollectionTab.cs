using System.Collections;
using System.Collections.Generic;
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

        public override void OnEnable()
        {
            base.OnEnable();
            window.onRefreshTargetDic += refreshBuffer;
        }

        public override void OnDiable()
        {
            base.OnDiable();
            window.onRefreshTargetDic -= refreshBuffer;
            if (camera != null && buffer != null)
                camera.RemoveCommandBuffer(cameraEvent, buffer);
        }

        public override void Draw()
        {
            drawCollection();
            EditorGUILayout.Space(20);
            drawNormalViewUtil();
            EditorGUILayout.Space(10);
            drawTangentViewUtil();
            EditorGUILayout.Space(10);
            drawGridViewUtil();
            EditorGUILayout.Space(10);
            drawUVViewUtil();
        }

        Camera camera;
        CommandBuffer buffer;
        CameraEvent cameraEvent = CameraEvent.AfterForwardAlpha;

        Material _mat_normal;
        Material mat_normal {
            get { 
                if(_mat_normal == null)
                    _mat_normal = new Material(Shader.Find("ModEditor/ShowNormal"));
                return _mat_normal;
            }
        }
        Material _mat_tangent;
        Material mat_tangent {
            get { 
                if(_mat_tangent == null)
                    _mat_tangent = new Material(Shader.Find("ModEditor/ShowTangent"));
                return _mat_tangent;
            }
        }
        Material _mat_grid;
        Material mat_grid {
            get { 
                if(_mat_grid == null)
                    _mat_grid = new Material(Shader.Find("ModEditor/ShowGrid"));
                return _mat_grid;
            }
        }
        Material _mat_uv;
        Material mat_uv {
            get { 
                if(_mat_uv == null)
                    _mat_uv = new Material(Shader.Find("ModEditor/ShowUV"));
                return _mat_uv;
            }
        }

        public override void OnInspectorUpdate()
        {
            base.OnInspectorUpdate();
            Camera cam = SceneView.lastActiveSceneView.camera;
            if (cam != camera)
            {
                if (camera != null && buffer != null)
                    camera.RemoveCommandBuffer(cameraEvent, buffer);
                camera = cam;
                if (camera != null)
                {
                    if (buffer == null)
                    {
                        buffer = new CommandBuffer();
                        buffer.name = "ModEditor";
                    }
                    camera.AddCommandBuffer(cameraEvent, buffer);
                    refreshBuffer();
                }
            }
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
            EditorGUILayout.EndHorizontal();

            if (window.Manager.Target != null)
            {
                for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
                {
                    GameObject obj = window.Manager.TargetChildren[i];
                    if (obj == window.Manager.Target || obj.gameObject == null)
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
                GUILayout.Button(window.Manager.NormalViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 195)))
                window.Manager.NormalViewUnfold = !window.Manager.NormalViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.NormalViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.NormalView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Normal Color", labelStyle, GUILayout.Width(120));
                window.Manager.NormalColor = EditorGUILayout.ColorField(window.Manager.NormalColor, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Normal Length", labelStyle, GUILayout.Width(120));
                window.Manager.NormalLength = EditorGUILayout.Slider(window.Manager.NormalLength, 0.01f, 1, GUILayout.Width(170));
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
                GUILayout.Button(window.Manager.TangentViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 195)))
                window.Manager.TangentViewUnfold = !window.Manager.TangentViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.TangentViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.TangentView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tangent Color", labelStyle, GUILayout.Width(120));
                window.Manager.TangentColor = EditorGUILayout.ColorField(window.Manager.TangentColor, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tangent Length", labelStyle, GUILayout.Width(120));
                window.Manager.TangentLength = EditorGUILayout.Slider(window.Manager.TangentLength, 0.01f, 1, GUILayout.Width(170));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Binormal", GUI.skin.GetStyle("AnimationTimelineTick"));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Arrow Length", labelStyle, GUILayout.Width(120));
                window.Manager.ArrowLength = EditorGUILayout.Slider(window.Manager.ArrowLength, 0, 1, GUILayout.Width(170));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Arrow Size", labelStyle, GUILayout.Width(120));
                window.Manager.ArrowSize = EditorGUILayout.Slider(window.Manager.ArrowSize, 0, 1, GUILayout.Width(170));
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
                GUILayout.Button(window.Manager.GridViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 195)))
                window.Manager.GridViewUnfold = !window.Manager.GridViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.GridViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.GridView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Grid Color", labelStyle, GUILayout.Width(120));
                window.Manager.GridColor = EditorGUILayout.ColorField(window.Manager.GridColor, GUILayout.Width(80));
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
                GUILayout.Button(window.Manager.UVViewUnfold ? window.dropdownContent : window.dropdownRightContent, "AboutWIndowLicenseLabel", GUILayout.Width(window.position.width - 195)))
                window.Manager.UVViewUnfold = !window.Manager.UVViewUnfold;
            EditorGUILayout.EndHorizontal();
            if (window.Manager.UVViewUnfold)
            {
                EditorGUI.indentLevel = 2;
                GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
                EditorGUI.BeginDisabledGroup(!window.Manager.UVView);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Alpha", labelStyle, GUILayout.Width(120));
                window.Manager.UVAlpha = EditorGUILayout.Slider(window.Manager.UVAlpha, 0, 1, GUILayout.Width(170));
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        void refreshBuffer()
        {
            if(buffer != null)
                buffer.Clear();
            if (camera == null || window.Manager.Target == null || window.Manager.TargetChildren.Count == 0)
                return;
            updateMaterial();
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
                if (window.Manager.NormalView)
                    buffer.DrawRenderer(renderer, mat_normal);
                if (window.Manager.TangentView)
                    buffer.DrawRenderer(renderer, mat_tangent);
                if (window.Manager.GridView)
                    buffer.DrawRenderer(renderer, mat_grid);
                if (window.Manager.UVView)
                    buffer.DrawRenderer(renderer, mat_uv);
            }
        }

        void updateMaterial()
        {
            mat_normal.SetColor("_NormalColor", window.Manager.NormalColor);
            mat_normal.SetFloat("_NormalLength", window.Manager.NormalLength);

            mat_tangent.SetColor("_TangentColor", window.Manager.TangentColor);
            mat_tangent.SetFloat("_TangentLength", window.Manager.TangentLength);
            mat_tangent.SetFloat("_ArrowLength", window.Manager.ArrowLength);
            mat_tangent.SetFloat("_ArrowSize", window.Manager.ArrowSize);

            mat_grid.SetColor("_GridColor", window.Manager.GridColor);

            mat_uv.SetFloat("_Alpha", window.Manager.UVAlpha);
        }
    }
}