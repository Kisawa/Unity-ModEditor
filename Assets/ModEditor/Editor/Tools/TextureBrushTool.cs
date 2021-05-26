using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ModEditor
{
    public class TextureBrushTool : SceneToolBase
    {
        public override Rect Draw(EditorWindow window, GUIStyle txtStyle, GUIStyle hotKeyStyle, GUIStyle msgStyle)
        {
            Rect rect = new Rect(window.position.width - 250, window.position.height - 80, 240, 50);
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginVertical("dragtabdropwindow", GUILayout.Width(rect.width), GUILayout.Height(rect.height));
            GUILayout.Label("Texture Brush", "LODRenderersText");

            EditorGUILayout.LabelField("Texture Brush", txtStyle);

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
            return rect;
        }

        public override bool IsAvailable()
        {
            if (ModEditor.TabType == ModEditorTabType.TextureBrush && ModEditor.Tab_TextureBrush.TextureManager.IsAvailable)
                return true;
            return false;
        }
    }
}