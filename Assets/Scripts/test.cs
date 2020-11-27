using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class test : ScriptableObject
{
    [MenuItem("Tools/Create Test")]
    static void Create() 
    {
        test t = CreateInstance<test>();
        AssetDatabase.CreateAsset(t, "Assets/Scripts/test.asset");
        //AssetDatabase.ImportAsset("Assets/Scripts/test.asset");
        t.res = true;
        AssetDatabase.CreateAsset(t, "Assets/Scripts/test.asset");
        AssetDatabase.ImportAsset("Assets/Scripts/test.asset");
    }

    //public ExposedReference<Camera> sceneCamera;
    //public ExposedReference<GameObject> sceneGameObject;

    public bool res;
}
