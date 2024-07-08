using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ModEditor
{
    public class BakeSkinMeshTool
    {
        [MenuItem("Tools/ModEditor: Bake Skin Mesh")]
        static void BakeSkinMesh()
        {
            if (Selection.activeGameObject == null)
                return;
            SkinnedMeshRenderer skinnedMesh = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh == null || skinnedMesh.sharedMesh == null)
                return;
            Mesh mesh = new Mesh();
            skinnedMesh.BakeMesh(mesh);
            string path = $"{ModEditorWindow.ModEditorPath}/Meshs/{GetTimeStamp()}.mesh";
            if (!AssetDatabase.IsValidFolder($"{ModEditorWindow.ModEditorPath}/Meshs"))
                AssetDatabase.CreateFolder(ModEditorWindow.ModEditorPath, "Meshs");
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.ImportAsset($"{ModEditorWindow.ModEditorPath}/Meshs");
        }

        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

    }
}