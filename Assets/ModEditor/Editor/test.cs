using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class test
{
    [MenuItem("Tools/Delete ExposedManger")]
    static void DeleteExposedManager()
    {
        GameObject obj = GameObject.Find("ExposedManagement");
        if (obj != null)
        {
            Object.DestroyImmediate(obj);
            Debug.LogError(1);
        }
    }

    const string path = "Assets/ModEditor/parent.asset";
    [MenuItem("Tools/Create TestAsset")]
    static void CreateTestAsset()
    {
        Parent parent = ScriptableObject.CreateInstance<Parent>();
        parent.Id = 999;
        AssetDatabase.CreateAsset(parent, path);
        AssetDatabase.SetMainObject(parent, path);

        parent.child = ScriptableObject.CreateInstance<Child>();
        parent.child.Str = "Kisawa";
        parent.child.name = "child";
        AssetDatabase.AddObjectToAsset(parent.child, parent);

        AssetDatabase.ImportAsset(path);
    }

    [MenuItem("Tools/Change ChildName")]
    static void ChangeChildName()
    {
        Parent parent = AssetDatabase.LoadAssetAtPath<Parent>(path);
        if (parent != null)
        {
            //parent.Id = 01;
            Child child = parent.child;
            child.Str = "kisawa";
            EditorUtility.SetDirty(child);
            Debug.LogError(1);
        }
    }
}