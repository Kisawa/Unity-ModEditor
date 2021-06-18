using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ModEditor;

public class GUIStyleViewer : EditorWindow
{
    [MenuItem("Tools/GUIStyle View")]
    static void Open()
    {
        GUIStyleViewer window = GetWindow<GUIStyleViewer>("GUIStyle View");
        string path = $"{ModEditorWindow.ModEditorPath}/Textures/Sphere-Editing.png";
        window.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        window.subContent = EditorGUIUtility.IconContent("ShurikenToggleNormalMixed");
    }

    float innerWidth = 300;
    Vector2 scroll;
    Texture2D texture;
    GUIContent subContent;

    void OnGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll);
        GUI.Box(new Rect(0, 0, position.width, position.height), texture);
        int count = (int)(position.width / innerWidth);
        for (int i = 0; i < GUI.skin.customStyles.Length; i++)
        {
            GUIStyle style = GUI.skin.customStyles[i];
            //if (i % count == 0)
            //    EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.BeginFadeGroup(1);
            //EditorGUILayout.BeginVertical();

            //EditorGUILayout.BeginVertical(style);
            ////EditorGUILayout.LabelField("Binormal", style, GUILayout.Width(200));
            //GUILayout.Label(style.name);
            //if (GUILayout.Button(style.name))
            //    GUIUtility.systemCopyBuffer = style.name;
            //EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Popup(0, new string[] { "none", "first" }, style, GUILayout.Width(50));
            if (GUILayout.Button("Game Camera Follow"))
            {
                GUIUtility.systemCopyBuffer = style.name;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            //EditorGUILayout.EndVertical();

            //EditorGUILayout.EndFadeGroup();
            //if (i % count == count - 1 || i == GUI.skin.customStyles.Length - 1)
            //{
            //    EditorGUILayout.EndHorizontal();
            //    EditorGUILayout.Space(50);
            //}
        }
        GUILayout.EndScrollView();
    }
}