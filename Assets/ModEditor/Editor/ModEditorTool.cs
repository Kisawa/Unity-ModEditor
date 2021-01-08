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
        GUIContent toggleContent;
        GUIContent toggleOnContent;
        GUIContent lockContent;
        GUIContent unlockContent;

        public override GUIContent toolbarIcon => icon;

        bool brushLock;
        bool BrushLock
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushLock = ModEditor.BrushLock;
                return brushLock;
            }
        }

        Color unselectedVertexColor = Color.white;
        Color UnselectedVertexColor
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    unselectedVertexColor = ModEditor.Manager.UnselectedVertexColor;
                return unselectedVertexColor;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                unselectedVertexColor = value;
                ModEditor.Manager.UnselectedVertexColor = unselectedVertexColor;
            }
        }

        Color selectedVertexColor = Color.black;
        Color SelectedVertexColor
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    selectedVertexColor = ModEditor.Manager.SelectedVertexColor;
                return selectedVertexColor;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                selectedVertexColor = value;
                ModEditor.Manager.SelectedVertexColor = selectedVertexColor;
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
            if (icon == null)
                icon = EditorGUIUtility.IconContent("d_Mesh Icon");
            if (toggleContent == null)
                toggleContent = EditorGUIUtility.IconContent("ShurikenToggleHover");
            if (toggleOnContent == null)
                toggleOnContent = EditorGUIUtility.IconContent("ShurikenToggleFocusedOn");
            if (lockContent == null)
                lockContent = EditorGUIUtility.IconContent("IN LockButton on act");
            if (unlockContent == null)
                unlockContent = EditorGUIUtility.IconContent("IN LockButton");
        }

        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
            Handles.BeginGUI();
            Rect rect = new Rect(window.position.width - 250, window.position.height - 159, 240, 129);
            GUILayout.BeginArea(rect);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(BrushLock ? lockContent : unlockContent, "ToolbarButtonFlat");
            GUILayout.Label($"Current brush dpeth:  {BrushDepth}", "ToolbarButtonFlat");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("dragtabdropwindow", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
            GUILayout.Label("Vertex View", "LODRenderersText");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Unselected Vertex Color", "AboutWIndowLicenseLabel", GUILayout.Width(120));
            UnselectedVertexColor = EditorGUILayout.ColorField(new GUIContent(), UnselectedVertexColor, true, false, false, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Selected Vertex Color", "AboutWIndowLicenseLabel", GUILayout.Width(120));
            SelectedVertexColor = EditorGUILayout.ColorField(new GUIContent(), SelectedVertexColor, true, false, false, GUILayout.Width(70));
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
                ModEditor.OnSceneGUI = rect.Contains(Event.current.mousePosition);
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
            bool brushOn = ModEditor.TabType == ModEditorTabType.VertexBrush;
            for (int i = 0; i < ModEditor.CalcShaderDatas.Count; i++)
            {
                CalcShaderData.CalcVertexsData data = ModEditor.CalcShaderDatas[i];
                if (brushOn)
                    data.Update(ModEditor.camera, Mouse.ScreenTexcoord, BrushSize, BrushDepth);
                if (data.IsAvailable)
                {
                    data.material.SetInt("_BrushOn", brushOn ? 1 : 0);
                    data.material.SetInt("_HideNoSelectVertex", HideUnselectedVertex ? 1 : 0);
                    data.material.SetColor("_UnselectedVertexColor", UnselectedVertexColor);
                    data.material.SetColor("_SelectedVertexColor", SelectedVertexColor);
                    data.material.SetFloat("_VertexScale", VertexScale);
                    data.material.SetInt("_VertexWithZTest", VertexWithZTest ? (int)CompareFunction.LessEqual : (int)CompareFunction.Always);
                }
            }
            SceneView.RepaintAll();
        }
    }
}