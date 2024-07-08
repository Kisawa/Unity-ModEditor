using UnityEngine;
using UnityEditor;
using System;

class IconView : EditorWindow
{
    Texture2D[] icons;

    [MenuItem("Tools/Icon View")]
    public static void ShowWindow()
    {
        IconView iconView = GetWindow(typeof(IconView)) as IconView;
        iconView.icons = Resources.FindObjectsOfTypeAll<Texture2D>();
    }

    public Vector2 scrollPosition;

    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        //ƒ⁄÷√Õº±Í
        for (int i = 0; i < icons.Length; i += 8)
        {
            Texture2D icon = icons[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label(icon.name, GUILayout.Width(150));
            GUILayout.Button(icons[i], GUILayout.Width(50), GUILayout.Height(30));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}