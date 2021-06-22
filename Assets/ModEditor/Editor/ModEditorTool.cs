using System;
using System.Collections;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ModEditor
{
    [EditorTool("ModEditor Tool")]
    public class ModEditorTool : EditorTool
    {
        static ModEditorWindow ModEditor => ModEditorWindow.Self;

        GUIContent icon;
        public override GUIContent toolbarIcon => icon;

        VertexBrushTool vertexBrushTool;
        TextureBrushTool textureBrushTool;

        ModEditorToolType toolType = ModEditorToolType.None;
        ModEditorToolType ToolType
        {
            get
            {
                if (ModEditor != null)
                    toolType = ModEditor.ToolType;
                return toolType;
            }
        }

        private void OnEnable()
        {
            if (icon == null)
                icon = EditorGUIUtility.IconContent("d_PrefabModel On Icon");
            if (vertexBrushTool == null)
                vertexBrushTool = new VertexBrushTool();
            if (textureBrushTool == null)
                textureBrushTool = new TextureBrushTool();
            vertexBrushTool.OnEnable();
            textureBrushTool.OnEnable();
        }

        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
            GUIStyle txtStyle = GUI.skin.GetStyle("AboutWIndowLicenseLabel");
            GUIStyle hotKeyStyle = GUI.skin.GetStyle("LODSliderTextSelected");
            GUIStyle msgStyle = GUI.skin.GetStyle("LODRendererAddButton");
            switch (ToolType)
            {
                case ModEditorToolType.VertexBrush:
                    vertexBrushTool.OnSceneGUI(window);
                    break;
                case ModEditorToolType.TextureBrush:
                    textureBrushTool.OnSceneGUI(window);
                    break;
            }
            Handles.BeginGUI();
            Rect rect = new Rect(window.position.width - 250, window.position.height - 65, 240, 50);
            switch (ToolType)
            {
                case ModEditorToolType.None:
                    GUILayout.BeginArea(rect);
                    GUILayout.Label("ModEditor Tool Ready", "Badge");
                    GUILayout.Label("Switch tabs to select the desired workbench.", "Badge");
                    GUILayout.EndArea();
                    break;
                case ModEditorToolType.VertexBrush:
                    rect = vertexBrushTool.Draw(window, txtStyle, hotKeyStyle, msgStyle);
                    break;
                case ModEditorToolType.TextureBrush:
                    rect = textureBrushTool.Draw(window, txtStyle, hotKeyStyle, msgStyle);
                    break;
            }
            if (ModEditor != null)
            {
                if (GUI.changed)
                    ModEditor.Validate();
                ModEditor.OnSceneGUI = rect.Contains(Event.current.mousePosition);
            }
            Handles.EndGUI();
        }

        public override bool IsAvailable()
        {
            if (ModEditor == null)
                return false;
            if (ModEditor.Manager.Target != null && (ModEditor.Manager.LockTarget || ModEditor.Manager.Target == Selection.activeGameObject))
            {
                switch (ToolType)
                {
                    case ModEditorToolType.VertexBrush:
                        return vertexBrushTool.IsAvailable();
                    case ModEditorToolType.TextureBrush:
                        return textureBrushTool.IsAvailable();
                }
                return true;
            }
            return false;
        }
    }
}