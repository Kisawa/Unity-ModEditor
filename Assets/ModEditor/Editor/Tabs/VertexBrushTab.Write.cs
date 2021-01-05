using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ModEditor
{
    public partial class VertexBrushTab
    {
        void write()
        {
            switch (window.Manager.BrushType)
            {
                case BrushType.ScreenScope:
                    writeVertexColor_ScreenScope();
                    break;
            }
        }

        void writeVertexColor_ScreenScope()
        {
            if (objInOperation == null)
                return;
            for (int i = 0; i < objInOperation.Count; i++)
            {
                Transform trans = objInOperation[i].Item1;
                Mesh mesh = objInOperation[i].Item2;
                if (trans == null || mesh == null)
                    continue;
                CalcShaderData.CalcVertexsData data = window.CalcShaderDatas.FirstOrDefault(x => x.trans == trans);
                if (data != null && data.IsAvailable)
                {
                    if (mesh.colors.Length != mesh.vertexCount)
                        mesh.colors = Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                    mesh.colors = CalcUtil.WriteVertexColor_UseSelectData(window.Manager.BrushColor, data.RW_Selects, mesh.colors);
                }
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
            AvgNormalJob job = new AvgNormalJob()
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