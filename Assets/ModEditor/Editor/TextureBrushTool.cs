using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ModEditor
{
    [EditorTool("ModEditor TextureBrush Tool")]
    public class TextureBrushTool : EditorTool
    {
        static ModEditorWindow ModEditor => ModEditorWindow.Self;

        GUIContent icon;

        public override GUIContent toolbarIcon => icon;

        private void OnEnable()
        {
            if (icon == null)
                icon = EditorGUIUtility.IconContent("textureChecker");
        }

        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
            Handles.BeginGUI();
            GUIStyle txtStyle = GUI.skin.GetStyle("AboutWIndowLicenseLabel");
            GUIStyle hotKeyStyle = GUI.skin.GetStyle("LODSliderTextSelected");
            GUIStyle msgStyle = GUI.skin.GetStyle("LODRendererAddButton");

            Rect rect = new Rect(window.position.width - 250, window.position.height - 80, 240, 50);
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginVertical("dragtabdropwindow", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
            GUILayout.Label("Texture Brush", "LODRenderersText");

            GUILayout.Button("Test");

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
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
            if (ModEditor != null && ModEditor.Manager.Target != null && ModEditor.TabType == ModEditorTabType.TextureBrush && ModEditor.Tab_TextureBrush.TextureManager.Texture != null)
                return true;
            return false;
        }
    }
}