using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModEditor
{
    public class NormalEditorTab : WindowTabBase
    {
        new ModEditorWindow window;

        public NormalEditorTab(EditorWindow window) : base(window)
        {
            this.window = window as ModEditorWindow;
        }

        public override void Draw()
        {
            if (GUILayout.Button("Write avg normals to model's tangent."))
            {
                writeAcgNormalToTangent();
            }
        }

        void writeAcgNormalToTangent()
        {
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    continue;
                Mesh mesh = meshFilter.sharedMesh;
                Dictionary<Vector3, Vector3> avgNormalDic = new Dictionary<Vector3, Vector3>();
                for (var j = 0; j < mesh.vertexCount; j++)
                {
                    Vector3 vertex = mesh.vertices[j];
                    if (avgNormalDic.TryGetValue(vertex, out Vector3 avgNormal))
                        avgNormalDic[vertex] = (avgNormal + mesh.normals[j]).normalized;
                    else
                        avgNormalDic.Add(vertex, mesh.normals[j]);
                }
                Vector4[] avgNormals = new Vector4[mesh.vertexCount];
                for (int j = 0; j < mesh.vertexCount; j++)
                    avgNormals[j] = avgNormalDic[mesh.vertices[j]];
                mesh.tangents = avgNormals;
                Debug.LogError(AssetDatabase.GetAssetPath(mesh));
            }
        }
    }
}