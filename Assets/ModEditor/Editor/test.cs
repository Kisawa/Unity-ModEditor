using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class test
{
    [MenuItem("Tools/DeleteExposedManger")]
    static void DeleteExposedManager()
    {
        GameObject obj = GameObject.Find("ExposedManagement");
        if (obj != null)
        {
            Object.DestroyImmediate(obj);
            Debug.LogError(1);
        }
    }
}