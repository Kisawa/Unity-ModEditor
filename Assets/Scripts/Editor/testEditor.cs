using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExposedManagement))]
public class testEditor : Editor
{
    SerializedProperty asset;

    SerializedObject assetSerializedObject;

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        EditorGUILayout.Space(30);
        if(asset == null)
            asset = serializedObject.FindProperty("t");

        if (EditorGUI.EndChangeCheck())
        {
            if (asset != null && asset.objectReferenceValue != null)
            {
                assetSerializedObject = new SerializedObject(asset.objectReferenceValue, target);
            }
            else
                assetSerializedObject = null;
        }

        if (assetSerializedObject != null)
        {
            var iterator = assetSerializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }
}