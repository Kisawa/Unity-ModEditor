using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyEditorStyleViewer : EditorWindow
{
    Vector2 scrollPosition = new Vector2(0,0);
    string search = "";

    [MenuItem(("Tools/Style View"))]
    public static void Init()
    {
        MyEditorStyleViewer window = (MyEditorStyleViewer)EditorWindow.GetWindow(typeof(MyEditorStyleViewer));
        window.titleContent = new GUIContent("EditorStyleViewer");
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        GUILayout.Label("Click a Sample to copy its Name to your Clipboard", "MiniBoldLabel");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Search:");
        search = EditorGUILayout.TextField(search);

        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach (GUIStyle style in GUI.skin.customStyles)
        {
            if (style.name.ToLower().Contains(search.ToLower()))
            {
                //GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                //GUILayout.Space(7);
                //if (GUILayout.Button(style.name, style))
                //{
                //    EditorGUIUtility.systemCopyBuffer = "\"" + style.name + "\"";
                //}
                //GUILayout.FlexibleSpace();
                //EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                //GUILayout.EndHorizontal();
                //GUILayout.Space(11);
                GUILayout.Space(50);
                GUILayout.BeginVertical();
                EditorGUILayout.Toggle(true);
                //GUILayout.Button("Enable Route", style, GUILayout.Width(100));
                GUILayout.Label(style.name, style, GUILayout.Width(100));
                if (GUILayout.Button("Enable", GUILayout.Width(100)))
                {
                    GUIUtility.systemCopyBuffer = style.name;
                }
                
                GUILayout.EndVertical();
            }
        }
        GUILayout.EndScrollView();
    }
}