using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class testEditor: EditorWindow
{
    [MenuItem("Tools/Test Editor")]
    static void Open()
    {
        testEditor window = GetWindow<testEditor>("Test Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.ObjectField(asset, typeof(testAsset), true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create asset"))
        {
            createAsset();
        }

        if (GUILayout.Button("Remove asset"))
        {
            moveAsset();
        }

        if (GUILayout.Button("Check"))
        {
            if (asset == null)
                Debug.LogError("asset is null");
            else
            {
                string path = AssetDatabase.GetAssetPath(asset.test);
                if (string.IsNullOrEmpty(path))
                {
                    AssetDatabase.AddObjectToAsset(asset.test, asset);
                    Debug.LogError("Check:  " + AssetDatabase.GetAssetPath(asset.test));
                }
                else
                {
                    Debug.LogError(path);
                }
            }
        }

        if (GUILayout.Button("Nmae"))
        {
            if (asset == null)
                Debug.LogError("asset is null");
            else
                Debug.LogError(asset.test.Name);
        }
    }

    public Parent asset;

    void createAsset()
    {
        asset = CreateInstance<Parent>();
        AssetDatabase.CreateAsset(asset, "Assets/Parent.asset");

        asset.test = CreateInstance<testAsset>();
        asset.test.name = "test";
        asset.test.Name = "kisawa";
        AssetDatabase.AddObjectToAsset(asset.test, asset);

        AssetDatabase.ImportAsset("Assets/Parent.asset");
    }

    void moveAsset()
    {
        
        if (asset != null && asset.test != null)
        {
            AssetDatabase.RemoveObjectFromAsset(asset.test);
        }
    }
}