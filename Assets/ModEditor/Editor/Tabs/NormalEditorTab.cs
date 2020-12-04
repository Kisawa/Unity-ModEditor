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
            if (GUILayout.Button("Write avg normals to model's tangent"))
            {
                writeAcgNormalToTangent();
            }
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Save mesh"))
            {
                saveMesh();
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
                if (meshFilter != null)
                    writeAcgNormalToTangent(meshFilter.sharedMesh);
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                    writeAcgNormalToTangent(skinnedMeshRenderer.sharedMesh);
            }
        }

        void writeAcgNormalToTangent(Mesh mesh)
        {
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
        }

        void saveMesh()
        {
            for (int i = 0; i < window.Manager.TargetChildren.Count; i++)
            {
                GameObject target = window.Manager.TargetChildren[i];
                if (target == null)
                    continue;
                if (!window.Manager.ActionableDic[target])
                    continue;
                string path = $"{ModEditorWindow.ModEditorPath}{target.name}.mesh";
                MeshFilter meshFilter = target.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Mesh mesh = Object.Instantiate(meshFilter.sharedMesh);
                    AssetDatabase.CreateAsset(mesh, path);
                    AssetDatabase.ImportAsset(path);
                }
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    Mesh mesh = Object.Instantiate(skinnedMeshRenderer.sharedMesh);
                    AssetDatabase.CreateAsset(mesh, path);
                    AssetDatabase.ImportAsset(path);
                }
            }
        }
    }
}