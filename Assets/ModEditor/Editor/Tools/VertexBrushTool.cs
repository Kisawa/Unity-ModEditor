using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModEditor
{
    public class VertexBrushTool : SceneToolBase
    {
        GUIContent toggleContent;
        GUIContent toggleOnContent;
        GUIContent lockContent;
        GUIContent unlockContent;
        GUIContent lockBrushContent;
        GUIContent unlockBrushContent;

        bool zoneLock;
        bool ZoneLock
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    zoneLock = ModEditor.Tab_VertexBrush.VertexZoneLock;
                return zoneLock;
            }
        }

        bool brushLock;
        bool BrushLock
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushLock = ModEditor.Tab_VertexBrush.VertexBrushLock;
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

        bool hideUnselectedVertex = false;
        bool HideUnselectedVertex
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    hideUnselectedVertex = ModEditor.Manager.HideUnselectedVertex;
                return hideUnselectedVertex;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                hideUnselectedVertex = value;
                ModEditor.Manager.HideUnselectedVertex = hideUnselectedVertex;
            }
        }

        bool brushColorView = false;
        bool BrushColorView
        {
            get
            {
                if (ModEditor != null)
                    brushColorView = ModEditor.Tab_VertexBrush.BrushColorView;
                return brushColorView;
            }
            set
            {
                if (ModEditor == null)
                    return;
                brushColorView = value;
                ModEditor.Tab_VertexBrush.BrushColorView = brushColorView;
            }
        }

        float brushSize = 0.05f;
        float BrushSize
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushSize = ModEditor.Manager.VertexBrushSize;
                return brushSize;
            }
        }

        float brushDepth = ModEditorConstants.VertexBrushMaxDepth;
        float BrushDepth
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushDepth = ModEditor.Manager.VertexBrushDepth;
                return brushDepth;
            }
        }

        VertexBrushType vertexBrushType = VertexBrushType.Color;
        VertexBrushType VertexBrushType
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    vertexBrushType = ModEditor.Manager.VertexBrushType;
                return vertexBrushType;
            }
        }

        float brushStrength = 1;
        float BrushStrength
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushStrength = ModEditor.Manager.VertexBrushStrength;
                return brushStrength;
            }
        }

        Color brushColor = Color.white;
        Color BrushColor
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColor = ModEditor.Manager.VertexBrushColor;
                return brushColor;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColor = value;
                ModEditor.Manager.VertexBrushColor = brushColor;
            }
        }

        Color brushColorFrom = Color.red;
        Color BrushColorFrom
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColorFrom = ModEditor.Manager.VertexBrushColorFrom;
                return brushColorFrom;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColorFrom = value;
                ModEditor.Manager.VertexBrushColorFrom = brushColorFrom;
            }
        }

        float brushColorFromStep = 0;
        float BrushColorFromStep
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColorFromStep = ModEditor.Manager.VertexBrushColorFromStep;
                return brushColorFromStep;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColorFromStep = value;
                ModEditor.Manager.VertexBrushColorFromStep = brushColorFromStep;
            }
        }

        Color brushColorTo = Color.red;
        Color BrushColorTo
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColorTo = ModEditor.Manager.VertexBrushColorTo;
                return brushColorTo;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColorTo = value;
                ModEditor.Manager.VertexBrushColorTo = brushColorTo;
            }
        }

        float brushColorToStep = 0;
        float BrushColorToStep
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    brushColorToStep = ModEditor.Manager.VertexBrushColorToStep;
                return brushColorToStep;
            }
            set
            {
                if (ModEditor == null || ModEditor.Manager == null)
                    return;
                brushColorToStep = value;
                ModEditor.Manager.VertexBrushColorToStep = brushColorToStep;
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
                    GradientColorKey fromColor = new GradientColorKey() { color = ModEditor.Manager.VertexBrushColorFrom, time = ModEditor.Manager.VertexBrushColorFromStep };
                    GradientAlphaKey fromAlpha = new GradientAlphaKey() { alpha = ModEditor.Manager.VertexBrushColorFrom.a, time = ModEditor.Manager.VertexBrushColorFromStep };
                    GradientColorKey toColor = new GradientColorKey() { color = ModEditor.Manager.VertexBrushColorTo, time = ModEditor.Manager.VertexBrushColorToStep };
                    GradientAlphaKey toAlpha = new GradientAlphaKey() { alpha = ModEditor.Manager.VertexBrushColorTo.a, time = ModEditor.Manager.VertexBrushColorToStep };
                    brushGradient.SetKeys(new GradientColorKey[] { fromColor, toColor }, new GradientAlphaKey[] { fromAlpha, toAlpha });
                }
                return brushGradient;
            }
        }

        WriteType writeType = WriteType.Replace;
        WriteType WriteType
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    writeType = ModEditor.Manager.WriteType;
                return writeType;
            }
        }

        WriteTargetType writeTargetType = WriteTargetType.VertexColor;
        WriteTargetType WriteTargetType
        {
            get
            {
                if (ModEditor != null && ModEditor.Manager != null)
                    writeTargetType = ModEditor.Manager.WriteTargetType;
                return writeTargetType;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (toggleContent == null)
                toggleContent = EditorGUIUtility.IconContent("ShurikenToggleHover");
            if (toggleOnContent == null)
                toggleOnContent = EditorGUIUtility.IconContent("ShurikenToggleFocusedOn");
            if (lockContent == null)
                lockContent = EditorGUIUtility.TrIconContent("IN LockButton on act", "Lock or unlock available vertices.   /CapsLock");
            if (unlockContent == null)
                unlockContent = EditorGUIUtility.TrIconContent("IN LockButton", "Lock or unlock available vertices.   /CapsLock");
            if (lockBrushContent == null)
                lockBrushContent = EditorGUIUtility.TrIconContent("d_scenepicking_notpickable-mixed", "Lock or unlock the brush position.   /Alt-CapsLock");
            if (unlockBrushContent == null)
                unlockBrushContent = EditorGUIUtility.TrIconContent("d_scenepicking_pickable-mixed_hover", "Lock or unlock the brush position.   /Alt-CapsLock");
        }

        public override bool IsAvailable()
        {
            return ModEditor.TabType == ModEditorTabType.VertexBrush;
        }

        public override Rect Draw(EditorWindow window, GUIStyle txtStyle, GUIStyle hotKeyStyle, GUIStyle msgStyle)
        {
            Rect rect = new Rect(window.position.width - 250, window.position.height - 175, 240, 145);
            switch (VertexBrushType)
            {
                case VertexBrushType.TwoColorGradient:
                    rect = new Rect(window.position.width - 250, window.position.height - 215, 240, 185);
                    break;
            }
            GUILayout.BeginArea(rect);
            switch (VertexBrushType)
            {
                case VertexBrushType.Color:
                    break;
                case VertexBrushType.TwoColorGradient:
                    EditorGUI.LabelField(new Rect(0, 0, 240, 40), "", GUI.skin.GetStyle("dockarea"));
                    EditorGUI.BeginDisabledGroup(brushGradientDisabled());
                    EditorGUI.LabelField(new Rect(0, 0, 20, 15), "/A", hotKeyStyle);
                    BrushColorFrom = EditorGUI.ColorField(new Rect(20, 2, 70, 12), BrushColorFrom);
                    EditorGUI.LabelField(new Rect(90, -1, 30, 15), "From", txtStyle);
                    EditorGUI.LabelField(new Rect(130, 0, 80, 15), $"{BrushStrength} - {WriteType}", msgStyle);
                    EditorGUI.LabelField(new Rect(223, 0, 20, 15), "/D", hotKeyStyle);

                    EditorGUI.LabelField(new Rect(3, 12, 30, 15), $"{BrushColorFromStep}", txtStyle);
                    EditorGUI.GradientField(new Rect(30, 15, 180, 12), BrushGradient);
                    EditorGUI.LabelField(new Rect(210, 12, 30, 15), $"{1 - BrushColorToStep}", txtStyle);

                    EditorGUI.LabelField(new Rect(0, 24, 20, 15), "/Z", hotKeyStyle);
                    EditorGUI.LabelField(new Rect(35, 24, 80, 15), $"to {WriteTargetType}", msgStyle);
                    EditorGUI.LabelField(new Rect(130, 24, 30, 15), "To", txtStyle);
                    BrushColorTo = EditorGUI.ColorField(new Rect(150, 27, 70, 12), BrushColorTo);
                    EditorGUI.LabelField(new Rect(223, 24, 20, 15), "/C", hotKeyStyle);
                    EditorGUI.EndDisabledGroup();
                    GUILayout.Space(40);
                    break;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(BrushLock);
            GUILayout.Label(ZoneLock ? lockContent : unlockContent, "ToolbarButtonFlat");
            EditorGUI.EndDisabledGroup();
            GUILayout.Label(BrushLock ? lockBrushContent : unlockBrushContent, "ToolbarButtonFlat");
            GUILayout.Label($"Current brush dpeth:  {BrushDepth}", "ToolbarButtonFlat");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("dragtabdropwindow", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
            GUILayout.Label("Vertex Brush", "LODRenderersText");

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
            updateVertexViewBuffer();
            return rect;
        }

        void updateVertexViewBuffer()
        {
            if (ModEditor == null || ModEditor.camera == null)
                return;
            ModEditor.Mat_Util.SetVector("_MouseTexcoord", Mouse.ScreenTexcoord);
            ModEditor.Mat_Util.SetFloat("_BrushSize", BrushSize);
            ModEditor.Mat_Util.SetColor("_BrushScopeViewColor", ModEditor.Manager.VertexBrushScopeViewColor * (BrushLock ? 0 : 1));
            switch (VertexBrushType)
            {
                case VertexBrushType.Color:
                    ModEditor.Mat_Util.SetFloat("_FromStep", 0);
                    ModEditor.Mat_Util.SetFloat("_ToStep", 1);
                    break;
                case VertexBrushType.TwoColorGradient:
                    ModEditor.Mat_Util.SetFloat("_FromStep", BrushColorFromStep);
                    ModEditor.Mat_Util.SetFloat("_ToStep", BrushColorToStep);
                    break;
            }
            bool brushOn = ModEditor.TabType == ModEditorTabType.VertexBrush;
            for (int i = 0; i < ModEditor.Tab_VertexBrush.CalcShaderDatas.Count; i++)
            {
                CalcManager data = ModEditor.Tab_VertexBrush.CalcShaderDatas[i];
                if (brushOn)
                {
                    if (!BrushLock)
                        data.Update(ModEditor.camera, Mouse.ScreenTexcoord, BrushSize, BrushDepth);
                    if (BrushColorView)
                    {
                        switch (VertexBrushType)
                        {
                            case VertexBrushType.Color:
                                data.Cala(BrushColor, 1);
                                break;
                            case VertexBrushType.TwoColorGradient:
                                data.Cala(BrushColorFrom, BrushColorTo, BrushColorFromStep, BrushColorToStep, 1);
                                break;
                        }
                    }
                }
                if (data.IsAvailable)
                {
                    data.Material.SetInt("_BrushOn", brushOn ? 1 : 0);
                    bool hideUnselectedVertex = HideUnselectedVertex;
                    if (ZoneLock && Key.Shift)
                        hideUnselectedVertex = false;
                    data.Material.SetInt("_HideNoSelectVertex", hideUnselectedVertex ? 1 : 0);
                    data.Material.SetColor("_UnselectedVertexColor", UnselectedVertexColor);
                    data.Material.SetColor("_SelectedVertexColor", SelectedVertexColor);
                    data.Material.SetFloat("_VertexScale", VertexScale);
                    data.Material.SetInt("_VertexWithZTest", VertexWithZTest ? (int)CompareFunction.LessEqual : (int)CompareFunction.Always);
                    bool hideColorView = !BrushColorView;
                    if (ZoneLock && Key.Shift && !Key.Control)
                        hideColorView = true;
                    data.Material.SetInt("_HideColorView", hideColorView ? 1 : 0);
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
                else if (!Key.Alt && !Key.Control)
                    return false;
            }
            return true;
        }
    }
}