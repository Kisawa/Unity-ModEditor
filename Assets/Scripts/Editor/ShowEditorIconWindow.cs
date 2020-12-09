using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShowEditorIconWindow : EditorWindow
{
    [MenuItem(("Tools/Show Editor Icons"))]
    static void Open()
    {
        ShowEditorIconWindow window = GetWindow<ShowEditorIconWindow>("Show Editor Icons");

        if (window.iconNames == null)
        {
            window.iconNames = new List<string>();
            Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            Debug.unityLogger.logEnabled = false;
            for (int i = 0; i < textures.Length; i++)
            {
                GUIContent content = EditorGUIUtility.IconContent(textures[i].name);
                if(content != null && content.image != null)
                    window.iconNames.Add(textures[i].name);
            }
            Debug.unityLogger.logEnabled = true;
        }
    }

    float innerWidth = 150;
    Vector2 scroll;
    List<string> iconNames;

    void OnGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll);
        int count = (int)(position.width / innerWidth);
        for (int i = 0; i < iconNames.Count; i++)
        {
            if (i % count == 0)
                EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginFadeGroup(1);
            EditorGUILayout.LabelField(iconNames[i], GUILayout.Width(innerWidth));
            if (GUILayout.Button(EditorGUIUtility.IconContent(iconNames[i]), GUILayout.Width(innerWidth), GUILayout.Height(50)))
                GUIUtility.systemCopyBuffer = iconNames[i];
            EditorGUILayout.EndFadeGroup();
            if (i % count == count - 1 || i == iconNames.Count - 1)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(20);
            }
        }
        GUILayout.EndScrollView();
    }
}