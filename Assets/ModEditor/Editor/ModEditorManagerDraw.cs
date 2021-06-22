using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ModEditor
{
    [CustomEditor(typeof(ModEditorManager))]
    public class ModEditorManagerDraw : Editor
    {
        public override void OnInspectorGUI()
        {
            GUIStyle labelStyle = GUI.skin.GetStyle("LODRenderersText");
            EditorGUILayout.LabelField("Just some custom data.", labelStyle);
        }
    }
}