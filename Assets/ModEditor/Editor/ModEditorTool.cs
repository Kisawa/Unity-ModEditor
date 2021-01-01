using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModEditor
{
    [EditorTool("ModEditor Tool")]
    public class ModEditorTool : EditorTool
    {
        static ModEditorWindow ModEditor => ModEditorWindow.Self;

        GUIContent icon;
        public GUIContent toggleContent;
        public GUIContent toggleOnContent;

        public override GUIContent toolbarIcon => icon;

        Color vertexColor = Color.black;
        Color VertexColor
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    vertexColor = ModEditor.Manager.VertexColor;
                return vertexColor;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                vertexColor = value;
                ModEditor.Manager.VertexColor = vertexColor;
            }
        }

        float vertexScale = 0.5f;
        float VertexScale
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    vertexScale = ModEditor.Manager.VertexScale;
                return vertexScale;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                vertexScale = value;
                ModEditor.Manager.VertexScale = vertexScale;
            }
        }

        bool vertexWithZTest = true;
        bool VertexWithZTest
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    vertexWithZTest = ModEditor.Manager.VertexWithZTest;
                return vertexWithZTest;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                vertexWithZTest = value;
                ModEditor.Manager.VertexWithZTest = vertexWithZTest;
            }
        }

        bool hideNoSelectVertex = false;
        bool HideUnselectedVertex
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    hideNoSelectVertex = ModEditor.Manager.HideUnselectedVertex;
                return hideNoSelectVertex;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                hideNoSelectVertex = value;
                ModEditor.Manager.HideUnselectedVertex = hideNoSelectVertex;
            }
        }

        Vector3 screenTexcoord;
        Vector3 ScreenTexcoord
        {
            get
            {
                if (ModEditor != null)
                    screenTexcoord = Mouse.ScreenTexcoord;
                return screenTexcoord;
            }
        }

        float brushSize = 0.05f;
        float BrushSize
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushSize = ModEditor.Manager.BrushSize;
                return brushSize;
            }
        }

        float brushDepth = 10;
        float BrushDepth
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushDepth = ModEditor.Manager.BrushDepth;
                return brushDepth;
            }
        }

        private void OnEnable()
        {
            icon = EditorGUIUtility.IconContent("d_Mesh Icon");

            if (toggleContent == null)
                toggleContent = EditorGUIUtility.IconContent("ShurikenToggleHover");
            if (toggleOnContent == null)
                toggleOnContent = EditorGUIUtility.IconContent("ShurikenToggleFocusedOn");
        }

        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
            Handles.BeginGUI();
            Rect rect = new Rect(window.position.width - 250, window.position.height - 142, 240, 112);
            GUILayout.BeginArea(rect);
            GUILayout.Label($"Current brush dpeth:  {BrushDepth}", "ToolbarButtonFlat");
            EditorGUILayout.BeginVertical("dragtabdropwindow", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
            GUILayout.Label("Vertex View", "LODRenderersText");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertex's Color", "AboutWIndowLicenseLabel", GUILayout.Width(120));
            VertexColor = EditorGUILayout.ColorField(new GUIContent(), VertexColor, true, false, false, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertex Scale", "AboutWIndowLicenseLabel", GUILayout.Width(120));
            VertexScale = EditorGUILayout.Slider(VertexScale, 0.001f, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertex With ZTest", "AboutWIndowLicenseLabel", GUILayout.Width(120));
            if (GUILayout.Button(VertexWithZTest ? toggleOnContent : toggleContent, "AboutWIndowLicenseLabel"))
                VertexWithZTest = !VertexWithZTest;
            GUILayout.Label("       /Tab", "AboutWIndowLicenseLabel", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Hide Unselected Vertex", "AboutWIndowLicenseLabel", GUILayout.Width(120));
            if (GUILayout.Button(HideUnselectedVertex ? toggleOnContent : toggleContent, "AboutWIndowLicenseLabel"))
                HideUnselectedVertex = !HideUnselectedVertex;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
            if (ModEditor != null)
            {
                if (GUI.changed)
                    ModEditor.Validate();
                if (rect.Contains(Event.current.mousePosition))
                    ModEditor.SceneHandleType = SceneHandleType.SceneGUI;
                else
                    ModEditor.SceneHandleType = SceneHandleType.None;
            }
            Handles.EndGUI();
            updateVertexViewBuffer();
        }

        public override bool IsAvailable()
        {
            if (ModEditor != null && ModEditor.Manager.Target != null && (ModEditor.Manager.LockTarget || ModEditor.Manager.Target == Selection.activeGameObject))
                return true;
            return false;
        }

        void updateVertexViewBuffer()
        {
            if (ModEditor == null || ModEditor.camera == null)
                return;
            bool brushOn = ModEditor.TabType == ModEditorTabType.NormalEditor;
            for (int i = 0; i < ModEditor.CalcShaderDatas.Count; i++)
            {
                CalcShaderData.CalcVertexsData data = ModEditor.CalcShaderDatas[i];
                if (brushOn)
                    data.Update(ModEditor.camera, ScreenTexcoord, BrushSize, BrushDepth);
                if (data.IsAvailable)
                {
                    data.material.SetInt("_BrushOn", brushOn ? 1 : 0);
                    data.material.SetInt("_HideNoSelectVertex", HideUnselectedVertex ? 1 : 0);
                    data.material.SetColor("_VertexColor", VertexColor);
                    data.material.SetFloat("_VertexScale", VertexScale);
                    data.material.SetInt("_VertexWithZTest", VertexWithZTest ? (int)CompareFunction.LessEqual : (int)CompareFunction.Always);
                }
            }
        }
    }
}