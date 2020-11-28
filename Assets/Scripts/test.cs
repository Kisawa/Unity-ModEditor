using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class test : ScriptableObject
{
    const string path = "Assets/Scripts/test.asset";

    [MenuItem("Tools/Create test")]
    static void CreateTest()
    {
        test t = CreateInstance<test>();
        AssetDatabase.CreateAsset(t, path);
        AssetDatabase.ImportAsset(path);
    }

    public ExposedReference<GameObject> target;
}