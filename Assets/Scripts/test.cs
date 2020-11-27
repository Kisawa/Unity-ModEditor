using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class test : ScriptableObject
{
    const string path = "Assets/Scripts/test.asset";

    [MenuItem("Tools/Create Test")]
    static void Create() 
    {
        test t = CreateInstance<test>();
        //AssetDatabase.CreateAsset(t, "Assets/Scripts/test.asset");
        //AssetDatabase.ImportAsset("Assets/Scripts/test.asset");
        t.res = true;
        t.t1 = CreateInstance<test1>();
        t.t1.str = "viz";
        AssetDatabase.AddObjectToAsset(t.t1, path);
        AssetDatabase.CreateAsset(t, path);
        AssetDatabase.ImportAsset(path);
    }

    //public ExposedReference<Camera> sceneCamera;
    //public ExposedReference<GameObject> sceneGameObject;

    [MenuItem("Tools/Delete Test1")]
    static void DeleteTest1() 
    {
        test t = AssetDatabase.LoadAssetAtPath<test>(path);
        Object.DestroyImmediate(t.t1, true);
        t.t1 = null;
        AssetDatabase.ImportAsset(path);
    }

    public bool res;
    public test1 t1;
}

public class test1 : ScriptableObject
{
    public string str;
}