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

        ComputeShader _calcVertexShader;
        ComputeShader calcVertexShader
        {
            get
            {
                if (_calcVertexShader == null)
                    _calcVertexShader = AssetDatabase.LoadAssetAtPath<ComputeShader>($"{ModEditorWindow.ModEditorPath}Editor/Shaders/CalcViewVertex.compute");
                return _calcVertexShader;
            }
        }

        public override void Draw()
        {
            if (GUILayout.Button("Write avg normals to model's tangent"))
            {
                writeAcgNormalToTangent();
            }
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Compute Shader Test"))
            {
                calcTest();
            }
        }

        void calcTest()
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
                    calcMesh(meshFilter.sharedMesh);
                }
                SkinnedMeshRenderer skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    window.SetEditingMesh(target, skinnedMeshRenderer);
                    calcMesh(skinnedMeshRenderer.sharedMesh);
                }
            }
        }

        void calcMesh(Mesh mesh)
        {
            if (mesh == null)
                return;
            int kernel = calcVertexShader.FindKernel("CSMain");
            ComputeBuffer vertexs = new ComputeBuffer(mesh.vertexCount, 12);
            NativeArray<Vector3> _vertexs = new NativeArray<Vector3>(mesh.vertices, Allocator.Temp);
            vertexs.SetData(_vertexs);
            _vertexs.Dispose();
            calcVertexShader.SetBuffer(kernel, "vertexs", vertexs);
            calcVertexShader.Dispatch(kernel, Mathf.CeilToInt((float)mesh.vertexCount / 1024), 1, 1);
            Vector3[] newVertex = new Vector3[mesh.vertexCount];
            vertexs.GetData(newVertex);
            vertexs.Dispose();
            mesh.vertices = newVertex;
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