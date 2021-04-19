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

        bool brushColorView = false;
        bool BrushColorView
        {
            get
            {
                if (ModEditor != null)
                    brushColorView = ModEditor.BrushColorView;
                return brushColorView;
            }
            set
            {
                if (ModEditor == null)
                    return;
                brushColorView = value;
                ModEditor.BrushColorView = brushColorView;
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

        Color brushColorFrom = Color.red;
        Color BrushColorFrom
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColorFrom = ModEditor.Manager.BrushColorFrom;
                return brushColorFrom;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColorFrom = value;
                ModEditor.Manager.BrushColorFrom = brushColorFrom;
            }
        }

        float brushColorFromStep = 0;
        float BrushColorFromStep
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColorFromStep = ModEditor.Manager.BrushColorFromStep;
                return brushColorFromStep;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColorFromStep = value;
                ModEditor.Manager.BrushColorFromStep = brushColorFromStep;
            }
        }

        Color brushColorTo = Color.red;
        Color BrushColorTo
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColorTo = ModEditor.Manager.BrushColorTo;
                return brushColorTo;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColorTo = value;
                ModEditor.Manager.BrushColorTo = brushColorTo;
            }
        }

        float brushColorToStep = 0;
        float BrushColorToStep
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColorToStep = ModEditor.Manager.BrushColorToStep;
                return brushColorToStep;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColorToStep = value;
                ModEditor.Manager.BrushColorToStep = brushColorToStep;
            }
        }

        Gradient brushGradient;
        public Gradient BrushGradient
        {
            get
            {
                brushGradient = new Gradient();
                if (ModEditor != null && ModEditor.Manager != null)
                {
                    GradientColorKey fromColor = new GradientColorKey() { color = ModEditor.Manager.BrushColorFrom, time = ModEditor.Manager.BrushColorFromStep };
                    GradientAlphaKey fromAlpha = new GradientAlphaKey() { alpha = ModEditor.Manager.BrushColorFrom.a, time = ModEditor.Manager.BrushColorFromStep };
                    GradientColorKey toColor = new GradientColorKey() { color = ModEditor.Manager.BrushColorTo, time = ModEditor.Manager.BrushColorToStep };
                    GradientAlphaKey toAlpha = new GradientAlphaKey() { alpha = ModEditor.Manager.BrushColorTo.a, time = ModEditor.Manager.BrushColorToStep };
                    brushGradient.SetKeys(new GradientColorKey[] { fromColor, toColor }, new GradientAlphaKey[] { fromAlpha, toAlpha });
                }
                return brushGradient;
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
            Rect rect = new Rect(window.position.width - 250, window.position.height - 215, 240, 185);
            GUIStyle txtStyle = GUI.skin.GetStyle("AboutWIndowLicenseLabel");
            GUIStyle hotKeyStyle = GUI.skin.GetStyle("LODSliderTextSelected");
            GUILayout.BeginArea(rect);

            EditorGUI.LabelField(new Rect(0, 0, 240, 40), "", GUI.skin.GetStyle("dockarea"));
            EditorGUI.BeginDisabledGroup(brushGradientDisabled());
            EditorGUI.LabelField(new Rect(0, 0, 20, 15), "/A", hotKeyStyle);
            BrushColorFrom = EditorGUI.ColorField(new Rect(20, 2, 70, 12), BrushColorFrom);
            EditorGUI.LabelField(new Rect(90, 0, 30, 15), "From", txtStyle);
            EditorGUI.LabelField(new Rect(223, 0, 20, 15), "/D", hotKeyStyle);

            EditorGUI.LabelField(new Rect(3, 12, 30, 15), $"{BrushColorFromStep}", txtStyle);
            EditorGUI.GradientField(new Rect(30, 15, 180, 12), BrushGradient);
            EditorGUI.LabelField(new Rect(210, 12, 30, 15), $"{1 - BrushColorToStep}", txtStyle);

            EditorGUI.LabelField(new Rect(0, 24, 20, 15), "/Z", hotKeyStyle);
            EditorGUI.LabelField(new Rect(130, 24, 30, 15), "To", txtStyle);
            BrushColorTo = EditorGUI.ColorField(new Rect(150, 27, 70, 12), BrushColorTo);
            EditorGUI.LabelField(new Rect(223, 24, 20, 15), "/C", hotKeyStyle);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(40);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(BrushLock ? lockContent : unlockContent, "ToolbarButtonFlat");
            GUILayout.Label($"Current brush dpeth:  {BrushDepth}", "ToolbarButtonFlat");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("dragtabdropwindow", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
            GUILayout.Label("Vertex View", "LODRenderersText");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Unselected Vertex Color", txtStyle, GUILayout.Width(120));
            UnselectedVertexColor = EditorGUILayout.ColorField(new GUIContent(), UnselectedVertexColor, true, true, false, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Selected Vertex Color", txtStyle, GUILayout.Width(120));
            SelectedVertexColor = EditorGUILayout.ColorField(new GUIContent(), SelectedVertexColor, true, true, false, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertex Scale", txtStyle, GUILayout.Width(120));
            VertexScale = EditorGUILayout.Slider(VertexScale, 0.001f, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertex With ZTest", txtStyle, GUILayout.Width(120));
            if (GUILayout.Button(VertexWithZTest ? toggleOnContent : toggleContent, txtStyle))
                VertexWithZTest = !VertexWithZTest;
            GUILayout.Label("       /Tab", hotKeyStyle, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Hide Unselected Vertex", txtStyle, GUILayout.Width(120));
            if (GUILayout.Button(HideUnselectedVertex ? toggleOnContent : toggleContent, txtStyle))
                HideUnselectedVertex = !HideUnselectedVertex;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Brush Color View", txtStyle, GUILayout.Width(120));
            if (GUILayout.Button(BrushColorView ? toggleOnContent : toggleContent, txtStyle))
                BrushColorView = !BrushColorView;
            GUILayout.Label("       /V", hotKeyStyle, GUILayout.Width(50));
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

        void updateVertexViewBuffer(bool triggerCala = false)
        {
            if (ModEditor == null || ModEditor.camera == null)
                return;
            ModEditor.Mat_Util.SetVector("_MouseTexcoord", Mouse.ScreenTexcoord);
            ModEditor.Mat_Util.SetFloat("_BrushSize", BrushSize);
            ModEditor.Mat_Util.SetFloat("_BrushDepth", BrushDepth);
            ModEditor.Mat_Util.SetColor("_BrushViewColor", ModEditor.Manager.BrushScopeViewColor);
            bool brushOn = ModEditor.TabType == ModEditorTabType.VertexBrush;
            for (int i = 0; i < ModEditor.CalcShaderDatas.Count; i++)
            {
                CalcShaderData.CalcVertexsData data = ModEditor.CalcShaderDatas[i];
                if (brushOn)
                {
                    data.Update(ModEditor.camera, Mouse.ScreenTexcoord, BrushSize, BrushDepth);
                    if(BrushColorView || triggerCala)
                        data.Cala(ModEditor.Manager.BrushColorFrom, ModEditor.Manager.BrushColorTo, ModEditor.Manager.BrushColorFromStep, ModEditor.Manager.BrushColorToStep);
                }
                if (data.IsAvailable)
                {
                    data.material.SetInt("_BrushOn", brushOn ? 1 : 0);
                    data.material.SetInt("_HideNoSelectVertex", HideUnselectedVertex ? 1 : 0);
                    data.material.SetColor("_UnselectedVertexColor", UnselectedVertexColor);
                    data.material.SetColor("_SelectedVertexColor", SelectedVertexColor);
                    data.material.SetFloat("_VertexScale", VertexScale);
                    data.material.SetInt("_VertexWithZTest", VertexWithZTest ? (int)CompareFunction.LessEqual : (int)CompareFunction.Always);
                    data.material.SetInt("_OnlyZone", BrushLock && Key.Shift ? 1 : 0);
                    data.material.SetInt("_Hide", BrushLock && Key.Shift && !Key.Control ? 1 : 0);
                }
            }
            SceneView.RepaintAll();
        }

        bool brushGradientDisabled()
        {
            if (ModEditor != null && ModEditor.TabType == ModEditorTabType.VertexBrush)
            {
                if (Key.Shift)
                {
                    if (Key.Control)
                        return false;
                }
                else
                {
                    if (!Key.Alt)
                        return false;
                }
            }
            return true;
        }
    }
}