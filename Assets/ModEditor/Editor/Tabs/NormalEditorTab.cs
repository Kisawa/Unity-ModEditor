using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

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
                {
                    window.SetEditingMesh(target, meshFilter);
                    writeAcgNormalToTangent(meshFilter.sharedMesh);
                }
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    window.SetEditingMesh(target, skinnedMeshRenderer);
                    writeAcgNormalToTangent(skinnedMeshRenderer.sharedMesh);
                }
            }
        }

        void writeAcgNormalToTangent(Mesh mesh)
        {
            if (mesh == null)
                return;
            var job = new AvgNormal()
            {
                vertexs = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob),
                normals = new NativeArray<Vector3>(mesh.normals, Allocator.TempJob),
                output = new NativeArray<Vector4>(mesh.vertexCount, Allocator.TempJob)
            };
            JobHandle jobHandle = job.Schedule();
            jobHandle.Complete();
            mesh.tangents = job.output.ToArray();
            job.output.Dispose();
        }
    }
}