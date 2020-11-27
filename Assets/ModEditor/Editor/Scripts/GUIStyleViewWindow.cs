using UnityEngine;
using UnityEditor;

public class GUIStyleViewer : EditorWindow
{
    [MenuItem("Tools/GUIStyle View")]
    static void Open()
    {
        GUIStyleViewer window = GetWindow<GUIStyleViewer>("GUIStyle View");
    }

    float innerWidth = 300;
    Vector2 scroll;

    void OnGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll);
        int count = (int)(position.width / innerWidth); ;
        for (int i = 0; i < GUI.skin.customStyles.Length; i++)
        {
            if (i % count == 0)
                EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginFadeGroup(1);
            GUIStyle style = GUI.skin.customStyles[i];

            EditorGUILayout.BeginVertical();
            GUILayout.Toolbar(0, new string[] { "0", "1", "2" }, style.name);
            if (GUILayout.Button(style.name))
                GUIUtility.systemCopyBuffer = style.name;
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndFadeGroup();
            if (i % count == count - 1 || i == GUI.skin.customStyles.Length - 1)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(50);
            }
        }
        GUILayout.EndScrollView();
    }
}